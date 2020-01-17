namespace V2RayGCon.Views.WinForms
{
    partial class FormConfiger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfiger));
            this.cboxConfigSection = new System.Windows.Forms.ComboBox();
            this.tabCtrlToolPanel = new System.Windows.Forms.TabControl();
            this.tabPageProtocol = new System.Windows.Forms.TabPage();
            this.chkIsV4 = new System.Windows.Forms.CheckBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.btnInsertSSSettings = new System.Windows.Forms.Button();
            this.chkSSIsShowPassword = new System.Windows.Forms.CheckBox();
            this.chkSSIsUseOTA = new System.Windows.Forms.CheckBox();
            this.cboxSSMethod = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tboxSSPassword = new System.Windows.Forms.TextBox();
            this.tboxSSAddr = new System.Windows.Forms.TextBox();
            this.rbtnIsServerMode = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkStreamUseSockopt = new System.Windows.Forms.CheckBox();
            this.chkStreamUseTls = new System.Windows.Forms.CheckBox();
            this.cboxStreamType = new System.Windows.Forms.ComboBox();
            this.cboxStreamParam = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btnInsertStream = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnVMessInsertClient = new System.Windows.Forms.Button();
            this.btnVMessGenUUID = new System.Windows.Forms.Button();
            this.tboxVMessIPaddr = new System.Windows.Forms.TextBox();
            this.tboxVMessAid = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tboxVMessLevel = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tboxVMessID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageMisc = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.tboxEnvValue = new System.Windows.Forms.TextBox();
            this.btnInsertEnv = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.cboxEnvName = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tboxImportURL = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnInsertImport = new System.Windows.Forms.Button();
            this.cboxImportAlias = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnQConMTProto = new System.Windows.Forms.Button();
            this.btnQConSkipCN = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tboxVGCDesc = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.btnInsertVGC = new System.Windows.Forms.Button();
            this.tboxVGCAlias = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tabPageExpanseImport = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveExpansedConfigToFile = new System.Windows.Forms.Button();
            this.btnExpandImport = new System.Windows.Forms.Button();
            this.cboxGlobalImport = new System.Windows.Forms.CheckBox();
            this.btnCopyExpansedConfig = new System.Windows.Forms.Button();
            this.btnImportClearCache = new System.Windows.Forms.Button();
            this.panelExpandConfig = new System.Windows.Forms.Panel();
            this.btnClearModify = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newWinToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.loadJsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.loadServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceExistServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showLeftPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideLeftPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlTools = new System.Windows.Forms.Panel();
            this.pnlEditor = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.btnFormat = new System.Windows.Forms.Button();
            this.cboxExamples = new System.Windows.Forms.ComboBox();
            this.panelScintilla = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.tabCtrlToolPanel.SuspendLayout();
            this.tabPageProtocol.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageMisc.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPageExpanseImport.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.panel1.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.pnlTools.SuspendLayout();
            this.pnlEditor.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboxConfigSection
            // 
            this.cboxConfigSection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cboxConfigSection, "cboxConfigSection");
            this.cboxConfigSection.FormattingEnabled = true;
            this.cboxConfigSection.Items.AddRange(new object[] {
            resources.GetString("cboxConfigSection.Items"),
            resources.GetString("cboxConfigSection.Items1")});
            this.cboxConfigSection.Name = "cboxConfigSection";
            // 
            // tabCtrlToolPanel
            // 
            resources.ApplyResources(this.tabCtrlToolPanel, "tabCtrlToolPanel");
            this.tabCtrlToolPanel.Controls.Add(this.tabPageProtocol);
            this.tabCtrlToolPanel.Controls.Add(this.tabPageMisc);
            this.tabCtrlToolPanel.Controls.Add(this.tabPageExpanseImport);
            this.tabCtrlToolPanel.Multiline = true;
            this.tabCtrlToolPanel.Name = "tabCtrlToolPanel";
            this.tabCtrlToolPanel.SelectedIndex = 0;
            this.tabCtrlToolPanel.MouseLeave += new System.EventHandler(this.TabCtrlToolPanel_MouseLeave);
            this.tabCtrlToolPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TabCtrlToolPanel_MouseMove);
            // 
            // tabPageProtocol
            // 
            this.tabPageProtocol.Controls.Add(this.chkIsV4);
            this.tabPageProtocol.Controls.Add(this.radioButton1);
            this.tabPageProtocol.Controls.Add(this.groupBox10);
            this.tabPageProtocol.Controls.Add(this.rbtnIsServerMode);
            this.tabPageProtocol.Controls.Add(this.groupBox2);
            this.tabPageProtocol.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.tabPageProtocol, "tabPageProtocol");
            this.tabPageProtocol.Name = "tabPageProtocol";
            this.tabPageProtocol.UseVisualStyleBackColor = true;
            // 
            // chkIsV4
            // 
            resources.ApplyResources(this.chkIsV4, "chkIsV4");
            this.chkIsV4.Name = "chkIsV4";
            this.toolTip1.SetToolTip(this.chkIsV4, resources.GetString("chkIsV4.ToolTip"));
            this.chkIsV4.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            resources.ApplyResources(this.radioButton1, "radioButton1");
            this.radioButton1.Checked = true;
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.TabStop = true;
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.btnInsertSSSettings);
            this.groupBox10.Controls.Add(this.chkSSIsShowPassword);
            this.groupBox10.Controls.Add(this.chkSSIsUseOTA);
            this.groupBox10.Controls.Add(this.cboxSSMethod);
            this.groupBox10.Controls.Add(this.label24);
            this.groupBox10.Controls.Add(this.label23);
            this.groupBox10.Controls.Add(this.label12);
            this.groupBox10.Controls.Add(this.tboxSSPassword);
            this.groupBox10.Controls.Add(this.tboxSSAddr);
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // btnInsertSSSettings
            // 
            resources.ApplyResources(this.btnInsertSSSettings, "btnInsertSSSettings");
            this.btnInsertSSSettings.Name = "btnInsertSSSettings";
            this.btnInsertSSSettings.UseVisualStyleBackColor = true;
            // 
            // chkSSIsShowPassword
            // 
            resources.ApplyResources(this.chkSSIsShowPassword, "chkSSIsShowPassword");
            this.chkSSIsShowPassword.Name = "chkSSIsShowPassword";
            this.toolTip1.SetToolTip(this.chkSSIsShowPassword, resources.GetString("chkSSIsShowPassword.ToolTip"));
            this.chkSSIsShowPassword.UseVisualStyleBackColor = true;
            // 
            // chkSSIsUseOTA
            // 
            resources.ApplyResources(this.chkSSIsUseOTA, "chkSSIsUseOTA");
            this.chkSSIsUseOTA.Name = "chkSSIsUseOTA";
            this.toolTip1.SetToolTip(this.chkSSIsUseOTA, resources.GetString("chkSSIsUseOTA.ToolTip"));
            this.chkSSIsUseOTA.UseVisualStyleBackColor = true;
            // 
            // cboxSSMethod
            // 
            this.cboxSSMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxSSMethod.FormattingEnabled = true;
            resources.ApplyResources(this.cboxSSMethod, "cboxSSMethod");
            this.cboxSSMethod.Name = "cboxSSMethod";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // tboxSSPassword
            // 
            resources.ApplyResources(this.tboxSSPassword, "tboxSSPassword");
            this.tboxSSPassword.Name = "tboxSSPassword";
            // 
            // tboxSSAddr
            // 
            resources.ApplyResources(this.tboxSSAddr, "tboxSSAddr");
            this.tboxSSAddr.Name = "tboxSSAddr";
            this.toolTip1.SetToolTip(this.tboxSSAddr, resources.GetString("tboxSSAddr.ToolTip"));
            // 
            // rbtnIsServerMode
            // 
            resources.ApplyResources(this.rbtnIsServerMode, "rbtnIsServerMode");
            this.rbtnIsServerMode.Name = "rbtnIsServerMode";
            this.rbtnIsServerMode.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkStreamUseSockopt);
            this.groupBox2.Controls.Add(this.chkStreamUseTls);
            this.groupBox2.Controls.Add(this.cboxStreamType);
            this.groupBox2.Controls.Add(this.cboxStreamParam);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.btnInsertStream);
            this.groupBox2.Controls.Add(this.label11);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // chkStreamUseSockopt
            // 
            resources.ApplyResources(this.chkStreamUseSockopt, "chkStreamUseSockopt");
            this.chkStreamUseSockopt.Name = "chkStreamUseSockopt";
            this.chkStreamUseSockopt.UseVisualStyleBackColor = true;
            // 
            // chkStreamUseTls
            // 
            resources.ApplyResources(this.chkStreamUseTls, "chkStreamUseTls");
            this.chkStreamUseTls.Name = "chkStreamUseTls";
            this.chkStreamUseTls.UseVisualStyleBackColor = true;
            // 
            // cboxStreamType
            // 
            this.cboxStreamType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxStreamType.FormattingEnabled = true;
            this.cboxStreamType.Items.AddRange(new object[] {
            resources.GetString("cboxStreamType.Items")});
            resources.ApplyResources(this.cboxStreamType, "cboxStreamType");
            this.cboxStreamType.Name = "cboxStreamType";
            // 
            // cboxStreamParam
            // 
            this.cboxStreamParam.FormattingEnabled = true;
            this.cboxStreamParam.Items.AddRange(new object[] {
            resources.GetString("cboxStreamParam.Items")});
            resources.ApplyResources(this.cboxStreamParam, "cboxStreamParam");
            this.cboxStreamParam.Name = "cboxStreamParam";
            this.toolTip1.SetToolTip(this.cboxStreamParam, resources.GetString("cboxStreamParam.ToolTip"));
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // btnInsertStream
            // 
            resources.ApplyResources(this.btnInsertStream, "btnInsertStream");
            this.btnInsertStream.Name = "btnInsertStream";
            this.btnInsertStream.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnVMessInsertClient);
            this.groupBox1.Controls.Add(this.btnVMessGenUUID);
            this.groupBox1.Controls.Add(this.tboxVMessIPaddr);
            this.groupBox1.Controls.Add(this.tboxVMessAid);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tboxVMessLevel);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tboxVMessID);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnVMessInsertClient
            // 
            resources.ApplyResources(this.btnVMessInsertClient, "btnVMessInsertClient");
            this.btnVMessInsertClient.Name = "btnVMessInsertClient";
            this.btnVMessInsertClient.UseVisualStyleBackColor = true;
            // 
            // btnVMessGenUUID
            // 
            resources.ApplyResources(this.btnVMessGenUUID, "btnVMessGenUUID");
            this.btnVMessGenUUID.Name = "btnVMessGenUUID";
            this.btnVMessGenUUID.UseVisualStyleBackColor = true;
            // 
            // tboxVMessIPaddr
            // 
            resources.ApplyResources(this.tboxVMessIPaddr, "tboxVMessIPaddr");
            this.tboxVMessIPaddr.Name = "tboxVMessIPaddr";
            this.toolTip1.SetToolTip(this.tboxVMessIPaddr, resources.GetString("tboxVMessIPaddr.ToolTip"));
            // 
            // tboxVMessAid
            // 
            resources.ApplyResources(this.tboxVMessAid, "tboxVMessAid");
            this.tboxVMessAid.Name = "tboxVMessAid";
            this.toolTip1.SetToolTip(this.tboxVMessAid, resources.GetString("tboxVMessAid.ToolTip"));
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tboxVMessLevel
            // 
            resources.ApplyResources(this.tboxVMessLevel, "tboxVMessLevel");
            this.tboxVMessLevel.Name = "tboxVMessLevel";
            this.toolTip1.SetToolTip(this.tboxVMessLevel, resources.GetString("tboxVMessLevel.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tboxVMessID
            // 
            resources.ApplyResources(this.tboxVMessID, "tboxVMessID");
            this.tboxVMessID.Name = "tboxVMessID";
            this.toolTip1.SetToolTip(this.tboxVMessID, resources.GetString("tboxVMessID.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tabPageMisc
            // 
            this.tabPageMisc.Controls.Add(this.groupBox8);
            this.tabPageMisc.Controls.Add(this.groupBox7);
            this.tabPageMisc.Controls.Add(this.groupBox5);
            this.tabPageMisc.Controls.Add(this.groupBox6);
            resources.ApplyResources(this.tabPageMisc, "tabPageMisc");
            this.tabPageMisc.Name = "tabPageMisc";
            this.tabPageMisc.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.tboxEnvValue);
            this.groupBox8.Controls.Add(this.btnInsertEnv);
            this.groupBox8.Controls.Add(this.label22);
            this.groupBox8.Controls.Add(this.cboxEnvName);
            this.groupBox8.Controls.Add(this.label21);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // tboxEnvValue
            // 
            resources.ApplyResources(this.tboxEnvValue, "tboxEnvValue");
            this.tboxEnvValue.Name = "tboxEnvValue";
            // 
            // btnInsertEnv
            // 
            resources.ApplyResources(this.btnInsertEnv, "btnInsertEnv");
            this.btnInsertEnv.Name = "btnInsertEnv";
            this.btnInsertEnv.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // cboxEnvName
            // 
            this.cboxEnvName.FormattingEnabled = true;
            resources.ApplyResources(this.cboxEnvName, "cboxEnvName");
            this.cboxEnvName.Name = "cboxEnvName";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.tboxImportURL);
            this.groupBox7.Controls.Add(this.label20);
            this.groupBox7.Controls.Add(this.btnInsertImport);
            this.groupBox7.Controls.Add(this.cboxImportAlias);
            this.groupBox7.Controls.Add(this.label13);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // tboxImportURL
            // 
            resources.ApplyResources(this.tboxImportURL, "tboxImportURL");
            this.tboxImportURL.Name = "tboxImportURL";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // btnInsertImport
            // 
            resources.ApplyResources(this.btnInsertImport, "btnInsertImport");
            this.btnInsertImport.Name = "btnInsertImport";
            this.btnInsertImport.UseVisualStyleBackColor = true;
            // 
            // cboxImportAlias
            // 
            this.cboxImportAlias.FormattingEnabled = true;
            resources.ApplyResources(this.cboxImportAlias, "cboxImportAlias");
            this.cboxImportAlias.Name = "cboxImportAlias";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnQConMTProto);
            this.groupBox5.Controls.Add(this.btnQConSkipCN);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // btnQConMTProto
            // 
            resources.ApplyResources(this.btnQConMTProto, "btnQConMTProto");
            this.btnQConMTProto.Name = "btnQConMTProto";
            this.toolTip1.SetToolTip(this.btnQConMTProto, resources.GetString("btnQConMTProto.ToolTip"));
            this.btnQConMTProto.UseVisualStyleBackColor = true;
            // 
            // btnQConSkipCN
            // 
            resources.ApplyResources(this.btnQConSkipCN, "btnQConSkipCN");
            this.btnQConSkipCN.Name = "btnQConSkipCN";
            this.toolTip1.SetToolTip(this.btnQConSkipCN, resources.GetString("btnQConSkipCN.ToolTip"));
            this.btnQConSkipCN.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tboxVGCDesc);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.btnInsertVGC);
            this.groupBox6.Controls.Add(this.tboxVGCAlias);
            this.groupBox6.Controls.Add(this.label15);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // tboxVGCDesc
            // 
            resources.ApplyResources(this.tboxVGCDesc, "tboxVGCDesc");
            this.tboxVGCDesc.Name = "tboxVGCDesc";
            this.toolTip1.SetToolTip(this.tboxVGCDesc, resources.GetString("tboxVGCDesc.ToolTip"));
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // btnInsertVGC
            // 
            resources.ApplyResources(this.btnInsertVGC, "btnInsertVGC");
            this.btnInsertVGC.Name = "btnInsertVGC";
            this.btnInsertVGC.UseVisualStyleBackColor = true;
            // 
            // tboxVGCAlias
            // 
            resources.ApplyResources(this.tboxVGCAlias, "tboxVGCAlias");
            this.tboxVGCAlias.Name = "tboxVGCAlias";
            this.toolTip1.SetToolTip(this.tboxVGCAlias, resources.GetString("tboxVGCAlias.ToolTip"));
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // tabPageExpanseImport
            // 
            this.tabPageExpanseImport.Controls.Add(this.groupBox9);
            resources.ApplyResources(this.tabPageExpanseImport, "tabPageExpanseImport");
            this.tabPageExpanseImport.Name = "tabPageExpanseImport";
            this.tabPageExpanseImport.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Controls.Add(this.panel1);
            this.groupBox9.Controls.Add(this.panelExpandConfig);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.btnSaveExpansedConfigToFile);
            this.panel1.Controls.Add(this.btnExpandImport);
            this.panel1.Controls.Add(this.cboxGlobalImport);
            this.panel1.Controls.Add(this.btnCopyExpansedConfig);
            this.panel1.Controls.Add(this.btnImportClearCache);
            this.panel1.Name = "panel1";
            // 
            // btnSaveExpansedConfigToFile
            // 
            resources.ApplyResources(this.btnSaveExpansedConfigToFile, "btnSaveExpansedConfigToFile");
            this.btnSaveExpansedConfigToFile.Name = "btnSaveExpansedConfigToFile";
            this.toolTip1.SetToolTip(this.btnSaveExpansedConfigToFile, resources.GetString("btnSaveExpansedConfigToFile.ToolTip"));
            this.btnSaveExpansedConfigToFile.UseVisualStyleBackColor = true;
            // 
            // btnExpandImport
            // 
            resources.ApplyResources(this.btnExpandImport, "btnExpandImport");
            this.btnExpandImport.Name = "btnExpandImport";
            this.btnExpandImport.UseVisualStyleBackColor = true;
            // 
            // cboxGlobalImport
            // 
            resources.ApplyResources(this.cboxGlobalImport, "cboxGlobalImport");
            this.cboxGlobalImport.Checked = true;
            this.cboxGlobalImport.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxGlobalImport.Name = "cboxGlobalImport";
            this.toolTip1.SetToolTip(this.cboxGlobalImport, resources.GetString("cboxGlobalImport.ToolTip"));
            this.cboxGlobalImport.UseVisualStyleBackColor = true;
            // 
            // btnCopyExpansedConfig
            // 
            resources.ApplyResources(this.btnCopyExpansedConfig, "btnCopyExpansedConfig");
            this.btnCopyExpansedConfig.Name = "btnCopyExpansedConfig";
            this.btnCopyExpansedConfig.UseVisualStyleBackColor = true;
            // 
            // btnImportClearCache
            // 
            resources.ApplyResources(this.btnImportClearCache, "btnImportClearCache");
            this.btnImportClearCache.Name = "btnImportClearCache";
            this.btnImportClearCache.UseVisualStyleBackColor = true;
            // 
            // panelExpandConfig
            // 
            resources.ApplyResources(this.panelExpandConfig, "panelExpandConfig");
            this.panelExpandConfig.Name = "panelExpandConfig";
            // 
            // btnClearModify
            // 
            resources.ApplyResources(this.btnClearModify, "btnClearModify");
            this.btnClearModify.Name = "btnClearModify";
            this.btnClearModify.UseVisualStyleBackColor = true;
            // 
            // mainMenu
            // 
            resources.ApplyResources(this.mainMenu, "mainMenu");
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.configToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.mainMenu.Name = "mainMenu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newWinToolStripMenuItem1,
            this.toolStripSeparator4,
            this.loadJsonToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // newWinToolStripMenuItem1
            // 
            this.newWinToolStripMenuItem1.Name = "newWinToolStripMenuItem1";
            resources.ApplyResources(this.newWinToolStripMenuItem1, "newWinToolStripMenuItem1");
            this.newWinToolStripMenuItem1.Click += new System.EventHandler(this.NewWinToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // loadJsonToolStripMenuItem
            // 
            this.loadJsonToolStripMenuItem.Name = "loadJsonToolStripMenuItem";
            resources.ApplyResources(this.loadJsonToolStripMenuItem, "loadJsonToolStripMenuItem");
            this.loadJsonToolStripMenuItem.Click += new System.EventHandler(this.LoadJsonToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewServerToolStripMenuItem,
            this.saveConfigStripMenuItem,
            this.toolStripSeparator2,
            this.loadServerToolStripMenuItem,
            this.replaceExistServerToolStripMenuItem});
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            resources.ApplyResources(this.configToolStripMenuItem, "configToolStripMenuItem");
            // 
            // addNewServerToolStripMenuItem
            // 
            this.addNewServerToolStripMenuItem.Name = "addNewServerToolStripMenuItem";
            resources.ApplyResources(this.addNewServerToolStripMenuItem, "addNewServerToolStripMenuItem");
            this.addNewServerToolStripMenuItem.Click += new System.EventHandler(this.AddNewServerToolStripMenuItem_Click);
            // 
            // saveConfigStripMenuItem
            // 
            this.saveConfigStripMenuItem.Name = "saveConfigStripMenuItem";
            resources.ApplyResources(this.saveConfigStripMenuItem, "saveConfigStripMenuItem");
            this.saveConfigStripMenuItem.Click += new System.EventHandler(this.SaveConfigStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // loadServerToolStripMenuItem
            // 
            this.loadServerToolStripMenuItem.Name = "loadServerToolStripMenuItem";
            resources.ApplyResources(this.loadServerToolStripMenuItem, "loadServerToolStripMenuItem");
            // 
            // replaceExistServerToolStripMenuItem
            // 
            this.replaceExistServerToolStripMenuItem.Name = "replaceExistServerToolStripMenuItem";
            resources.ApplyResources(this.replaceExistServerToolStripMenuItem, "replaceExistServerToolStripMenuItem");
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchBoxToolStripMenuItem,
            this.toolStripSeparator3,
            this.showLeftPanelToolStripMenuItem,
            this.hideLeftPanelToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // searchBoxToolStripMenuItem
            // 
            this.searchBoxToolStripMenuItem.Name = "searchBoxToolStripMenuItem";
            resources.ApplyResources(this.searchBoxToolStripMenuItem, "searchBoxToolStripMenuItem");
            this.searchBoxToolStripMenuItem.Click += new System.EventHandler(this.SearchBoxToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // showLeftPanelToolStripMenuItem
            // 
            this.showLeftPanelToolStripMenuItem.Checked = true;
            this.showLeftPanelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showLeftPanelToolStripMenuItem.Name = "showLeftPanelToolStripMenuItem";
            resources.ApplyResources(this.showLeftPanelToolStripMenuItem, "showLeftPanelToolStripMenuItem");
            this.showLeftPanelToolStripMenuItem.Click += new System.EventHandler(this.ShowLeftPanelToolStripMenuItem_Click);
            // 
            // hideLeftPanelToolStripMenuItem
            // 
            this.hideLeftPanelToolStripMenuItem.Name = "hideLeftPanelToolStripMenuItem";
            resources.ApplyResources(this.hideLeftPanelToolStripMenuItem, "hideLeftPanelToolStripMenuItem");
            this.hideLeftPanelToolStripMenuItem.Click += new System.EventHandler(this.HideLeftPanelToolStripMenuItem_Click);
            // 
            // pnlTools
            // 
            resources.ApplyResources(this.pnlTools, "pnlTools");
            this.pnlTools.Controls.Add(this.tabCtrlToolPanel);
            this.pnlTools.Name = "pnlTools";
            // 
            // pnlEditor
            // 
            resources.ApplyResources(this.pnlEditor, "pnlEditor");
            this.pnlEditor.Controls.Add(this.tableLayoutPanel1);
            this.pnlEditor.Controls.Add(this.label5);
            this.pnlEditor.Name = "pnlEditor";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelScintilla, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.cboxConfigSection);
            this.panel2.Controls.Add(this.btnClearModify);
            this.panel2.Controls.Add(this.btnFormat);
            this.panel2.Controls.Add(this.cboxExamples);
            this.panel2.Name = "panel2";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // btnFormat
            // 
            resources.ApplyResources(this.btnFormat, "btnFormat");
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.UseVisualStyleBackColor = true;
            // 
            // cboxExamples
            // 
            this.cboxExamples.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cboxExamples, "cboxExamples");
            this.cboxExamples.FormattingEnabled = true;
            this.cboxExamples.Name = "cboxExamples";
            // 
            // panelScintilla
            // 
            resources.ApplyResources(this.panelScintilla, "panelScintilla");
            this.panelScintilla.Name = "panelScintilla";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // FormConfiger
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlTools);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.pnlEditor);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "FormConfiger";
            this.Shown += new System.EventHandler(this.FormConfiger_Shown);
            this.tabCtrlToolPanel.ResumeLayout(false);
            this.tabPageProtocol.ResumeLayout(false);
            this.tabPageProtocol.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageMisc.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPageExpanseImport.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.pnlTools.ResumeLayout(false);
            this.pnlEditor.ResumeLayout(false);
            this.pnlEditor.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cboxConfigSection;
        private System.Windows.Forms.TabControl tabCtrlToolPanel;
        private System.Windows.Forms.TabPage tabPageProtocol;
        private System.Windows.Forms.Button btnClearModify;
        private System.Windows.Forms.Button btnVMessInsertClient;
        private System.Windows.Forms.TextBox tboxVMessIPaddr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tboxVMessAid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tboxVMessLevel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tboxVMessID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnVMessGenUUID;
        private System.Windows.Forms.TabPage tabPageMisc;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadJsonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLeftPanelToolStripMenuItem;
        private System.Windows.Forms.Panel pnlTools;
        private System.Windows.Forms.Panel pnlEditor;
        private System.Windows.Forms.Panel panelScintilla;
        private System.Windows.Forms.ToolStripMenuItem hideLeftPanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceExistServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem newWinToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ComboBox cboxExamples;
        private System.Windows.Forms.ToolStripMenuItem searchBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton rbtnIsServerMode;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboxStreamType;
        private System.Windows.Forms.ComboBox cboxStreamParam;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnInsertStream;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox tboxVGCDesc;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btnInsertVGC;
        private System.Windows.Forms.TextBox tboxVGCAlias;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnQConSkipCN;
        private System.Windows.Forms.ToolStripMenuItem saveConfigStripMenuItem;
        private System.Windows.Forms.TabPage tabPageExpanseImport;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Panel panelExpandConfig;
        private System.Windows.Forms.Button btnExpandImport;
        private System.Windows.Forms.Button btnCopyExpansedConfig;
        private System.Windows.Forms.Button btnQConMTProto;
        private System.Windows.Forms.Button btnImportClearCache;
        private System.Windows.Forms.CheckBox cboxGlobalImport;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TextBox tboxEnvValue;
        private System.Windows.Forms.Button btnInsertEnv;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox cboxEnvName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox tboxImportURL;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnInsertImport;
        private System.Windows.Forms.ComboBox cboxImportAlias;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox chkStreamUseSockopt;
        private System.Windows.Forms.CheckBox chkStreamUseTls;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Button btnInsertSSSettings;
        private System.Windows.Forms.CheckBox chkSSIsShowPassword;
        private System.Windows.Forms.CheckBox chkSSIsUseOTA;
        private System.Windows.Forms.ComboBox cboxSSMethod;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tboxSSPassword;
        private System.Windows.Forms.TextBox tboxSSAddr;
        private System.Windows.Forms.CheckBox chkIsV4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSaveExpansedConfigToFile;
    }
}
