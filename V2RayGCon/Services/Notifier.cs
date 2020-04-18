using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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
        static readonly long SpeedtestTimeout = VgcApis.Models.Consts.Core.SpeedtestTimeout;

        NotifyIcon ni;
        Settings setting;
        Servers servers;
        ShareLinkMgr slinkMgr;
        Bitmap orgIcon = null;
        Updater updater;

        ToolStripMenuItem pluginRootMenuItem = null;
        ToolStripMenuItem serversRootMenuItem = null;

        readonly int notifyIconUpdateInterval = VgcApis.Models.Consts.Intervals.NotifierTextUpdateIntreval;
        VgcApis.Libs.Tasks.LazyGuy lazyNotifyIconUpdater;
        bool isMenuOpened = false;

        enum qswmCompos
        {
            StopAllServer,
            QuickSwitchMenuRoot,
            SwitchToRandomServer,
            SwitchToRandomTlsServer,
        }

        Dictionary<qswmCompos, ToolStripMenuItem> quickSwitchMenuComponents;

        Notifier()
        {
            lazyNotifyIconUpdater = new VgcApis.Libs.Tasks.LazyGuy(
                UpdateNotifyIcon, notifyIconUpdateInterval);
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

            GenQuickSwitchMenuComponents();

            CreateNotifyIcon();

            BindServerEvents();

            BindMouseClickEvent();

            UpdateNotifyIcon();
        }

        #region public method
        public void RefreshNotifyIcon() => UpdateNotifyIcon();

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

        public void RunInUiThreadIgnoreErrorThen(Action updater, Action next) =>
            VgcApis.Misc.UI.RunInUiThreadIgnoreErrorThen(ni.ContextMenuStrip, updater, next);

#if DEBUG
        public void InjectDebugMenuItem(ToolStripMenuItem menu)
        {
            ni.ContextMenuStrip.Items.Insert(0, new ToolStripSeparator());
            ni.ContextMenuStrip.Items.Insert(0, menu);
        }
