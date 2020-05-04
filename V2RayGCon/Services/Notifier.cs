using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using VgcApis.Interfaces;

namespace V2RayGCon.Services
{
    public class Notifier :
         BaseClasses.SingletonService<Notifier>,
         VgcApis.Interfaces.Services.INotifierService
    {
        Settings setting;
        Servers servers;
        ShareLinkMgr slinkMgr;
        Updater updater;
        NotifyIcon ni;

        static readonly long SpeedtestTimeout = VgcApis.Models.Consts.Core.SpeedtestTimeout;
        static readonly int UpdateInterval = VgcApis.Models.Consts.Intervals.NotifierMenuUpdateIntreval;

        Bitmap orgIcon;
        bool isMenuOpened = false;

        VgcApis.Libs.Tasks.LazyGuy lazyNotifierMenuUpdater;

        enum qsMenuNames
        {
            StopAllServer,
            QuickSwitchMenuRoot,
            SwitchToRandomServer,
            SwitchToRandomTlsServer,
        }

        readonly Dictionary<qsMenuNames, ToolStripMenuItem> qsMenuCompos = null;
        readonly ToolStripMenuItem miPluginsRoot = null;
        readonly ToolStripMenuItem miServersRoot = null;
        readonly ContextMenuStrip niMenuRoot = null;

        Notifier()
        {
            qsMenuCompos = CreateQsMenuCompos();
            miServersRoot = CreateRootMenuItem(I18N.Servers, Properties.Resources.RemoteServer_16x);
            miPluginsRoot = CreateRootMenuItem(I18N.Plugins, Properties.Resources.Module_16x);

            ni = new NotifyIcon()
            {
                Text = I18N.Description,
                Icon = VgcApis.Misc.UI.GetAppIcon(),
                BalloonTipTitle = VgcApis.Misc.Utils.GetAppName(),
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true,
            };

            orgIcon = ni.Icon.ToBitmap();
            niMenuRoot = ni.ContextMenuStrip;
            niMenuRoot.CreateControl();
            var handle = niMenuRoot.Handle;
            VgcApis.Libs.Sys.FileLogger.Info($"Create notify icon {handle}");

            CreateContextMenuStrip(niMenuRoot, miServersRoot, miPluginsRoot);

            VgcApis.Misc.UI.Invoke = Invoke;
            VgcApis.Misc.UI.InvokeThen = InvokeThen;
        }

        public void Run(
            Settings setting,
            Servers servers,
            ShareLinkMgr shareLinkMgr,
            Updater updater)
        {
            this.setting = setting;
            this.servers = servers;
            this.slinkMgr = shareLinkMgr;
            this.updater = updater;

            lazyNotifierMenuUpdater = new VgcApis.Libs.Tasks.LazyGuy(
                UpdateNotifyIconWorker, UpdateInterval, 5000)
            {
                Name = "Notifier.MenuUpdater", // disable debug logging
            };

            BindNiMenuEvents(ni);
            BindServerEvents();
            RefreshNotifyIconLater();

        }

        #region hotkey window

        VgcApis.WinForms.HotKeyWindow hkWindow = null;

        ConcurrentDictionary<string, VgcApis.Models.Datas.HotKeyContext> hkContexts =
            new ConcurrentDictionary<string, VgcApis.Models.Datas.HotKeyContext>();

        int currentEvCode = 0;

        void DestroyHotKeyWindow()
        {
            if (hkWindow == null)
            {
                return;
            }

            VgcApis.Libs.Sys.FileLogger.Info("Destroy hot key window.");
            var wnd = hkWindow;
            hkWindow = null;
            wnd.ReleaseHandle();
            wnd.OnHotKeyMessage -= HandleHotKeyEvent;
            var handles = hkContexts.Keys;
            foreach (var handle in handles)
            {
                if (hkContexts.TryGetValue(handle, out var context))
                {
                    try
                    {
                        wnd.UnregisterHotKey(context);
                    }
                    catch { }
                }
            }
        }

        void CreateHotKeyWindow()
        {
            if (hkWindow != null)
            {
                return;
            }

            hkWindow = new VgcApis.WinForms.HotKeyWindow(niMenuRoot);
            var wndHandle = hkWindow.Handle;
            VgcApis.Libs.Sys.FileLogger.Info($"Create hot key window {wndHandle}.");
            var handles = hkContexts.Keys;
            foreach (var handle in handles)
            {
                if (hkContexts.TryGetValue(handle, out var context))
                {
                    try
                    {
                        hkWindow.RegisterHotKey(context);
                    }
                    catch { }
                }
            }

            hkWindow.OnHotKeyMessage += HandleHotKeyEvent;
        }

        void HandleHotKeyEvent(string keyMsg)
        {
            try
            {
                if (hkContexts.TryGetValue(keyMsg, out var context))
                {
                    VgcApis.Misc.Utils.RunInBackground(() =>
                    {
                        try
                        {
                            context.handler?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            VgcApis.Libs.Sys.FileLogger.Error($"Handle hotkey error!\n{ex}");
                        }
                    });
                }
            }
            catch { }
        }

        string RegisterHotKeyWorker(
            Action hotKeyHandler, string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {
            if (hkWindow == null)
            {
                CreateHotKeyWindow();
            }

            var context = VgcApis.Models.Datas.HotKeyContext.Create(
                hotKeyHandler, keyName, hasAlt, hasCtrl, hasShift);

            if (context == null)
            {
                return null;
            }

            context.evCode = currentEvCode++;
            var keyMsg = context.KeyMessage;
            var handle = keyMsg.ToString();
            if (hkContexts.ContainsKey(handle)
                || !hkWindow.RegisterHotKey(context))
            {
                return null;
            }

            if (hkContexts.TryAdd(handle, context))
            {
                return handle;
            }
            return null;
        }

        bool UnregisterHotKeyWorker(string handle)
        {
            try
            {
                if (!string.IsNullOrEmpty(handle)
                    && hkContexts.TryRemove(handle, out var context)
                    && hkWindow != null)
                {
                    return hkWindow.UnregisterHotKey(context);
                }
            }
            catch { }
            return false;
        }


        public string RegisterHotKey(
             Action hotKeyHandler,
             string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {
            string handle = null;
            Invoke(() =>
           {
               try
               {
                   handle = RegisterHotKeyWorker(hotKeyHandler, keyName, hasAlt, hasCtrl, hasShift);
               }
               catch { }
           });
            return handle;
        }
        public bool UnregisterHotKey(string hotKeyHandle)
        {
            var r = false;
            Invoke(() =>
            {
                try
                {
                    r = UnregisterHotKeyWorker(hotKeyHandle);
                }
                catch { }
            });
            return r;
        }

        #endregion


        #region INotifier.WinForms
        public void ShowFormOption() => Views.WinForms.FormOption.ShowForm();

        public void ShowFormMain() => Views.WinForms.FormMain.ShowForm();

        public void ShowFormLog() => Views.WinForms.FormLog.ShowForm();

        public void ShowFormQrcode() => Views.WinForms.FormQRCode.ShowForm();

        #endregion


        #region public method
        public void BlockingWaitOne(AutoResetEvent autoEv) =>
            autoEv.WaitOne();

        public void RefreshNotifyIconLater() => lazyNotifierMenuUpdater?.Postpone();

        public void ScanQrcode()
        {
            void Success(string link)
            {
                // no comment ^v^
                if (link == StrConst.Nobody3uVideoUrl)
                {
                    Misc.UI.VisitUrl(I18N.VisitWebPage, StrConst.Nobody3uVideoUrl);
                    return;
                }

                var msg = VgcApis.Misc.Utils.AutoEllipsis(link, VgcApis.Models.Consts.AutoEllipsis.QrcodeTextMaxLength);
                setting.SendLog($"QRCode: {msg}");
                slinkMgr.ImportLinkWithOutV2cfgLinks(link);
            }

            void Fail()
            {
                MessageBox.Show(I18N.NoQRCode);
            }

            Libs.QRCode.QRCode.ScanQRCode(Success, Fail);
        }

        public void Invoke(Action updater) => InvokeThen(updater, null);

        public void InvokeThen(Action updater, Action next) =>
            InvokeThenWorker(niMenuRoot, updater, next);

        /// <summary>
        /// null means delete menu
        /// </summary>
        /// <param name="pluginMenu"></param>
        public void UpdatePluginMenu(IEnumerable<ToolStripMenuItem> children)
        {
            Invoke(() =>
            {
                miPluginsRoot.DropDownItems.Clear();
                if (children == null || children.Count() < 1)
                {
                    miPluginsRoot.Visible = false;
                    return;
                }

                miPluginsRoot.DropDownItems.AddRange(children.ToArray());
                miPluginsRoot.Visible = true;
            });
        }

        #endregion

        #region private method
        static void LogContrlExAndCs(Control control, Exception ex)
        {
            var th = Thread.CurrentThread;

            VgcApis.Libs.Sys.FileLogger.Error(
                $"Invoke updater() error by control {control.Name}\n" +
                $"Current thread info: [{th.ManagedThreadId}] {th.Name}\n" +
                $"{ex}\n" +

                $"{VgcApis.Misc.Utils.GetCurCallStack()}");
        }

        static void InvokeThenWorker(
           Control control, Action updater, Action next)
        {
            Action worker = () =>
            {
                try
                {
                    updater?.Invoke();
                }
                catch (Exception ex)
                {
                    LogContrlExAndCs(control, ex);
                }
            };

            Action tail = () =>
            {
                if (next != null)
                {
                    VgcApis.Misc.Utils.RunInBackground(next);
                }
            };

            try
            {
                if (control == null || control.IsDisposed)
                {
                    tail();
                    return;
                }

                control.Invoke((MethodInvoker)delegate
                {
                    if (!VgcApis.Misc.UI.IsInUiThread())
                    {
                        VgcApis.Libs.Sys.FileLogger.DumpCallStack("!invoke error!");
                    }
                    worker();
                    tail();
                });
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch (Exception ex)
            {
                LogContrlExAndCs(control, ex);
            }
            tail();
        }

        private void BindNiMenuEvents(NotifyIcon ni)
        {
            Microsoft.Win32.SystemEvents.SessionSwitch += (s, a) =>
            {
                switch (a.Reason)
                {
                    case Microsoft.Win32.SessionSwitchReason.SessionLogoff:
                    case Microsoft.Win32.SessionSwitchReason.SessionLock:
                        // Invoke(DestroyHotKeyWindow);
                        setting.SetScreenLockingState(true);
                        break;
                    case Microsoft.Win32.SessionSwitchReason.SessionLogon:
                    case Microsoft.Win32.SessionSwitchReason.SessionUnlock:
                        // Invoke(CreateHotKeyWindow);
                        setting.SetScreenLockingState(false);
                        break;
                    default:
                        break;
                }
            };

            ni.ContextMenuStrip.Opening += (s, a) => isMenuOpened = true;
            ni.ContextMenuStrip.Closed += (s, a) => isMenuOpened = false;

            ni.MouseClick += (s, a) =>
            {
                switch (a.Button)
                {
                    case MouseButtons.Left:
                        // https://stackoverflow.com/questions/2208690/invoke-notifyicons-context-menu
                        // MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                        // mi.Invoke(ni, null);
                        Views.WinForms.FormMain.ShowForm();
                        break;
                }
            };
        }

        private void BindServerEvents()
        {
            //删除一个运行中的core时，core stop监听器会在core停止前移除，所以stop事件丢失
            servers.OnServerCountChange += UpdateNotifyIconHandler;
            servers.OnCoreStart += UpdateNotifyIconHandler;
            servers.OnCoreStop += UpdateNotifyIconHandler;
            servers.OnServerPropertyChange += UpdateNotifyIconHandler;
        }

        private void ReleaseServerEvents()
        {
            servers.OnServerCountChange -= UpdateNotifyIconHandler;
            servers.OnCoreStart -= UpdateNotifyIconHandler;
            servers.OnCoreStop -= UpdateNotifyIconHandler;
            servers.OnServerPropertyChange -= UpdateNotifyIconHandler;
        }

        List<ToolStripMenuItem> ServerList2MenuItems(
            IEnumerable<ICoreServCtrl> serverList)
        {
            var menuItems = new List<ToolStripMenuItem>();

            foreach (var serv in serverList)
            {
                menuItems.Add(CoreServ2MenuItem(serv));
            }

            return menuItems;
        }

        private ToolStripMenuItem CoreServ2MenuItem(ICoreServCtrl coreServ)
        {
            var coreState = coreServ.GetCoreStates();
            var name = coreState.GetLongName();
            var idx = ((int)coreState.GetIndex()).ToString();

            var title = $"{idx}.{name}";
            var dely = coreState.GetSpeedTestResult();
            if (dely == SpeedtestTimeout)
            {
                title = $"{title} - ({I18N.Timeout})";
            }
            else if (dely > 0)
            {
                title = $"{title} - ({dely}ms)";
            }

            Action done = () => coreServ.GetCoreCtrl().RestartCoreThen();
            Action onClick = () => servers.StopAllServersThen(done);
            var item = new ToolStripMenuItem(title, null, (s, a) => onClick());
            item.Checked = coreServ.GetCoreCtrl().IsCoreRunning();
            return item;
        }

        Dictionary<qsMenuNames, ToolStripMenuItem> CreateQsMenuCompos()
        {
            var qm = new Dictionary<qsMenuNames, ToolStripMenuItem>();

            qm[qsMenuNames.QuickSwitchMenuRoot] = new ToolStripMenuItem(
                I18N.QuickSwitch, Properties.Resources.SwitchSourceOrTarget_16x);

            qm[qsMenuNames.StopAllServer] = new ToolStripMenuItem(
                    I18N.StopAllServers,
                    Properties.Resources.Stop_16x,
                    (s, a) => servers.StopAllServersThen());

            qm[qsMenuNames.SwitchToRandomServer] = new ToolStripMenuItem(
                    I18N.SwitchToRandomServer,
                    Properties.Resources.FTPConnection_16x,
                    (s, a) => SwitchToRandomServer());

            qm[qsMenuNames.SwitchToRandomTlsServer] =
                new ToolStripMenuItem(
                    I18N.SwitchToRandomTlsserver,
                    Properties.Resources.SFTPConnection_16x,
                    (s, a) => SwitchRandomTlsServer());

            return qm;
        }

        void SwitchRandomTlsServer()
        {
            var latency = setting.QuickSwitchServerLantency;
            var list = servers.GetAllServersOrderByIndex()
                .Where(s =>
                {
                    var st = s.GetCoreStates();
                    var summary = st.GetSummary()?.ToLower();
                    var d = st.GetSpeedTestResult();
                    return
                        !string.IsNullOrEmpty(summary)
                        && summary.Contains(@".tls@")
                        && (latency <= 0 || (d > 0 && d <= latency));
                })
                .ToList();
            StartRandomServerInList(list);
        }

        void SwitchToRandomServer()
        {
            var latency = setting.QuickSwitchServerLantency;
            var list = servers.GetAllServersOrderByIndex()
                .Where(s =>
                {
                    var d = s.GetCoreStates().GetSpeedTestResult();
                    return latency <= 0 || (d > 0 && d <= latency);
                })
                .ToList();
            StartRandomServerInList(list);
        }

        private void StartRandomServerInList(List<ICoreServCtrl> list)
        {
            var len = list.Count;
            if (len <= 0)
            {
                VgcApis.Misc.UI.MsgBoxAsync(I18N.NoServerAvailable);
                return;
            }

            var picked = list[new Random().Next(len)].GetCoreCtrl();
            servers.StopAllServersThen(() => picked.RestartCore());
        }

        void ReplaceServersMenuWith(
            bool isGrouped,
            List<ToolStripMenuItem> miGroupedServers,
            List<ToolStripMenuItem> miTopNthServers)
        {
            var root = miServersRoot.DropDownItems;
            root.Clear();

            var count = miGroupedServers.Count;
            if (count < 1)
            {
                miServersRoot.Visible = false;
                return;
            }

            List<ToolStripItem> mis = new List<ToolStripItem>();
            mis.Add(qsMenuCompos[qsMenuNames.StopAllServer]);
            if (isGrouped)
            {
                var qs = qsMenuCompos[qsMenuNames.QuickSwitchMenuRoot];
                var items = qs.DropDownItems;
                items.Clear();
                items.Add(qsMenuCompos[qsMenuNames.SwitchToRandomServer]);
                items.Add(qsMenuCompos[qsMenuNames.SwitchToRandomTlsServer]);
                items.Add(new ToolStripSeparator());
                items.AddRange(miTopNthServers.ToArray());
                mis.Add(qs);
            }
            else
            {
                mis.Add(qsMenuCompos[qsMenuNames.SwitchToRandomServer]);
                mis.Add(qsMenuCompos[qsMenuNames.SwitchToRandomTlsServer]);
            }
            mis.Add(new ToolStripSeparator());

            root.AddRange(mis.ToArray());
            root.AddRange(miGroupedServers.ToArray());
            miServersRoot.Visible = true;
        }

        void UpdateServersMenuThen(Action done = null)
        {
            try
            {
                var serverList = servers.GetAllServersOrderByIndex();
                var miAllServers = ServerList2MenuItems(serverList);

                var num = VgcApis.Models.Consts.Config.QuickSwitchMenuItemNum;
                var groupSize = VgcApis.Models.Consts.Config.MenuItemGroupSize;
                var isGrouped = miAllServers.Count > groupSize;

                var miGroupedServers = VgcApis.Misc.UI.AutoGroupMenuItems(miAllServers, groupSize);
                var miTopNthServers = isGrouped ?
                    ServerList2MenuItems(serverList.Take(num).ToList()) :
                    new List<ToolStripMenuItem>();

                InvokeThen(
                    () => ReplaceServersMenuWith(isGrouped, miGroupedServers, miTopNthServers),
                    done);
            }
            catch (Exception e)
            {
                VgcApis.Libs.Sys.FileLogger.Error($"Notifier.UpdateServersMenuThen() \n {e}");
                done?.Invoke();
            }
        }

        void UpdateNotifyIconWorker(Action done)
        {
            if (setting.IsClosing())
            {
                done();
                return;
            }

            if (isMenuOpened || setting.IsScreenLocked())
            {
                VgcApis.Misc.Utils.RunInBackground(() =>
                {
                    VgcApis.Misc.Utils.Sleep(1000);
                    done();
                    VgcApis.Misc.Utils.Sleep(1000);
                    lazyNotifierMenuUpdater?.Postpone();
                    // VgcApis.Libs.Sys.FileLogger.Info("Notifier.UpdateNotifyIconWorker() menu is opened update later");
                });
                return;
            }

            var start = DateTime.Now.Millisecond;

            Action finished = () => VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var relex = UpdateInterval - (DateTime.Now.Millisecond - start);
                VgcApis.Misc.Utils.Sleep(Math.Max(0, relex));
                done();
            });

            Action next = () => UpdateServersMenuThen(finished);

            try
            {
                var list = servers.GetAllServersOrderByIndex()
                    .Where(s => s.GetCoreCtrl().IsCoreRunning())
                    .ToList();
                var icon = CreateNotifyIconImage(list);

                InvokeThen(() =>
                {
                    if (icon != null)
                    {
                        var org = ni.Icon;
                        ni.Icon = Icon.FromHandle(icon.GetHicon());
                        org?.Dispose();
                    }
                }, null);
                UpdateNotifyIconTextThen(list, next);
            }
            catch (Exception e)
            {
                VgcApis.Libs.Sys.FileLogger.Error($"Notifier update icon error!\n{e}");
            }
            done();
        }

        private Bitmap CreateNotifyIconImage(List<ICoreServCtrl> coreCtrls)
        {
            var activeServNum = coreCtrls.Count;
            var isFirstServ = false;

            if (activeServNum == 1)
            {
                var idx = coreCtrls.First().GetCoreStates().GetIndex();
                if ((int)idx == 1)
                {
                    isFirstServ = true;
                }
            }

            var icon = orgIcon.Clone() as Bitmap;
            var size = icon.Size;

            using (Graphics g = Graphics.FromImage(icon))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;

                DrawProxyModeCornerCircle(g, size);
                DrawIsRunningCornerMark(g, size, activeServNum, isFirstServ);
            }

            return icon;
        }

        void DrawProxyModeCornerCircle(
            Graphics graphics, Size size)
        {
            Brush br;

            switch (ProxySetter.Libs.Sys.WinInet.GetProxySettings().proxyMode)
            {
                case (int)ProxySetter.Libs.Sys.WinInet.ProxyModes.PAC:
                    br = Brushes.DeepPink;
                    break;
                case (int)ProxySetter.Libs.Sys.WinInet.ProxyModes.Proxy:
                    br = Brushes.Blue;
                    break;
                default:
                    br = Brushes.ForestGreen;
                    break;
            }

            var w = size.Width;
            var x = w * 0.4f;
            var s = w * 0.6f;
            graphics.FillEllipse(br, x, x, s, s);
        }

        private void DrawIsRunningCornerMark(
            Graphics graphics, Size size, int activeServNum, bool isFirstServ)
        {
            var w = size.Width;
            var cx = w * 0.7f;

            switch (activeServNum)
            {
                case 0:
                    DrawOneLine(graphics, w, cx, false);
                    break;
                case 1:
                    DrawTriangle(graphics, w, cx, isFirstServ);
                    break;
                default:
                    DrawOneLine(graphics, w, cx, false);
                    DrawOneLine(graphics, w, cx, true);
                    break;
            }
        }

        private static void DrawTriangle(
            Graphics graphics, int w, float cx, bool isFirstServ)
        {
            var lw = w * 0.07f;
            var cr = w * 0.22f;
            if (isFirstServ)
            {
                cr -= lw / 2;
            }
            var dh = Math.Sqrt(3) * cr / 2f;
            var tri = new Point[] {
                    new Point((int)(cx - cr / 2f),(int)(cx - dh)),
                    new Point((int)(cx + cr),(int)cx),
                    new Point((int)(cx - cr / 2f),(int)(cx + dh)),
                };

            if (!isFirstServ)
            {
                graphics.FillPolygon(Brushes.White, tri);
            }
            else
            {
                var pen = new Pen(Brushes.White, lw);
                graphics.DrawPolygon(pen, tri);
            }

        }

        private static void DrawOneLine(
            Graphics graphics, int w, float cx, bool isVertical)
        {

            var rw = w * 0.44f;
            var rh = w * 0.14f;

            if (isVertical)
            {
                var swp = rw;
                rw = rh;
                rh = swp;
            }

            var rect = new Rectangle((int)(cx - rw / 2f), (int)(cx - rh / 2f), (int)rw, (int)rh);
            graphics.FillRectangle(Brushes.White, rect);
        }

        void UpdateNotifyIconHandler(object sender, EventArgs args) =>
            RefreshNotifyIconLater();

        string GetterSysProxyInfo()
        {
            var proxySetting = ProxySetter.Libs.Sys.ProxySetter.GetProxySetting();

            switch (proxySetting.proxyMode)
            {
                case (int)ProxySetter.Libs.Sys.WinInet.ProxyModes.PAC:
                    return proxySetting.pacUrl;
                case (int)ProxySetter.Libs.Sys.WinInet.ProxyModes.Proxy:
                    return proxySetting.proxyAddr;
            }
            return null;
        }

        void UpdateNotifyIconTextThen(List<ICoreServCtrl> list, Action finished)
        {
            var count = list.Count;
            var texts = new List<string>();

            void done()
            {
                try
                {
                    var sysProxyInfo = GetterSysProxyInfo();
                    if (!string.IsNullOrEmpty(sysProxyInfo))
                    {
                        int len = VgcApis.Models.Consts.AutoEllipsis.NotifierSysProxyInfoMaxLength;
                        texts.Add(I18N.CurSysProxy + VgcApis.Misc.Utils.AutoEllipsis(sysProxyInfo, len));
                    }
                    SetNotifyText(string.Join(Environment.NewLine, texts));
                }
                catch { }
                finished?.Invoke();
            }

            void worker(int index, Action next)
            {
                try
                {
                    list[index].GetConfiger().GetterInfoForNotifyIconf(s =>
                    {
                        texts.Add(s);
                        next?.Invoke();
                    });
                    return;
                }
                catch { }
                next?.Invoke();
            }

            if (count <= 0 || count > 2)
            {
                var t = count <= 0 ? I18N.Description : count.ToString() + I18N.ServersAreRunning;
                texts.Add(t);
                done();
                return;
            }

            Misc.Utils.ChainActionHelperAsync(count, worker, done);
        }

        private void SetNotifyText(string rawText)
        {
            var text = string.IsNullOrEmpty(rawText) ?
                I18N.Description :
                VgcApis.Misc.Utils.AutoEllipsis(rawText, VgcApis.Models.Consts.AutoEllipsis.NotifierTextMaxLength);

            if (ni.Text == text)
            {
                return;
            }

            // https://stackoverflow.com/questions/579665/how-can-i-show-a-systray-tooltip-longer-than-63-chars
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(ni, text);
            if ((bool)t.GetField("added", hidden).GetValue(ni))
                t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
        }

        void CreateContextMenuStrip(
            ContextMenuStrip menu,
            ToolStripMenuItem serversRootMenuItem,
            ToolStripMenuItem pluginRootMenuItem)
        {
            var factor = Misc.UI.GetScreenScalingFactor();
            if (factor > 1)
            {
                menu.ImageScalingSize = new Size(
                    (int)(menu.ImageScalingSize.Width * factor),
                    (int)(menu.ImageScalingSize.Height * factor));
            }

            menu.Items.AddRange(new ToolStripMenuItem[] {
                new ToolStripMenuItem(
                    I18N.Windows,
                    Properties.Resources.CPPWin32Project_16x,
                    new ToolStripMenuItem[]{
                        new ToolStripMenuItem(
                            I18N.MainWin,
                            Properties.Resources.WindowsForm_16x,
                            (s,a)=>Views.WinForms.FormMain.ShowForm()),
                        new ToolStripMenuItem(
                            I18N.ConfigEditor,
                            Properties.Resources.EditWindow_16x,
                            (s,a)=>Views.WinForms.FormConfiger.ShowConfig()),
                        new ToolStripMenuItem(
                            I18N.GenQRCode,
                            Properties.Resources.AzureVirtualMachineExtension_16x,
                            (s,a)=>Views.WinForms.FormQRCode.ShowForm()),
                        new ToolStripMenuItem(
                            I18N.Log,
                            Properties.Resources.FSInteractiveWindow_16x,
                            (s,a)=> Views.WinForms.FormLog.ShowForm() ),
                         new ToolStripMenuItem(
                            I18N.DownloadV2rayCore,
                            Properties.Resources.ASX_TransferDownload_blue_16x,
                            (s,a)=>Views.WinForms.FormDownloadCore.ShowForm()),
                    }),

                serversRootMenuItem,

                pluginRootMenuItem,

                new ToolStripMenuItem(
                    I18N.ScanQRCode,
                    Properties.Resources.ExpandScope_16x,
                    (s,a)=> ScanQrcode()),

                new ToolStripMenuItem(
                    I18N.ImportLink,
                    Properties.Resources.CopyLongTextToClipboard_16x,
                    (s,a)=>{
                        string links = Misc.Utils.GetClipboardText();
                        slinkMgr.ImportLinkWithOutV2cfgLinks(links);
                    }),

                new ToolStripMenuItem(
                        I18N.Options,
                        Properties.Resources.Settings_16x,
                        (s,a)=>Invoke(()=> Views.WinForms.FormOption.ShowForm())),
            });

            menu.Items.Add(new ToolStripSeparator());

            menu.Items.AddRange(
                new ToolStripMenuItem[] {
                    CreateAboutMenu(),

                    new ToolStripMenuItem(
                        I18N.Exit,
                        Properties.Resources.CloseSolution_16x,
                        (s, a) => {
                            if (Misc.UI.Confirm(I18N.ConfirmExitApp))
                            {
                                setting.ShutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser;
                                Application.Exit();
                            }
                        }),
                });
        }

        private ToolStripMenuItem CreateRootMenuItem(string title, Bitmap icon)
        {
            var mi = new ToolStripMenuItem(title, icon);
            mi.Visible = false;
            return mi;
        }

        ToolStripMenuItem CreateAboutMenu()
        {
            var aboutMenu = new ToolStripMenuItem(
                I18N.About,
                Properties.Resources.StatusHelp_16x);

            var children = aboutMenu.DropDownItems;

            children.Add(
                I18N.ProjectPage,
                null,
                (s, a) => Misc.UI.VisitUrl(I18N.VistProjectPage, Properties.Resources.ProjectLink));

            children.Add(
               I18N.CheckForUpdate,
               null,
                (s, a) => updater.CheckForUpdate(true));

            children.Add(
                I18N.Feedback,
                null,
                (s, a) => Misc.UI.VisitUrl(
                    I18N.VisitVGCIssuePage, Properties.Resources.IssueLink));

            children.Add(
                I18N.ProjectWiki,
                null,
                (s, a) => Misc.UI.VisitUrl(
                    I18N.VistWikiPage, Properties.Resources.WikiLink));

            return aboutMenu;
        }


        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            lazyNotifierMenuUpdater?.Dispose();
            DestroyHotKeyWindow();
            ReleaseServerEvents();
            ni.Visible = false;
        }
        #endregion
    }
}
