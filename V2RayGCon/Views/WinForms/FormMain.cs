using System;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormMain : Form
    {

        #region single instance thing
        static readonly VgcApis.Models.BaseClasses.AuxSiWinForm<FormMain> auxSiForm =
            new VgcApis.Models.BaseClasses.AuxSiWinForm<FormMain>();
        static public FormMain GetForm() => auxSiForm.GetForm();
        static public void ShowForm() => auxSiForm.ShowForm();
        #endregion

        Service.Setting setting;
        Service.ShareLinkMgr slinkMgr;

        Controller.FormMainCtrl formMainCtrl;
        Timer updateTitleTimer = null;
        string formTitle = "";

        public FormMain()
        {
            setting = Service.Setting.Instance;
            slinkMgr = Service.ShareLinkMgr.Instance;

            InitializeComponent();
            VgcApis.Libs.UI.AutoSetFormIcon(this);
            Lib.UI.AutoScaleToolStripControls(this, 16);
            GenFormTitle();
        }

        public void FormMain_Shown(object sender, EventArgs e)
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
            if (Lib.UI.Confirm(I18N.ConfirmExitApp))
            {
                Application.Exit();
            }
        }

        private void ToolStripButtonScanQrcode_Click(object sender, EventArgs e)
        {
            var notifier = Service.Notifier.Instance;
            notifier.ScanQrcode();
        }

        private void GenFormTitle()
        {
            var version = Lib.Utils.GetAssemblyVersion();
            formTitle = string.Format(
                "{0} v{1}",
                Properties.Resources.AppName,
                Lib.Utils.TrimVersionString(version));
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
                        this.Activate();
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
                string text = Lib.Utils.GetClipboardText();
                slinkMgr.ImportLinkWithOutV2cfgLinks(text);
            };

            bind(toolStripButtonAddServerSimple, toolMenuItemSimAddVmessServer, false);
            bind(toolStripButtonSelectAllCurPage, selectAllCurPageToolStripMenuItem);
            bind(toolStripButtonInverseSelectionCurPage, invertSelectionCurPageToolStripMenuItem);
            bind(toolStripButtonSelectNoneCurPage, selectNoneCurPageToolStripMenuItem1);

            bind(toolStripButtonAllServerSelectAll, selectAllAllServersToolStripMenuItem);
            bind(toolStripButtonAllServerSelectNone, selectNoneAllServersToolStripMenuItem);

            bind(toolStripButtonModifySelected, toolStripMenuItemModifySettings, false);
            bind(toolStripButtonRunSpeedTest, toolStripMenuItemSpeedTestOnSelected);
            bind(toolStripButtonSortSelectedBySpeedTestResult, toolStripMenuItemSortBySpeedTest);

            bind(toolStripButtonFormOption, toolMenuItemOptions, false);
            bind(toolStripButtonHelp, toolMenuItemHelp, false);
            bind(toolStripButtonShowFormLog, toolMenuItemLog, false);
        }

        private Controller.FormMainCtrl InitFormMainCtrl()
        {
            var ctrl = new Controller.FormMainCtrl();

            ctrl.Plug(new Controller.FormMainComponent.FlyServer(
                this,
                flyServerListContainer,
                toolStripLabelMarkFilter,
                toolStripComboBoxMarkFilter,
                toolStripStatusLabelTotal,
                toolStripDropDownButtonPager,
                toolStripStatusLabelPrePage,
                toolStripStatusLabelNextPage,
                toolStripMenuItemResize));

            ctrl.Plug(new Controller.FormMainComponent.MenuItemsBasic(
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

            ctrl.Plug(new Controller.FormMainComponent.MenuItemsSelect(
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

            ctrl.Plug(new Controller.FormMainComponent.MenuItemsServer(
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
                //ToolStripMenuItem speedTestOnSelected,
                //ToolStripMenuItem modifySelected,
                //ToolStripMenuItem stopSelected,
                //ToolStripMenuItem restartSelected,
                toolStripMenuItemSpeedTestOnSelected,
                toolStripMenuItemModifySettings,
                toolStripMenuItemStopSelected,
                toolStripMenuItemRestartSelected,

                //// view
                //ToolStripMenuItem moveToTop,
                //ToolStripMenuItem moveToBottom,
                //ToolStripMenuItem foldPanel,
                //ToolStripMenuItem expansePanel,
                //ToolStripMenuItem sortBySpeed,
                //ToolStripMenuItem sortBySummary)
                toolStripMenuItemMoveToTop,
                toolStripMenuItemMoveToBottom,
                toolStripMenuItemFoldingPanel,
                toolStripMenuItemExpansePanel,
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
