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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabUsage = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tboxUsageReadMe = new System.Windows.Forms.TextBox();
            this.tabBasic = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBasicStopPacServer = new System.Windows.Forms.Button();
            this.btnBasicStartPacServer = new System.Windows.Forms.Button();
            this.btnBasicDebugPacServer = new System.Windows.Forms.Button();
            this.btnBaiscCopyProxyLink = new System.Windows.Forms.Button();
            this.lbBasicCurPacServerStatus = new System.Windows.Forms.Label();
            this.lbBasicProxyLink = new System.Windows.Forms.Label();
            this.btnBasicViewInNotepad = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tboxBasicGlobalPort = new System.Windows.Forms.TextBox();
            this.tboxBaiscPacPort = new System.Windows.Forms.TextBox();
            this.tboxBasicCustomPacPath = new System.Windows.Forms.TextBox();
            this.chkBasicPacAlwaysOn = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkBasicAutoUpdateSysProxy = new System.Windows.Forms.CheckBox();
            this.btnBasicBrowseCustomPac = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboxBasicSysProxyMode = new System.Windows.Forms.ComboBox();
            this.cboxBasicPacMode = new System.Windows.Forms.ComboBox();
            this.chkBasicUseCustomPac = new System.Windows.Forms.CheckBox();
            this.cboxBasicPacProtocol = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tboxBasicHotkey = new System.Windows.Forms.TextBox();
            this.chkBasicUseAlt = new System.Windows.Forms.CheckBox();
            this.chkBasicUseShift = new System.Windows.Forms.CheckBox();
            this.chkBasicUseHotkey = new System.Windows.Forms.CheckBox();
            this.btnPacModifyList = new System.Windows.Forms.Button();
            this.tabTun = new System.Windows.Forms.TabPage();
            this.btnTunaDetect = new System.Windows.Forms.Button();
            this.btnTunaStart = new System.Windows.Forms.Button();
            this.btnTunaStop = new System.Windows.Forms.Button();
            this.tboxTunaProxy = new System.Windows.Forms.TextBox();
            this.tboxTunaNicIpv4 = new System.Windows.Forms.TextBox();
            this.tboxTunaTunIpv6 = new System.Windows.Forms.TextBox();
            this.tboxTunaTunIpv4 = new System.Windows.Forms.TextBox();
            this.tboxTunaExePath = new System.Windows.Forms.TextBox();
            this.tboxTunaTunName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lbTunaStatus = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rtboxTunaStartupScript = new System.Windows.Forms.RichTextBox();
            this.btnTunaBrowseExe = new System.Windows.Forms.Button();
            this.chkTunaAutoUpdateArguments = new System.Windows.Forms.CheckBox();
            this.chkTunaDebug = new System.Windows.Forms.CheckBox();
            this.chkTunaAutorun = new System.Windows.Forms.CheckBox();
            this.chkTunaEnableIpv6 = new System.Windows.Forms.CheckBox();
            this.chkTunaModifySendThrough = new System.Windows.Forms.CheckBox();
            this.rtboxTunaDns = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabUsage.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabBasic.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabTun.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tabUsage
            // 
            this.tabUsage.Controls.Add(this.panel2);
            resources.ApplyResources(this.tabUsage, "tabUsage");
            this.tabUsage.Name = "tabUsage";
            this.tabUsage.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox5);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tboxUsageReadMe);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // tboxUsageReadMe
            // 
            resources.ApplyResources(this.tboxUsageReadMe, "tboxUsageReadMe");
            this.tboxUsageReadMe.Name = "tboxUsageReadMe";
            this.tboxUsageReadMe.ReadOnly = true;
            // 
            // tabBasic
            // 
            this.tabBasic.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.tabBasic, "tabBasic");
            this.tabBasic.Name = "tabBasic";
            this.tabBasic.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnBasicViewInNotepad);
            this.groupBox2.Controls.Add(this.lbBasicProxyLink);
            this.groupBox2.Controls.Add(this.lbBasicCurPacServerStatus);
            this.groupBox2.Controls.Add(this.btnBaiscCopyProxyLink);
            this.groupBox2.Controls.Add(this.btnBasicDebugPacServer);
            this.groupBox2.Controls.Add(this.btnBasicStartPacServer);
            this.groupBox2.Controls.Add(this.btnBasicStopPacServer);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnBasicStopPacServer
            // 
            resources.ApplyResources(this.btnBasicStopPacServer, "btnBasicStopPacServer");
            this.btnBasicStopPacServer.Name = "btnBasicStopPacServer";
            this.toolTip1.SetToolTip(this.btnBasicStopPacServer, resources.GetString("btnBasicStopPacServer.ToolTip"));
            this.btnBasicStopPacServer.UseVisualStyleBackColor = true;
            // 
            // btnBasicStartPacServer
            // 
            resources.ApplyResources(this.btnBasicStartPacServer, "btnBasicStartPacServer");
            this.btnBasicStartPacServer.Name = "btnBasicStartPacServer";
            this.toolTip1.SetToolTip(this.btnBasicStartPacServer, resources.GetString("btnBasicStartPacServer.ToolTip"));
            this.btnBasicStartPacServer.UseVisualStyleBackColor = true;
            // 
            // btnBasicDebugPacServer
            // 
            resources.ApplyResources(this.btnBasicDebugPacServer, "btnBasicDebugPacServer");
            this.btnBasicDebugPacServer.Name = "btnBasicDebugPacServer";
            this.toolTip1.SetToolTip(this.btnBasicDebugPacServer, resources.GetString("btnBasicDebugPacServer.ToolTip"));
            this.btnBasicDebugPacServer.UseVisualStyleBackColor = true;
            // 
            // btnBaiscCopyProxyLink
            // 
            resources.ApplyResources(this.btnBaiscCopyProxyLink, "btnBaiscCopyProxyLink");
            this.btnBaiscCopyProxyLink.Name = "btnBaiscCopyProxyLink";
            this.toolTip1.SetToolTip(this.btnBaiscCopyProxyLink, resources.GetString("btnBaiscCopyProxyLink.ToolTip"));
            this.btnBaiscCopyProxyLink.UseVisualStyleBackColor = true;
            // 
            // lbBasicCurPacServerStatus
            // 
            resources.ApplyResources(this.lbBasicCurPacServerStatus, "lbBasicCurPacServerStatus");
            this.lbBasicCurPacServerStatus.Name = "lbBasicCurPacServerStatus";
            // 
            // lbBasicProxyLink
            // 
            resources.ApplyResources(this.lbBasicProxyLink, "lbBasicProxyLink");
            this.lbBasicProxyLink.AutoEllipsis = true;
            this.lbBasicProxyLink.Name = "lbBasicProxyLink";
            this.toolTip1.SetToolTip(this.lbBasicProxyLink, resources.GetString("lbBasicProxyLink.ToolTip"));
            // 
            // btnBasicViewInNotepad
            // 
            resources.ApplyResources(this.btnBasicViewInNotepad, "btnBasicViewInNotepad");
            this.btnBasicViewInNotepad.Name = "btnBasicViewInNotepad";
            this.toolTip1.SetToolTip(this.btnBasicViewInNotepad, resources.GetString("btnBasicViewInNotepad.ToolTip"));
            this.btnBasicViewInNotepad.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnPacModifyList);
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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tboxBasicGlobalPort
            // 
            resources.ApplyResources(this.tboxBasicGlobalPort, "tboxBasicGlobalPort");
            this.tboxBasicGlobalPort.Name = "tboxBasicGlobalPort";
            // 
            // tboxBaiscPacPort
            // 
            resources.ApplyResources(this.tboxBaiscPacPort, "tboxBaiscPacPort");
            this.tboxBaiscPacPort.Name = "tboxBaiscPacPort";
            // 
            // tboxBasicCustomPacPath
            // 
            resources.ApplyResources(this.tboxBasicCustomPacPath, "tboxBasicCustomPacPath");
            this.tboxBasicCustomPacPath.Name = "tboxBasicCustomPacPath";
            // 
            // chkBasicPacAlwaysOn
            // 
            resources.ApplyResources(this.chkBasicPacAlwaysOn, "chkBasicPacAlwaysOn");
            this.chkBasicPacAlwaysOn.Name = "chkBasicPacAlwaysOn";
            this.toolTip1.SetToolTip(this.chkBasicPacAlwaysOn, resources.GetString("chkBasicPacAlwaysOn.ToolTip"));
            this.chkBasicPacAlwaysOn.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // chkBasicAutoUpdateSysProxy
            // 
            resources.ApplyResources(this.chkBasicAutoUpdateSysProxy, "chkBasicAutoUpdateSysProxy");
            this.chkBasicAutoUpdateSysProxy.Name = "chkBasicAutoUpdateSysProxy";
            this.toolTip1.SetToolTip(this.chkBasicAutoUpdateSysProxy, resources.GetString("chkBasicAutoUpdateSysProxy.ToolTip"));
            this.chkBasicAutoUpdateSysProxy.UseVisualStyleBackColor = true;
            this.chkBasicAutoUpdateSysProxy.CheckedChanged += new System.EventHandler(this.chkBasicAutoUpdateSysProxy_CheckedChanged);
            // 
            // btnBasicBrowseCustomPac
            // 
            resources.ApplyResources(this.btnBasicBrowseCustomPac, "btnBasicBrowseCustomPac");
            this.btnBasicBrowseCustomPac.Name = "btnBasicBrowseCustomPac";
            this.toolTip1.SetToolTip(this.btnBasicBrowseCustomPac, resources.GetString("btnBasicBrowseCustomPac.ToolTip"));
            this.btnBasicBrowseCustomPac.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
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
            // 
            // chkBasicUseCustomPac
            // 
            resources.ApplyResources(this.chkBasicUseCustomPac, "chkBasicUseCustomPac");
            this.chkBasicUseCustomPac.Name = "chkBasicUseCustomPac";
            this.toolTip1.SetToolTip(this.chkBasicUseCustomPac, resources.GetString("chkBasicUseCustomPac.ToolTip"));
            this.chkBasicUseCustomPac.UseVisualStyleBackColor = true;
            this.chkBasicUseCustomPac.CheckedChanged += new System.EventHandler(this.chkBasicUseCustomPac_CheckedChanged);
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
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // tboxBasicHotkey
            // 
            resources.ApplyResources(this.tboxBasicHotkey, "tboxBasicHotkey");
            this.tboxBasicHotkey.Name = "tboxBasicHotkey";
            // 
            // chkBasicUseAlt
            // 
            resources.ApplyResources(this.chkBasicUseAlt, "chkBasicUseAlt");
            this.chkBasicUseAlt.Name = "chkBasicUseAlt";
            this.chkBasicUseAlt.UseVisualStyleBackColor = true;
            // 
            // chkBasicUseShift
            // 
            resources.ApplyResources(this.chkBasicUseShift, "chkBasicUseShift");
            this.chkBasicUseShift.Name = "chkBasicUseShift";
            this.chkBasicUseShift.UseVisualStyleBackColor = true;
            // 
            // chkBasicUseHotkey
            // 
            resources.ApplyResources(this.chkBasicUseHotkey, "chkBasicUseHotkey");
            this.chkBasicUseHotkey.Name = "chkBasicUseHotkey";
            this.toolTip1.SetToolTip(this.chkBasicUseHotkey, resources.GetString("chkBasicUseHotkey.ToolTip"));
            this.chkBasicUseHotkey.UseVisualStyleBackColor = true;
            // 
            // btnPacModifyList
            // 
            resources.ApplyResources(this.btnPacModifyList, "btnPacModifyList");
            this.btnPacModifyList.Name = "btnPacModifyList";
            this.toolTip1.SetToolTip(this.btnPacModifyList, resources.GetString("btnPacModifyList.ToolTip"));
            this.btnPacModifyList.UseVisualStyleBackColor = true;
            this.btnPacModifyList.Click += new System.EventHandler(this.btnPacModifyList_Click);
            // 
            // tabTun
            // 
            resources.ApplyResources(this.tabTun, "tabTun");
            this.tabTun.Controls.Add(this.rtboxTunaDns);
            this.tabTun.Controls.Add(this.tboxTunaTunName);
            this.tabTun.Controls.Add(this.tboxTunaExePath);
            this.tabTun.Controls.Add(this.tboxTunaTunIpv4);
            this.tabTun.Controls.Add(this.tboxTunaTunIpv6);
            this.tabTun.Controls.Add(this.tboxTunaNicIpv4);
            this.tabTun.Controls.Add(this.tboxTunaProxy);
            this.tabTun.Controls.Add(this.chkTunaModifySendThrough);
            this.tabTun.Controls.Add(this.chkTunaEnableIpv6);
            this.tabTun.Controls.Add(this.chkTunaAutorun);
            this.tabTun.Controls.Add(this.chkTunaDebug);
            this.tabTun.Controls.Add(this.chkTunaAutoUpdateArguments);
            this.tabTun.Controls.Add(this.btnTunaBrowseExe);
            this.tabTun.Controls.Add(this.panel3);
            this.tabTun.Controls.Add(this.lbTunaStatus);
            this.tabTun.Controls.Add(this.label11);
            this.tabTun.Controls.Add(this.label12);
            this.tabTun.Controls.Add(this.label14);
            this.tabTun.Controls.Add(this.label10);
            this.tabTun.Controls.Add(this.label7);
            this.tabTun.Controls.Add(this.label5);
            this.tabTun.Controls.Add(this.btnTunaStop);
            this.tabTun.Controls.Add(this.btnTunaStart);
            this.tabTun.Controls.Add(this.btnTunaDetect);
            this.tabTun.Name = "tabTun";
            this.tabTun.UseVisualStyleBackColor = true;
            // 
            // btnTunaDetect
            // 
            resources.ApplyResources(this.btnTunaDetect, "btnTunaDetect");
            this.btnTunaDetect.Name = "btnTunaDetect";
            this.toolTip1.SetToolTip(this.btnTunaDetect, resources.GetString("btnTunaDetect.ToolTip"));
            this.btnTunaDetect.UseVisualStyleBackColor = true;
            // 
            // btnTunaStart
            // 
            resources.ApplyResources(this.btnTunaStart, "btnTunaStart");
            this.btnTunaStart.Name = "btnTunaStart";
            this.btnTunaStart.UseVisualStyleBackColor = true;
            // 
            // btnTunaStop
            // 
            resources.ApplyResources(this.btnTunaStop, "btnTunaStop");
            this.btnTunaStop.Name = "btnTunaStop";
            this.btnTunaStop.UseVisualStyleBackColor = true;
            // 
            // tboxTunaProxy
            // 
            resources.ApplyResources(this.tboxTunaProxy, "tboxTunaProxy");
            this.tboxTunaProxy.Name = "tboxTunaProxy";
            // 
            // tboxTunaNicIpv4
            // 
            resources.ApplyResources(this.tboxTunaNicIpv4, "tboxTunaNicIpv4");
            this.tboxTunaNicIpv4.Name = "tboxTunaNicIpv4";
            // 
            // tboxTunaTunIpv6
            // 
            resources.ApplyResources(this.tboxTunaTunIpv6, "tboxTunaTunIpv6");
            this.tboxTunaTunIpv6.Name = "tboxTunaTunIpv6";
            // 
            // tboxTunaTunIpv4
            // 
            resources.ApplyResources(this.tboxTunaTunIpv4, "tboxTunaTunIpv4");
            this.tboxTunaTunIpv4.Name = "tboxTunaTunIpv4";
            // 
            // tboxTunaExePath
            // 
            resources.ApplyResources(this.tboxTunaExePath, "tboxTunaExePath");
            this.tboxTunaExePath.Name = "tboxTunaExePath";
            // 
            // tboxTunaTunName
            // 
            resources.ApplyResources(this.tboxTunaTunName, "tboxTunaTunName");
            this.tboxTunaTunName.Name = "tboxTunaTunName";
            this.toolTip1.SetToolTip(this.tboxTunaTunName, resources.GetString("tboxTunaTunName.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            this.toolTip1.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.toolTip1.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // lbTunaStatus
            // 
            resources.ApplyResources(this.lbTunaStatus, "lbTunaStatus");
            this.lbTunaStatus.Name = "lbTunaStatus";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.groupBox6);
            this.panel3.Name = "panel3";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.rtboxTunaStartupScript);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // rtboxTunaStartupScript
            // 
            resources.ApplyResources(this.rtboxTunaStartupScript, "rtboxTunaStartupScript");
            this.rtboxTunaStartupScript.Name = "rtboxTunaStartupScript";
            // 
            // btnTunaBrowseExe
            // 
            resources.ApplyResources(this.btnTunaBrowseExe, "btnTunaBrowseExe");
            this.btnTunaBrowseExe.Name = "btnTunaBrowseExe";
            this.toolTip1.SetToolTip(this.btnTunaBrowseExe, resources.GetString("btnTunaBrowseExe.ToolTip"));
            this.btnTunaBrowseExe.UseVisualStyleBackColor = true;
            // 
            // chkTunaAutoUpdateArguments
            // 
            resources.ApplyResources(this.chkTunaAutoUpdateArguments, "chkTunaAutoUpdateArguments");
            this.chkTunaAutoUpdateArguments.Name = "chkTunaAutoUpdateArguments";
            this.toolTip1.SetToolTip(this.chkTunaAutoUpdateArguments, resources.GetString("chkTunaAutoUpdateArguments.ToolTip"));
            this.chkTunaAutoUpdateArguments.UseVisualStyleBackColor = true;
            // 
            // chkTunaDebug
            // 
            resources.ApplyResources(this.chkTunaDebug, "chkTunaDebug");
            this.chkTunaDebug.Name = "chkTunaDebug";
            this.toolTip1.SetToolTip(this.chkTunaDebug, resources.GetString("chkTunaDebug.ToolTip"));
            this.chkTunaDebug.UseVisualStyleBackColor = true;
            // 
            // chkTunaAutorun
            // 
            resources.ApplyResources(this.chkTunaAutorun, "chkTunaAutorun");
            this.chkTunaAutorun.Name = "chkTunaAutorun";
            this.toolTip1.SetToolTip(this.chkTunaAutorun, resources.GetString("chkTunaAutorun.ToolTip"));
            this.chkTunaAutorun.UseVisualStyleBackColor = true;
            // 
            // chkTunaEnableIpv6
            // 
            resources.ApplyResources(this.chkTunaEnableIpv6, "chkTunaEnableIpv6");
            this.chkTunaEnableIpv6.Name = "chkTunaEnableIpv6";
            this.toolTip1.SetToolTip(this.chkTunaEnableIpv6, resources.GetString("chkTunaEnableIpv6.ToolTip"));
            this.chkTunaEnableIpv6.UseVisualStyleBackColor = true;
            // 
            // chkTunaModifySendThrough
            // 
            resources.ApplyResources(this.chkTunaModifySendThrough, "chkTunaModifySendThrough");
            this.chkTunaModifySendThrough.Name = "chkTunaModifySendThrough";
            this.toolTip1.SetToolTip(this.chkTunaModifySendThrough, resources.GetString("chkTunaModifySendThrough.ToolTip"));
            this.chkTunaModifySendThrough.UseVisualStyleBackColor = true;
            // 
            // rtboxTunaDns
            // 
            resources.ApplyResources(this.rtboxTunaDns, "rtboxTunaDns");
            this.rtboxTunaDns.Name = "rtboxTunaDns";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabTun);
            this.tabControl1.Controls.Add(this.tabBasic);
            this.tabControl1.Controls.Add(this.tabUsage);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormMain";
            this.Shown += new System.EventHandler(this.FormPluginMain_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabUsage.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabBasic.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabTun.ResumeLayout(false);
            this.tabTun.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabTun;
        private System.Windows.Forms.RichTextBox rtboxTunaDns;
        private System.Windows.Forms.TextBox tboxTunaTunName;
        private System.Windows.Forms.TextBox tboxTunaExePath;
        private System.Windows.Forms.TextBox tboxTunaTunIpv4;
        private System.Windows.Forms.TextBox tboxTunaTunIpv6;
        private System.Windows.Forms.TextBox tboxTunaNicIpv4;
        private System.Windows.Forms.TextBox tboxTunaProxy;
        private System.Windows.Forms.CheckBox chkTunaModifySendThrough;
        private System.Windows.Forms.CheckBox chkTunaEnableIpv6;
        private System.Windows.Forms.CheckBox chkTunaAutorun;
        private System.Windows.Forms.CheckBox chkTunaDebug;
        private System.Windows.Forms.CheckBox chkTunaAutoUpdateArguments;
        private System.Windows.Forms.Button btnTunaBrowseExe;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RichTextBox rtboxTunaStartupScript;
        private System.Windows.Forms.Label lbTunaStatus;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnTunaStop;
        private System.Windows.Forms.Button btnTunaStart;
        private System.Windows.Forms.Button btnTunaDetect;
        private System.Windows.Forms.TabPage tabBasic;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPacModifyList;
        private System.Windows.Forms.CheckBox chkBasicUseHotkey;
        private System.Windows.Forms.CheckBox chkBasicUseShift;
        private System.Windows.Forms.CheckBox chkBasicUseAlt;
        private System.Windows.Forms.TextBox tboxBasicHotkey;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cboxBasicPacProtocol;
        private System.Windows.Forms.CheckBox chkBasicUseCustomPac;
        private System.Windows.Forms.ComboBox cboxBasicPacMode;
        private System.Windows.Forms.ComboBox cboxBasicSysProxyMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBasicBrowseCustomPac;
        private System.Windows.Forms.CheckBox chkBasicAutoUpdateSysProxy;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkBasicPacAlwaysOn;
        private System.Windows.Forms.TextBox tboxBasicCustomPacPath;
        private System.Windows.Forms.TextBox tboxBaiscPacPort;
        private System.Windows.Forms.TextBox tboxBasicGlobalPort;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBasicViewInNotepad;
        private System.Windows.Forms.Label lbBasicProxyLink;
        private System.Windows.Forms.Label lbBasicCurPacServerStatus;
        private System.Windows.Forms.Button btnBaiscCopyProxyLink;
        private System.Windows.Forms.Button btnBasicDebugPacServer;
        private System.Windows.Forms.Button btnBasicStartPacServer;
        private System.Windows.Forms.Button btnBasicStopPacServer;
        private System.Windows.Forms.TabPage tabUsage;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox tboxUsageReadMe;
    }
}
