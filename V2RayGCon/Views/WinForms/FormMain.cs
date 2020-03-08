using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormMain : Form
    {

        #region single instance thing
        static readonly VgcApis.BaseClasses.AuxSiWinForm<FormMain> auxSiForm =
            new VgcApis.BaseClasses.AuxSiWinForm<FormMain>();
        static public FormMain GetForm() => auxSiForm.GetForm();
        static public void ShowForm() => auxSiForm.ShowForm();
        #endregion

        Services.Settings setting;
        Services.ShareLinkMgr slinkMgr;

        Controllers.FormMainCtrl formMainCtrl;
        Timer updateTitleTimer = null;
        string formTitle = "";

        public FormMain()
        {
            setting = Services.Settings.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;

            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            Misc.UI.AutoScaleToolStripControls(this, 16);
            formTitle = Misc.Utils.GetAppNameAndVer();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            UpdateFormTitle(this, EventArgs.Empty);
            setting.RestoreFormRect(this);

            // https://alexpkent.wordpress.com/2011/05/11/25/
            // 添加新控件的时候会有bug,不显示新控件
            // ToolStripManager.LoadSettings(this); 

            this.FormClosing += (s, a) =>
            {
                if (updateTitleTimer != null)
                {
                    updateTitleTimer.Stop();
                    updateTitleTimer.Tick -= UpdateFormTitle;
                    updateTitleTimer.Dispose();
                }
            };

            this.FormClosed += (s, a) =>
            {
                setting.SaveFormRect(this);
                // ToolStripManager.SaveSettings(this);
                formMainCtrl.Cleanup();
                setting.LazyGC();
            };

            formMainCtrl = InitFormMainCtrl();
            BindToolStripButtonToMenuItem();

            updateTitleTimer = new Timer
            {
                Interval = 2000,
            };
            updateTitleTimer.Tick += UpdateFormTitle;
            updateTitleTimer.Start();
        }

        #region private method
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Misc.UI.Confirm(I18N.ConfirmExitApp))
            {
                setting.ShutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser;
                Application.Exit();
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

            this.Invoke((MethodInvoker)delegate
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
                toolStripMenuItemSortBySummary));

            return ctrl;
        }

        #endregion

        #region UI event handler
        private void closeWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion


    }
}
