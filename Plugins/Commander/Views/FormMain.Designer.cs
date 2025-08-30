namespace Commander.Views
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rtboxStdInContent = new VgcApis.UserControls.ExRichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lsboxNames = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnEnvVars = new System.Windows.Forms.Button();
            this.btnArgs = new System.Windows.Forms.Button();
            this.btnWrkDir = new System.Windows.Forms.Button();
            this.btnExe = new System.Windows.Forms.Button();
            this.chkWriteStdIn = new System.Windows.Forms.CheckBox();
            this.chkUseShell = new System.Windows.Forms.CheckBox();
            this.chkHideWindow = new System.Windows.Forms.CheckBox();
            this.cboxStdOutEncoding = new System.Windows.Forms.ComboBox();
            this.lbStdOutEncoding = new System.Windows.Forms.Label();
            this.lbStdInEncoding = new System.Windows.Forms.Label();
            this.cboxStdInEncoding = new System.Windows.Forms.ComboBox();
            this.tboxEnvVar = new System.Windows.Forms.TextBox();
            this.tboxArgs = new System.Windows.Forms.TextBox();
            this.tboxExe = new System.Windows.Forms.TextBox();
            this.tboxWrkDir = new System.Windows.Forms.TextBox();
            this.panelRight = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rtboxLogs = new VgcApis.UserControls.ExRichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.executeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sTDINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipboardToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteFromClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelLeft);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelRight);
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.groupBox3);
            this.panelLeft.Controls.Add(this.groupBox1);
            this.panelLeft.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.panelLeft, "panelLeft");
            this.panelLeft.Name = "panelLeft";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.rtboxStdInContent);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // rtboxStdInContent
            // 
            resources.ApplyResources(this.rtboxStdInContent, "rtboxStdInContent");
            this.rtboxStdInContent.Name = "rtboxStdInContent";
            this.rtboxStdInContent.TextChanged += new System.EventHandler(this.rtboxStdInContent_TextChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.lsboxNames);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lsboxNames
            // 
            resources.ApplyResources(this.lsboxNames, "lsboxNames");
            this.lsboxNames.FormattingEnabled = true;
            this.lsboxNames.Name = "lsboxNames";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.btnEnvVars);
            this.groupBox2.Controls.Add(this.btnArgs);
            this.groupBox2.Controls.Add(this.btnWrkDir);
            this.groupBox2.Controls.Add(this.btnExe);
            this.groupBox2.Controls.Add(this.chkWriteStdIn);
            this.groupBox2.Controls.Add(this.chkUseShell);
            this.groupBox2.Controls.Add(this.chkHideWindow);
            this.groupBox2.Controls.Add(this.cboxStdOutEncoding);
            this.groupBox2.Controls.Add(this.lbStdOutEncoding);
            this.groupBox2.Controls.Add(this.lbStdInEncoding);
            this.groupBox2.Controls.Add(this.cboxStdInEncoding);
            this.groupBox2.Controls.Add(this.tboxEnvVar);
            this.groupBox2.Controls.Add(this.tboxArgs);
            this.groupBox2.Controls.Add(this.tboxExe);
            this.groupBox2.Controls.Add(this.tboxWrkDir);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnEnvVars
            // 
            resources.ApplyResources(this.btnEnvVars, "btnEnvVars");
            this.btnEnvVars.Name = "btnEnvVars";
            this.toolTip1.SetToolTip(this.btnEnvVars, resources.GetString("btnEnvVars.ToolTip"));
            this.btnEnvVars.UseVisualStyleBackColor = true;
            this.btnEnvVars.Click += new System.EventHandler(this.btnEnvVars_Click);
            // 
            // btnArgs
            // 
            resources.ApplyResources(this.btnArgs, "btnArgs");
            this.btnArgs.Name = "btnArgs";
            this.toolTip1.SetToolTip(this.btnArgs, resources.GetString("btnArgs.ToolTip"));
            this.btnArgs.UseVisualStyleBackColor = true;
            this.btnArgs.Click += new System.EventHandler(this.btnArgs_Click);
            // 
            // btnWrkDir
            // 
            resources.ApplyResources(this.btnWrkDir, "btnWrkDir");
            this.btnWrkDir.Name = "btnWrkDir";
            this.toolTip1.SetToolTip(this.btnWrkDir, resources.GetString("btnWrkDir.ToolTip"));
            this.btnWrkDir.UseVisualStyleBackColor = true;
            this.btnWrkDir.Click += new System.EventHandler(this.btnWrkDir_Click);
            // 
            // btnExe
            // 
            resources.ApplyResources(this.btnExe, "btnExe");
            this.btnExe.Name = "btnExe";
            this.toolTip1.SetToolTip(this.btnExe, resources.GetString("btnExe.ToolTip"));
            this.btnExe.UseVisualStyleBackColor = true;
            this.btnExe.Click += new System.EventHandler(this.btnExe_Click);
            // 
            // chkWriteStdIn
            // 
            resources.ApplyResources(this.chkWriteStdIn, "chkWriteStdIn");
            this.chkWriteStdIn.Name = "chkWriteStdIn";
            this.toolTip1.SetToolTip(this.chkWriteStdIn, resources.GetString("chkWriteStdIn.ToolTip"));
            this.chkWriteStdIn.UseVisualStyleBackColor = true;
            this.chkWriteStdIn.CheckedChanged += new System.EventHandler(this.chkWriteStdIn_CheckedChanged);
            // 
            // chkUseShell
            // 
            resources.ApplyResources(this.chkUseShell, "chkUseShell");
            this.chkUseShell.Name = "chkUseShell";
            this.toolTip1.SetToolTip(this.chkUseShell, resources.GetString("chkUseShell.ToolTip"));
            this.chkUseShell.UseVisualStyleBackColor = true;
            this.chkUseShell.CheckedChanged += new System.EventHandler(this.chkUseShell_CheckedChanged);
            // 
            // chkHideWindow
            // 
            resources.ApplyResources(this.chkHideWindow, "chkHideWindow");
            this.chkHideWindow.Name = "chkHideWindow";
            this.toolTip1.SetToolTip(this.chkHideWindow, resources.GetString("chkHideWindow.ToolTip"));
            this.chkHideWindow.UseVisualStyleBackColor = true;
            this.chkHideWindow.CheckedChanged += new System.EventHandler(this.chkHideWindow_CheckedChanged);
            // 
            // cboxStdOutEncoding
            // 
            resources.ApplyResources(this.cboxStdOutEncoding, "cboxStdOutEncoding");
            this.cboxStdOutEncoding.FormattingEnabled = true;
            this.cboxStdOutEncoding.Items.AddRange(new object[] {
            resources.GetString("cboxStdOutEncoding.Items"),
            resources.GetString("cboxStdOutEncoding.Items1"),
            resources.GetString("cboxStdOutEncoding.Items2"),
            resources.GetString("cboxStdOutEncoding.Items3")});
            this.cboxStdOutEncoding.Name = "cboxStdOutEncoding";
            this.cboxStdOutEncoding.TextChanged += new System.EventHandler(this.cboxStdOutEncoding_TextChanged);
            // 
            // lbStdOutEncoding
            // 
            resources.ApplyResources(this.lbStdOutEncoding, "lbStdOutEncoding");
            this.lbStdOutEncoding.Name = "lbStdOutEncoding";
            this.toolTip1.SetToolTip(this.lbStdOutEncoding, resources.GetString("lbStdOutEncoding.ToolTip"));
            // 
            // lbStdInEncoding
            // 
            resources.ApplyResources(this.lbStdInEncoding, "lbStdInEncoding");
            this.lbStdInEncoding.Name = "lbStdInEncoding";
            this.toolTip1.SetToolTip(this.lbStdInEncoding, resources.GetString("lbStdInEncoding.ToolTip"));
            // 
            // cboxStdInEncoding
            // 
            resources.ApplyResources(this.cboxStdInEncoding, "cboxStdInEncoding");
            this.cboxStdInEncoding.FormattingEnabled = true;
            this.cboxStdInEncoding.Items.AddRange(new object[] {
            resources.GetString("cboxStdInEncoding.Items"),
            resources.GetString("cboxStdInEncoding.Items1"),
            resources.GetString("cboxStdInEncoding.Items2"),
            resources.GetString("cboxStdInEncoding.Items3")});
            this.cboxStdInEncoding.Name = "cboxStdInEncoding";
            this.cboxStdInEncoding.TextChanged += new System.EventHandler(this.cboxStdInEncoding_TextChanged);
            // 
            // tboxEnvVar
            // 
            resources.ApplyResources(this.tboxEnvVar, "tboxEnvVar");
            this.tboxEnvVar.Name = "tboxEnvVar";
            this.tboxEnvVar.ReadOnly = true;
            // 
            // tboxArgs
            // 
            resources.ApplyResources(this.tboxArgs, "tboxArgs");
            this.tboxArgs.Name = "tboxArgs";
            this.tboxArgs.ReadOnly = true;
            // 
            // tboxExe
            // 
            resources.ApplyResources(this.tboxExe, "tboxExe");
            this.tboxExe.Name = "tboxExe";
            this.tboxExe.TextChanged += new System.EventHandler(this.tboxExe_TextChanged);
            // 
            // tboxWrkDir
            // 
            resources.ApplyResources(this.tboxWrkDir, "tboxWrkDir");
            this.tboxWrkDir.Name = "tboxWrkDir";
            this.tboxWrkDir.TextChanged += new System.EventHandler(this.tboxWrkDir_TextChanged);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.groupBox4);
            resources.ApplyResources(this.panelRight, "panelRight");
            this.panelRight.Name = "panelRight";
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.rtboxLogs);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // rtboxLogs
            // 
            this.rtboxLogs.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.rtboxLogs, "rtboxLogs");
            this.rtboxLogs.Name = "rtboxLogs";
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.runToolStripMenuItem,
            this.sTDINToolStripMenuItem,
            this.logToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveToolStripMenuItem1,
            this.saveAsToolStripMenuItem1,
            this.deleteToolStripMenuItem1,
            this.toolStripMenuItem4,
            this.closeToolStripMenuItem1});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            resources.ApplyResources(this.newToolStripMenuItem, "newToolStripMenuItem");
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            resources.ApplyResources(this.saveToolStripMenuItem1, "saveToolStripMenuItem1");
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // saveAsToolStripMenuItem1
            // 
            this.saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            resources.ApplyResources(this.saveAsToolStripMenuItem1, "saveAsToolStripMenuItem1");
            this.saveAsToolStripMenuItem1.Click += new System.EventHandler(this.saveAsToolStripMenuItem1_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            resources.ApplyResources(this.deleteToolStripMenuItem1, "deleteToolStripMenuItem1");
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            resources.ApplyResources(this.closeToolStripMenuItem1, "closeToolStripMenuItem1");
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.closeToolStripMenuItem1_Click);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.executeToolStripMenuItem,
            this.toolStripMenuItem2,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.killToolStripMenuItem});
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            resources.ApplyResources(this.runToolStripMenuItem, "runToolStripMenuItem");
            this.runToolStripMenuItem.DropDownOpening += new System.EventHandler(this.runToolStripMenuItem_DropDownOpening);
            // 
            // executeToolStripMenuItem
            // 
            this.executeToolStripMenuItem.Name = "executeToolStripMenuItem";
            resources.ApplyResources(this.executeToolStripMenuItem, "executeToolStripMenuItem");
            this.executeToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            resources.ApplyResources(this.startToolStripMenuItem, "startToolStripMenuItem");
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
            // 
            // killToolStripMenuItem
            // 
            this.killToolStripMenuItem.Name = "killToolStripMenuItem";
            resources.ApplyResources(this.killToolStripMenuItem, "killToolStripMenuItem");
            // 
            // sTDINToolStripMenuItem
            // 
            this.sTDINToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearContentToolStripMenuItem,
            this.copyToClipboardToolStripMenuItem1,
            this.pasteFromClipboardToolStripMenuItem});
            this.sTDINToolStripMenuItem.Name = "sTDINToolStripMenuItem";
            resources.ApplyResources(this.sTDINToolStripMenuItem, "sTDINToolStripMenuItem");
            // 
            // clearContentToolStripMenuItem
            // 
            this.clearContentToolStripMenuItem.Name = "clearContentToolStripMenuItem";
            resources.ApplyResources(this.clearContentToolStripMenuItem, "clearContentToolStripMenuItem");
            this.clearContentToolStripMenuItem.Click += new System.EventHandler(this.clearContentToolStripMenuItem_Click);
            // 
            // copyToClipboardToolStripMenuItem1
            // 
            this.copyToClipboardToolStripMenuItem1.Name = "copyToClipboardToolStripMenuItem1";
            resources.ApplyResources(this.copyToClipboardToolStripMenuItem1, "copyToClipboardToolStripMenuItem1");
            this.copyToClipboardToolStripMenuItem1.Click += new System.EventHandler(this.copyToClipboardToolStripMenuItem1_Click);
            // 
            // pasteFromClipboardToolStripMenuItem
            // 
            this.pasteFromClipboardToolStripMenuItem.Name = "pasteFromClipboardToolStripMenuItem";
            resources.ApplyResources(this.pasteFromClipboardToolStripMenuItem, "pasteFromClipboardToolStripMenuItem");
            this.pasteFromClipboardToolStripMenuItem.Click += new System.EventHandler(this.pasteFromClipboardToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pauseToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.copyToClipboardToolStripMenuItem});
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            resources.ApplyResources(this.logToolStripMenuItem, "logToolStripMenuItem");
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            resources.ApplyResources(this.pauseToolStripMenuItem, "pauseToolStripMenuItem");
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            resources.ApplyResources(this.clearToolStripMenuItem, "clearToolStripMenuItem");
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // copyToClipboardToolStripMenuItem
            // 
            this.copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            resources.ApplyResources(this.copyToClipboardToolStripMenuItem, "copyToClipboardToolStripMenuItem");
            this.copyToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyToClipboardToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lsboxNames;
        private System.Windows.Forms.TextBox tboxExe;
        private System.Windows.Forms.TextBox tboxWrkDir;
        private System.Windows.Forms.Label lbStdInEncoding;
        private System.Windows.Forms.TextBox tboxEnvVar;
        private System.Windows.Forms.TextBox tboxArgs;
        private VgcApis.UserControls.ExRichTextBox rtboxStdInContent;
        private System.Windows.Forms.GroupBox groupBox4;
        private VgcApis.UserControls.ExRichTextBox rtboxLogs;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem executeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem killToolStripMenuItem;
        private System.Windows.Forms.ComboBox cboxStdOutEncoding;
        private System.Windows.Forms.Label lbStdOutEncoding;
        private System.Windows.Forms.ComboBox cboxStdInEncoding;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkWriteStdIn;
        private System.Windows.Forms.CheckBox chkHideWindow;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkUseShell;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem sTDINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearContentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pasteFromClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.Button btnEnvVars;
        private System.Windows.Forms.Button btnArgs;
        private System.Windows.Forms.Button btnWrkDir;
        private System.Windows.Forms.Button btnExe;
    }
}
