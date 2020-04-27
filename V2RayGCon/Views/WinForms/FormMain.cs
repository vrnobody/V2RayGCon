using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormMain : Form
    {
        Services.Settings setting;
        Services.ShareLinkMgr slinkMgr;
        Services.Launcher launcher;

        Controllers.FormMainCtrl formMainCtrl;
        string formTitle = "";

        public NotifyIcon notifyIcon => this.ni;
        readonly VgcApis.WinForms.HotKeyWindow hkWindow;

        public FormMain()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            Misc.UI.AutoScaleToolStripControls(this, 16);
            formTitle = Misc.Utils.GetAppNameAndVer();

            hkWindow = new VgcApis.WinForms.HotKeyWindow();
            setting = Services.Settings.Instance;
            launcher = new Services.Launcher(setting, this);

            VgcApis.Libs.Sys.FileLogger.Raw("\n");
            VgcApis.Libs.Sys.FileLogger.Info($"{formTitle} start");
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            BindExitEvents();

            if (!launcher.Warmup())
            {
                setting.ShutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser;
                VgcApis.Misc.UI.RunInUiThreadIgnoreError(this, () => Application.Exit());
                return;
            }

            launcher.Run();

            slinkMgr = Services.ShareLinkMgr.Instance;
            setting.RestoreFormRect(this);

            // https://alexpkent.wordpress.com/2011/05/11/25/
            // 添加新控件的时候会有bug,不显示新控件
            // ToolStripManager.LoadSettings(this); 

            formMainCtrl = InitFormMainCtrl();
            BindToolStripButtonToMenuItem();

            setting.OnPortableModeChanged += UpdateFormTitle;
            UpdateFormTitle(this, EventArgs.Empty);
        }

        #region exit 
        void Cleanup()
        {
            VgcApis.Libs.Sys.FileLogger.Info("");
            hkWindow?.Dispose();
            formMainCtrl?.Cleanup();
            launcher?.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info($"{formTitle} end");
        }


        void BindExitEvents()
        {
            Microsoft.Win32.SystemEvents.SessionSwitch += (s, a) =>
            {
                switch (a.Reason)
                {
                    case Microsoft.Win32.SessionSwitchReason.SessionLock:
                        setting.SetScreenLockingState(true);
                        break;
                    case Microsoft.Win32.SessionSwitchReason.SessionUnlock:
                        setting.SetScreenLockingState(false);
                        break;
                    default:
                        break;
                }
            };

            Application.ApplicationExit += (s, a) => Cleanup();

            Microsoft.Win32.SystemEvents.SessionEnding += (s, a) =>
            {
                setting.ShutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.Poweroff;
                Close();
            };

            Application.ThreadException += (s, a) => ShowExceptionDetailAndExit(a.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, a) => ShowExceptionDetailAndExit(a.ExceptionObject as Exception);
        }

        void ShowExceptionDetailAndExit(Exception exception)
        {
            if (setting.ShutdownReason != VgcApis.Models.Datas.Enums.ShutdownReasons.Poweroff)
            {
                ShowExceptionDetails(exception);
            }
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(this, () => Application.Exit());
        }

        private void ShowExceptionDetails(Exception exception)
        {
            var nl = Environment.NewLine;
            var verInfo = Misc.Utils.GetAppNameAndVer();
            var log = $"{I18N.LooksLikeABug}{nl}{nl}{verInfo}";

            try
            {
                log += nl + nl + exception.ToString();
                log += nl + nl + setting.GetLogContent();
            }
            catch { }

            VgcApis.Libs.Sys.NotepadHelper.ShowMessage(log, "V2RayGCon bug report");
        }
        #endregion
        #region public methods
        public void Restore()
        {
            ShowInTaskbar = true;
            Show();
            Activate();
        }

        public void HideToSystray()
        {
            this.Hide();
            ShowInTaskbar = false;
        }
        #endregion

        #region hotkey window
        public string RegisterHotKey(
             Action hotKeyHandler,
             string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {
            string handle = null;
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(this, () =>
            {
                try
                {
                    handle = hkWindow.RegisterHotKey(hotKeyHandler, keyName, hasAlt, hasCtrl, hasShift);
                }
                catch { }
            });
            return handle;
        }
        public bool UnregisterHotKey(string hotKeyHandle)
        {
            var r = false;
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(this, () =>
            {
                try
                {
                    r = hkWindow.UnregisterHotKey(hotKeyHandle);
                }
                catch { }
            });
            return r;
        }

        #endregion

        /*
        #region HotKey thinggy

        const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        ConcurrentDictionary<long, Action> handlers = new ConcurrentDictionary<long, Action>();
        ConcurrentDictionary<string, Tuple<int, long>> contexts = new ConcurrentDictionary<string, Tuple<int, long>>();
        int currentEvCode = 0;

        public string RegisterHotKey(
            Action hotKeyHandler,
            string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {
            string handle = null;
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(this, () =>
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
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(this, () =>
            {
                try
                {
                    r = UnregisterHotKeyWorker(hotKeyHandle);
                }
                catch { }
            });
            return r;
        }

        string RegisterHotKeyWorker(
            Action hotKeyHandler,
            string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {

            var evCode = currentEvCode++;

            if (!VgcApis.Misc.Utils.TryParseKeyMesssage(keyName, hasAlt, hasCtrl, hasShift,
                out uint modifier, out uint key))
            {
                return null;
            }

            long hkMsg = (key << 16) | modifier;
            var handlerKeys = handlers.Keys;
            if (handlerKeys.Contains(hkMsg) || !RegisterHotKey(Handle, evCode, modifier, key))
            {
                return null;
            }

            for (int failsafe = 0; failsafe < 1000; failsafe++)
            {
                var hkHandle = Guid.NewGuid().ToString();
                var keys = contexts.Keys;
                if (!keys.Contains(hkHandle))
                {
                    var hkParma = new Tuple<int, long>(evCode, hkMsg);
                    contexts.TryAdd(hkHandle, hkParma);
                    handlers.TryAdd(hkMsg, hotKeyHandler);
                    return hkHandle;
                }
            }

            return null;
        }

        void CleanupHotKeys()
        {
            var handles = contexts.Keys;
            foreach (var handle in handles)
            {
                UnregisterHotKey(handle);
            }
        }

        bool UnregisterHotKeyWorker(string hotKeyHandle)
        {
            try
            {
                if (!string.IsNullOrEmpty(hotKeyHandle)
                    && contexts.TryRemove(hotKeyHandle, out var context))
                {
                    var evCode = context.Item1;
                    var keyMsg = context.Item2;
                    var ok = UnregisterHotKey(this.Handle, evCode);
                    handlers.TryRemove(keyMsg, out _);
                    return ok;
                }
            }
            catch { }
            return false;
        }

        void HotkeyEventHandler(long key)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                try
                {
                    if (handlers.TryGetValue(key, out var handler))
                    {
                        handler?.Invoke();
                    }
                }
                catch (Exception e)
                {
                    VgcApis.Libs.Sys.FileLogger.Error($"Handle wnd event error\n key code:{key} \n {e}");
                }
            });
        }

        protected override void WndProc(ref Message m)
        {
            // check if we got a hot key pressed.
            if (m.Msg == WM_HOTKEY)
            {
                long key = (uint)m.LParam & 0xffffffff;
                HotkeyEventHandler(key);
            }

            base.WndProc(ref m);
        }
        #endregion
            */
        #region private method


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Misc.UI.Confirm(I18N.ConfirmExitApp))
            {
                setting.ShutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser;
                VgcApis.Misc.UI.RunInUiThreadIgnoreError(this, () => Application.Exit());
            }
        }

        private void ToolStripButtonScanQrcode_Click(object sender, EventArgs e)
        {
            var notifier = Services.Notifier.Instance;
            notifier.ScanQrcode();
        }

        private void UpdateFormTitle(object sender, EventArgs args)
        {
            var title = formTitle;
            if (setting.isPortable)
            {
                title += " - " + I18N.Portable;
            }

            VgcApis.Misc.UI.RunInUiThreadIgnoreError(this, () =>
            {
                if (this.Text != title)
                {
                    this.Text = title;
                }
            });
        }

        void BindToolStripButtonToMenuItem()
        {
            void bind(ToolStripButton button, ToolStripMenuItem menu, bool activate = true)
            {
                if (activate)
                {
                    button.Click += (s, a) =>
                    {
                        menu.PerformClick();

                        // Do not know why, form main will lost focus sometimes.
                        Focus();
                    };
                }
                else
                {
                    button.Click += (s, a) => menu.PerformClick();
                }
            }

            // for security 
            // bind(toolStripButtonImportFromClipboard, toolMenuItemImportLinkFromClipboard, false);
            toolStripButtonImportFromClipboard.Click += (s, a) =>
            {
                string text = Misc.Utils.GetClipboardText();
                slinkMgr.ImportLinkWithOutV2cfgLinks(text);
            };

            bind(toolStripButtonAddServerSimple, toolMenuItemSimAddVmessServer, false);
            bind(toolStripButtonSelectAllCurPage, selectAllCurPageToolStripMenuItem);
            bind(toolStripButtonInverseSelectionCurPage, invertSelectionCurPageToolStripMenuItem);
            bind(toolStripButtonSelectNoneCurPage, selectNoneCurPageToolStripMenuItem1);

            bind(toolStripButtonAllServerSelectAll, selectAllAllServersToolStripMenuItem);
            bind(toolStripButtonAllServerSelectNone, selectNoneAllServersToolStripMenuItem);

            bind(toolStripButtonModifySelected, toolStripMenuItemModifySettings, false);
            bind(toolStripButtonRunSpeedTest, toolStripMenuItemRunBatchSpeedTest);
            bind(toolStripButtonSortSelectedBySpeedTestResult, toolStripMenuItemSortBySpeedTest);

            bind(toolStripButtonFormOption, toolMenuItemOptions, false);
            bind(toolStripButtonShowFormLog, toolMenuItemLog, false);
        }

        private Controllers.FormMainCtrl InitFormMainCtrl()
        {
            var ctrl = new Controllers.FormMainCtrl();

            ctrl.Plug(new Controllers.FormMainComponent.FlyServer(
                this,
                flyServerListContainer,
                toolStripLabelMarkFilter,
                toolStripComboBoxMarkFilter,
                toolStripStatusLabelTotal,
                toolStripDropDownButtonPager,
                toolStripStatusLabelPrePage,
                toolStripStatusLabelNextPage,
                toolStripMenuItemResize));

            ctrl.Plug(new Controllers.FormMainComponent.MenuItemsBasic(
                this,
                pluginToolStripMenuItem,

                toolMenuItemSimAddVmessServer,
                toolMenuItemImportLinkFromClipboard,
                toolMenuItemExportAllServerToFile,
                toolMenuItemImportFromFile,
                toolMenuItemAbout,
                toolMenuItemHelp,
                toolMenuItemConfigEditor,
                toolMenuItemQRCode,
                toolMenuItemLog,
                toolMenuItemOptions,
                toolStripMenuItemDownLoadV2rayCore,
                toolStripMenuItemRemoveV2rayCore,
                toolMenuItemCheckUpdate));

            ctrl.Plug(new Controllers.FormMainComponent.MenuItemsSelect(
                /*
                ToolStripMenuItem selectAllCurPage,
                ToolStripMenuItem invertSelectionCurPage,
                ToolStripMenuItem selectNoneCurPage,
                */
                selectAllCurPageToolStripMenuItem,
                invertSelectionCurPageToolStripMenuItem,
                selectNoneCurPageToolStripMenuItem1,

                /*
                ToolStripMenuItem selectAllAllPages,
                ToolStripMenuItem invertSelectionAllPages,
                ToolStripMenuItem selectNoneAllPages,       
                */
                selectAllAllPagesToolStripMenuItem,
                invertSelectionAllPagesToolStripMenuItem,
                selectNoneAllPagesToolStripMenuItem,

                /*
                ToolStripMenuItem selectAutorunAllPages,
                ToolStripMenuItem selectNoMarkAllPages,
                ToolStripMenuItem selectNoSpeedTestAllPages,
                ToolStripMenuItem selectRunningAllPages,
                ToolStripMenuItem selectTimeoutAllPages,
                ToolStripMenuItem selectUntrackAllPages,
                */
                selectAutorunAllPagesToolStripMenuItem,
                selectNoMarkAllPagesToolStripMenuItem,
                selectNoSpeedTestAllPagesToolStripMenuItem,
                selectRunningAllPagesToolStripMenuItem,
                selectTimeoutAllPagesToolStripMenuItem,
                selectUntrackAllPagesToolStripMenuItem,

                /*
                ToolStripMenuItem selectAllAllServers,
                ToolStripMenuItem invertSelectionAllServers,
                ToolStripMenuItem selectNoneAllServers,
                */
                selectAllAllServersToolStripMenuItem,
                invertSelectionAllServersToolStripMenuItem,
                selectNoneAllServersToolStripMenuItem,

                /*
                ToolStripMenuItem selectAutorunAllServers,
                ToolStripMenuItem selectNoMarkAllServers,
                ToolStripMenuItem selectNoSpeedTestAllServers,
                ToolStripMenuItem selectRunningAllServers,
                ToolStripMenuItem selectTimeoutAllServers,
                ToolStripMenuItem selectUntrackAllServers,
                */
                selectAutorunAllServersToolStripMenuItem,
                selectNoMarkAllServersToolStripMenuItem,
                selectNoSpeedTestAllServersToolStripMenuItem,
                selectRunningAllServersToolStripMenuItem,
                selectTimeoutAllServersToolStripMenuItem,
                selectUntrackAllServersToolStripMenuItem));

            ctrl.Plug(new Controllers.FormMainComponent.MenuItemsServer(
                //// misc
                //ToolStripMenuItem refreshSummary,
                //ToolStripMenuItem deleteAllServers,
                //ToolStripMenuItem deleteSelected,
                refreshSummaryToolStripMenuItem,
                deleteAllServersToolStripMenuItem,
                deleteSelectedServersToolStripMenuItem,

                //// copy
                //ToolStripMenuItem copyAsV2cfgLinks,
                //ToolStripMenuItem copyAsVmessLinks,
                //ToolStripMenuItem copyAsSubscriptions,
                toolStripMenuItemCopyAsV2cfgLink,
                toolStripMenuItemCopyAsVmessLink,
                toolStripMenuItemCopyAsVeeLink,
                toolStripMenuItemCopyAsVmessSubscription,
                toolStripMenuItemCopyAsVeeSubscription,

                //// batch op
                toolStripMenuItemStopBatchSpeedTest,
                toolStripMenuItemRunBatchSpeedTest,
                toolStripMenuItemClearSpeedTestResults,

                toolStripMenuItemModifySettings,
                toolStripMenuItemStopSelected,
                toolStripMenuItemRestartSelected,

                //// view
                //ToolStripMenuItem moveToTop,
                //ToolStripMenuItem moveToBottom,
                //ToolStripMenuItem sortBySpeed,
                //ToolStripMenuItem sortBySummary)
                toolStripMenuItemMoveToTop,
                toolStripMenuItemMoveToBottom,
                toolStripMenuItemSortBySpeedTest,
                toolStripMenuItemSortByDateT,
                toolStripMenuItemSortBySummary,
                toolStripMenuItemReverseByIndex));

            return ctrl;
        }

        #endregion

        #region UI event handler
        private void closeWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void flyServerListContainer_Scroll(object sender, ScrollEventArgs e)
        {
            flyServerListContainer.Refresh();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            setting.SaveFormRect(this);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                HideToSystray();
                setting.LazyGC();
                return;
            }
        }

        #endregion


    }
}
