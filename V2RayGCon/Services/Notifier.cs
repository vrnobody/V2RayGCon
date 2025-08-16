﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using VgcApis.Interfaces;

namespace V2RayGCon.Services
{
    public class Notifier
        : BaseClasses.SingletonService<Notifier>,
            VgcApis.Interfaces.Services.INotifierService
    {
        Settings setting;
        Servers servers;
        ShareLinkMgr slinkMgr;
        Updater updater;
        readonly NotifyIcon ni;

        static readonly long SpeedtestTimeout = VgcApis.Models.Consts.Core.SpeedtestTimeout;
        static readonly int UpdateInterval = VgcApis
            .Models
            .Consts
            .Intervals
            .NotifierMenuUpdateIntreval;
        readonly Bitmap orgIcon,
            tunIcon;

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
                BalloonTipTitle = Misc.Utils.GetAppNameAndVer(),
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true,
            };

            orgIcon = ni.Icon.ToBitmap();
            tunIcon = VgcApis.Misc.UI.GetTunModeIcon().ToBitmap();

            niMenuRoot = ni.ContextMenuStrip;
            niMenuRoot.CreateControl();
            var handle = niMenuRoot.Handle;
            VgcApis.Libs.Sys.FileLogger.Info($"Create notify icon {handle}");

            CreateContextMenuStrip(niMenuRoot, miServersRoot, miPluginsRoot);

