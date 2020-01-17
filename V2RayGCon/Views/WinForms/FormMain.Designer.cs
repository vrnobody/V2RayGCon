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
            this.toolStripButtonAllServerSelectAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAllServerSelectNone = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonModifySelected = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRunSpeedTest = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSortSelectedBySpeedTestResult = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonFormOption = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowFormLog = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelMarkFilter = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxMarkFilter = new System.Windows.Forms.ToolStripComboBox();
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
            this.toolStripMenuItemModifySelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemModifySettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSortBySpeedTest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSortByDateT = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSortBySummary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemMoveToTop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMoveToBottom = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemFoldingPanel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExpansePanel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsV2cfgLink = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsVmessLink = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsVeeLink = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsVmessSubscription = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopyAsVeeSubscription = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemRestartSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStopSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSpeedTestOnSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshSummaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemConfigEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuItemQRCode = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripContainer2.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer2.ContentPanel
            // 
            this.toolStripContainer2.ContentPanel.Controls.Add(this.flyServerListContainer);
            resources.ApplyResources(this.toolStripContainer2.ContentPanel, "toolStripContainer2.ContentPanel");
            resources.ApplyResources(this.toolStripContainer2, "toolStripContainer2");
            this.toolStripContainer2.LeftToolStripPanelVisible = false;
            this.toolStripContainer2.Name = "toolStripContainer2";
            this.toolStripContainer2.RightToolStripPanelVisible = false;
            // 
            // toolStripContainer2.TopToolStripPanel
            // 
            this.toolStripContainer2.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // flyServerListContainer
            // 
            this.flyServerListContainer.AllowDrop = true;
            resources.ApplyResources(this.flyServerListContainer, "flyServerListContainer");
            this.flyServerListContainer.BackColor = System.Drawing.Color.White;
            this.flyServerListContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flyServerListContainer.Name = "flyServerListContainer";
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
            this.toolStripButtonAllServerSelectAll,
            this.toolStripButtonAllServerSelectNone,
            this.toolStripSeparator6,
            this.toolStripButtonModifySelected,
            this.toolStripButtonRunSpeedTest,
            this.toolStripButtonSortSelectedBySpeedTestResult,
            this.toolStripSeparator9,
            this.toolStripButtonFormOption,
            this.toolStripButtonShowFormLog,
            this.toolStripButtonHelp,
            this.toolStripSeparator10,
            this.toolStripLabelMarkFilter,
            this.toolStripComboBoxMarkFilter});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonAddServerSimple
            // 
            this.toolStripButtonAddServerSimple.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAddServerSimple, "toolStripButtonAddServerSimple");
            this.toolStripButtonAddServerSimple.Name = "toolStripButtonAddServerSimple";
            // 
            // toolStripButtonImportFromClipboard
            // 
            this.toolStripButtonImportFromClipboard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonImportFromClipboard, "toolStripButtonImportFromClipboard");
            this.toolStripButtonImportFromClipboard.Name = "toolStripButtonImportFromClipboard";
            // 
            // toolStripButtonScanQrcode
            // 
            this.toolStripButtonScanQrcode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonScanQrcode, "toolStripButtonScanQrcode");
            this.toolStripButtonScanQrcode.Name = "toolStripButtonScanQrcode";
            this.toolStripButtonScanQrcode.Click += new System.EventHandler(this.ToolStripButtonScanQrcode_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonSelectAllCurPage
            // 
            this.toolStripButtonSelectAllCurPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSelectAllCurPage, "toolStripButtonSelectAllCurPage");
            this.toolStripButtonSelectAllCurPage.Name = "toolStripButtonSelectAllCurPage";
            // 
            // toolStripButtonInverseSelectionCurPage
            // 
            this.toolStripButtonInverseSelectionCurPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonInverseSelectionCurPage, "toolStripButtonInverseSelectionCurPage");
            this.toolStripButtonInverseSelectionCurPage.Name = "toolStripButtonInverseSelectionCurPage";
            // 
            // toolStripButtonSelectNoneCurPage
            // 
            this.toolStripButtonSelectNoneCurPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSelectNoneCurPage, "toolStripButtonSelectNoneCurPage");
            this.toolStripButtonSelectNoneCurPage.Name = "toolStripButtonSelectNoneCurPage";
            // 
            // toolStripButtonAllServerSelectAll
            // 
            this.toolStripButtonAllServerSelectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAllServerSelectAll, "toolStripButtonAllServerSelectAll");
            this.toolStripButtonAllServerSelectAll.Name = "toolStripButtonAllServerSelectAll";
            // 
            // toolStripButtonAllServerSelectNone
            // 
            this.toolStripButtonAllServerSelectNone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAllServerSelectNone, "toolStripButtonAllServerSelectNone");
            this.toolStripButtonAllServerSelectNone.Name = "toolStripButtonAllServerSelectNone";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // toolStripButtonModifySelected
            // 
            this.toolStripButtonModifySelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonModifySelected, "toolStripButtonModifySelected");
            this.toolStripButtonModifySelected.Name = "toolStripButtonModifySelected";
            // 
            // toolStripButtonRunSpeedTest
            // 
            this.toolStripButtonRunSpeedTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonRunSpeedTest, "toolStripButtonRunSpeedTest");
            this.toolStripButtonRunSpeedTest.Name = "toolStripButtonRunSpeedTest";
            // 
            // toolStripButtonSortSelectedBySpeedTestResult
            // 
            this.toolStripButtonSortSelectedBySpeedTestResult.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSortSelectedBySpeedTestResult, "toolStripButtonSortSelectedBySpeedTestResult");
            this.toolStripButtonSortSelectedBySpeedTestResult.Name = "toolStripButtonSortSelectedBySpeedTestResult";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // toolStripButtonFormOption
            // 
            this.toolStripButtonFormOption.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonFormOption, "toolStripButtonFormOption");
            this.toolStripButtonFormOption.Name = "toolStripButtonFormOption";
            // 
            // toolStripButtonShowFormLog
            // 
            this.toolStripButtonShowFormLog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonShowFormLog, "toolStripButtonShowFormLog");
            this.toolStripButtonShowFormLog.Name = "toolStripButtonShowFormLog";
            // 
            // toolStripButtonHelp
            // 
            this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonHelp, "toolStripButtonHelp");
            this.toolStripButtonHelp.Name = "toolStripButtonHelp";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // toolStripLabelMarkFilter
            // 
            resources.ApplyResources(this.toolStripLabelMarkFilter, "toolStripLabelMarkFilter");
            this.toolStripLabelMarkFilter.Name = "toolStripLabelMarkFilter";
            // 
            // toolStripComboBoxMarkFilter
            // 
            this.toolStripComboBoxMarkFilter.Name = "toolStripComboBoxMarkFilter";
            resources.ApplyResources(this.toolStripComboBoxMarkFilter, "toolStripComboBoxMarkFilter");
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
            // 
            // operationToolStripMenuItem
            // 
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
            resources.ApplyResources(this.operationToolStripMenuItem, "operationToolStripMenuItem");
            // 
            // toolMenuItemSimAddVmessServer
            // 
            this.toolMenuItemSimAddVmessServer.Name = "toolMenuItemSimAddVmessServer";
            resources.ApplyResources(this.toolMenuItemSimAddVmessServer, "toolMenuItemSimAddVmessServer");
            // 
            // toolMenuItemImportLinkFromClipboard
            // 
            this.toolMenuItemImportLinkFromClipboard.Name = "toolMenuItemImportLinkFromClipboard";
            resources.ApplyResources(this.toolMenuItemImportLinkFromClipboard, "toolMenuItemImportLinkFromClipboard");
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // toolMenuItemExportAllServerToFile
            // 
            this.toolMenuItemExportAllServerToFile.Name = "toolMenuItemExportAllServerToFile";
            resources.ApplyResources(this.toolMenuItemExportAllServerToFile, "toolMenuItemExportAllServerToFile");
            // 
            // toolMenuItemImportFromFile
            // 
            this.toolMenuItemImportFromFile.Name = "toolMenuItemImportFromFile";
            resources.ApplyResources(this.toolMenuItemImportFromFile, "toolMenuItemImportFromFile");
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // closeWindowToolStripMenuItem
            // 
            this.closeWindowToolStripMenuItem.Name = "closeWindowToolStripMenuItem";
            resources.ApplyResources(this.closeWindowToolStripMenuItem, "closeWindowToolStripMenuItem");
            this.closeWindowToolStripMenuItem.Click += new System.EventHandler(this.closeWindowToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentPageToolStripMenuItem,
            this.allPagesToolStripMenuItem,
            this.allServersToolStripMenuItem});
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            resources.ApplyResources(this.selectToolStripMenuItem, "selectToolStripMenuItem");
            // 
            // currentPageToolStripMenuItem
            // 
            this.currentPageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllCurPageToolStripMenuItem,
            this.invertSelectionCurPageToolStripMenuItem,
            this.selectNoneCurPageToolStripMenuItem1});
            this.currentPageToolStripMenuItem.Name = "currentPageToolStripMenuItem";
            resources.ApplyResources(this.currentPageToolStripMenuItem, "currentPageToolStripMenuItem");
            // 
            // selectAllCurPageToolStripMenuItem
            // 
            this.selectAllCurPageToolStripMenuItem.Name = "selectAllCurPageToolStripMenuItem";
            resources.ApplyResources(this.selectAllCurPageToolStripMenuItem, "selectAllCurPageToolStripMenuItem");
            // 
            // invertSelectionCurPageToolStripMenuItem
            // 
            this.invertSelectionCurPageToolStripMenuItem.Name = "invertSelectionCurPageToolStripMenuItem";
            resources.ApplyResources(this.invertSelectionCurPageToolStripMenuItem, "invertSelectionCurPageToolStripMenuItem");
            // 
            // selectNoneCurPageToolStripMenuItem1
            // 
            this.selectNoneCurPageToolStripMenuItem1.Name = "selectNoneCurPageToolStripMenuItem1";
            resources.ApplyResources(this.selectNoneCurPageToolStripMenuItem1, "selectNoneCurPageToolStripMenuItem1");
            // 
            // allPagesToolStripMenuItem
            // 
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
            resources.ApplyResources(this.allPagesToolStripMenuItem, "allPagesToolStripMenuItem");
            // 
            // selectAllAllPagesToolStripMenuItem
            // 
            this.selectAllAllPagesToolStripMenuItem.Name = "selectAllAllPagesToolStripMenuItem";
            resources.ApplyResources(this.selectAllAllPagesToolStripMenuItem, "selectAllAllPagesToolStripMenuItem");
            // 
            // invertSelectionAllPagesToolStripMenuItem
            // 
            this.invertSelectionAllPagesToolStripMenuItem.Name = "invertSelectionAllPagesToolStripMenuItem";
            resources.ApplyResources(this.invertSelectionAllPagesToolStripMenuItem, "invertSelectionAllPagesToolStripMenuItem");
            // 
            // selectNoneAllPagesToolStripMenuItem
            // 
            this.selectNoneAllPagesToolStripMenuItem.Name = "selectNoneAllPagesToolStripMenuItem";
            resources.ApplyResources(this.selectNoneAllPagesToolStripMenuItem, "selectNoneAllPagesToolStripMenuItem");
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // selectNoSpeedTestAllPagesToolStripMenuItem
            // 
            this.selectNoSpeedTestAllPagesToolStripMenuItem.Name = "selectNoSpeedTestAllPagesToolStripMenuItem";
            resources.ApplyResources(this.selectNoSpeedTestAllPagesToolStripMenuItem, "selectNoSpeedTestAllPagesToolStripMenuItem");
            // 
            // selectTimeoutAllPagesToolStripMenuItem
            // 
            this.selectTimeoutAllPagesToolStripMenuItem.Name = "selectTimeoutAllPagesToolStripMenuItem";
            resources.ApplyResources(this.selectTimeoutAllPagesToolStripMenuItem, "selectTimeoutAllPagesToolStripMenuItem");
            // 
            // selectNoMarkAllPagesToolStripMenuItem
            // 
            this.selectNoMarkAllPagesToolStripMenuItem.Name = "selectNoMarkAllPagesToolStripMenuItem";
            resources.ApplyResources(this.selectNoMarkAllPagesToolStripMenuItem, "selectNoMarkAllPagesToolStripMenuItem");
            // 
            // selectAutorunAllPagesToolStripMenuItem
            // 
            this.selectAutorunAllPagesToolStripMenuItem.Name = "selectAutorunAllPagesToolStripMenuItem";
            resources.ApplyResources(this.selectAutorunAllPagesToolStripMenuItem, "selectAutorunAllPagesToolStripMenuItem");
            // 
            // selectRunningAllPagesToolStripMenuItem
            // 
            this.selectRunningAllPagesToolStripMenuItem.Name = "selectRunningAllPagesToolStripMenuItem";
            resources.ApplyResources(this.selectRunningAllPagesToolStripMenuItem, "selectRunningAllPagesToolStripMenuItem");
            // 
            // selectUntrackAllPagesToolStripMenuItem
            // 
            this.selectUntrackAllPagesToolStripMenuItem.Name = "selectUntrackAllPagesToolStripMenuItem";
            resources.ApplyResources(this.selectUntrackAllPagesToolStripMenuItem, "selectUntrackAllPagesToolStripMenuItem");
            // 
            // allServersToolStripMenuItem
            // 
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
            resources.ApplyResources(this.allServersToolStripMenuItem, "allServersToolStripMenuItem");
            // 
            // selectAllAllServersToolStripMenuItem
            // 
            this.selectAllAllServersToolStripMenuItem.Name = "selectAllAllServersToolStripMenuItem";
            resources.ApplyResources(this.selectAllAllServersToolStripMenuItem, "selectAllAllServersToolStripMenuItem");
            // 
            // invertSelectionAllServersToolStripMenuItem
            // 
            this.invertSelectionAllServersToolStripMenuItem.Name = "invertSelectionAllServersToolStripMenuItem";
            resources.ApplyResources(this.invertSelectionAllServersToolStripMenuItem, "invertSelectionAllServersToolStripMenuItem");
            // 
            // selectNoneAllServersToolStripMenuItem
            // 
            this.selectNoneAllServersToolStripMenuItem.Name = "selectNoneAllServersToolStripMenuItem";
            resources.ApplyResources(this.selectNoneAllServersToolStripMenuItem, "selectNoneAllServersToolStripMenuItem");
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // selectNoSpeedTestAllServersToolStripMenuItem
            // 
            this.selectNoSpeedTestAllServersToolStripMenuItem.Name = "selectNoSpeedTestAllServersToolStripMenuItem";
            resources.ApplyResources(this.selectNoSpeedTestAllServersToolStripMenuItem, "selectNoSpeedTestAllServersToolStripMenuItem");
            // 
            // selectTimeoutAllServersToolStripMenuItem
            // 
            this.selectTimeoutAllServersToolStripMenuItem.Name = "selectTimeoutAllServersToolStripMenuItem";
            resources.ApplyResources(this.selectTimeoutAllServersToolStripMenuItem, "selectTimeoutAllServersToolStripMenuItem");
            // 
            // selectNoMarkAllServersToolStripMenuItem
            // 
            this.selectNoMarkAllServersToolStripMenuItem.Name = "selectNoMarkAllServersToolStripMenuItem";
            resources.ApplyResources(this.selectNoMarkAllServersToolStripMenuItem, "selectNoMarkAllServersToolStripMenuItem");
            // 
            // selectAutorunAllServersToolStripMenuItem
            // 
            this.selectAutorunAllServersToolStripMenuItem.Name = "selectAutorunAllServersToolStripMenuItem";
            resources.ApplyResources(this.selectAutorunAllServersToolStripMenuItem, "selectAutorunAllServersToolStripMenuItem");
            // 
            // selectRunningAllServersToolStripMenuItem
            // 
            this.selectRunningAllServersToolStripMenuItem.Name = "selectRunningAllServersToolStripMenuItem";
            resources.ApplyResources(this.selectRunningAllServersToolStripMenuItem, "selectRunningAllServersToolStripMenuItem");
            // 
            // selectUntrackAllServersToolStripMenuItem
            // 
            this.selectUntrackAllServersToolStripMenuItem.Name = "selectUntrackAllServersToolStripMenuItem";
            resources.ApplyResources(this.selectUntrackAllServersToolStripMenuItem, "selectUntrackAllServersToolStripMenuItem");
            // 
            // toolMenuItemServer
            // 
            this.toolMenuItemServer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemModifySelected,
            this.toolStripMenuItem1,
            this.toolStripSeparator12,
            this.toolStripMenuItemRestartSelected,
            this.toolStripMenuItemStopSelected,
            this.toolStripMenuItemSpeedTestOnSelected,
            this.toolStripSeparator1,
            this.refreshSummaryToolStripMenuItem,
            this.deleteSelectedServersToolStripMenuItem});
            this.toolMenuItemServer.Name = "toolMenuItemServer";
            resources.ApplyResources(this.toolMenuItemServer, "toolMenuItemServer");
            // 
            // toolStripMenuItemModifySelected
            // 
            this.toolStripMenuItemModifySelected.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemModifySettings,
            this.toolStripSeparator11,
            this.toolStripMenuItemSortBySpeedTest,
            this.toolStripMenuItemSortByDateT,
            this.toolStripMenuItemSortBySummary,
            this.toolStripSeparator3,
            this.toolStripMenuItemMoveToTop,
            this.toolStripMenuItemMoveToBottom,
            this.toolStripSeparator4,
            this.toolStripMenuItemFoldingPanel,
            this.toolStripMenuItemExpansePanel});
            this.toolStripMenuItemModifySelected.Name = "toolStripMenuItemModifySelected";
            resources.ApplyResources(this.toolStripMenuItemModifySelected, "toolStripMenuItemModifySelected");
            // 
            // toolStripMenuItemModifySettings
            // 
            this.toolStripMenuItemModifySettings.Name = "toolStripMenuItemModifySettings";
            resources.ApplyResources(this.toolStripMenuItemModifySettings, "toolStripMenuItemModifySettings");
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // toolStripMenuItemSortBySpeedTest
            // 
            this.toolStripMenuItemSortBySpeedTest.Name = "toolStripMenuItemSortBySpeedTest";
            resources.ApplyResources(this.toolStripMenuItemSortBySpeedTest, "toolStripMenuItemSortBySpeedTest");
            // 
            // toolStripMenuItemSortByDateT
            // 
            this.toolStripMenuItemSortByDateT.Name = "toolStripMenuItemSortByDateT";
            resources.ApplyResources(this.toolStripMenuItemSortByDateT, "toolStripMenuItemSortByDateT");
            // 
            // toolStripMenuItemSortBySummary
            // 
            this.toolStripMenuItemSortBySummary.Name = "toolStripMenuItemSortBySummary";
            resources.ApplyResources(this.toolStripMenuItemSortBySummary, "toolStripMenuItemSortBySummary");
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripMenuItemMoveToTop
            // 
            this.toolStripMenuItemMoveToTop.Name = "toolStripMenuItemMoveToTop";
            resources.ApplyResources(this.toolStripMenuItemMoveToTop, "toolStripMenuItemMoveToTop");
            // 
            // toolStripMenuItemMoveToBottom
            // 
            this.toolStripMenuItemMoveToBottom.Name = "toolStripMenuItemMoveToBottom";
            resources.ApplyResources(this.toolStripMenuItemMoveToBottom, "toolStripMenuItemMoveToBottom");
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripMenuItemFoldingPanel
            // 
            this.toolStripMenuItemFoldingPanel.Name = "toolStripMenuItemFoldingPanel";
            resources.ApplyResources(this.toolStripMenuItemFoldingPanel, "toolStripMenuItemFoldingPanel");
            // 
            // toolStripMenuItemExpansePanel
            // 
            this.toolStripMenuItemExpansePanel.Name = "toolStripMenuItemExpansePanel";
            resources.ApplyResources(this.toolStripMenuItemExpansePanel, "toolStripMenuItemExpansePanel");
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCopyAsV2cfgLink,
            this.toolStripMenuItemCopyAsVmessLink,
            this.toolStripMenuItemCopyAsVeeLink,
            this.toolStripMenuItemCopyAsVmessSubscription,
            this.toolStripMenuItemCopyAsVeeSubscription});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // toolStripMenuItemCopyAsV2cfgLink
            // 
            this.toolStripMenuItemCopyAsV2cfgLink.Name = "toolStripMenuItemCopyAsV2cfgLink";
            resources.ApplyResources(this.toolStripMenuItemCopyAsV2cfgLink, "toolStripMenuItemCopyAsV2cfgLink");
            // 
            // toolStripMenuItemCopyAsVmessLink
            // 
            this.toolStripMenuItemCopyAsVmessLink.Name = "toolStripMenuItemCopyAsVmessLink";
            resources.ApplyResources(this.toolStripMenuItemCopyAsVmessLink, "toolStripMenuItemCopyAsVmessLink");
            // 
            // toolStripMenuItemCopyAsVeeLink
            // 
            this.toolStripMenuItemCopyAsVeeLink.Name = "toolStripMenuItemCopyAsVeeLink";
            resources.ApplyResources(this.toolStripMenuItemCopyAsVeeLink, "toolStripMenuItemCopyAsVeeLink");
            // 
            // toolStripMenuItemCopyAsVmessSubscription
            // 
            this.toolStripMenuItemCopyAsVmessSubscription.Name = "toolStripMenuItemCopyAsVmessSubscription";
            resources.ApplyResources(this.toolStripMenuItemCopyAsVmessSubscription, "toolStripMenuItemCopyAsVmessSubscription");
            // 
            // toolStripMenuItemCopyAsVeeSubscription
            // 
            this.toolStripMenuItemCopyAsVeeSubscription.Name = "toolStripMenuItemCopyAsVeeSubscription";
            resources.ApplyResources(this.toolStripMenuItemCopyAsVeeSubscription, "toolStripMenuItemCopyAsVeeSubscription");
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // toolStripMenuItemRestartSelected
            // 
            this.toolStripMenuItemRestartSelected.Name = "toolStripMenuItemRestartSelected";
            resources.ApplyResources(this.toolStripMenuItemRestartSelected, "toolStripMenuItemRestartSelected");
            // 
            // toolStripMenuItemStopSelected
            // 
            this.toolStripMenuItemStopSelected.Name = "toolStripMenuItemStopSelected";
            resources.ApplyResources(this.toolStripMenuItemStopSelected, "toolStripMenuItemStopSelected");
            // 
            // toolStripMenuItemSpeedTestOnSelected
            // 
            this.toolStripMenuItemSpeedTestOnSelected.Name = "toolStripMenuItemSpeedTestOnSelected";
            resources.ApplyResources(this.toolStripMenuItemSpeedTestOnSelected, "toolStripMenuItemSpeedTestOnSelected");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // refreshSummaryToolStripMenuItem
            // 
            this.refreshSummaryToolStripMenuItem.Name = "refreshSummaryToolStripMenuItem";
            resources.ApplyResources(this.refreshSummaryToolStripMenuItem, "refreshSummaryToolStripMenuItem");
            // 
            // deleteSelectedServersToolStripMenuItem
            // 
            this.deleteSelectedServersToolStripMenuItem.Name = "deleteSelectedServersToolStripMenuItem";
            resources.ApplyResources(this.deleteSelectedServersToolStripMenuItem, "deleteSelectedServersToolStripMenuItem");
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolMenuItemConfigEditor,
            this.toolMenuItemQRCode,
            this.toolMenuItemLog,
            this.toolMenuItemOptions,
            this.toolStripMenuItem4,
            this.toolStripMenuItemResize});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            resources.ApplyResources(this.windowToolStripMenuItem, "windowToolStripMenuItem");
            // 
            // toolMenuItemConfigEditor
            // 
            this.toolMenuItemConfigEditor.Name = "toolMenuItemConfigEditor";
            resources.ApplyResources(this.toolMenuItemConfigEditor, "toolMenuItemConfigEditor");
            // 
            // toolMenuItemQRCode
            // 
            this.toolMenuItemQRCode.Name = "toolMenuItemQRCode";
            resources.ApplyResources(this.toolMenuItemQRCode, "toolMenuItemQRCode");
            // 
            // toolMenuItemLog
            // 
            this.toolMenuItemLog.Name = "toolMenuItemLog";
            resources.ApplyResources(this.toolMenuItemLog, "toolMenuItemLog");
            // 
            // toolMenuItemOptions
            // 
            this.toolMenuItemOptions.Name = "toolMenuItemOptions";
            resources.ApplyResources(this.toolMenuItemOptions, "toolMenuItemOptions");
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // toolStripMenuItemResize
            // 
            this.toolStripMenuItemResize.Name = "toolStripMenuItemResize";
            resources.ApplyResources(this.toolStripMenuItemResize, "toolStripMenuItemResize");
            // 
            // pluginToolStripMenuItem
            // 
            this.pluginToolStripMenuItem.Name = "pluginToolStripMenuItem";
            resources.ApplyResources(this.pluginToolStripMenuItem, "pluginToolStripMenuItem");
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemDownLoadV2rayCore,
            this.toolMenuItemCheckUpdate,
            this.toolMenuItemAbout,
            this.toolMenuItemHelp,
            this.toolStripMenuItem5,
            this.toolStripMenuItemRemoveV2rayCore,
            this.deleteAllServersToolStripMenuItem});
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            resources.ApplyResources(this.aboutToolStripMenuItem1, "aboutToolStripMenuItem1");
            // 
            // toolStripMenuItemDownLoadV2rayCore
            // 
            this.toolStripMenuItemDownLoadV2rayCore.Name = "toolStripMenuItemDownLoadV2rayCore";
            resources.ApplyResources(this.toolStripMenuItemDownLoadV2rayCore, "toolStripMenuItemDownLoadV2rayCore");
            // 
            // toolMenuItemCheckUpdate
            // 
            this.toolMenuItemCheckUpdate.Name = "toolMenuItemCheckUpdate";
            resources.ApplyResources(this.toolMenuItemCheckUpdate, "toolMenuItemCheckUpdate");
            // 
            // toolMenuItemAbout
            // 
            this.toolMenuItemAbout.Name = "toolMenuItemAbout";
            resources.ApplyResources(this.toolMenuItemAbout, "toolMenuItemAbout");
            // 
            // toolMenuItemHelp
            // 
            this.toolMenuItemHelp.Name = "toolMenuItemHelp";
            resources.ApplyResources(this.toolMenuItemHelp, "toolMenuItemHelp");
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            // 
            // toolStripMenuItemRemoveV2rayCore
            // 
            this.toolStripMenuItemRemoveV2rayCore.Name = "toolStripMenuItemRemoveV2rayCore";
            resources.ApplyResources(this.toolStripMenuItemRemoveV2rayCore, "toolStripMenuItemRemoveV2rayCore");
            // 
            // deleteAllServersToolStripMenuItem
            // 
            this.deleteAllServersToolStripMenuItem.Name = "deleteAllServersToolStripMenuItem";
            resources.ApplyResources(this.deleteAllServersToolStripMenuItem, "deleteAllServersToolStripMenuItem");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelTotal,
            this.toolStripStatusLabel1,
            this.toolStripDropDownButtonPager,
            this.toolStripStatusLabelPrePage,
            this.toolStripStatusLabelNextPage});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabelTotal
            // 
            this.toolStripStatusLabelTotal.Name = "toolStripStatusLabelTotal";
            resources.ApplyResources(this.toolStripStatusLabelTotal, "toolStripStatusLabelTotal");
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
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
            this.panel1.Controls.Add(this.toolStripContainer2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainMneuStrip);
            this.MainMenuStrip = this.mainMneuStrip;
            this.Name = "FormMain";
            this.Shown += new System.EventHandler(this.FormMain_Shown);
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
        private System.Windows.Forms.ToolStripMenuItem toolMenuItemQRCode;
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
        private ToolStripMenuItem toolStripMenuItemSpeedTestOnSelected;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItemCopyAsV2cfgLink;
        private ToolStripMenuItem toolStripMenuItemCopyAsVmessLink;
        private ToolStripMenuItem toolStripMenuItemDownLoadV2rayCore;
        private ToolStripMenuItem toolStripMenuItemRemoveV2rayCore;
        private ToolStripMenuItem toolStripMenuItemCopyAsVmessSubscription;
        private ToolStripMenuItem toolStripMenuItemModifySelected;
        private ToolStripMenuItem toolStripMenuItemModifySettings;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItemMoveToTop;
        private ToolStripMenuItem toolStripMenuItemMoveToBottom;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem toolStripMenuItemFoldingPanel;
        private ToolStripMenuItem toolStripMenuItemExpansePanel;
        private ToolStripMenuItem toolStripMenuItemSortBySpeedTest;
        private ToolStripMenuItem selectToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemSortBySummary;
        private ToolStripContainer toolStripContainer2;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonSelectAllCurPage;
        private ToolStripButton toolStripButtonInverseSelectionCurPage;
        private ToolStripButton toolStripButtonSelectNoneCurPage;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonAllServerSelectAll;
        private ToolStripButton toolStripButtonAllServerSelectNone;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton toolStripButtonModifySelected;
        private ToolStripButton toolStripButtonRunSpeedTest;
        private ToolStripButton toolStripButtonSortSelectedBySpeedTestResult;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripButton toolStripButtonFormOption;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelTotal;
        private Panel panel1;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripLabel toolStripLabelMarkFilter;
        private ToolStripComboBox toolStripComboBoxMarkFilter;
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
        private ToolStripSeparator toolStripSeparator11;
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
        private ToolStripButton toolStripButtonAddServerSimple;
        private ToolStripButton toolStripButtonHelp;
        private ToolStripButton toolStripButtonImportFromClipboard;
        private ToolStripButton toolStripButtonShowFormLog;
        private ToolStripMenuItem toolStripMenuItemSortByDateT;
        private ToolStripButton toolStripButtonScanQrcode;
        private ToolStripMenuItem pluginToolStripMenuItem;
    }
}
