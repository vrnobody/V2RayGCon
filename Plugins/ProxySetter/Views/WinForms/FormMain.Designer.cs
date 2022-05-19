namespace ProxySetter.Views.WinForms
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabBasic = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBasicUseHotkey = new System.Windows.Forms.CheckBox();
            this.chkBasicUseShift = new System.Windows.Forms.CheckBox();
            this.chkBasicUseAlt = new System.Windows.Forms.CheckBox();
            this.tboxBasicHotkey = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cboxBasicPacProtocol = new System.Windows.Forms.ComboBox();
            this.chkBasicUseCustomPac = new System.Windows.Forms.CheckBox();
            this.cboxBasicPacMode = new System.Windows.Forms.ComboBox();
            this.cboxBasicSysProxyMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBasicBrowseCustomPac = new System.Windows.Forms.Button();
            this.chkBasicAutoUpdateSysProxy = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkBasicPacAlwaysOn = new System.Windows.Forms.CheckBox();
            this.tboxBasicCustomPacPath = new System.Windows.Forms.TextBox();
            this.tboxBaiscPacPort = new System.Windows.Forms.TextBox();
            this.tboxBasicGlobalPort = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBasicViewInNotepad = new System.Windows.Forms.Button();
            this.lbBasicProxyLink = new System.Windows.Forms.Label();
            this.lbBasicCurPacServerStatus = new System.Windows.Forms.Label();
            this.btnBaiscCopyProxyLink = new System.Windows.Forms.Button();
            this.btnBasicDebugPacServer = new System.Windows.Forms.Button();
            this.btnBasicStartPacServer = new System.Windows.Forms.Button();
            this.btnBasicStopPacServer = new System.Windows.Forms.Button();
            this.tabPac = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSetSortWhitelist = new System.Windows.Forms.Button();
            this.rtboxPacWhiteList = new System.Windows.Forms.RichTextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnSetSortBlacklist = new System.Windows.Forms.Button();
            this.rtboxPacBlackList = new System.Windows.Forms.RichTextBox();
            this.tabUsage = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tboxUsageReadMe = new System.Windows.Forms.TextBox();
            this.linkLabelUsageTxthinkingPac = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabBasic.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPac.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabUsage.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.toolTip1.SetToolTip(this.tableLayoutPanel1, resources.GetString("tableLayoutPanel1.ToolTip"));
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabBasic);
            this.tabControl1.Controls.Add(this.tabPac);
            this.tabControl1.Controls.Add(this.tabUsage);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.toolTip1.SetToolTip(this.tabControl1, resources.GetString("tabControl1.ToolTip"));
            // 
            // tabBasic
            // 
            resources.ApplyResources(this.tabBasic, "tabBasic");
            this.tabBasic.Controls.Add(this.tableLayoutPanel2);
            this.tabBasic.Name = "tabBasic";
            this.toolTip1.SetToolTip(this.tabBasic, resources.GetString("tabBasic.ToolTip"));
            this.tabBasic.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.toolTip1.SetToolTip(this.tableLayoutPanel2, resources.GetString("tableLayoutPanel2.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.chkBasicUseHotkey);
            this.groupBox1.Controls.Add(this.chkBasicUseShift);
            this.groupBox1.Controls.Add(this.chkBasicUseAlt);
            this.groupBox1.Controls.Add(this.tboxBasicHotkey);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cboxBasicPacProtocol);
            this.groupBox1.Controls.Add(this.chkBasicUseCustomPac);
            this.groupBox1.Controls.Add(this.cboxBasicPacMode);
            this.groupBox1.Controls.Add(this.cboxBasicSysProxyMode);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnBasicBrowseCustomPac);
            this.groupBox1.Controls.Add(this.chkBasicAutoUpdateSysProxy);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkBasicPacAlwaysOn);
            this.groupBox1.Controls.Add(this.tboxBasicCustomPacPath);
            this.groupBox1.Controls.Add(this.tboxBaiscPacPort);
            this.groupBox1.Controls.Add(this.tboxBasicGlobalPort);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // chkBasicUseHotkey
            // 
            resources.ApplyResources(this.chkBasicUseHotkey, "chkBasicUseHotkey");
            this.chkBasicUseHotkey.Name = "chkBasicUseHotkey";
            this.toolTip1.SetToolTip(this.chkBasicUseHotkey, resources.GetString("chkBasicUseHotkey.ToolTip"));
            this.chkBasicUseHotkey.UseVisualStyleBackColor = true;
            // 
            // chkBasicUseShift
            // 
            resources.ApplyResources(this.chkBasicUseShift, "chkBasicUseShift");
            this.chkBasicUseShift.Name = "chkBasicUseShift";
            this.toolTip1.SetToolTip(this.chkBasicUseShift, resources.GetString("chkBasicUseShift.ToolTip"));
            this.chkBasicUseShift.UseVisualStyleBackColor = true;
            // 
            // chkBasicUseAlt
            // 
            resources.ApplyResources(this.chkBasicUseAlt, "chkBasicUseAlt");
            this.chkBasicUseAlt.Name = "chkBasicUseAlt";
            this.toolTip1.SetToolTip(this.chkBasicUseAlt, resources.GetString("chkBasicUseAlt.ToolTip"));
            this.chkBasicUseAlt.UseVisualStyleBackColor = true;
            // 
            // tboxBasicHotkey
            // 
            resources.ApplyResources(this.tboxBasicHotkey, "tboxBasicHotkey");
            this.tboxBasicHotkey.Name = "tboxBasicHotkey";
            this.toolTip1.SetToolTip(this.tboxBasicHotkey, resources.GetString("tboxBasicHotkey.ToolTip"));
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            this.toolTip1.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // cboxBasicPacProtocol
            // 
            resources.ApplyResources(this.cboxBasicPacProtocol, "cboxBasicPacProtocol");
            this.cboxBasicPacProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxBasicPacProtocol.FormattingEnabled = true;
            this.cboxBasicPacProtocol.Items.AddRange(new object[] {
            resources.GetString("cboxBasicPacProtocol.Items"),
            resources.GetString("cboxBasicPacProtocol.Items1")});
            this.cboxBasicPacProtocol.Name = "cboxBasicPacProtocol";
            this.toolTip1.SetToolTip(this.cboxBasicPacProtocol, resources.GetString("cboxBasicPacProtocol.ToolTip"));
            // 
            // chkBasicUseCustomPac
            // 
            resources.ApplyResources(this.chkBasicUseCustomPac, "chkBasicUseCustomPac");
            this.chkBasicUseCustomPac.Name = "chkBasicUseCustomPac";
            this.toolTip1.SetToolTip(this.chkBasicUseCustomPac, resources.GetString("chkBasicUseCustomPac.ToolTip"));
            this.chkBasicUseCustomPac.UseVisualStyleBackColor = true;
            this.chkBasicUseCustomPac.CheckedChanged += new System.EventHandler(this.chkBasicUseCustomPac_CheckedChanged);
            // 
            // cboxBasicPacMode
            // 
            resources.ApplyResources(this.cboxBasicPacMode, "cboxBasicPacMode");
            this.cboxBasicPacMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxBasicPacMode.FormattingEnabled = true;
            this.cboxBasicPacMode.Items.AddRange(new object[] {
            resources.GetString("cboxBasicPacMode.Items"),
            resources.GetString("cboxBasicPacMode.Items1")});
            this.cboxBasicPacMode.Name = "cboxBasicPacMode";
            this.toolTip1.SetToolTip(this.cboxBasicPacMode, resources.GetString("cboxBasicPacMode.ToolTip"));
            // 
            // cboxBasicSysProxyMode
            // 
            resources.ApplyResources(this.cboxBasicSysProxyMode, "cboxBasicSysProxyMode");
            this.cboxBasicSysProxyMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxBasicSysProxyMode.FormattingEnabled = true;
            this.cboxBasicSysProxyMode.Items.AddRange(new object[] {
            resources.GetString("cboxBasicSysProxyMode.Items"),
            resources.GetString("cboxBasicSysProxyMode.Items1"),
            resources.GetString("cboxBasicSysProxyMode.Items2"),
            resources.GetString("cboxBasicSysProxyMode.Items3")});
            this.cboxBasicSysProxyMode.Name = "cboxBasicSysProxyMode";
            this.toolTip1.SetToolTip(this.cboxBasicSysProxyMode, resources.GetString("cboxBasicSysProxyMode.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // btnBasicBrowseCustomPac
            // 
            resources.ApplyResources(this.btnBasicBrowseCustomPac, "btnBasicBrowseCustomPac");
            this.btnBasicBrowseCustomPac.Name = "btnBasicBrowseCustomPac";
            this.toolTip1.SetToolTip(this.btnBasicBrowseCustomPac, resources.GetString("btnBasicBrowseCustomPac.ToolTip"));
            this.btnBasicBrowseCustomPac.UseVisualStyleBackColor = true;
            // 
            // chkBasicAutoUpdateSysProxy
            // 
            resources.ApplyResources(this.chkBasicAutoUpdateSysProxy, "chkBasicAutoUpdateSysProxy");
            this.chkBasicAutoUpdateSysProxy.Name = "chkBasicAutoUpdateSysProxy";
            this.toolTip1.SetToolTip(this.chkBasicAutoUpdateSysProxy, resources.GetString("chkBasicAutoUpdateSysProxy.ToolTip"));
            this.chkBasicAutoUpdateSysProxy.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // chkBasicPacAlwaysOn
            // 
            resources.ApplyResources(this.chkBasicPacAlwaysOn, "chkBasicPacAlwaysOn");
            this.chkBasicPacAlwaysOn.Name = "chkBasicPacAlwaysOn";
            this.toolTip1.SetToolTip(this.chkBasicPacAlwaysOn, resources.GetString("chkBasicPacAlwaysOn.ToolTip"));
            this.chkBasicPacAlwaysOn.UseVisualStyleBackColor = true;
            // 
            // tboxBasicCustomPacPath
            // 
            resources.ApplyResources(this.tboxBasicCustomPacPath, "tboxBasicCustomPacPath");
            this.tboxBasicCustomPacPath.Name = "tboxBasicCustomPacPath";
            this.toolTip1.SetToolTip(this.tboxBasicCustomPacPath, resources.GetString("tboxBasicCustomPacPath.ToolTip"));
            // 
            // tboxBaiscPacPort
            // 
            resources.ApplyResources(this.tboxBaiscPacPort, "tboxBaiscPacPort");
            this.tboxBaiscPacPort.Name = "tboxBaiscPacPort";
            this.toolTip1.SetToolTip(this.tboxBaiscPacPort, resources.GetString("tboxBaiscPacPort.ToolTip"));
            // 
            // tboxBasicGlobalPort
            // 
            resources.ApplyResources(this.tboxBasicGlobalPort, "tboxBasicGlobalPort");
            this.tboxBasicGlobalPort.Name = "tboxBasicGlobalPort";
            this.toolTip1.SetToolTip(this.tboxBasicGlobalPort, resources.GetString("tboxBasicGlobalPort.ToolTip"));
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.btnBasicViewInNotepad);
            this.groupBox2.Controls.Add(this.lbBasicProxyLink);
            this.groupBox2.Controls.Add(this.lbBasicCurPacServerStatus);
            this.groupBox2.Controls.Add(this.btnBaiscCopyProxyLink);
            this.groupBox2.Controls.Add(this.btnBasicDebugPacServer);
            this.groupBox2.Controls.Add(this.btnBasicStartPacServer);
            this.groupBox2.Controls.Add(this.btnBasicStopPacServer);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // btnBasicViewInNotepad
            // 
            resources.ApplyResources(this.btnBasicViewInNotepad, "btnBasicViewInNotepad");
            this.btnBasicViewInNotepad.Name = "btnBasicViewInNotepad";
            this.toolTip1.SetToolTip(this.btnBasicViewInNotepad, resources.GetString("btnBasicViewInNotepad.ToolTip"));
            this.btnBasicViewInNotepad.UseVisualStyleBackColor = true;
            // 
            // lbBasicProxyLink
            // 
            resources.ApplyResources(this.lbBasicProxyLink, "lbBasicProxyLink");
            this.lbBasicProxyLink.AutoEllipsis = true;
            this.lbBasicProxyLink.Name = "lbBasicProxyLink";
            this.toolTip1.SetToolTip(this.lbBasicProxyLink, resources.GetString("lbBasicProxyLink.ToolTip"));
            // 
            // lbBasicCurPacServerStatus
            // 
            resources.ApplyResources(this.lbBasicCurPacServerStatus, "lbBasicCurPacServerStatus");
            this.lbBasicCurPacServerStatus.Name = "lbBasicCurPacServerStatus";
            this.toolTip1.SetToolTip(this.lbBasicCurPacServerStatus, resources.GetString("lbBasicCurPacServerStatus.ToolTip"));
            // 
            // btnBaiscCopyProxyLink
            // 
            resources.ApplyResources(this.btnBaiscCopyProxyLink, "btnBaiscCopyProxyLink");
            this.btnBaiscCopyProxyLink.Name = "btnBaiscCopyProxyLink";
            this.toolTip1.SetToolTip(this.btnBaiscCopyProxyLink, resources.GetString("btnBaiscCopyProxyLink.ToolTip"));
            this.btnBaiscCopyProxyLink.UseVisualStyleBackColor = true;
            // 
            // btnBasicDebugPacServer
            // 
            resources.ApplyResources(this.btnBasicDebugPacServer, "btnBasicDebugPacServer");
            this.btnBasicDebugPacServer.Name = "btnBasicDebugPacServer";
            this.toolTip1.SetToolTip(this.btnBasicDebugPacServer, resources.GetString("btnBasicDebugPacServer.ToolTip"));
            this.btnBasicDebugPacServer.UseVisualStyleBackColor = true;
            // 
            // btnBasicStartPacServer
            // 
            resources.ApplyResources(this.btnBasicStartPacServer, "btnBasicStartPacServer");
            this.btnBasicStartPacServer.Name = "btnBasicStartPacServer";
            this.toolTip1.SetToolTip(this.btnBasicStartPacServer, resources.GetString("btnBasicStartPacServer.ToolTip"));
            this.btnBasicStartPacServer.UseVisualStyleBackColor = true;
            // 
            // btnBasicStopPacServer
            // 
            resources.ApplyResources(this.btnBasicStopPacServer, "btnBasicStopPacServer");
            this.btnBasicStopPacServer.Name = "btnBasicStopPacServer";
            this.toolTip1.SetToolTip(this.btnBasicStopPacServer, resources.GetString("btnBasicStopPacServer.ToolTip"));
            this.btnBasicStopPacServer.UseVisualStyleBackColor = true;
            // 
            // tabPac
            // 
            resources.ApplyResources(this.tabPac, "tabPac");
            this.tabPac.Controls.Add(this.tableLayoutPanel3);
            this.tabPac.Name = "tabPac";
            this.toolTip1.SetToolTip(this.tabPac, resources.GetString("tabPac.ToolTip"));
            this.tabPac.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.groupBox4, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.toolTip1.SetToolTip(this.tableLayoutPanel3, resources.GetString("tableLayoutPanel3.ToolTip"));
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.btnSetSortWhitelist);
            this.groupBox3.Controls.Add(this.rtboxPacWhiteList);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // btnSetSortWhitelist
            // 
            resources.ApplyResources(this.btnSetSortWhitelist, "btnSetSortWhitelist");
            this.btnSetSortWhitelist.Name = "btnSetSortWhitelist";
            this.toolTip1.SetToolTip(this.btnSetSortWhitelist, resources.GetString("btnSetSortWhitelist.ToolTip"));
            this.btnSetSortWhitelist.UseVisualStyleBackColor = true;
            // 
            // rtboxPacWhiteList
            // 
            resources.ApplyResources(this.rtboxPacWhiteList, "rtboxPacWhiteList");
            this.rtboxPacWhiteList.Name = "rtboxPacWhiteList";
            this.toolTip1.SetToolTip(this.rtboxPacWhiteList, resources.GetString("rtboxPacWhiteList.ToolTip"));
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.btnSetSortBlacklist);
            this.groupBox4.Controls.Add(this.rtboxPacBlackList);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox4, resources.GetString("groupBox4.ToolTip"));
            // 
            // btnSetSortBlacklist
            // 
            resources.ApplyResources(this.btnSetSortBlacklist, "btnSetSortBlacklist");
            this.btnSetSortBlacklist.Name = "btnSetSortBlacklist";
            this.toolTip1.SetToolTip(this.btnSetSortBlacklist, resources.GetString("btnSetSortBlacklist.ToolTip"));
            this.btnSetSortBlacklist.UseVisualStyleBackColor = true;
            // 
            // rtboxPacBlackList
            // 
            resources.ApplyResources(this.rtboxPacBlackList, "rtboxPacBlackList");
            this.rtboxPacBlackList.Name = "rtboxPacBlackList";
            this.toolTip1.SetToolTip(this.rtboxPacBlackList, resources.GetString("rtboxPacBlackList.ToolTip"));
            // 
            // tabUsage
            // 
            resources.ApplyResources(this.tabUsage, "tabUsage");
            this.tabUsage.Controls.Add(this.panel2);
            this.tabUsage.Name = "tabUsage";
            this.toolTip1.SetToolTip(this.tabUsage, resources.GetString("tabUsage.ToolTip"));
            this.tabUsage.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.groupBox5);
            this.panel2.Name = "panel2";
            this.toolTip1.SetToolTip(this.panel2, resources.GetString("panel2.ToolTip"));
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.tboxUsageReadMe);
            this.groupBox5.Controls.Add(this.linkLabelUsageTxthinkingPac);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox5, resources.GetString("groupBox5.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // tboxUsageReadMe
            // 
            resources.ApplyResources(this.tboxUsageReadMe, "tboxUsageReadMe");
            this.tboxUsageReadMe.Name = "tboxUsageReadMe";
            this.tboxUsageReadMe.ReadOnly = true;
            this.toolTip1.SetToolTip(this.tboxUsageReadMe, resources.GetString("tboxUsageReadMe.ToolTip"));
            // 
            // linkLabelUsageTxthinkingPac
            // 
            resources.ApplyResources(this.linkLabelUsageTxthinkingPac, "linkLabelUsageTxthinkingPac");
            this.linkLabelUsageTxthinkingPac.Name = "linkLabelUsageTxthinkingPac";
            this.linkLabelUsageTxthinkingPac.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabelUsageTxthinkingPac, resources.GetString("linkLabelUsageTxthinkingPac.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Name = "panel1";
            this.toolTip1.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.toolTip1.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormMain";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Shown += new System.EventHandler(this.FormPluginMain_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabBasic.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPac.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tabUsage.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabBasic;
        private System.Windows.Forms.TextBox tboxBasicGlobalPort;
        private System.Windows.Forms.TextBox tboxBaiscPacPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboxBasicSysProxyMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPac;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkBasicAutoUpdateSysProxy;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkBasicPacAlwaysOn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBasicBrowseCustomPac;
        private System.Windows.Forms.TextBox tboxBasicCustomPacPath;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbBasicCurPacServerStatus;
        private System.Windows.Forms.Button btnBasicDebugPacServer;
        private System.Windows.Forms.Button btnBasicStartPacServer;
        private System.Windows.Forms.Button btnBasicStopPacServer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox rtboxPacWhiteList;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RichTextBox rtboxPacBlackList;
        private System.Windows.Forms.ComboBox cboxBasicPacMode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbBasicProxyLink;
        private System.Windows.Forms.Button btnBaiscCopyProxyLink;
        private System.Windows.Forms.CheckBox chkBasicUseCustomPac;
        private System.Windows.Forms.ComboBox cboxBasicPacProtocol;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBasicViewInNotepad;
        private System.Windows.Forms.TabPage tabUsage;
        private System.Windows.Forms.TextBox tboxUsageReadMe;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel linkLabelUsageTxthinkingPac;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnSetSortWhitelist;
        private System.Windows.Forms.Button btnSetSortBlacklist;
        private System.Windows.Forms.CheckBox chkBasicUseHotkey;
        private System.Windows.Forms.CheckBox chkBasicUseShift;
        private System.Windows.Forms.CheckBox chkBasicUseAlt;
        private System.Windows.Forms.TextBox tboxBasicHotkey;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
    }
}