            VgcApis.Misc.UI.BindInvokeThenFunc(InvokeThen);
        }

        public void Run(
            Settings setting,
            Servers servers,
            ShareLinkMgr shareLinkMgr,
            Updater updater
        )
        {
            this.setting = setting;
            this.servers = servers;
            this.slinkMgr = shareLinkMgr;
            this.updater = updater;

            lazyNotifierMenuUpdater = new VgcApis.Libs.Tasks.LazyGuy(
                UpdateNotifyIconWorker,
                UpdateInterval,
                5000
            )
            {
                Name = "Notifier.MenuUpdater", // disable debug logging
            };

            BindNiMenuEvents(ni);
            BindServerEvents();
            RefreshNotifyIconLater();

            OnNotifyIconMenuOpeningHandler(this, EventArgs.Empty);
        }

        #region hotkey window

        VgcApis.WinForms.HotKeyWindow hkWindow = null;
        readonly ConcurrentDictionary<string, VgcApis.Models.Datas.HotKeyContext> hkContexts =
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
            Action hotKeyHandler,
            string keyName,
            bool hasAlt,
            bool hasCtrl,
            bool hasShift
        )
        {
            if (hkWindow == null)
            {
                CreateHotKeyWindow();
            }

            var context = VgcApis.Models.Datas.HotKeyContext.Create(
                hotKeyHandler,
                keyName,
                hasAlt,
                hasCtrl,
                hasShift
            );

            if (context == null)
            {
                return null;
            }

            context.evCode = currentEvCode++;
            var keyMsg = context.KeyMessage;
            var handle = keyMsg.ToString();
            if (hkContexts.ContainsKey(handle) || !hkWindow.RegisterHotKey(context))
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
                if (
                    !string.IsNullOrEmpty(handle)
                    && hkContexts.TryRemove(handle, out var context)
                    && hkWindow != null
                )
                {
                    return hkWindow.UnregisterHotKey(context);
                }
            }
            catch { }
            return false;
        }

        public string RegisterHotKey(
            Action hotKeyHandler,
            string keyName,
            bool hasAlt,
            bool hasCtrl,
            bool hasShift
        )
        {
            string handle = null;
            Invoke(() =>
            {
                try
                {
                    handle = RegisterHotKeyWorker(
                        hotKeyHandler,
                        keyName,
                        hasAlt,
                        hasCtrl,
                        hasShift
                    );
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
        public void ShowFormTextEditor(string config)
        {
            Views.WinForms.FormTextConfigEditor.ShowConfig(string.Empty, config, false);
        }

        public void ShowFormServerSettings(ICoreServCtrl coreServ)
        {
            if (coreServ == null)
            {
                VgcApis.Misc.UI.MsgBox(I18N.NullParamError);
                return;
            }
            Views.WinForms.FormModifyServerSettings.ShowForm(coreServ);
        }

        public void ShowFormSimpleEditor(ICoreServCtrl coreServ)
        {
            // backward compact for Luna
        }

        public void ShowFormOption() => Views.WinForms.FormOption.ShowForm();

        public void ShowFormWebUI() => Views.WinForms.FormWebUI.ShowForm();

        public void ShowFormMain() => Views.WinForms.FormMain.ShowForm();

        public void ShowFormLog() => Views.WinForms.FormLog.ShowForm();

        #endregion

        #region public method


        public void SetNotifyIconTag(string tag)
        {
            if (niTag == tag)
            {
                return;
            }
            niTag = tag;
            requiredRedrawIcon = true;
            RefreshNotifyIconLater();
        }

        public void Notify(string title, string content, int timeout)
        {
            Invoke(() => ni.ShowBalloonTip(timeout, title, content, ToolTipIcon.None));
        }

        public void RefreshNotifyIconLater() => lazyNotifierMenuUpdater?.Deadline();

        public void ScanQrcode()
        {
            void Success(string link)
            {
                // no comment ^v^
                var video = VgcApis.Models.Consts.Webs.Nobody3uVideoUrl;
                if (link == video)
                {
                    VgcApis.Misc.UI.VisitUrl(I18N.VisitWebPage, video);
                    return;
                }

                var msg = VgcApis.Misc.Utils.AutoEllipsis(
                    link,
                    VgcApis.Models.Consts.AutoEllipsis.QrcodeTextMaxLength
                );
                VgcApis.Misc.Logger.Log($"QRCode: {msg}");
                slinkMgr.ImportLinkWithOutV2cfgLinks(link);
            }

            void Fail()
            {
                VgcApis.Misc.UI.MsgBox(I18N.NoQRCode);
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
                miPluginsRoot.DropDown.PerformLayout();
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
                $"Invoke updater() error by control {control.Name}\n"
                    + $"Current thread info: [{th.ManagedThreadId}] {th.Name}\n"
                    + $"{ex}\n"
#if DEBUG
                    + $"{VgcApis.Misc.Utils.GetCurCallStack()}"
#endif

            );
        }

        static void InvokeThenWorker(Control control, Action updater, Action next)
        {
            void worker()
            {
                try
                {
                    updater?.Invoke();
                }
                catch (Exception ex)
                {
                    LogContrlExAndCs(control, ex);
                }
            }

            void tail()
            {
                if (next != null)
                {
                    VgcApis.Misc.Utils.RunInBackground(next);
                }
            }

            try
            {
                if (control == null || control.IsDisposed)
                {
                    tail();
                    return;
                }

                control.Invoke(
                    (MethodInvoker)
                        delegate
                        {
#if DEBUG
                            if (!VgcApis.Misc.UI.IsInUiThread())
                            {
                                VgcApis.Libs.Sys.FileLogger.DumpCallStack("!invoke error!");
                            }
#endif
                            worker();
                            tail();
                        }
                );
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

            ni.MouseClick += (s, a) =>
            {
                switch (a.Button)
                {
                    case MouseButtons.Left:
                        // https://stackoverflow.com/questions/2208690/invoke-notifyicons-context-menu
                        // MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                        // mi.Invoke(ni, null);

                        if (setting.isEnableSystrayLeftClickCommand)
                        {
                            try
                            {
                                var cmd = setting.SystrayLeftClickCommand;
                                if (Regex.IsMatch(cmd, VgcApis.Models.Consts.Patterns.VgcWebUiUrl))
                                {
                                    Views.WinForms.FormWebUI.ShowForm();
                                }
                                else
                                {
                                    System.Diagnostics.Process.Start(cmd);
                                }
                            }
                            catch (Exception err)
                            {
                                VgcApis.Misc.UI.MsgBox(err.ToString());
                            }
                        }
                        else
                        {
                            Views.WinForms.FormMain.ShowForm();
                        }
                        break;
                }
            };

            ni.ContextMenuStrip.Opening += OnNotifyIconMenuOpeningHandler;
            ni.ContextMenuStrip.Closed += OnNotifyIconMenuClosedHandler;
        }

        void OnNotifyIconMenuClosedHandler(object sender, EventArgs args)
        {
            var menus = new List<ToolStripMenuItem>()
            {
                qsMenuCompos[qsMenuNames.QuickSwitchMenuRoot],
                miServersRoot,
            };

            foreach (var menu in menus)
            {
                menu.DropDownItems.Clear();
            }
        }

        void OnNotifyIconMenuOpeningHandler(object sender, EventArgs args)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                VgcApis.Misc.Utils.Sleep(10);
                UpdateServersMenuThen();
            });
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

        List<ToolStripMenuItem> ServerList2MenuItems(IEnumerable<ICoreServCtrl> serverList)
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
            var title = coreState.GetTitle();
            var uid = coreState.GetUid();
            var delay = coreState.GetSpeedTestResult();
            if (delay == SpeedtestTimeout)
            {
                title = $"{title} - ({I18N.Timeout})";
            }
            else if (delay > 0)
            {
                title = $"{title} - ({delay}ms)";
            }

            void onClick(object s, EventArgs a) => servers.RestartOneServerByUid(uid);
            var item = new ToolStripMenuItem(title, null, onClick)
            {
                Checked = coreServ.GetCoreCtrl().IsCoreRunning(),
            };
            item.Disposed += (s, a) => item.Click -= onClick;
            return item;
        }

        Dictionary<qsMenuNames, ToolStripMenuItem> CreateQsMenuCompos()
        {
            var qm = new Dictionary<qsMenuNames, ToolStripMenuItem>
            {
                [qsMenuNames.QuickSwitchMenuRoot] = new ToolStripMenuItem(
                    I18N.QuickSwitch,
                    Properties.Resources.SwitchSourceOrTarget_16x
                ),
                [qsMenuNames.StopAllServer] = new ToolStripMenuItem(
                    I18N.StopAllServers,
                    Properties.Resources.Stop_16x,
                    (s, a) => servers.StopAllServersThen()
                ),
                [qsMenuNames.SwitchToRandomServer] = new ToolStripMenuItem(
                    I18N.SwitchToRandomServer,
                    Properties.Resources.FTPConnection_16x,
                    (s, a) => SwitchToRandomServer()
                ),
                [qsMenuNames.SwitchToRandomTlsServer] = new ToolStripMenuItem(
                    I18N.SwitchToRandomTlsserver,
                    Properties.Resources.SFTPConnection_16x,
                    (s, a) => SwitchRandomTlsServer()
                ),
            };

            return qm;
        }

        void SwitchRandomTlsServer()
        {
            var latency = setting.QuickSwitchServerLantency;
            var list = servers
                .GetAllServersOrderByIndex()
                .Where(s =>
                {
                    var st = s.GetCoreStates();
                    var summary = st.GetSummary()?.ToLower();
                    var d = st.GetSpeedTestResult();
                    return !string.IsNullOrEmpty(summary)
                        && summary.Contains(@".tls@")
                        && (latency <= 0 || (d > 0 && d <= latency));
                })
                .ToList();
            StartRandomServerInList(list);
        }

        void SwitchToRandomServer()
        {
            var latency = setting.QuickSwitchServerLantency;
            var list = servers
                .GetAllServersOrderByIndex()
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

            var picked = list[VgcApis.Libs.Infr.PseudoRandom.Next(len)].GetCoreCtrl();
            servers.StopAllServersThen(() => picked.RestartCore());
        }

        void ReplaceServersMenuWith(
            List<ToolStripMenuItem> miGroupedServers,
            List<ToolStripMenuItem> miTopNthServers
        )
        {
            var root = miServersRoot.DropDownItems;
            root.Clear();
            miServersRoot.DropDown.PerformLayout();

            var count = miGroupedServers.Count;
            if (count < 1)
            {
                miServersRoot.Visible = false;
                return;
            }

            List<ToolStripItem> mis = new List<ToolStripItem>
            {
                qsMenuCompos[qsMenuNames.StopAllServer],
            };

            if (miTopNthServers != null && miTopNthServers.Count > 0)
            {
                var qs = qsMenuCompos[qsMenuNames.QuickSwitchMenuRoot];
                var items = qs.DropDownItems;
                items.Clear();
                qs.DropDown.PerformLayout();
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

        readonly VgcApis.Libs.Tasks.Bar serversMenuUpdateBar = new VgcApis.Libs.Tasks.Bar();

        void UpdateServersMenuThen()
        {
            if (!serversMenuUpdateBar.Install())
            {
                return;
            }

            void done() => serversMenuUpdateBar.Remove();

            var serverList = servers.GetAllServersOrderByIndex();
            var num = VgcApis.Models.Consts.Config.QuickSwitchMenuItemNum;
            var groupSize = VgcApis.Models.Consts.Config.MenuItemGroupSize;

            InvokeThen(
                () =>
                {
                    List<ToolStripMenuItem> miGroupedServers = null;
                    if (serverList.Count < VgcApis.Models.Consts.Config.MinDynamicMenuSize)
                    {
                        var miAllServers = ServerList2MenuItems(serverList);
                        miGroupedServers = VgcApis.Misc.UI.AutoGroupMenuItems(
                            miAllServers,
                            groupSize
                        );
                    }
                    else
                    {
                        miGroupedServers = CreateDynamicServerMenus(groupSize, 0, serverList.Count);
                    }

                    var miTopNthServers =
                        serverList.Count > groupSize
                            ? ServerList2MenuItems(serverList.Take(num).ToList())
                            : new List<ToolStripMenuItem>();

                    ReplaceServersMenuWith(miGroupedServers, miTopNthServers);
                },
                done
            );
        }

        List<ToolStripMenuItem> CreateDynamicServerMenus(int groupSize, int start, int end)
        {
            var n = end - start;
            var step = 1;
            while (n > groupSize)
            {
                n /= groupSize;
                step *= groupSize;
            }

            if (step == 1)
            {
                var cs = new List<ICoreServCtrl>();
                var servs = servers.GetAllServersOrderByIndex();
                var count = 0;
                foreach (var serv in servs)
                {
                    if (count >= start && count < end)
                    {
                        cs.Add(serv);
                    }
                    count++;
                }
                return ServerList2MenuItems(cs);
            }

            var gmis = new List<ToolStripMenuItem>();
            for (int i = start; i < end; i += step)
            {
                var s = i;
                var e = Math.Min(i + step, end);
                var text = string.Format("{0,5:D5} - {1,5:D5}", s + 1, e);
                var mi = new ToolStripMenuItem(text, null);
                mi.DropDownOpening += (o, a) =>
                {
                    var root = mi.DropDownItems;
                    if (root.Count < 1)
                    {
                        var dm = CreateDynamicServerMenus(groupSize, s, e);
                        root.AddRange(dm.ToArray());
                    }
                };
                gmis.Add(mi);
            }

            return gmis;
        }

        enum SysTrayIconTypes
        {
            None = 0,
            ProxyDirect = 1,
            ProxyPac = 1 << 1,
            ProxyGlobal = 1 << 2,
            NoServRunning = 1 << 3,
            FirstServRunning = 1 << 4,
            OthServRunning = 1 << 5,
            MultiServRunning = 1 << 6,
        }

        SysTrayIconTypes curSysTrayIconType = SysTrayIconTypes.None;
        bool curTunMode = false;
        bool requiredRedrawIcon = false;
        string niTag = "";

        void UpdateNotifyIconWorker(Action done)
        {
            if (setting.IsClosing())
            {
                done();
                return;
            }

            if (setting.IsScreenLocked())
            {
                VgcApis.Misc.Utils.RunInBackground(() =>
                {
                    VgcApis.Misc.Utils.Sleep(1000);
                    done();
                    VgcApis.Misc.Utils.Sleep(1000);
                    lazyNotifierMenuUpdater?.Postpone();
                });
                return;
            }

            var start = DateTime.Now.Millisecond;
            void finished() =>
                VgcApis.Misc.Utils.RunInBackground(() =>
                {
                    var relex = UpdateInterval - (DateTime.Now.Millisecond - start);
                    VgcApis.Misc.Utils.Sleep(Math.Max(0, relex));
                    done();
                });

            try
            {
                var coreServs = servers.GetRunningServers().OrderBy(cs => cs).ToList();

                var iconType = AnalyzeSysTrayIconType(coreServs);
                var tunMode = setting.isTunMode;

                if (iconType != curSysTrayIconType || curTunMode != tunMode || requiredRedrawIcon)
                {
                    requiredRedrawIcon = false;
                    Invoke(() =>
                    {
                        var icon = CreateNotifyIconImage(iconType);
                        var org = ni.Icon;
                        ni.Icon = Icon.FromHandle(icon.GetHicon());
                        org?.Dispose();
                    });
                    curSysTrayIconType = iconType;
                    curTunMode = tunMode;
                }
                UpdateNotifyIconTextThen(coreServs, finished);
            }
            catch (Exception e)
            {
                VgcApis.Libs.Sys.FileLogger.Error($"Notifier update icon error!\n{e}");
                done();
            }
        }

        SysTrayIconTypes AnalyzeSysTrayIconType(List<ICoreServCtrl> coreCtrls)
        {
            var r = SysTrayIconTypes.None;

            // corner mark
            switch (coreCtrls.Count)
            {
                case 0:
                    r |= SysTrayIconTypes.NoServRunning;
                    break;
                case 1:
                    var idx = coreCtrls.First().GetCoreStates().GetIndex();
                    r |= (
                        idx == 1
                            ? SysTrayIconTypes.FirstServRunning
                            : SysTrayIconTypes.OthServRunning
                    );
                    break;
                default:
                    r |= SysTrayIconTypes.MultiServRunning;
                    break;
            }

            // proxy type
            switch (ProxySetter.Libs.Sys.WinInet.GetProxySettings().proxyMode)
            {
                case (int)ProxySetter.Libs.Sys.WinInet.ProxyModes.PAC:
                    r |= SysTrayIconTypes.ProxyPac;
                    break;
                case (int)ProxySetter.Libs.Sys.WinInet.ProxyModes.Proxy:
                    r |= SysTrayIconTypes.ProxyGlobal;
                    break;
                default:
                    r |= SysTrayIconTypes.ProxyDirect;
                    break;
            }

            return r;
        }

        #region write tag to systry icon
        private Font FindFont(Graphics g, string longString, Size Room, Font PreferedFont)
        {
            // https://stackoverflow.com/questions/19674743/dynamically-resizing-font-to-fit-space-while-using-graphics-drawstring
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;

            float ScaleRatio =
                (HeightScaleRatio < WidthScaleRatio) ? HeightScaleRatio : WidthScaleRatio;

            float ScaleFontSize = PreferedFont.Size * ScaleRatio;

            return new Font(PreferedFont.FontFamily, ScaleFontSize);
        }

        void DrawTag(Graphics g, Bitmap bmp)
        {
            var tag = string.IsNullOrEmpty(niTag)
                ? VgcApis.Misc.Utils.GetAppTagFirstChar()
                : niTag.Substring(0, 1);
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }
            // SimHei Microsoft YaHei
            using (Font font1 = new Font("Microsoft YaHei", 32, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                var rect = CalcDrawTagRect(bmp.Size);
                StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near,
                };

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                Font goodFont = FindFont(g, tag, rect.Size.ToSize(), font1);
                g.DrawString(tag, goodFont, Brushes.White, rect, stringFormat);
            }
        }

        RectangleF CalcDrawTagRect(Size size)
        {
            var w = size.Width * 0.75f;
            var h = size.Height * 0.75f;
            return new RectangleF(0f, 0f, w, h);
        }

        #endregion

        private Bitmap CreateNotifyIconImage(SysTrayIconTypes iconType)
        {
            var cache = setting.isTunMode ? tunIcon : orgIcon;
            var icon = new Bitmap(cache);
            var size = icon.Size;

            using (Graphics g = Graphics.FromImage(icon))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;

                DrawProxyModeCornerCircle(g, size, iconType);
                DrawIsRunningCornerMark(g, size, iconType);
                DrawTag(g, icon);
            }

            return icon;
        }

        void DrawProxyModeCornerCircle(Graphics graphics, Size size, SysTrayIconTypes iconType)
        {
            Brush br = Brushes.ForestGreen;

            if (0 != (iconType & SysTrayIconTypes.ProxyPac))
            {
                br = Brushes.DeepPink;
            }
            else if (0 != (iconType & SysTrayIconTypes.ProxyGlobal))
            {
                br = Brushes.Blue;
            }

            var w = size.Width;
            var x = w * 0.4f;
            var s = w * 0.6f;
            graphics.FillEllipse(br, x, x, s, s);
        }

        private void DrawIsRunningCornerMark(
            Graphics graphics,
            Size size,
            SysTrayIconTypes iconType
        )
        {
            var w = size.Width;
            var cx = w * 0.7f;

            if (0 != (iconType & SysTrayIconTypes.FirstServRunning))
            {
                DrawTriangle(graphics, w, cx, true);
            }
            else if (0 != (iconType & SysTrayIconTypes.OthServRunning))
            {
                DrawTriangle(graphics, w, cx, false);
            }
            else if (0 != (iconType & SysTrayIconTypes.MultiServRunning))
            {
                DrawOneLine(graphics, w, cx, false);
                DrawOneLine(graphics, w, cx, true);
            }
            else
            {
                DrawOneLine(graphics, w, cx, false);
            }
        }

        private static void DrawTriangle(Graphics graphics, int w, float cx, bool isFirstServ)
        {
            var lw = w * 0.07f;
            var cr = w * 0.22f;

            if (isFirstServ)
            {
                cr -= lw / 2;
            }

            var dh = Math.Sqrt(3) * cr / 2f;
            var tri = new Point[]
            {
                new Point((int)(cx - cr / 2f), (int)(cx - dh)),
                new Point((int)(cx + cr), (int)cx),
                new Point((int)(cx - cr / 2f), (int)(cx + dh)),
            };

            if (!isFirstServ)
            {
                graphics.FillPolygon(Brushes.White, tri);
                return;
            }

            var pen = new Pen(Brushes.White, lw)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
            };
            graphics.DrawPolygon(pen, tri);
        }

        private static void DrawOneLine(Graphics graphics, int w, float cx, bool isVertical)
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

        void UpdateNotifyIconHandler(object sender, EventArgs args) => RefreshNotifyIconLater();

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

        void UpdateNotifyIconTextThen(List<ICoreServCtrl> coreServs, Action finished)
        {
            var count = coreServs.Count;

            var texts = new List<string>
            {
                $"{Misc.Utils.GetAppNameAndVer()} - {I18N.Servers}: {servers.Count()}",
            };

            if (curTunMode)
            {
                texts.Add(I18N.TunModeEanbled);
            }

            void done()
            {
                try
                {
                    var sysProxyInfo = GetterSysProxyInfo();
                    if (!string.IsNullOrEmpty(sysProxyInfo))
                    {
                        var maxLen = VgcApis
                            .Models
                            .Consts
                            .AutoEllipsis
                            .NotifierSysProxyInfoMaxLength;
                        texts.Add(
                            I18N.CurSysProxy + VgcApis.Misc.Utils.AutoEllipsis(sysProxyInfo, maxLen)
                        );
                    }
                    SetNotifyIconText(string.Join(Environment.NewLine, texts));
                }
                catch { }
                finished?.Invoke();
            }

            var textLen = texts.Select(t => t.Length).Sum();
            var niTextMaxLen = VgcApis.Models.Consts.AutoEllipsis.NotifierTextMaxLength;
            void worker(int index, Action next)
            {
                if (textLen > niTextMaxLen)
                {
                    next?.Invoke();
                    return;
                }

                try
                {
                    coreServs[index]
                        .GetConfiger()
                        .GatherInfoForNotifyIcon(s =>
                        {
                            texts.Add(s);
                            textLen += s.Length;
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

            VgcApis.Misc.Utils.InvokeChainActionsAsync(count, worker, done);
        }

        private void SetNotifyIconText(string rawText)
        {
            var maxLen = VgcApis.Models.Consts.AutoEllipsis.NotifierTextMaxLength;
            var text = string.IsNullOrEmpty(rawText)
                ? I18N.Description
                : VgcApis.Misc.Utils.AutoEllipsis(rawText, maxLen);

            if (ni.Text == text)
            {
                return;
            }

            Invoke(() =>
            {
                // https://stackoverflow.com/questions/579665/how-can-i-show-a-systray-tooltip-longer-than-63-chars
                Type t = typeof(NotifyIcon);
                BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
                t.GetField("text", hidden).SetValue(ni, text);
                if ((bool)t.GetField("added", hidden).GetValue(ni))
                {
                    t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
                }
            });
        }

        void CreateContextMenuStrip(
            ContextMenuStrip menu,
            ToolStripMenuItem serversRootMenuItem,
            ToolStripMenuItem pluginRootMenuItem
        )
        {
            var factor = Misc.UI.GetScreenScalingFactor();
            if (factor > 1)
            {
                menu.ImageScalingSize = new Size(
                    (int)(menu.ImageScalingSize.Width * factor),
                    (int)(menu.ImageScalingSize.Height * factor)
                );
            }

            menu.Items.AddRange(
                new ToolStripMenuItem[]
                {
                    new ToolStripMenuItem(
                        I18N.Windows,
                        Properties.Resources.CPPWin32Project_16x,
                        new ToolStripMenuItem[]
                        {
                            new ToolStripMenuItem(
                                I18N.MainWin,
                                Properties.Resources.WindowsForm_16x,
                                (s, a) => Views.WinForms.FormMain.ShowForm()
                            ),
                            new ToolStripMenuItem(
                                I18N.TextEditor,
                                Properties.Resources.EditPage_16x,
                                (s, a) => Views.WinForms.FormTextConfigEditor.ShowEmptyConfig()
                            ),
                            new ToolStripMenuItem(
                                I18N.KeyGenForm,
                                Properties.Resources.NewKey_16x,
                                (s, a) => Views.WinForms.FormKeyGen.ShowForm()
                            ),
                            new ToolStripMenuItem(
                                I18N.Log,
                                Properties.Resources.FSInteractiveWindow_16x,
                                (s, a) => Views.WinForms.FormLog.ShowForm()
                            ),
                        }
                    ),
                    serversRootMenuItem,
                    pluginRootMenuItem,
                    new ToolStripMenuItem(
                        I18N.ScanQRCode,
                        Properties.Resources.ExpandScope_16x,
                        (s, a) => ScanQrcode()
                    ),
                    new ToolStripMenuItem(
                        I18N.ImportLink,
                        Properties.Resources.CopyLongTextToClipboard_16x,
                        (s, a) =>
                        {
                            string links = VgcApis.Misc.Utils.ReadFromClipboard();
                            slinkMgr.ImportLinkWithOutV2cfgLinks(links);
                        }
                    ),
                    new ToolStripMenuItem(
                        I18N.Options,
                        Properties.Resources.Settings_16x,
                        (s, a) => Invoke(() => Views.WinForms.FormOption.ShowForm())
                    ),
                }
            );

            menu.Items.Add(new ToolStripSeparator());

            menu.Items.AddRange(
                new ToolStripMenuItem[]
                {
                    CreateAboutMenu(),
                    new ToolStripMenuItem(
                        I18N.Exit,
                        Properties.Resources.CloseSolution_16x,
                        (s, a) =>
                        {
                            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmExitApp))
                            {
                                setting.SetShutdownReason(
                                    VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser
                                );
                                Application.Exit();
                            }
                        }
                    ),
                }
            );
        }

        private ToolStripMenuItem CreateRootMenuItem(string title, Bitmap icon)
        {
            var mi = new ToolStripMenuItem(title, icon) { Visible = false };
            return mi;
        }

        ToolStripMenuItem CreateAboutMenu()
        {
            var aboutMenu = new ToolStripMenuItem(I18N.About, Properties.Resources.StatusHelp_16x);

            var children = aboutMenu.DropDownItems;

            children.Add(
                I18N.DownloadV2rayCore,
                Properties.Resources.ASX_TransferDownload_blue_16x,
                (s, a) => Views.WinForms.FormDownloadCore.ShowForm()
            );

            children.Add(
                I18N.CheckForVgcUpdate,
                Properties.Resources.CloudSearch_16x,
                (s, a) => updater.CheckForUpdate(true)
            );

            children.Add(
                I18N.ProjectPage,
                null,
                (s, a) =>
                    VgcApis.Misc.UI.VisitUrl(I18N.VistProjectPage, Properties.Resources.ProjectLink)
            );

            children.Add(
                I18N.Feedback,
                null,
                (s, a) =>
                    VgcApis.Misc.UI.VisitUrl(I18N.VisitVGCIssuePage, Properties.Resources.IssueLink)
            );

            children.Add(
                I18N.ProjectWiki,
                null,
                (s, a) => VgcApis.Misc.UI.VisitUrl(I18N.VistWikiPage, Properties.Resources.WikiLink)
            );

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
