using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using VgcApis.Interfaces;

namespace V2RayGCon.Services
{
    class Notifier :
        BaseClasses.SingletonService<Notifier>,
        VgcApis.Interfaces.Services.INotifierService
    {
        Settings setting;
        Servers servers;
        ShareLinkMgr slinkMgr;
        Updater updater;

        public readonly NotifyIcon notifyIcon;

        static readonly long SpeedtestTimeout = VgcApis.Models.Consts.Core.SpeedtestTimeout;
        static readonly int UpdateInterval = VgcApis.Models.Consts.Intervals.NotifierTextUpdateIntreval;

        readonly public ContextMenuStrip niMenu;
        readonly VgcApis.WinForms.HotKeyWindow hkWin;
        readonly Bitmap orgIcon;
        bool isMenuOpened = false;

        VgcApis.Libs.Tasks.LazyGuy lazyNotifyIconUpdater;
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

        Notifier()
        {
            lazyNotifyIconUpdater = new VgcApis.Libs.Tasks.LazyGuy(UpdateNotifyIconWorker, UpdateInterval)
            {
                Name = "notifyIconUpdater",
            };

            // 其他组件有可能在初始化的时候引用菜单
            qsMenuCompos = CreateQsMenuCompos();
            miServersRoot = CreateRootMenuItem(I18N.Servers, Properties.Resources.RemoteServer_16x);
            miPluginsRoot = CreateRootMenuItem(I18N.Plugins, Properties.Resources.Module_16x);

            niMenu = CreateMenu(miServersRoot, miPluginsRoot);
            notifyIcon = CreateNotifyIcon(niMenu);
            orgIcon = notifyIcon.Icon.ToBitmap();

            // ????
            niMenu.CreateControl();
            niMenu.Show();
            niMenu.Close();

            hkWin = new VgcApis.WinForms.HotKeyWindow();
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

            BindMenuEvents();
            BindServerEvents();
            UpdateNotifyIconLater();
        }

        #region public method
        public string RegisterHotKey(
            Action hotKeyHandler,
            string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {
            var handle = "";
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(niMenu, () =>
            {
                handle = hkWin.RegisterHotKey(hotKeyHandler, keyName, hasAlt, hasCtrl, hasShift);
            });
            return handle;
        }

        public bool UnregisterHotKey(string hotKeyHandle)
        {
            var r = false;
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(niMenu, () =>
            {
                r = hkWin.UnregisterHotKey(hotKeyHandle);
            });
            return r;
        }


        public void RefreshNotifyIcon() => UpdateNotifyIconLater();

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

        public void RunInUiThreadIgnoreError(Action updater) =>
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(niMenu, updater);


        /// <summary>
        /// null means delete menu
        /// </summary>
        /// <param name="pluginMenu"></param>
        public void UpdatePluginMenu(IEnumerable<ToolStripMenuItem> children)
        {
            RunInUiThreadIgnoreError(() =>
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


        private void BindMenuEvents()
        {
            niMenu.Opening += (s, a) => isMenuOpened = true;
            niMenu.Closed += (s, a) => isMenuOpened = false;

            notifyIcon.MouseClick += (s, a) =>
            {
                switch (a.Button)
                {
                    case MouseButtons.Left:
                        // https://stackoverflow.com/questions/2208690/invoke-notifyicons-context-menu
                        // MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                        // mi.Invoke(ni, null);
                        Views.WinForms.FormMain.GetForm()?.Show();
                        break;

                        /*
                    case MouseButtons.Right:
                        RunInUiThreadIgnoreError(() =>
                        {
                            niMenu.Show(a.X - niMenu.Width, a.Y - niMenu.Height);
                        });
                        break;
                        */
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

            var picked = list[new Random().Next(len)];
            servers.StopAllServersThen(() => picked.GetCoreCtrl().RestartCore());
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

        void UpdateServersMenuThen(Action next = null)
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

            RunInUiThreadIgnoreError(
                () => ReplaceServersMenuWith(
                    isGrouped, miGroupedServers, miTopNthServers));

            next?.Invoke();
        }

        void UpdateNotifyIconLater() => lazyNotifyIconUpdater?.Throttle();

        void UpdateNotifyIconWorker(Action done)
        {
            if (isMenuOpened)
            {
                lazyNotifyIconUpdater?.Postpone();
                done();
                return;
            }

            var start = DateTime.Now.Millisecond;

            Action finished = () => Task.Run(async () =>
            {
                var relex = UpdateInterval - (DateTime.Now.Millisecond - start);
                await Task.Delay(Math.Max(0, relex));
                done();
            });

            var list = servers.GetAllServersOrderByIndex()
                .Where(s => s.GetCoreCtrl().IsCoreRunning())
                .ToList();

            UpdateNotifyIconImage(list.Count());
            UpdateNotifyIconTextThen(list, () => UpdateServersMenuThen(finished));
        }

        private void UpdateNotifyIconImage(int activeServNum)
        {
            var icon = orgIcon.Clone() as Bitmap;
            var size = icon.Size;

            using (Graphics g = Graphics.FromImage(icon))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;

                DrawProxyModeCornerCircle(g, size);
                DrawIsRunningCornerMark(g, size, activeServNum);
            }

            notifyIcon.Icon?.Dispose();
            notifyIcon.Icon = Icon.FromHandle(icon.GetHicon());
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
            Graphics graphics, Size size, int activeServNum)
        {
            var w = size.Width;
            var cx = w * 0.7f;

            switch (activeServNum)
            {
                case 0:
                    DrawOneLine(graphics, w, cx, false);
                    break;
                case 1:
                    DrawTriangle(graphics, w, cx);
                    break;
                default:
                    DrawOneLine(graphics, w, cx, false);
                    DrawOneLine(graphics, w, cx, true);
                    break;
            }
        }

        private static void DrawTriangle(Graphics graphics, int w, float cx)
        {
            var cr = w * 0.22f;
            var dh = Math.Sqrt(3) * cr / 2f;
            var tri = new Point[] {
                    new Point((int)(cx - cr / 2f),(int)(cx - dh)),
                    new Point((int)(cx + cr),(int)cx),
                    new Point((int)(cx - cr / 2f),(int)(cx + dh)),
                };
            graphics.FillPolygon(Brushes.White, tri);
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
            UpdateNotifyIconLater();

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

        void UpdateNotifyIconTextThen(
            List<ICoreServCtrl> list,
            Action finished)
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

            if (notifyIcon.Text == text)
            {
                return;
            }

            // https://stackoverflow.com/questions/579665/how-can-i-show-a-systray-tooltip-longer-than-63-chars
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(notifyIcon, text);
            if ((bool)t.GetField("added", hidden).GetValue(notifyIcon))
                t.GetMethod("UpdateIcon", hidden).Invoke(notifyIcon, new object[] { true });
        }

        NotifyIcon CreateNotifyIcon(ContextMenuStrip menu)
        {
            return new NotifyIcon
            {
                Text = I18N.Description,
                Icon = VgcApis.Misc.UI.GetAppIcon(),
                BalloonTipTitle = VgcApis.Misc.Utils.GetAppName(),
                ContextMenuStrip = menu,
                Visible = true
            };
        }

        ContextMenuStrip CreateMenu(
           ToolStripMenuItem serversRootMenuItem,
           ToolStripMenuItem pluginRootMenuItem)
        {
            var menu = new ContextMenuStrip();

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
                            (s,a)=> Views.WinForms.FormMain.ShowForm()),
                        new ToolStripMenuItem(
                            I18N.ConfigEditor,
                            Properties.Resources.EditWindow_16x,
                            (s,a)=>new Views.WinForms.FormConfiger() ),
                        new ToolStripMenuItem(
                            I18N.GenQRCode,
                            Properties.Resources.AzureVirtualMachineExtension_16x,
                            (s,a)=> Views.WinForms.FormQRCode.ShowForm()),
                        new ToolStripMenuItem(
                            I18N.Log,
                            Properties.Resources.FSInteractiveWindow_16x,
                            (s,a)=> Views.WinForms.FormLog.ShowForm() ),
                         new ToolStripMenuItem(
                            I18N.DownloadV2rayCore,
                            Properties.Resources.ASX_TransferDownload_blue_16x,
                            (s,a)=> Views.WinForms.FormDownloadCore.GetForm()),
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
                        (s,a)=> Views.WinForms.FormOption.ShowForm()),
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

            return menu;
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
            hkWin?.Dispose();
            ReleaseServerEvents();
            lazyNotifyIconUpdater?.Dispose();
        }
        #endregion
    }
}
