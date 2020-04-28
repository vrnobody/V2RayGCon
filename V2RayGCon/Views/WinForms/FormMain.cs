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
        Services.Notifier notifier;

        Controllers.FormMainCtrl formMainCtrl;
        string formTitle = "";

        public NotifyIcon notifyIcon => this.ni;
        VgcApis.WinForms.HotKeyWindow hkWindow;

        public FormMain()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            Misc.UI.AutoScaleToolStripControls(this, 16);
            formTitle = Misc.Utils.GetAppNameAndVer();

            setting = Services.Settings.Instance;
            notifier = Services.Notifier.Instance;
            notifier.InitNotifyIconFor(this);

            launcher = new Services.Launcher(setting, notifier, this);

            VgcApis.Libs.Sys.FileLogger.Raw("\n");
            VgcApis.Libs.Sys.FileLogger.Info($"{formTitle} start");
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            BindEvents();

            if (!launcher.Warmup())
            {
                setting.ShutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser;
                VgcApis.Misc.UI.RunInUiThreadIgnoreError(this, () => Application.Exit());
                return;
            }

            hkWindow = new VgcApis.WinForms.HotKeyWindow(this);
            hkWindow.OnMessage += (m) => WndProc(ref m);

            launcher.Run();
            slinkMgr = Services.ShareLinkMgr.Instance;
            setting.RestoreFormRect(this);

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

        void BindEvents()
        {
            Microsoft.Win32.SystemEvents.PowerModeChanged += (s, a) =>
            {
                switch (a.Mode)
                {
                    case Microsoft.Win32.PowerModes.Suspend:
                        setting.SetScreenLockingState(true);
                        break;
                    default:
                        break;
                }
            };

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