#endif

        /// <summary>
        /// null means delete menu
        /// </summary>
        /// <param name="pluginMenu"></param>
        public void UpdatePluginMenu(IEnumerable<ToolStripMenuItem> children)
        {
            RunInUiThreadIgnoreErrorThen(() =>
            {
                pluginRootMenuItem.DropDownItems.Clear();
                if (children == null || children.Count() < 1)
                {
                    pluginRootMenuItem.Visible = false;
                    return;
                }

                pluginRootMenuItem.DropDownItems.AddRange(children.ToArray());
                pluginRootMenuItem.Visible = true;
            }, null);
        }

        #endregion

        #region private method
        private void BindMouseClickEvent()
        {
            ni.MouseClick += (s, a) =>
            {
                if (a.Button != MouseButtons.Left)
                {
                    return;
                }

                // https://stackoverflow.com/questions/2208690/invoke-notifyicons-context-menu
                // MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                // mi.Invoke(ni, null);

                Views.WinForms.FormMain.GetForm()?.Show();
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

        void GenQuickSwitchMenuComponents()
        {
            var qm = new Dictionary<qswmCompos, ToolStripMenuItem>();

            qm[qswmCompos.QuickSwitchMenuRoot] = new ToolStripMenuItem(
                I18N.QuickSwitch, Properties.Resources.SwitchSourceOrTarget_16x);

            qm[qswmCompos.StopAllServer] = new ToolStripMenuItem(
                    I18N.StopAllServers,
                    Properties.Resources.Stop_16x,
                    (s, a) => servers.StopAllServersThen());

            qm[qswmCompos.SwitchToRandomServer] = new ToolStripMenuItem(
                    I18N.SwitchToRandomServer,
                    Properties.Resources.FTPConnection_16x,
                    (s, a) => SwitchToRandomServer());

            qm[qswmCompos.SwitchToRandomTlsServer] =
                new ToolStripMenuItem(
                    I18N.SwitchToRandomTlsserver,
                    Properties.Resources.SFTPConnection_16x,
                    (s, a) => SwitchRandomTlsServer());

            quickSwitchMenuComponents = qm;
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
            var root = serversRootMenuItem.DropDownItems;
            root.Clear();

            var count = miGroupedServers.Count;
            if (count < 1)
            {
                serversRootMenuItem.Visible = false;
                return;
            }

            List<ToolStripItem> mis = new List<ToolStripItem>();
            mis.Add(quickSwitchMenuComponents[qswmCompos.StopAllServer]);
            if (isGrouped)
            {
                var qs = quickSwitchMenuComponents[qswmCompos.QuickSwitchMenuRoot];
                var items = qs.DropDownItems;
                items.Clear();
                items.Add(quickSwitchMenuComponents[qswmCompos.SwitchToRandomServer]);
                items.Add(quickSwitchMenuComponents[qswmCompos.SwitchToRandomTlsServer]);
                items.Add(new ToolStripSeparator());
                items.AddRange(miTopNthServers.ToArray());
                mis.Add(qs);
            }
            else
            {
                mis.Add(quickSwitchMenuComponents[qswmCompos.SwitchToRandomServer]);
                mis.Add(quickSwitchMenuComponents[qswmCompos.SwitchToRandomTlsServer]);
            }
            mis.Add(new ToolStripSeparator());

            root.AddRange(mis.ToArray());
            root.AddRange(miGroupedServers.ToArray());
            serversRootMenuItem.Visible = true;
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

            RunInUiThreadIgnoreErrorThen(
                () => ReplaceServersMenuWith(isGrouped, miGroupedServers, miTopNthServers),
                next);
        }

        VgcApis.Libs.Tasks.Bar updateNotifyIconLock = new VgcApis.Libs.Tasks.Bar();
        void UpdateNotifyIcon()
        {
            if (isMenuOpened || !updateNotifyIconLock.Install())
            {
                lazyNotifyIconUpdater.DoItLater();
                return;
            }

            var start = DateTime.Now;
            Action finished = async () =>
            {
                var relex = notifyIconUpdateInterval - (DateTime.Now - start).TotalMilliseconds;
                if (relex > 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(relex));
                }
                updateNotifyIconLock.Remove();
            };

            _ = Task.Run(() =>
            {
                Action updateServersMenu = () => UpdateServersMenuThen(finished);

                var list = servers.GetAllServersOrderByIndex()
                    .Where(s => s.GetCoreCtrl().IsCoreRunning())
                    .ToList();

                UpdateNotifyIconImage(list.Count());
                UpdateNotifyIconTextThen(list, updateServersMenu);
            });
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

            ni.Icon?.Dispose();
            ni.Icon = Icon.FromHandle(icon.GetHicon());
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
            UpdateNotifyIcon();

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
            List<VgcApis.Interfaces.ICoreServCtrl> list,
            Action finished)
        {
            var count = list.Count;
            var texts = new List<string>();

            void done()
            {
                var sysProxyInfo = GetterSysProxyInfo();
                if (!string.IsNullOrEmpty(sysProxyInfo))
                {
                    int len = VgcApis.Models.Consts.AutoEllipsis.NotifierSysProxyInfoMaxLength;
                    texts.Add(I18N.CurSysProxy + VgcApis.Misc.Utils.AutoEllipsis(sysProxyInfo, len));
                }
                SetNotifyText(string.Join(Environment.NewLine, texts));
                finished?.Invoke();
            }

            void worker(int index, Action next)
            {
                list[index].GetConfiger().GetterInfoForNotifyIconf(s =>
                {
                    texts.Add(s);
                    next?.Invoke();
                });
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

        void CreateNotifyIcon()
        {
            var menu = CreateMenu();
            menu.Opening += (s, a) => isMenuOpened = true;
            menu.Closed += (s, a) => isMenuOpened = false;

            ni = new NotifyIcon
            {
                Text = I18N.Description,
                Icon = VgcApis.Misc.UI.GetAppIcon(),
                BalloonTipTitle = VgcApis.Misc.Utils.GetAppName(),

                ContextMenuStrip = menu,
                Visible = true
            };

            orgIcon = ni.Icon.ToBitmap();
        }

        ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            var factor = Misc.UI.GetScreenScalingFactor();
            if (factor > 1)
            {
                menu.ImageScalingSize = new Size(
                    (int)(menu.ImageScalingSize.Width * factor),
                    (int)(menu.ImageScalingSize.Height * factor));
            }

            pluginRootMenuItem = CreatePluginRootMenuItem();
            serversRootMenuItem = CreateServersRootMenuItem();

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

        private ToolStripMenuItem CreateServersRootMenuItem()
        {
            var mi = new ToolStripMenuItem(
                I18N.Servers,
                Properties.Resources.RemoteServer_16x);
            mi.Visible = false;
            return mi;
        }

        private ToolStripMenuItem CreatePluginRootMenuItem()
        {
            var mi = new ToolStripMenuItem(
                                 I18N.Plugins,
                                 Properties.Resources.Module_16x);
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
            ni.Visible = false;

            ReleaseServerEvents();
            lazyNotifyIconUpdater.Quit();

            ni.Visible = false;
        }
        #endregion
    }
}
