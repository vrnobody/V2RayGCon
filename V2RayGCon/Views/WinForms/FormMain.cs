using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormMain : Form
    {
        #region Sigleton
        static readonly VgcApis.BaseClasses.AuxSiWinForm<FormMain> auxSiForm =
            new VgcApis.BaseClasses.AuxSiWinForm<FormMain>();

        public static FormMain GetForm() => auxSiForm.GetForm();

        public static void ShowForm() => auxSiForm.ShowForm();
        #endregion

        Services.Settings setting;
        Services.ShareLinkMgr slinkMgr;

        Controllers.FormMainCtrl formMainCtrl;
        readonly string formTitle = "";

        public FormMain()
        {
            CreateHandle();
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            Misc.UI.AutoScaleToolStripControls(this, 16);

            formTitle = Misc.Utils.GetAppNameAndVer();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            slinkMgr = Services.ShareLinkMgr.Instance;
            setting = Services.Settings.Instance;

            setting.RestoreFormRect(this);

            formMainCtrl = InitFormMainCtrl();
            BindToolStripButtonToMenuItem();

            setting.OnPortableModeChanged += UpdateFormTitle;
            UpdateFormTitle(this, EventArgs.Empty);
        }

        #region private method

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmExitApp))
            {
                setting.SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser);
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
                title += " " + I18N.Portable;
            }

            if (VgcApis.Misc.Utils.IsAdmin())
            {
                title += " " + I18N.AdministratorMode;
            }

            VgcApis.Misc.UI.Invoke(() =>
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
                string text = VgcApis.Misc.Utils.ReadFromClipboard();
                slinkMgr.ImportLinkWithOutV2cfgLinks(text);
            };

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

            ctrl.Plug(
                new Controllers.FormMainComponent.FlyServer(
                    this,
                    flyServerListContainer,
                    toolStripLabelSearch,
                    toolStripComboBoxMarkFilter,
                    toolStripStatusLabelTotal,
                    toolStripDropDownButtonPager,
                    toolStripStatusLabelPrePage,
                    toolStripStatusLabelNextPage,
                    toolStripMenuItemResize
                )
            );

            ctrl.Plug(
                new Controllers.FormMainComponent.MenuItemsBasic(
                    pluginToolStripMenuItem,
                    toolMenuItemImportLinkFromClipboard,
                    toolMenuItemAbout,
                    toolMenuItemHelp,
                    toolStripMenuItemTextEditor,
                    toolStripMenuItemKeyGenForm,
                    toolMenuItemLog,
                    toolMenuItemOptions,
                    toolStripMenuItemDownLoadV2rayCore,
                    toolStripMenuItemCleanupProgramData,
                    toolMenuItemCheckUpdate
                )
            );

            ctrl.Plug(
                new Controllers.FormMainComponent.MenuItemsSelect(
                    /*
                    ToolStripMenuItem selectAllCurPage,
                    ToolStripMenuItem invertSelectionCurPage,
                    ToolStripMenuItem selectNoneCurPage,
                    ToolStripMenuItem selectAutorunCurPage,
                    ToolStripMenuItem selectRunningCurPage,
                    ToolStripMenuItem selectTimeoutCurPage,
                    ToolStripMenuItem selectNoSpeedTestCurPage,
                    ToolStripMenuItem selectNoMarkCurPage,
                    ToolStripMenuItem selectUntrackCurPage
                   */
                    selectAllCurPageToolStripMenuItem,
                    invertSelectionCurPageToolStripMenuItem,
                    selectNoneCurPageToolStripMenuItem1,
                    selectAutorunCurPageToolStripMenuItem,
                    selectRunningCurPageToolStripMenuItem,
                    selectTimeoutCurPageToolStripMenuItem,
                    selectNotSpeedTestedCurPageToolStripMenuItem,
                    selectNoMarkCurPageToolStripMenuItem,
                    selectUntrackCurPageToolStripMenuItem,
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
                    selectUntrackAllServersToolStripMenuItem
                )
            );

            ctrl.Plug(
                new Controllers.FormMainComponent.MenuItemsServer(
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
                    toolStripMenuItemCopyAsVmixLink,
                    //// batch op
                    toolStripMenuItemStopBatchSpeedTest,
                    toolStripMenuItemRunBatchSpeedTest,
                    toolStripMenuItemClearSpeedTestResults,
                    toolStripMenuItemClearStatisticsRecord,
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
                    toolStripMenuItemMoveToCustomIndex,
                    toolStripMenuItemReverseByIndex,
                    toolStripMenuItemSortBySpeedTest,
                    toolStripMenuItemSortByDateT,
                    toolStripMenuItemSortBySummary,
                    toolStripMenuItemSortByDownloadTotal,
                    toolStripMenuItemSortByUploadTotal
                )
            );

            return ctrl;
        }

        #endregion

        #region bind hotkey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyCode)
        {
            const int WM_KEYDOWN = 0x100;
            if (msg.Msg == WM_KEYDOWN)
            {
                switch (keyCode)
                {
                    case Keys.Control | Keys.F:
                        toolStripComboBoxMarkFilter.SelectAll();
                        toolStripComboBoxMarkFilter.Focus();
                        break;
                    default:
                        break;
                }
            }
            return base.ProcessCmdKey(ref msg, keyCode);
        }
        #endregion

        #region UI event handler

        readonly List<ToolStripItem> tsItems = new List<ToolStripItem>();

        List<ToolStripItem> GetAllToolStripItems()
        {
            lock (tsItems)
            {
                if (tsItems.Count < 1)
                {
                    var tmp = new List<ToolStripItem>();
                    foreach (ToolStripItem control in toolStrip1.Items)
                    {
                        if (
                            control == toolStripComboBoxMarkFilter
                            || control == toolStripLabelSearch
                        )
                        {
                            continue;
                        }
                        tmp.Add(control);
                    }
                    tsItems.AddRange(tmp.OrderBy(c => c.Bounds.Left));
                }
            }
            return tsItems;
        }

        bool isMarkFilterEntered = false;

        void FixFilterBoxSize()
        {
            var box = toolStripComboBoxMarkFilter;
            toolStrip1.SuspendLayout();
            if (!isMarkFilterEntered && box.Width != 1)
            {
                box.Width = 1;
            }
            foreach (var c in GetAllToolStripItems())
            {
                if (c.Visible == isMarkFilterEntered)
                {
                    c.Visible = !isMarkFilterEntered;
                }
            }
            toolStrip1.ResumeLayout();
            var margin = toolStripLabelSearch.Width;
            var size = this.Width - box.Bounds.Left - (4 * margin);
            if (box.Width != size)
            {
                box.Width = size;
            }
        }

        private void toolStripComboBoxMarkFilter_Enter(object sender, EventArgs e)
        {
            isMarkFilterEntered = true;
            FixFilterBoxSize();
        }

        private void toolStripComboBoxMarkFilter_Leave(object sender, EventArgs e)
        {
            isMarkFilterEntered = false;
            FixFilterBoxSize();
        }

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
            setting.OnPortableModeChanged -= UpdateFormTitle;
            formMainCtrl?.Cleanup();
            setting.SaveFormRect(this);
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            FixFilterBoxSize();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            FixFilterBoxSize();
            formMainCtrl?.ResetServUiBorders();
        }
        #endregion
    }
}
