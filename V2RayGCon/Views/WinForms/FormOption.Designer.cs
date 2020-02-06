namespace V2RayGCon.Views.WinForms
{
    partial class FormOption
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOption));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSubscribe = new System.Windows.Forms.TabPage();
            this.btnSubsInvertSelection = new System.Windows.Forms.Button();
            this.btnSubsUseAll = new System.Windows.Forms.Button();
            this.chkSubsIsUseProxy = new System.Windows.Forms.CheckBox();
            this.btnUpdateViaSubscription = new System.Windows.Forms.Button();
            this.btnAddSubsUrl = new System.Windows.Forms.Button();
            this.flySubsUrlContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPageDefaults = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkDefImportInjectGlobalImport = new System.Windows.Forms.CheckBox();
            this.chkDefImportBypassCnSite = new System.Windows.Forms.CheckBox();
            this.chkDefImportSsShareLink = new System.Windows.Forms.CheckBox();
            this.tboxDefImportAddr = new System.Windows.Forms.TextBox();
            this.cboxDefImportMode = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tboxDefSpeedtestTimeout = new System.Windows.Forms.TextBox();
            this.tboxDefSpeedtestExpectedSize = new System.Windows.Forms.TextBox();
            this.tboxDefSpeedtestCycles = new System.Windows.Forms.TextBox();
            this.tboxDefSpeedtestUrl = new System.Windows.Forms.TextBox();
            this.chkDefSpeedtestIsUse = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPageSetting = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkSetCheckWhenStart = new System.Windows.Forms.CheckBox();
            this.chkSetUpgradeUseProxy = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnSetOpenStartupFolder = new System.Windows.Forms.Button();
            this.chkSetUseV4 = new System.Windows.Forms.CheckBox();
            this.chkSetSysPortable = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tboxSettingsMaxCoreNum = new System.Windows.Forms.TextBox();
            this.chkSetServStatistics = new System.Windows.Forms.CheckBox();
            this.chkSetServAutotrack = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboxSettingPageSize = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboxSettingLanguage = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageImport = new System.Windows.Forms.TabPage();
            this.btnImportAdd = new System.Windows.Forms.Button();
            this.flyImportPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPagePlugins = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.flyPluginsItemsContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBakBackup = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnOptionSave = new System.Windows.Forms.Button();
            this.btnBakRestore = new System.Windows.Forms.Button();
            this.btnOptionExit = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageSubscribe.SuspendLayout();
            this.tabPageDefaults.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPageSetting.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageImport.SuspendLayout();
            this.tabPagePlugins.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPageSubscribe);
            this.tabControl1.Controls.Add(this.tabPageDefaults);
            this.tabControl1.Controls.Add(this.tabPageSetting);
            this.tabControl1.Controls.Add(this.tabPageImport);
            this.tabControl1.Controls.Add(this.tabPagePlugins);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPageSubscribe
            // 
            this.tabPageSubscribe.Controls.Add(this.btnSubsInvertSelection);
            this.tabPageSubscribe.Controls.Add(this.btnSubsUseAll);
            this.tabPageSubscribe.Controls.Add(this.chkSubsIsUseProxy);
            this.tabPageSubscribe.Controls.Add(this.btnUpdateViaSubscription);
            this.tabPageSubscribe.Controls.Add(this.btnAddSubsUrl);
            this.tabPageSubscribe.Controls.Add(this.flySubsUrlContainer);
            resources.ApplyResources(this.tabPageSubscribe, "tabPageSubscribe");
            this.tabPageSubscribe.Name = "tabPageSubscribe";
            this.tabPageSubscribe.UseVisualStyleBackColor = true;
            // 
            // btnSubsInvertSelection
            // 
            resources.ApplyResources(this.btnSubsInvertSelection, "btnSubsInvertSelection");
            this.btnSubsInvertSelection.Name = "btnSubsInvertSelection";
            this.toolTip1.SetToolTip(this.btnSubsInvertSelection, resources.GetString("btnSubsInvertSelection.ToolTip"));
            this.btnSubsInvertSelection.UseVisualStyleBackColor = true;
            // 
            // btnSubsUseAll
            // 
            resources.ApplyResources(this.btnSubsUseAll, "btnSubsUseAll");
            this.btnSubsUseAll.Name = "btnSubsUseAll";
            this.toolTip1.SetToolTip(this.btnSubsUseAll, resources.GetString("btnSubsUseAll.ToolTip"));
            this.btnSubsUseAll.UseVisualStyleBackColor = true;
            // 
            // chkSubsIsUseProxy
            // 
            resources.ApplyResources(this.chkSubsIsUseProxy, "chkSubsIsUseProxy");
            this.chkSubsIsUseProxy.Name = "chkSubsIsUseProxy";
            this.toolTip1.SetToolTip(this.chkSubsIsUseProxy, resources.GetString("chkSubsIsUseProxy.ToolTip"));
            this.chkSubsIsUseProxy.UseVisualStyleBackColor = true;
            // 
            // btnUpdateViaSubscription
            // 
            resources.ApplyResources(this.btnUpdateViaSubscription, "btnUpdateViaSubscription");
            this.btnUpdateViaSubscription.Name = "btnUpdateViaSubscription";
            this.toolTip1.SetToolTip(this.btnUpdateViaSubscription, resources.GetString("btnUpdateViaSubscription.ToolTip"));
            this.btnUpdateViaSubscription.UseVisualStyleBackColor = true;
            // 
            // btnAddSubsUrl
            // 
            resources.ApplyResources(this.btnAddSubsUrl, "btnAddSubsUrl");
            this.btnAddSubsUrl.Name = "btnAddSubsUrl";
            this.toolTip1.SetToolTip(this.btnAddSubsUrl, resources.GetString("btnAddSubsUrl.ToolTip"));
            this.btnAddSubsUrl.UseVisualStyleBackColor = true;
            // 
            // flySubsUrlContainer
            // 
            this.flySubsUrlContainer.AllowDrop = true;
            resources.ApplyResources(this.flySubsUrlContainer, "flySubsUrlContainer");
            this.flySubsUrlContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flySubsUrlContainer.Name = "flySubsUrlContainer";
            // 
            // tabPageDefaults
            // 
            this.tabPageDefaults.Controls.Add(this.groupBox4);
            this.tabPageDefaults.Controls.Add(this.groupBox3);
            resources.ApplyResources(this.tabPageDefaults, "tabPageDefaults");
            this.tabPageDefaults.Name = "tabPageDefaults";
            this.tabPageDefaults.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.chkDefImportInjectGlobalImport);
            this.groupBox4.Controls.Add(this.chkDefImportBypassCnSite);
            this.groupBox4.Controls.Add(this.chkDefImportSsShareLink);
            this.groupBox4.Controls.Add(this.tboxDefImportAddr);
            this.groupBox4.Controls.Add(this.cboxDefImportMode);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // chkDefImportInjectGlobalImport
            // 
            resources.ApplyResources(this.chkDefImportInjectGlobalImport, "chkDefImportInjectGlobalImport");
            this.chkDefImportInjectGlobalImport.Name = "chkDefImportInjectGlobalImport";
            this.toolTip1.SetToolTip(this.chkDefImportInjectGlobalImport, resources.GetString("chkDefImportInjectGlobalImport.ToolTip"));
            this.chkDefImportInjectGlobalImport.UseVisualStyleBackColor = true;
            // 
            // chkDefImportBypassCnSite
            // 
            resources.ApplyResources(this.chkDefImportBypassCnSite, "chkDefImportBypassCnSite");
            this.chkDefImportBypassCnSite.Name = "chkDefImportBypassCnSite";
            this.toolTip1.SetToolTip(this.chkDefImportBypassCnSite, resources.GetString("chkDefImportBypassCnSite.ToolTip"));
            this.chkDefImportBypassCnSite.UseVisualStyleBackColor = true;
            // 
            // chkDefImportSsShareLink
            // 
            resources.ApplyResources(this.chkDefImportSsShareLink, "chkDefImportSsShareLink");
            this.chkDefImportSsShareLink.Name = "chkDefImportSsShareLink";
            this.toolTip1.SetToolTip(this.chkDefImportSsShareLink, resources.GetString("chkDefImportSsShareLink.ToolTip"));
            this.chkDefImportSsShareLink.UseVisualStyleBackColor = true;
            // 
            // tboxDefImportAddr
            // 
            resources.ApplyResources(this.tboxDefImportAddr, "tboxDefImportAddr");
            this.tboxDefImportAddr.Name = "tboxDefImportAddr";
            this.toolTip1.SetToolTip(this.tboxDefImportAddr, resources.GetString("tboxDefImportAddr.ToolTip"));
            // 
            // cboxDefImportMode
            // 
            this.cboxDefImportMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxDefImportMode.FormattingEnabled = true;
            this.cboxDefImportMode.Items.AddRange(new object[] {
            resources.GetString("cboxDefImportMode.Items"),
            resources.GetString("cboxDefImportMode.Items1"),
            resources.GetString("cboxDefImportMode.Items2")});
            resources.ApplyResources(this.cboxDefImportMode, "cboxDefImportMode");
            this.cboxDefImportMode.Name = "cboxDefImportMode";
            this.toolTip1.SetToolTip(this.cboxDefImportMode, resources.GetString("cboxDefImportMode.ToolTip"));
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            this.toolTip1.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.toolTip1.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.tboxDefSpeedtestTimeout);
            this.groupBox3.Controls.Add(this.tboxDefSpeedtestExpectedSize);
            this.groupBox3.Controls.Add(this.tboxDefSpeedtestCycles);
            this.groupBox3.Controls.Add(this.tboxDefSpeedtestUrl);
            this.groupBox3.Controls.Add(this.chkDefSpeedtestIsUse);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // tboxDefSpeedtestTimeout
            // 
            resources.ApplyResources(this.tboxDefSpeedtestTimeout, "tboxDefSpeedtestTimeout");
            this.tboxDefSpeedtestTimeout.Name = "tboxDefSpeedtestTimeout";
            this.toolTip1.SetToolTip(this.tboxDefSpeedtestTimeout, resources.GetString("tboxDefSpeedtestTimeout.ToolTip"));
            // 
            // tboxDefSpeedtestExpectedSize
            // 
            resources.ApplyResources(this.tboxDefSpeedtestExpectedSize, "tboxDefSpeedtestExpectedSize");
            this.tboxDefSpeedtestExpectedSize.Name = "tboxDefSpeedtestExpectedSize";
            this.toolTip1.SetToolTip(this.tboxDefSpeedtestExpectedSize, resources.GetString("tboxDefSpeedtestExpectedSize.ToolTip"));
            // 
            // tboxDefSpeedtestCycles
            // 
            resources.ApplyResources(this.tboxDefSpeedtestCycles, "tboxDefSpeedtestCycles");
            this.tboxDefSpeedtestCycles.Name = "tboxDefSpeedtestCycles";
            this.toolTip1.SetToolTip(this.tboxDefSpeedtestCycles, resources.GetString("tboxDefSpeedtestCycles.ToolTip"));
            // 
            // tboxDefSpeedtestUrl
            // 
            resources.ApplyResources(this.tboxDefSpeedtestUrl, "tboxDefSpeedtestUrl");
            this.tboxDefSpeedtestUrl.Name = "tboxDefSpeedtestUrl";
            this.toolTip1.SetToolTip(this.tboxDefSpeedtestUrl, resources.GetString("tboxDefSpeedtestUrl.ToolTip"));
            // 
            // chkDefSpeedtestIsUse
            // 
            resources.ApplyResources(this.chkDefSpeedtestIsUse, "chkDefSpeedtestIsUse");
            this.chkDefSpeedtestIsUse.Name = "chkDefSpeedtestIsUse";
            this.toolTip1.SetToolTip(this.chkDefSpeedtestIsUse, resources.GetString("chkDefSpeedtestIsUse.ToolTip"));
            this.chkDefSpeedtestIsUse.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.toolTip1.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // tabPageSetting
            // 
            this.tabPageSetting.Controls.Add(this.groupBox2);
            this.tabPageSetting.Controls.Add(this.groupBox6);
            this.tabPageSetting.Controls.Add(this.groupBox5);
            this.tabPageSetting.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.tabPageSetting, "tabPageSetting");
            this.tabPageSetting.Name = "tabPageSetting";
            this.tabPageSetting.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.chkSetCheckWhenStart);
            this.groupBox2.Controls.Add(this.chkSetUpgradeUseProxy);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // chkSetCheckWhenStart
            // 
            resources.ApplyResources(this.chkSetCheckWhenStart, "chkSetCheckWhenStart");
            this.chkSetCheckWhenStart.Name = "chkSetCheckWhenStart";
            this.toolTip1.SetToolTip(this.chkSetCheckWhenStart, resources.GetString("chkSetCheckWhenStart.ToolTip"));
            this.chkSetCheckWhenStart.UseVisualStyleBackColor = true;
            // 
            // chkSetUpgradeUseProxy
            // 
            resources.ApplyResources(this.chkSetUpgradeUseProxy, "chkSetUpgradeUseProxy");
            this.chkSetUpgradeUseProxy.Name = "chkSetUpgradeUseProxy";
            this.toolTip1.SetToolTip(this.chkSetUpgradeUseProxy, resources.GetString("chkSetUpgradeUseProxy.ToolTip"));
            this.chkSetUpgradeUseProxy.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.btnSetOpenStartupFolder);
            this.groupBox6.Controls.Add(this.chkSetUseV4);
            this.groupBox6.Controls.Add(this.chkSetSysPortable);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // btnSetOpenStartupFolder
            // 
            resources.ApplyResources(this.btnSetOpenStartupFolder, "btnSetOpenStartupFolder");
            this.btnSetOpenStartupFolder.Name = "btnSetOpenStartupFolder";
            this.toolTip1.SetToolTip(this.btnSetOpenStartupFolder, resources.GetString("btnSetOpenStartupFolder.ToolTip"));
            this.btnSetOpenStartupFolder.UseVisualStyleBackColor = true;
            this.btnSetOpenStartupFolder.Click += new System.EventHandler(this.btnSetOpenStartupFolder_Click);
            // 
            // chkSetUseV4
            // 
            resources.ApplyResources(this.chkSetUseV4, "chkSetUseV4");
            this.chkSetUseV4.Name = "chkSetUseV4";
            this.toolTip1.SetToolTip(this.chkSetUseV4, resources.GetString("chkSetUseV4.ToolTip"));
            this.chkSetUseV4.UseVisualStyleBackColor = true;
            // 
            // chkSetSysPortable
            // 
            resources.ApplyResources(this.chkSetSysPortable, "chkSetSysPortable");
            this.chkSetSysPortable.Name = "chkSetSysPortable";
            this.toolTip1.SetToolTip(this.chkSetSysPortable, resources.GetString("chkSetSysPortable.ToolTip"));
            this.chkSetSysPortable.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.tboxSettingsMaxCoreNum);
            this.groupBox5.Controls.Add(this.chkSetServStatistics);
            this.groupBox5.Controls.Add(this.chkSetServAutotrack);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // tboxSettingsMaxCoreNum
            // 
            resources.ApplyResources(this.tboxSettingsMaxCoreNum, "tboxSettingsMaxCoreNum");
            this.tboxSettingsMaxCoreNum.Name = "tboxSettingsMaxCoreNum";
            // 
            // chkSetServStatistics
            // 
            resources.ApplyResources(this.chkSetServStatistics, "chkSetServStatistics");
            this.chkSetServStatistics.Name = "chkSetServStatistics";
            this.toolTip1.SetToolTip(this.chkSetServStatistics, resources.GetString("chkSetServStatistics.ToolTip"));
            this.chkSetServStatistics.UseVisualStyleBackColor = true;
            // 
            // chkSetServAutotrack
            // 
            resources.ApplyResources(this.chkSetServAutotrack, "chkSetServAutotrack");
            this.chkSetServAutotrack.Name = "chkSetServAutotrack";
            this.toolTip1.SetToolTip(this.chkSetServAutotrack, resources.GetString("chkSetServAutotrack.ToolTip"));
            this.chkSetServAutotrack.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            this.toolTip1.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.cboxSettingPageSize);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboxSettingLanguage);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cboxSettingPageSize
            // 
            this.cboxSettingPageSize.FormattingEnabled = true;
            this.cboxSettingPageSize.Items.AddRange(new object[] {
            resources.GetString("cboxSettingPageSize.Items"),
            resources.GetString("cboxSettingPageSize.Items1"),
            resources.GetString("cboxSettingPageSize.Items2"),
            resources.GetString("cboxSettingPageSize.Items3"),
            resources.GetString("cboxSettingPageSize.Items4")});
            resources.ApplyResources(this.cboxSettingPageSize, "cboxSettingPageSize");
            this.cboxSettingPageSize.Name = "cboxSettingPageSize";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // cboxSettingLanguage
            // 
            this.cboxSettingLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxSettingLanguage.FormattingEnabled = true;
            this.cboxSettingLanguage.Items.AddRange(new object[] {
            resources.GetString("cboxSettingLanguage.Items"),
            resources.GetString("cboxSettingLanguage.Items1"),
            resources.GetString("cboxSettingLanguage.Items2")});
            resources.ApplyResources(this.cboxSettingLanguage, "cboxSettingLanguage");
            this.cboxSettingLanguage.Name = "cboxSettingLanguage";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tabPageImport
            // 
            this.tabPageImport.Controls.Add(this.btnImportAdd);
            this.tabPageImport.Controls.Add(this.flyImportPanel);
            resources.ApplyResources(this.tabPageImport, "tabPageImport");
            this.tabPageImport.Name = "tabPageImport";
            this.tabPageImport.UseVisualStyleBackColor = true;
            // 
            // btnImportAdd
            // 
            resources.ApplyResources(this.btnImportAdd, "btnImportAdd");
            this.btnImportAdd.Name = "btnImportAdd";
            this.toolTip1.SetToolTip(this.btnImportAdd, resources.GetString("btnImportAdd.ToolTip"));
            this.btnImportAdd.UseVisualStyleBackColor = true;
            // 
            // flyImportPanel
            // 
            this.flyImportPanel.AllowDrop = true;
            resources.ApplyResources(this.flyImportPanel, "flyImportPanel");
            this.flyImportPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flyImportPanel.Name = "flyImportPanel";
            // 
            // tabPagePlugins
            // 
            this.tabPagePlugins.Controls.Add(this.label4);
            this.tabPagePlugins.Controls.Add(this.flyPluginsItemsContainer);
            resources.ApplyResources(this.tabPagePlugins, "tabPagePlugins");
            this.tabPagePlugins.Name = "tabPagePlugins";
            this.tabPagePlugins.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // flyPluginsItemsContainer
            // 
            resources.ApplyResources(this.flyPluginsItemsContainer, "flyPluginsItemsContainer");
            this.flyPluginsItemsContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flyPluginsItemsContainer.Name = "flyPluginsItemsContainer";
            // 
            // btnBakBackup
            // 
            resources.ApplyResources(this.btnBakBackup, "btnBakBackup");
            this.btnBakBackup.Name = "btnBakBackup";
            this.toolTip1.SetToolTip(this.btnBakBackup, resources.GetString("btnBakBackup.ToolTip"));
            this.btnBakBackup.UseVisualStyleBackColor = true;
            this.btnBakBackup.Click += new System.EventHandler(this.btnBakBackup_Click);
            // 
            // btnOptionSave
            // 
            resources.ApplyResources(this.btnOptionSave, "btnOptionSave");
            this.btnOptionSave.Name = "btnOptionSave";
            this.toolTip1.SetToolTip(this.btnOptionSave, resources.GetString("btnOptionSave.ToolTip"));
            this.btnOptionSave.UseVisualStyleBackColor = true;
            this.btnOptionSave.Click += new System.EventHandler(this.btnOptionSave_Click);
            // 
            // btnBakRestore
            // 
            resources.ApplyResources(this.btnBakRestore, "btnBakRestore");
            this.btnBakRestore.Name = "btnBakRestore";
            this.toolTip1.SetToolTip(this.btnBakRestore, resources.GetString("btnBakRestore.ToolTip"));
            this.btnBakRestore.UseVisualStyleBackColor = true;
            this.btnBakRestore.Click += new System.EventHandler(this.btnBakRestore_Click);
            // 
            // btnOptionExit
            // 
            resources.ApplyResources(this.btnOptionExit, "btnOptionExit");
            this.btnOptionExit.Name = "btnOptionExit";
            this.btnOptionExit.UseVisualStyleBackColor = true;
            this.btnOptionExit.Click += new System.EventHandler(this.btnOptionExit_Click);
            // 
            // FormOption
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnBakRestore);
            this.Controls.Add(this.btnBakBackup);
            this.Controls.Add(this.btnOptionExit);
            this.Controls.Add(this.btnOptionSave);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormOption";
            this.Shown += new System.EventHandler(this.FormOption_Shown);
            this.tabControl1.ResumeLayout(false);
            this.tabPageSubscribe.ResumeLayout(false);
            this.tabPageSubscribe.PerformLayout();
            this.tabPageDefaults.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPageSetting.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageImport.ResumeLayout(false);
            this.tabPagePlugins.ResumeLayout(false);
            this.tabPagePlugins.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSubscribe;
        private System.Windows.Forms.FlowLayoutPanel flySubsUrlContainer;
        private System.Windows.Forms.Button btnAddSubsUrl;
        private System.Windows.Forms.Button btnUpdateViaSubscription;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnOptionSave;
        private System.Windows.Forms.Button btnOptionExit;
        private System.Windows.Forms.TabPage tabPageImport;
        private System.Windows.Forms.Button btnImportAdd;
        private System.Windows.Forms.FlowLayoutPanel flyImportPanel;
        private System.Windows.Forms.Button btnBakBackup;
        private System.Windows.Forms.Button btnBakRestore;
        private System.Windows.Forms.TabPage tabPageSetting;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboxSettingLanguage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboxSettingPageSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox chkSetServAutotrack;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox chkSetSysPortable;
        private System.Windows.Forms.CheckBox chkSetUseV4;
        private System.Windows.Forms.TabPage tabPagePlugins;
        private System.Windows.Forms.FlowLayoutPanel flyPluginsItemsContainer;
        private System.Windows.Forms.CheckBox chkSetServStatistics;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkSetUpgradeUseProxy;
        private System.Windows.Forms.CheckBox chkSubsIsUseProxy;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkSetCheckWhenStart;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabPage tabPageDefaults;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tboxDefImportAddr;
        private System.Windows.Forms.ComboBox cboxDefImportMode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tboxDefSpeedtestExpectedSize;
        private System.Windows.Forms.TextBox tboxDefSpeedtestCycles;
        private System.Windows.Forms.TextBox tboxDefSpeedtestUrl;
        private System.Windows.Forms.CheckBox chkDefSpeedtestIsUse;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tboxDefSpeedtestTimeout;
        private System.Windows.Forms.CheckBox chkDefImportSsShareLink;
        private System.Windows.Forms.CheckBox chkDefImportInjectGlobalImport;
        private System.Windows.Forms.CheckBox chkDefImportBypassCnSite;
        private System.Windows.Forms.Button btnSetOpenStartupFolder;
        private System.Windows.Forms.Button btnSubsInvertSelection;
        private System.Windows.Forms.Button btnSubsUseAll;
        private System.Windows.Forms.TextBox tboxSettingsMaxCoreNum;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
    }
}
