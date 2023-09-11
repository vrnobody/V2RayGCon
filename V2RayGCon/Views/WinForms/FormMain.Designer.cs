using System.Windows.Forms;

namespace V2RayGCon.Views.WinForms
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.toolStripContainer2 = new System.Windows.Forms.ToolStripContainer();
            this.flyServerListContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddServerSimple = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonImportFromClipboard = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonScanQrcode = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSelectAllCurPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInverseSelectionCurPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSelectNoneCurPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAllServerSelectAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAllServerSelectNone = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonModifySelected = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRunSpeedTest = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSortSelectedBySpeedTestResult = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonFormOption = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonShowFormLog = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBoxMarkFilter = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabelSearch = new System.Windows.Forms.ToolStripLabel();
            this.mainMneuStrip = new System.Windows.Forms.MenuStrip();
            this.operationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemSimAddVmessServer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemImportLinkFromClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolMenuItemExportAllServerToFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemImportFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.closeWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllCurPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionCurPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneCurPageToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.allPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllAllPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionAllPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneAllPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.selectNoSpeedTestAllPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectTimeoutAllPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoMarkAllPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAutorunAllPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectRunningAllPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectUntrackAllPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.selectNoSpeedTestAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectTimeoutAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoMarkAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAutorunAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectRunningAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectUntrackAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemServer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsV2cfgLink = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsVmixLink = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsVeeLink = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsVmessSubscription = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsVeeSubscription = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMoveToTop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMoveToBottom = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemModifySelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemReverseByIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSortBySpeedTest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSortByDateT = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSortBySummary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSortByDownloadTotal = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSortByUploadTotal = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemRestartSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStopSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemRunBatchSpeedTest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStopBatchSpeedTest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClearSpeedTestResults = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClearStatisticsRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemModifySettings = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshSummaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemConfigEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemLog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemResize = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDownLoadV2rayCore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemCheckUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemRemoveV2rayCore = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownButtonPager = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripStatusLabelPrePage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelNextPage = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStripContainer2.ContentPanel.SuspendLayout();
            this.toolStripContainer2.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.mainMneuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer2
            // 
            resources.ApplyResources(this.toolStripContainer2, "toolStripContainer2");
            // 
            // toolStripContainer2.BottomToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer2.BottomToolStripPanel, "toolStripContainer2.BottomToolStripPanel");
            this.toolTip1.SetToolTip(this.toolStripContainer2.BottomToolStripPanel, resources.GetString("toolStripContainer2.BottomToolStripPanel.ToolTip"));
            this.toolStripContainer2.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer2.ContentPanel
            // 
            resources.ApplyResources(this.toolStripContainer2.ContentPanel, "toolStripContainer2.ContentPanel");
            this.toolStripContainer2.ContentPanel.Controls.Add(this.flyServerListContainer);
            this.toolTip1.SetToolTip(this.toolStripContainer2.ContentPanel, resources.GetString("toolStripContainer2.ContentPanel.ToolTip"));
            // 
            // toolStripContainer2.LeftToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer2.LeftToolStripPanel, "toolStripContainer2.LeftToolStripPanel");
            this.toolTip1.SetToolTip(this.toolStripContainer2.LeftToolStripPanel, resources.GetString("toolStripContainer2.LeftToolStripPanel.ToolTip"));
            this.toolStripContainer2.LeftToolStripPanelVisible = false;
            this.toolStripContainer2.Name = "toolStripContainer2";
            // 
            // toolStripContainer2.RightToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer2.RightToolStripPanel, "toolStripContainer2.RightToolStripPanel");
            this.toolTip1.SetToolTip(this.toolStripContainer2.RightToolStripPanel, resources.GetString("toolStripContainer2.RightToolStripPanel.ToolTip"));
            this.toolStripContainer2.RightToolStripPanelVisible = false;
            this.toolTip1.SetToolTip(this.toolStripContainer2, resources.GetString("toolStripContainer2.ToolTip"));
            // 
            // toolStripContainer2.TopToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer2.TopToolStripPanel, "toolStripContainer2.TopToolStripPanel");
            this.toolStripContainer2.TopToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolTip1.SetToolTip(this.toolStripContainer2.TopToolStripPanel, resources.GetString("toolStripContainer2.TopToolStripPanel.ToolTip"));
            // 
            // flyServerListContainer
            // 
            resources.ApplyResources(this.flyServerListContainer, "flyServerListContainer");
            this.flyServerListContainer.AllowDrop = true;
            this.flyServerListContainer.BackColor = System.Drawing.Color.White;
            this.flyServerListContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flyServerListContainer.Name = "flyServerListContainer";
            this.toolTip1.SetToolTip(this.flyServerListContainer, resources.GetString("flyServerListContainer.ToolTip"));
            this.flyServerListContainer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.flyServerListContainer_Scroll);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddServerSimple,
            this.toolStripButtonImportFromClipboard,
            this.toolStripButtonScanQrcode,
            this.toolStripSeparator2,
            this.toolStripButtonSelectAllCurPage,
            this.toolStripButtonInverseSelectionCurPage,
            this.toolStripButtonSelectNoneCurPage,
            this.toolStripSeparator13,
            this.toolStripButtonAllServerSelectAll,
            this.toolStripButtonAllServerSelectNone,
            this.toolStripSeparator6,
            this.toolStripButtonModifySelected,
            this.toolStripButtonRunSpeedTest,
            this.toolStripButtonSortSelectedBySpeedTestResult,
            this.toolStripSeparator9,
            this.toolStripButtonFormOption,
            this.toolStripSeparator7,
            this.toolStripButtonShowFormLog,
            this.toolStripSeparator10,
            this.toolStripComboBoxMarkFilter,
            this.toolStripLabelSearch});
            this.toolStrip1.Name = "toolStrip1";
            this.toolTip1.SetToolTip(this.toolStrip1, resources.GetString("toolStrip1.ToolTip"));
            // 
            // toolStripButtonAddServerSimple
            // 
            resources.ApplyResources(this.toolStripButtonAddServerSimple, "toolStripButtonAddServerSimple");
            this.toolStripButtonAddServerSimple.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddServerSimple.Name = "toolStripButtonAddServerSimple";
            // 
            // toolStripButtonImportFromClipboard
            // 
            resources.ApplyResources(this.toolStripButtonImportFromClipboard, "toolStripButtonImportFromClipboard");
            this.toolStripButtonImportFromClipboard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonImportFromClipboard.Name = "toolStripButtonImportFromClipboard";
            // 
            // toolStripButtonScanQrcode
            // 
            resources.ApplyResources(this.toolStripButtonScanQrcode, "toolStripButtonScanQrcode");
            this.toolStripButtonScanQrcode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonScanQrcode.Name = "toolStripButtonScanQrcode";
            this.toolStripButtonScanQrcode.Click += new System.EventHandler(this.ToolStripButtonScanQrcode_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripButtonSelectAllCurPage
            // 
            resources.ApplyResources(this.toolStripButtonSelectAllCurPage, "toolStripButtonSelectAllCurPage");
            this.toolStripButtonSelectAllCurPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelectAllCurPage.Name = "toolStripButtonSelectAllCurPage";
            // 
            // toolStripButtonInverseSelectionCurPage
            // 
            resources.ApplyResources(this.toolStripButtonInverseSelectionCurPage, "toolStripButtonInverseSelectionCurPage");
            this.toolStripButtonInverseSelectionCurPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonInverseSelectionCurPage.Name = "toolStripButtonInverseSelectionCurPage";
            // 
            // toolStripButtonSelectNoneCurPage
            // 
            resources.ApplyResources(this.toolStripButtonSelectNoneCurPage, "toolStripButtonSelectNoneCurPage");
            this.toolStripButtonSelectNoneCurPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelectNoneCurPage.Name = "toolStripButtonSelectNoneCurPage";
            // 
            // toolStripSeparator13
            // 
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            // 
            // toolStripButtonAllServerSelectAll
            // 
            resources.ApplyResources(this.toolStripButtonAllServerSelectAll, "toolStripButtonAllServerSelectAll");
            this.toolStripButtonAllServerSelectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAllServerSelectAll.Name = "toolStripButtonAllServerSelectAll";
            // 
            // toolStripButtonAllServerSelectNone
            // 
            resources.ApplyResources(this.toolStripButtonAllServerSelectNone, "toolStripButtonAllServerSelectNone");
            this.toolStripButtonAllServerSelectNone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAllServerSelectNone.Name = "toolStripButtonAllServerSelectNone";
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // toolStripButtonModifySelected
            // 
            resources.ApplyResources(this.toolStripButtonModifySelected, "toolStripButtonModifySelected");
            this.toolStripButtonModifySelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonModifySelected.Name = "toolStripButtonModifySelected";
            // 
            // toolStripButtonRunSpeedTest
            // 
            resources.ApplyResources(this.toolStripButtonRunSpeedTest, "toolStripButtonRunSpeedTest");
            this.toolStripButtonRunSpeedTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRunSpeedTest.Name = "toolStripButtonRunSpeedTest";
            // 
            // toolStripButtonSortSelectedBySpeedTestResult
            // 
            resources.ApplyResources(this.toolStripButtonSortSelectedBySpeedTestResult, "toolStripButtonSortSelectedBySpeedTestResult");
            this.toolStripButtonSortSelectedBySpeedTestResult.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSortSelectedBySpeedTestResult.Name = "toolStripButtonSortSelectedBySpeedTestResult";
            // 
            // toolStripSeparator9
            // 
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            // 
            // toolStripButtonFormOption
            // 
            resources.ApplyResources(this.toolStripButtonFormOption, "toolStripButtonFormOption");
            this.toolStripButtonFormOption.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFormOption.Name = "toolStripButtonFormOption";
            // 
            // toolStripSeparator7
            // 
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            // 
            // toolStripButtonShowFormLog
            // 
            resources.ApplyResources(this.toolStripButtonShowFormLog, "toolStripButtonShowFormLog");
            this.toolStripButtonShowFormLog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowFormLog.Name = "toolStripButtonShowFormLog";
            // 
            // toolStripSeparator10
            // 
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            // 
            // toolStripComboBoxMarkFilter
            // 
            resources.ApplyResources(this.toolStripComboBoxMarkFilter, "toolStripComboBoxMarkFilter");
            this.toolStripComboBoxMarkFilter.Name = "toolStripComboBoxMarkFilter";
            // 
            // toolStripLabelSearch
            // 
            resources.ApplyResources(this.toolStripLabelSearch, "toolStripLabelSearch");
            this.toolStripLabelSearch.Name = "toolStripLabelSearch";
            // 
            // mainMneuStrip
            // 
            resources.ApplyResources(this.mainMneuStrip, "mainMneuStrip");
            this.mainMneuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMneuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.operationToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.toolMenuItemServer,
            this.windowToolStripMenuItem,
            this.pluginToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.mainMneuStrip.Name = "mainMneuStrip";
            this.toolTip1.SetToolTip(this.mainMneuStrip, resources.GetString("mainMneuStrip.ToolTip"));
            // 
            // operationToolStripMenuItem
            // 
            resources.ApplyResources(this.operationToolStripMenuItem, "operationToolStripMenuItem");
            this.operationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolMenuItemSimAddVmessServer,
            this.toolMenuItemImportLinkFromClipboard,
            this.toolStripSeparator5,
            this.toolMenuItemExportAllServerToFile,
            this.toolMenuItemImportFromFile,
            this.toolStripSeparator8,
            this.closeWindowToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.operationToolStripMenuItem.Name = "operationToolStripMenuItem";
            // 
            // toolMenuItemSimAddVmessServer
            // 
            resources.ApplyResources(this.toolMenuItemSimAddVmessServer, "toolMenuItemSimAddVmessServer");
            this.toolMenuItemSimAddVmessServer.Name = "toolMenuItemSimAddVmessServer";
            // 
            // toolMenuItemImportLinkFromClipboard
            // 
            resources.ApplyResources(this.toolMenuItemImportLinkFromClipboard, "toolMenuItemImportLinkFromClipboard");
            this.toolMenuItemImportLinkFromClipboard.Name = "toolMenuItemImportLinkFromClipboard";
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // toolMenuItemExportAllServerToFile
            // 
            resources.ApplyResources(this.toolMenuItemExportAllServerToFile, "toolMenuItemExportAllServerToFile");
            this.toolMenuItemExportAllServerToFile.Name = "toolMenuItemExportAllServerToFile";
            // 
            // toolMenuItemImportFromFile
            // 
            resources.ApplyResources(this.toolMenuItemImportFromFile, "toolMenuItemImportFromFile");
            this.toolMenuItemImportFromFile.Name = "toolMenuItemImportFromFile";
            // 
            // toolStripSeparator8
            // 
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            // 
            // closeWindowToolStripMenuItem
            // 
            resources.ApplyResources(this.closeWindowToolStripMenuItem, "closeWindowToolStripMenuItem");
            this.closeWindowToolStripMenuItem.Name = "closeWindowToolStripMenuItem";
            this.closeWindowToolStripMenuItem.Click += new System.EventHandler(this.closeWindowToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Image = global::V2RayGCon.Properties.Resources.CloseSolution_16x;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            resources.ApplyResources(this.selectToolStripMenuItem, "selectToolStripMenuItem");
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentPageToolStripMenuItem,
            this.allPagesToolStripMenuItem,
            this.allServersToolStripMenuItem});
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            // 
            // currentPageToolStripMenuItem
            // 
            resources.ApplyResources(this.currentPageToolStripMenuItem, "currentPageToolStripMenuItem");
            this.currentPageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllCurPageToolStripMenuItem,
            this.invertSelectionCurPageToolStripMenuItem,
            this.selectNoneCurPageToolStripMenuItem1});
            this.currentPageToolStripMenuItem.Name = "currentPageToolStripMenuItem";
            // 
            // selectAllCurPageToolStripMenuItem
            // 
            resources.ApplyResources(this.selectAllCurPageToolStripMenuItem, "selectAllCurPageToolStripMenuItem");
            this.selectAllCurPageToolStripMenuItem.Name = "selectAllCurPageToolStripMenuItem";
            // 
            // invertSelectionCurPageToolStripMenuItem
            // 
            resources.ApplyResources(this.invertSelectionCurPageToolStripMenuItem, "invertSelectionCurPageToolStripMenuItem");
            this.invertSelectionCurPageToolStripMenuItem.Name = "invertSelectionCurPageToolStripMenuItem";
            // 
            // selectNoneCurPageToolStripMenuItem1
            // 
            resources.ApplyResources(this.selectNoneCurPageToolStripMenuItem1, "selectNoneCurPageToolStripMenuItem1");
            this.selectNoneCurPageToolStripMenuItem1.Name = "selectNoneCurPageToolStripMenuItem1";
            // 
            // allPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.allPagesToolStripMenuItem, "allPagesToolStripMenuItem");
            this.allPagesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllAllPagesToolStripMenuItem,
            this.invertSelectionAllPagesToolStripMenuItem,
            this.selectNoneAllPagesToolStripMenuItem,
            this.toolStripMenuItem3,
            this.selectNoSpeedTestAllPagesToolStripMenuItem,
            this.selectTimeoutAllPagesToolStripMenuItem,
            this.selectNoMarkAllPagesToolStripMenuItem,
            this.selectAutorunAllPagesToolStripMenuItem,
            this.selectRunningAllPagesToolStripMenuItem,
            this.selectUntrackAllPagesToolStripMenuItem});
            this.allPagesToolStripMenuItem.Name = "allPagesToolStripMenuItem";
            // 
            // selectAllAllPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.selectAllAllPagesToolStripMenuItem, "selectAllAllPagesToolStripMenuItem");
            this.selectAllAllPagesToolStripMenuItem.Name = "selectAllAllPagesToolStripMenuItem";
            // 
            // invertSelectionAllPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.invertSelectionAllPagesToolStripMenuItem, "invertSelectionAllPagesToolStripMenuItem");
            this.invertSelectionAllPagesToolStripMenuItem.Name = "invertSelectionAllPagesToolStripMenuItem";
            // 
            // selectNoneAllPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.selectNoneAllPagesToolStripMenuItem, "selectNoneAllPagesToolStripMenuItem");
            this.selectNoneAllPagesToolStripMenuItem.Name = "selectNoneAllPagesToolStripMenuItem";
            // 
            // toolStripMenuItem3
            // 
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            // 
            // selectNoSpeedTestAllPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.selectNoSpeedTestAllPagesToolStripMenuItem, "selectNoSpeedTestAllPagesToolStripMenuItem");
            this.selectNoSpeedTestAllPagesToolStripMenuItem.Name = "selectNoSpeedTestAllPagesToolStripMenuItem";
            // 
            // selectTimeoutAllPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.selectTimeoutAllPagesToolStripMenuItem, "selectTimeoutAllPagesToolStripMenuItem");
            this.selectTimeoutAllPagesToolStripMenuItem.Name = "selectTimeoutAllPagesToolStripMenuItem";
            // 
            // selectNoMarkAllPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.selectNoMarkAllPagesToolStripMenuItem, "selectNoMarkAllPagesToolStripMenuItem");
            this.selectNoMarkAllPagesToolStripMenuItem.Name = "selectNoMarkAllPagesToolStripMenuItem";
            // 
            // selectAutorunAllPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.selectAutorunAllPagesToolStripMenuItem, "selectAutorunAllPagesToolStripMenuItem");
            this.selectAutorunAllPagesToolStripMenuItem.Name = "selectAutorunAllPagesToolStripMenuItem";
            // 
            // selectRunningAllPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.selectRunningAllPagesToolStripMenuItem, "selectRunningAllPagesToolStripMenuItem");
            this.selectRunningAllPagesToolStripMenuItem.Name = "selectRunningAllPagesToolStripMenuItem";
            // 
            // selectUntrackAllPagesToolStripMenuItem
            // 
            resources.ApplyResources(this.selectUntrackAllPagesToolStripMenuItem, "selectUntrackAllPagesToolStripMenuItem");
            this.selectUntrackAllPagesToolStripMenuItem.Name = "selectUntrackAllPagesToolStripMenuItem";
            // 
            // allServersToolStripMenuItem
            // 
            resources.ApplyResources(this.allServersToolStripMenuItem, "allServersToolStripMenuItem");
            this.allServersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllAllServersToolStripMenuItem,
            this.invertSelectionAllServersToolStripMenuItem,
            this.selectNoneAllServersToolStripMenuItem,
            this.toolStripMenuItem2,
            this.selectNoSpeedTestAllServersToolStripMenuItem,
            this.selectTimeoutAllServersToolStripMenuItem,
            this.selectNoMarkAllServersToolStripMenuItem,
            this.selectAutorunAllServersToolStripMenuItem,
            this.selectRunningAllServersToolStripMenuItem,
            this.selectUntrackAllServersToolStripMenuItem});
            this.allServersToolStripMenuItem.Name = "allServersToolStripMenuItem";
            // 
            // selectAllAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.selectAllAllServersToolStripMenuItem, "selectAllAllServersToolStripMenuItem");
            this.selectAllAllServersToolStripMenuItem.Name = "selectAllAllServersToolStripMenuItem";
            // 
            // invertSelectionAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.invertSelectionAllServersToolStripMenuItem, "invertSelectionAllServersToolStripMenuItem");
            this.invertSelectionAllServersToolStripMenuItem.Name = "invertSelectionAllServersToolStripMenuItem";
            // 
            // selectNoneAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.selectNoneAllServersToolStripMenuItem, "selectNoneAllServersToolStripMenuItem");
            this.selectNoneAllServersToolStripMenuItem.Name = "selectNoneAllServersToolStripMenuItem";
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // selectNoSpeedTestAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.selectNoSpeedTestAllServersToolStripMenuItem, "selectNoSpeedTestAllServersToolStripMenuItem");
            this.selectNoSpeedTestAllServersToolStripMenuItem.Name = "selectNoSpeedTestAllServersToolStripMenuItem";
            // 
            // selectTimeoutAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.selectTimeoutAllServersToolStripMenuItem, "selectTimeoutAllServersToolStripMenuItem");
            this.selectTimeoutAllServersToolStripMenuItem.Name = "selectTimeoutAllServersToolStripMenuItem";
            // 
            // selectNoMarkAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.selectNoMarkAllServersToolStripMenuItem, "selectNoMarkAllServersToolStripMenuItem");
            this.selectNoMarkAllServersToolStripMenuItem.Name = "selectNoMarkAllServersToolStripMenuItem";
            // 
            // selectAutorunAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.selectAutorunAllServersToolStripMenuItem, "selectAutorunAllServersToolStripMenuItem");
            this.selectAutorunAllServersToolStripMenuItem.Name = "selectAutorunAllServersToolStripMenuItem";
            // 
            // selectRunningAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.selectRunningAllServersToolStripMenuItem, "selectRunningAllServersToolStripMenuItem");
            this.selectRunningAllServersToolStripMenuItem.Name = "selectRunningAllServersToolStripMenuItem";
            // 
            // selectUntrackAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.selectUntrackAllServersToolStripMenuItem, "selectUntrackAllServersToolStripMenuItem");
            this.selectUntrackAllServersToolStripMenuItem.Name = "selectUntrackAllServersToolStripMenuItem";
            // 
            // toolMenuItemServer
            // 
            resources.ApplyResources(this.toolMenuItemServer, "toolMenuItemServer");
            this.toolMenuItemServer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem7,
            this.toolStripMenuItemModifySelected,
            this.toolStripSeparator12,
            this.toolStripMenuItemRestartSelected,
            this.toolStripMenuItemStopSelected,
            this.toolStripSeparator1,
            this.toolStripMenuItemRunBatchSpeedTest,
            this.toolStripMenuItemStopBatchSpeedTest,
            this.toolStripMenuItemClearSpeedTestResults,
            this.toolStripMenuItemClearStatisticsRecord,
            this.toolStripMenuItem6,
            this.toolStripMenuItemModifySettings,
            this.refreshSummaryToolStripMenuItem,
            this.deleteSelectedServersToolStripMenuItem});
            this.toolMenuItemServer.Name = "toolMenuItemServer";
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCopyAsV2cfgLink,
            this.toolStripMenuItemCopyAsVmixLink,
            this.toolStripMenuItemCopyAsVeeLink,
            this.toolStripMenuItemCopyAsVmessSubscription,
            this.toolStripMenuItemCopyAsVeeSubscription});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // toolStripMenuItemCopyAsV2cfgLink
            // 
            resources.ApplyResources(this.toolStripMenuItemCopyAsV2cfgLink, "toolStripMenuItemCopyAsV2cfgLink");
            this.toolStripMenuItemCopyAsV2cfgLink.Name = "toolStripMenuItemCopyAsV2cfgLink";
            // 
            // toolStripMenuItemCopyAsVmixLink
            // 
            resources.ApplyResources(this.toolStripMenuItemCopyAsVmixLink, "toolStripMenuItemCopyAsVmixLink");
            this.toolStripMenuItemCopyAsVmixLink.Name = "toolStripMenuItemCopyAsVmixLink";
            // 
            // toolStripMenuItemCopyAsVeeLink
            // 
            resources.ApplyResources(this.toolStripMenuItemCopyAsVeeLink, "toolStripMenuItemCopyAsVeeLink");
            this.toolStripMenuItemCopyAsVeeLink.Name = "toolStripMenuItemCopyAsVeeLink";
            // 
            // toolStripMenuItemCopyAsVmessSubscription
            // 
            resources.ApplyResources(this.toolStripMenuItemCopyAsVmessSubscription, "toolStripMenuItemCopyAsVmessSubscription");
            this.toolStripMenuItemCopyAsVmessSubscription.Name = "toolStripMenuItemCopyAsVmessSubscription";
            // 
            // toolStripMenuItemCopyAsVeeSubscription
            // 
            resources.ApplyResources(this.toolStripMenuItemCopyAsVeeSubscription, "toolStripMenuItemCopyAsVeeSubscription");
            this.toolStripMenuItemCopyAsVeeSubscription.Name = "toolStripMenuItemCopyAsVeeSubscription";
            // 
            // toolStripMenuItem7
            // 
            resources.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");
            this.toolStripMenuItem7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemMoveToTop,
            this.toolStripMenuItemMoveToBottom});
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            // 
            // toolStripMenuItemMoveToTop
            // 
            resources.ApplyResources(this.toolStripMenuItemMoveToTop, "toolStripMenuItemMoveToTop");
            this.toolStripMenuItemMoveToTop.Name = "toolStripMenuItemMoveToTop";
            // 
            // toolStripMenuItemMoveToBottom
            // 
            resources.ApplyResources(this.toolStripMenuItemMoveToBottom, "toolStripMenuItemMoveToBottom");
            this.toolStripMenuItemMoveToBottom.Name = "toolStripMenuItemMoveToBottom";
            // 
            // toolStripMenuItemModifySelected
            // 
            resources.ApplyResources(this.toolStripMenuItemModifySelected, "toolStripMenuItemModifySelected");
            this.toolStripMenuItemModifySelected.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemReverseByIndex,
            this.toolStripSeparator3,
            this.toolStripMenuItemSortBySpeedTest,
            this.toolStripMenuItemSortByDateT,
            this.toolStripMenuItemSortBySummary,
            this.toolStripMenuItemSortByDownloadTotal,
            this.toolStripMenuItemSortByUploadTotal});
            this.toolStripMenuItemModifySelected.Name = "toolStripMenuItemModifySelected";
            // 
            // toolStripMenuItemReverseByIndex
            // 
            resources.ApplyResources(this.toolStripMenuItemReverseByIndex, "toolStripMenuItemReverseByIndex");
            this.toolStripMenuItemReverseByIndex.Name = "toolStripMenuItemReverseByIndex";
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // toolStripMenuItemSortBySpeedTest
            // 
            resources.ApplyResources(this.toolStripMenuItemSortBySpeedTest, "toolStripMenuItemSortBySpeedTest");
            this.toolStripMenuItemSortBySpeedTest.Name = "toolStripMenuItemSortBySpeedTest";
            // 
            // toolStripMenuItemSortByDateT
            // 
            resources.ApplyResources(this.toolStripMenuItemSortByDateT, "toolStripMenuItemSortByDateT");
            this.toolStripMenuItemSortByDateT.Name = "toolStripMenuItemSortByDateT";
            // 
            // toolStripMenuItemSortBySummary
            // 
            resources.ApplyResources(this.toolStripMenuItemSortBySummary, "toolStripMenuItemSortBySummary");
            this.toolStripMenuItemSortBySummary.Name = "toolStripMenuItemSortBySummary";
            // 
            // toolStripMenuItemSortByDownloadTotal
            // 
            resources.ApplyResources(this.toolStripMenuItemSortByDownloadTotal, "toolStripMenuItemSortByDownloadTotal");
            this.toolStripMenuItemSortByDownloadTotal.Name = "toolStripMenuItemSortByDownloadTotal";
            // 
            // toolStripMenuItemSortByUploadTotal
            // 
            resources.ApplyResources(this.toolStripMenuItemSortByUploadTotal, "toolStripMenuItemSortByUploadTotal");
            this.toolStripMenuItemSortByUploadTotal.Name = "toolStripMenuItemSortByUploadTotal";
            // 
            // toolStripSeparator12
            // 
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            // 
            // toolStripMenuItemRestartSelected
            // 
            resources.ApplyResources(this.toolStripMenuItemRestartSelected, "toolStripMenuItemRestartSelected");
            this.toolStripMenuItemRestartSelected.Name = "toolStripMenuItemRestartSelected";
            // 
            // toolStripMenuItemStopSelected
            // 
            resources.ApplyResources(this.toolStripMenuItemStopSelected, "toolStripMenuItemStopSelected");
            this.toolStripMenuItemStopSelected.Name = "toolStripMenuItemStopSelected";
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // toolStripMenuItemRunBatchSpeedTest
            // 
            resources.ApplyResources(this.toolStripMenuItemRunBatchSpeedTest, "toolStripMenuItemRunBatchSpeedTest");
            this.toolStripMenuItemRunBatchSpeedTest.Name = "toolStripMenuItemRunBatchSpeedTest";
            // 
            // toolStripMenuItemStopBatchSpeedTest
            // 
            resources.ApplyResources(this.toolStripMenuItemStopBatchSpeedTest, "toolStripMenuItemStopBatchSpeedTest");
            this.toolStripMenuItemStopBatchSpeedTest.Name = "toolStripMenuItemStopBatchSpeedTest";
            // 
            // toolStripMenuItemClearSpeedTestResults
            // 
            resources.ApplyResources(this.toolStripMenuItemClearSpeedTestResults, "toolStripMenuItemClearSpeedTestResults");
            this.toolStripMenuItemClearSpeedTestResults.Name = "toolStripMenuItemClearSpeedTestResults";
            // 
            // toolStripMenuItemClearStatisticsRecord
            // 
            resources.ApplyResources(this.toolStripMenuItemClearStatisticsRecord, "toolStripMenuItemClearStatisticsRecord");
            this.toolStripMenuItemClearStatisticsRecord.Name = "toolStripMenuItemClearStatisticsRecord";
            // 
            // toolStripMenuItem6
            // 
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            // 
            // toolStripMenuItemModifySettings
            // 
            resources.ApplyResources(this.toolStripMenuItemModifySettings, "toolStripMenuItemModifySettings");
            this.toolStripMenuItemModifySettings.Image = global::V2RayGCon.Properties.Resources.EditWindow_16x;
            this.toolStripMenuItemModifySettings.Name = "toolStripMenuItemModifySettings";
            // 
            // refreshSummaryToolStripMenuItem
            // 
            resources.ApplyResources(this.refreshSummaryToolStripMenuItem, "refreshSummaryToolStripMenuItem");
            this.refreshSummaryToolStripMenuItem.Name = "refreshSummaryToolStripMenuItem";
            // 
            // deleteSelectedServersToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteSelectedServersToolStripMenuItem, "deleteSelectedServersToolStripMenuItem");
            this.deleteSelectedServersToolStripMenuItem.Name = "deleteSelectedServersToolStripMenuItem";
            // 
            // windowToolStripMenuItem
            // 
            resources.ApplyResources(this.windowToolStripMenuItem, "windowToolStripMenuItem");
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolMenuItemConfigEditor,
            this.toolMenuItemLog,
            this.toolMenuItemOptions,
            this.toolStripMenuItem4,
            this.toolStripMenuItemResize});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            // 
            // toolMenuItemConfigEditor
            // 
            resources.ApplyResources(this.toolMenuItemConfigEditor, "toolMenuItemConfigEditor");
            this.toolMenuItemConfigEditor.Image = global::V2RayGCon.Properties.Resources.EditWindow_16x;
            this.toolMenuItemConfigEditor.Name = "toolMenuItemConfigEditor";
            // 
            // toolMenuItemLog
            // 
            resources.ApplyResources(this.toolMenuItemLog, "toolMenuItemLog");
            this.toolMenuItemLog.Image = global::V2RayGCon.Properties.Resources.FSInteractiveWindow_16x;
            this.toolMenuItemLog.Name = "toolMenuItemLog";
            // 
            // toolMenuItemOptions
            // 
            resources.ApplyResources(this.toolMenuItemOptions, "toolMenuItemOptions");
            this.toolMenuItemOptions.Image = global::V2RayGCon.Properties.Resources.Settings_16x;
            this.toolMenuItemOptions.Name = "toolMenuItemOptions";
            // 
            // toolStripMenuItem4
            // 
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            // 
            // toolStripMenuItemResize
            // 
            resources.ApplyResources(this.toolStripMenuItemResize, "toolStripMenuItemResize");
            this.toolStripMenuItemResize.Name = "toolStripMenuItemResize";
            // 
            // pluginToolStripMenuItem
            // 
            resources.ApplyResources(this.pluginToolStripMenuItem, "pluginToolStripMenuItem");
            this.pluginToolStripMenuItem.Name = "pluginToolStripMenuItem";
            // 
            // aboutToolStripMenuItem1
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem1, "aboutToolStripMenuItem1");
            this.aboutToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemDownLoadV2rayCore,
            this.toolMenuItemCheckUpdate,
            this.toolMenuItemAbout,
            this.toolMenuItemHelp,
            this.toolStripMenuItem5,
            this.toolStripMenuItemRemoveV2rayCore,
            this.deleteAllServersToolStripMenuItem});
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            // 
            // toolStripMenuItemDownLoadV2rayCore
            // 
            resources.ApplyResources(this.toolStripMenuItemDownLoadV2rayCore, "toolStripMenuItemDownLoadV2rayCore");
            this.toolStripMenuItemDownLoadV2rayCore.Image = global::V2RayGCon.Properties.Resources.ASX_TransferDownload_blue_16x;
            this.toolStripMenuItemDownLoadV2rayCore.Name = "toolStripMenuItemDownLoadV2rayCore";
            // 
            // toolMenuItemCheckUpdate
            // 
            resources.ApplyResources(this.toolMenuItemCheckUpdate, "toolMenuItemCheckUpdate");
            this.toolMenuItemCheckUpdate.Name = "toolMenuItemCheckUpdate";
            // 
            // toolMenuItemAbout
            // 
            resources.ApplyResources(this.toolMenuItemAbout, "toolMenuItemAbout");
            this.toolMenuItemAbout.Name = "toolMenuItemAbout";
            // 
            // toolMenuItemHelp
            // 
            resources.ApplyResources(this.toolMenuItemHelp, "toolMenuItemHelp");
            this.toolMenuItemHelp.Image = global::V2RayGCon.Properties.Resources.StatusHelp_16x;
            this.toolMenuItemHelp.Name = "toolMenuItemHelp";
            // 
            // toolStripMenuItem5
            // 
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            // 
            // toolStripMenuItemRemoveV2rayCore
            // 
            resources.ApplyResources(this.toolStripMenuItemRemoveV2rayCore, "toolStripMenuItemRemoveV2rayCore");
            this.toolStripMenuItemRemoveV2rayCore.Name = "toolStripMenuItemRemoveV2rayCore";
            // 
            // deleteAllServersToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteAllServersToolStripMenuItem, "deleteAllServersToolStripMenuItem");
            this.deleteAllServersToolStripMenuItem.Name = "deleteAllServersToolStripMenuItem";
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelTotal,
            this.toolStripStatusLabel1,
            this.toolStripDropDownButtonPager,
            this.toolStripStatusLabelPrePage,
            this.toolStripStatusLabelNextPage});
            this.statusStrip1.Name = "statusStrip1";
            this.toolTip1.SetToolTip(this.statusStrip1, resources.GetString("statusStrip1.ToolTip"));
            // 
            // toolStripStatusLabelTotal
            // 
            resources.ApplyResources(this.toolStripStatusLabelTotal, "toolStripStatusLabelTotal");
            this.toolStripStatusLabelTotal.Name = "toolStripStatusLabelTotal";
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Spring = true;
            // 
            // toolStripDropDownButtonPager
            // 
            resources.ApplyResources(this.toolStripDropDownButtonPager, "toolStripDropDownButtonPager");
            this.toolStripDropDownButtonPager.Name = "toolStripDropDownButtonPager";
            // 
            // toolStripStatusLabelPrePage
            // 
            resources.ApplyResources(this.toolStripStatusLabelPrePage, "toolStripStatusLabelPrePage");
            this.toolStripStatusLabelPrePage.Name = "toolStripStatusLabelPrePage";
            // 
            // toolStripStatusLabelNextPage
            // 
            resources.ApplyResources(this.toolStripStatusLabelNextPage, "toolStripStatusLabelNextPage");
            this.toolStripStatusLabelNextPage.Name = "toolStripStatusLabelNextPage";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.toolStripContainer2);
            this.panel1.Name = "panel1";
            this.toolTip1.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainMneuStrip);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.mainMneuStrip;
            this.Name = "FormMain";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.toolStripContainer2.ContentPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.PerformLayout();
            this.toolStripContainer2.ResumeLayout(false);
            this.toolStripContainer2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.mainMneuStrip.ResumeLayout(false);
            this.mainMneuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMneuStrip;
        private System.Windows.Forms.ToolStripMenuItem operationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolMenuItemImportLinkFromClipboard;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolMenuItemConfigEditor;
        private System.Windows.Forms.ToolStripMenuItem toolMenuItemLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem toolMenuItemSimAddVmessServer;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolMenuItemCheckUpdate;
        private System.Windows.Forms.ToolStripMenuItem toolMenuItemAbout;
        private System.Windows.Forms.ToolStripMenuItem toolMenuItemExportAllServerToFile;
        private System.Windows.Forms.ToolStripMenuItem toolMenuItemImportFromFile;
        private ToolStripMenuItem toolMenuItemHelp;
        private ToolStripMenuItem toolMenuItemOptions;
        private FlowLayoutPanel flyServerListContainer;
        private ToolStripMenuItem toolMenuItemServer;
        private ToolStripMenuItem toolStripMenuItemStopSelected;
        private ToolStripMenuItem toolStripMenuItemRestartSelected;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toolStripMenuItemRunBatchSpeedTest;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItemCopyAsV2cfgLink;
        private ToolStripMenuItem toolStripMenuItemCopyAsVmixLink;
        private ToolStripMenuItem toolStripMenuItemDownLoadV2rayCore;
        private ToolStripMenuItem toolStripMenuItemRemoveV2rayCore;
        private ToolStripMenuItem toolStripMenuItemCopyAsVmessSubscription;
        private ToolStripMenuItem toolStripMenuItemModifySelected;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItemSortBySpeedTest;
        private ToolStripMenuItem selectToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemSortBySummary;
        private ToolStripContainer toolStripContainer2;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelTotal;
        private Panel panel1;
        private ToolStripDropDownButton toolStripDropDownButtonPager;
        private ToolStripStatusLabel toolStripStatusLabelPrePage;
        private ToolStripStatusLabel toolStripStatusLabelNextPage;
        private ToolStripMenuItem currentPageToolStripMenuItem;
        private ToolStripMenuItem selectAllCurPageToolStripMenuItem;
        private ToolStripMenuItem invertSelectionCurPageToolStripMenuItem;
        private ToolStripMenuItem selectNoneCurPageToolStripMenuItem1;
        private ToolStripMenuItem allPagesToolStripMenuItem;
        private ToolStripMenuItem selectAllAllPagesToolStripMenuItem;
        private ToolStripMenuItem invertSelectionAllPagesToolStripMenuItem;
        private ToolStripMenuItem selectNoneAllPagesToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem selectNoSpeedTestAllPagesToolStripMenuItem;
        private ToolStripMenuItem selectTimeoutAllPagesToolStripMenuItem;
        private ToolStripMenuItem selectNoMarkAllPagesToolStripMenuItem;
        private ToolStripMenuItem selectAutorunAllPagesToolStripMenuItem;
        private ToolStripMenuItem selectRunningAllPagesToolStripMenuItem;
        private ToolStripMenuItem allServersToolStripMenuItem;
        private ToolStripMenuItem selectAllAllServersToolStripMenuItem;
        private ToolStripMenuItem invertSelectionAllServersToolStripMenuItem;
        private ToolStripMenuItem selectNoneAllServersToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem selectNoSpeedTestAllServersToolStripMenuItem;
        private ToolStripMenuItem selectTimeoutAllServersToolStripMenuItem;
        private ToolStripMenuItem selectNoMarkAllServersToolStripMenuItem;
        private ToolStripMenuItem selectAutorunAllServersToolStripMenuItem;
        private ToolStripMenuItem selectRunningAllServersToolStripMenuItem;
        private ToolStripMenuItem refreshSummaryToolStripMenuItem;
        private ToolStripMenuItem selectUntrackAllPagesToolStripMenuItem;
        private ToolStripMenuItem selectUntrackAllServersToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemCopyAsVeeLink;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItemResize;
        private ToolStripMenuItem toolStripMenuItemCopyAsVeeSubscription;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripMenuItem deleteSelectedServersToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem5;
        private ToolStripMenuItem deleteAllServersToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemSortByDateT;
        private ToolStripMenuItem pluginToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonAddServerSimple;
        private ToolStripButton toolStripButtonImportFromClipboard;
        private ToolStripButton toolStripButtonScanQrcode;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonSelectAllCurPage;
        private ToolStripButton toolStripButtonInverseSelectionCurPage;
        private ToolStripButton toolStripButtonSelectNoneCurPage;
        private ToolStripButton toolStripButtonAllServerSelectAll;
        private ToolStripButton toolStripButtonAllServerSelectNone;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton toolStripButtonModifySelected;
        private ToolStripButton toolStripButtonRunSpeedTest;
        private ToolStripButton toolStripButtonSortSelectedBySpeedTestResult;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripButton toolStripButtonFormOption;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton toolStripButtonShowFormLog;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripComboBox toolStripComboBoxMarkFilter;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem toolStripMenuItemStopBatchSpeedTest;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItemReverseByIndex;
        private ToolStripMenuItem toolStripMenuItemModifySettings;
        private ToolStripMenuItem toolStripMenuItemClearSpeedTestResults;
        private ToolStripMenuItem toolStripMenuItemClearStatisticsRecord;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem toolStripMenuItemMoveToTop;
        private ToolStripMenuItem toolStripMenuItemMoveToBottom;
        private ToolStripMenuItem toolStripMenuItemSortByDownloadTotal;
        private ToolStripMenuItem toolStripMenuItemSortByUploadTotal;
        private ToolStripLabel toolStripLabelSearch;
    }
}
