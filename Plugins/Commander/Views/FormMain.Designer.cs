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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.panelRight = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rtboxLogs = new VgcApis.UserControls.ExRichTextBox();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rtboxStdInContent = new VgcApis.UserControls.ExRichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lsboxNames = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkWriteStdIn = new System.Windows.Forms.CheckBox();
            this.chkHideWindow = new System.Windows.Forms.CheckBox();
            this.lbEnvVar = new System.Windows.Forms.Label();
            this.cboxStdOutEncoding = new System.Windows.Forms.ComboBox();
            this.lbStdOutEncoding = new System.Windows.Forms.Label();
            this.lbStdInEncoding = new System.Windows.Forms.Label();
            this.cboxStdInEncoding = new System.Windows.Forms.ComboBox();
            this.lbArgs = new System.Windows.Forms.Label();
            this.lbExe = new System.Windows.Forms.Label();
            this.lbWrkDir = new System.Windows.Forms.Label();
            this.tboxEnvVar = new System.Windows.Forms.TextBox();
            this.tboxArgs = new System.Windows.Forms.TextBox();
            this.tboxExe = new System.Windows.Forms.TextBox();
            this.tboxWrkDir = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToSTDINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.copySTDINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySTDOUTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.executeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panelRight);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panelLeft);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1026, 539);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1026, 564);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // panelRight
            // 
            this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelRight.Controls.Add(this.groupBox4);
            this.panelRight.Location = new System.Drawing.Point(542, 3);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(481, 533);
            this.panelRight.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.rtboxLogs);
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 10, 10, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(478, 530);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Logs";
            // 
            // rtboxLogs
            // 
            this.rtboxLogs.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rtboxLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtboxLogs.Location = new System.Drawing.Point(3, 17);
            this.rtboxLogs.Name = "rtboxLogs";
            this.rtboxLogs.ReadOnly = true;
            this.rtboxLogs.Size = new System.Drawing.Size(472, 510);
            this.rtboxLogs.TabIndex = 3;
            this.rtboxLogs.Text = "";
            // 
            // panelLeft
            // 
            this.panelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelLeft.Controls.Add(this.groupBox3);
            this.panelLeft.Controls.Add(this.groupBox1);
            this.panelLeft.Controls.Add(this.groupBox2);
            this.panelLeft.Location = new System.Drawing.Point(3, 3);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(543, 533);
            this.panelLeft.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.rtboxStdInContent);
            this.groupBox3.Location = new System.Drawing.Point(187, 219);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(349, 311);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "STDIN";
            // 
            // rtboxStdInContent
            // 
            this.rtboxStdInContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtboxStdInContent.Location = new System.Drawing.Point(3, 17);
            this.rtboxStdInContent.Name = "rtboxStdInContent";
            this.rtboxStdInContent.Size = new System.Drawing.Size(343, 291);
            this.rtboxStdInContent.TabIndex = 3;
            this.rtboxStdInContent.Text = "";
            this.rtboxStdInContent.TextChanged += new System.EventHandler(this.rtboxStdInContent_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.lsboxNames);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(178, 530);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cmds";
            // 
            // lsboxNames
            // 
            this.lsboxNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsboxNames.FormattingEnabled = true;
            this.lsboxNames.ItemHeight = 12;
            this.lsboxNames.Location = new System.Drawing.Point(3, 17);
            this.lsboxNames.Name = "lsboxNames";
            this.lsboxNames.Size = new System.Drawing.Size(172, 510);
            this.lsboxNames.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkWriteStdIn);
            this.groupBox2.Controls.Add(this.chkHideWindow);
            this.groupBox2.Controls.Add(this.lbEnvVar);
            this.groupBox2.Controls.Add(this.cboxStdOutEncoding);
            this.groupBox2.Controls.Add(this.lbStdOutEncoding);
            this.groupBox2.Controls.Add(this.lbStdInEncoding);
            this.groupBox2.Controls.Add(this.cboxStdInEncoding);
            this.groupBox2.Controls.Add(this.lbArgs);
            this.groupBox2.Controls.Add(this.lbExe);
            this.groupBox2.Controls.Add(this.lbWrkDir);
            this.groupBox2.Controls.Add(this.tboxEnvVar);
            this.groupBox2.Controls.Add(this.tboxArgs);
            this.groupBox2.Controls.Add(this.tboxExe);
            this.groupBox2.Controls.Add(this.tboxWrkDir);
            this.groupBox2.Location = new System.Drawing.Point(187, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(349, 530);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Basic";
            // 
            // chkWriteStdIn
            // 
            this.chkWriteStdIn.AutoSize = true;
            this.chkWriteStdIn.Location = new System.Drawing.Point(175, 180);
            this.chkWriteStdIn.Name = "chkWriteStdIn";
            this.chkWriteStdIn.Size = new System.Drawing.Size(90, 16);
            this.chkWriteStdIn.TabIndex = 3;
            this.chkWriteStdIn.Text = "Write STDIN";
            this.toolTip1.SetToolTip(this.chkWriteStdIn, "Write the content below to STDIN.");
            this.chkWriteStdIn.UseVisualStyleBackColor = true;
            this.chkWriteStdIn.CheckedChanged += new System.EventHandler(this.chkWriteStdIn_CheckedChanged);
            // 
            // chkHideWindow
            // 
            this.chkHideWindow.AutoSize = true;
            this.chkHideWindow.Location = new System.Drawing.Point(79, 180);
            this.chkHideWindow.Name = "chkHideWindow";
            this.chkHideWindow.Size = new System.Drawing.Size(90, 16);
            this.chkHideWindow.TabIndex = 3;
            this.chkHideWindow.Text = "Hide window";
            this.toolTip1.SetToolTip(this.chkHideWindow, "Create no window.");
            this.chkHideWindow.UseVisualStyleBackColor = true;
            this.chkHideWindow.CheckedChanged += new System.EventHandler(this.chkHideWindow_CheckedChanged);
            // 
            // lbEnvVar
            // 
            this.lbEnvVar.AutoSize = true;
            this.lbEnvVar.Location = new System.Drawing.Point(11, 105);
            this.lbEnvVar.Name = "lbEnvVar";
            this.lbEnvVar.Size = new System.Drawing.Size(53, 12);
            this.lbEnvVar.TabIndex = 1;
            this.lbEnvVar.Text = "Env vars";
            this.toolTip1.SetToolTip(this.lbEnvVar, "Click to edit enviroment variables.");
            this.lbEnvVar.Click += new System.EventHandler(this.lbEnvVar_Click);
            // 
            // cboxStdOutEncoding
            // 
            this.cboxStdOutEncoding.FormattingEnabled = true;
            this.cboxStdOutEncoding.Items.AddRange(new object[] {
            "utf8",
            "unicode",
            "ascii",
            "cp936"});
            this.cboxStdOutEncoding.Location = new System.Drawing.Point(79, 154);
            this.cboxStdOutEncoding.Name = "cboxStdOutEncoding";
            this.cboxStdOutEncoding.Size = new System.Drawing.Size(264, 20);
            this.cboxStdOutEncoding.TabIndex = 2;
            this.cboxStdOutEncoding.TextChanged += new System.EventHandler(this.cboxStdOutEncoding_TextChanged);
            // 
            // lbStdOutEncoding
            // 
            this.lbStdOutEncoding.AutoSize = true;
            this.lbStdOutEncoding.Location = new System.Drawing.Point(11, 158);
            this.lbStdOutEncoding.Name = "lbStdOutEncoding";
            this.lbStdOutEncoding.Size = new System.Drawing.Size(41, 12);
            this.lbStdOutEncoding.TabIndex = 1;
            this.lbStdOutEncoding.Text = "STDOUT";
            this.toolTip1.SetToolTip(this.lbStdOutEncoding, "STDOUT encoding.");
            // 
            // lbStdInEncoding
            // 
            this.lbStdInEncoding.AutoSize = true;
            this.lbStdInEncoding.Location = new System.Drawing.Point(11, 132);
            this.lbStdInEncoding.Name = "lbStdInEncoding";
            this.lbStdInEncoding.Size = new System.Drawing.Size(35, 12);
            this.lbStdInEncoding.TabIndex = 1;
            this.lbStdInEncoding.Text = "STDIN";
            this.toolTip1.SetToolTip(this.lbStdInEncoding, "STDIN encoding.");
            // 
            // cboxStdInEncoding
            // 
            this.cboxStdInEncoding.FormattingEnabled = true;
            this.cboxStdInEncoding.Items.AddRange(new object[] {
            "utf8",
            "unicode",
            "ascii",
            "cp936"});
            this.cboxStdInEncoding.Location = new System.Drawing.Point(79, 128);
            this.cboxStdInEncoding.Name = "cboxStdInEncoding";
            this.cboxStdInEncoding.Size = new System.Drawing.Size(264, 20);
            this.cboxStdInEncoding.TabIndex = 2;
            this.cboxStdInEncoding.TextChanged += new System.EventHandler(this.cboxStdInEncoding_TextChanged);
            // 
            // lbArgs
            // 
            this.lbArgs.AutoSize = true;
            this.lbArgs.Location = new System.Drawing.Point(11, 78);
            this.lbArgs.Name = "lbArgs";
            this.lbArgs.Size = new System.Drawing.Size(29, 12);
            this.lbArgs.TabIndex = 1;
            this.lbArgs.Text = "Args";
            this.toolTip1.SetToolTip(this.lbArgs, "Click to edit arguments.");
            this.lbArgs.Click += new System.EventHandler(this.lbArgs_Click);
            // 
            // lbExe
            // 
            this.lbExe.AutoSize = true;
            this.lbExe.Location = new System.Drawing.Point(11, 24);
            this.lbExe.Name = "lbExe";
            this.lbExe.Size = new System.Drawing.Size(23, 12);
            this.lbExe.TabIndex = 1;
            this.lbExe.Text = "Exe";
            this.toolTip1.SetToolTip(this.lbExe, "Click to select executable.");
            this.lbExe.Click += new System.EventHandler(this.lbExe_Click);
            // 
            // lbWrkDir
            // 
            this.lbWrkDir.AutoSize = true;
            this.lbWrkDir.Location = new System.Drawing.Point(11, 51);
            this.lbWrkDir.Name = "lbWrkDir";
            this.lbWrkDir.Size = new System.Drawing.Size(53, 12);
            this.lbWrkDir.TabIndex = 1;
            this.lbWrkDir.Text = "Work dir";
            this.toolTip1.SetToolTip(this.lbWrkDir, "Click to select working directory.");
            this.lbWrkDir.Click += new System.EventHandler(this.lbWrkDir_Click);
            // 
            // tboxEnvVar
            // 
            this.tboxEnvVar.Location = new System.Drawing.Point(79, 101);
            this.tboxEnvVar.Name = "tboxEnvVar";
            this.tboxEnvVar.ReadOnly = true;
            this.tboxEnvVar.Size = new System.Drawing.Size(264, 21);
            this.tboxEnvVar.TabIndex = 0;
            // 
            // tboxArgs
            // 
            this.tboxArgs.Location = new System.Drawing.Point(79, 74);
            this.tboxArgs.Name = "tboxArgs";
            this.tboxArgs.ReadOnly = true;
            this.tboxArgs.Size = new System.Drawing.Size(264, 21);
            this.tboxArgs.TabIndex = 0;
            // 
            // tboxExe
            // 
            this.tboxExe.Location = new System.Drawing.Point(79, 20);
            this.tboxExe.Name = "tboxExe";
            this.tboxExe.Size = new System.Drawing.Size(264, 21);
            this.tboxExe.TabIndex = 0;
            this.tboxExe.TextChanged += new System.EventHandler(this.tboxExe_TextChanged);
            // 
            // tboxWrkDir
            // 
            this.tboxWrkDir.Location = new System.Drawing.Point(79, 47);
            this.tboxWrkDir.Name = "tboxWrkDir";
            this.tboxWrkDir.Size = new System.Drawing.Size(264, 21);
            this.tboxWrkDir.TabIndex = 0;
            this.tboxWrkDir.TextChanged += new System.EventHandler(this.tboxWrkDir_TextChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.runToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1026, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem3,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(117, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(117, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pasteToSTDINToolStripMenuItem,
            this.toolStripMenuItem2,
            this.copySTDINToolStripMenuItem,
            this.copySTDOUTToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(42, 21);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // pasteToSTDINToolStripMenuItem
            // 
            this.pasteToSTDINToolStripMenuItem.Name = "pasteToSTDINToolStripMenuItem";
            this.pasteToSTDINToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.pasteToSTDINToolStripMenuItem.Text = "Paste to STDIN";
            this.pasteToSTDINToolStripMenuItem.Click += new System.EventHandler(this.pasteToSTDINToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(161, 6);
            // 
            // copySTDINToolStripMenuItem
            // 
            this.copySTDINToolStripMenuItem.Name = "copySTDINToolStripMenuItem";
            this.copySTDINToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copySTDINToolStripMenuItem.Text = "Copy STDIN";
            this.copySTDINToolStripMenuItem.Click += new System.EventHandler(this.copySTDINToolStripMenuItem_Click);
            // 
            // copySTDOUTToolStripMenuItem
            // 
            this.copySTDOUTToolStripMenuItem.Name = "copySTDOUTToolStripMenuItem";
            this.copySTDOUTToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copySTDOUTToolStripMenuItem.Text = "Copy logs";
            this.copySTDOUTToolStripMenuItem.Click += new System.EventHandler(this.copyLogsToolStripMenuItem_Click);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.executeToolStripMenuItem,
            this.killToolStripMenuItem});
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(42, 21);
            this.runToolStripMenuItem.Text = "Run";
            // 
            // executeToolStripMenuItem
            // 
            this.executeToolStripMenuItem.Name = "executeToolStripMenuItem";
            this.executeToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.executeToolStripMenuItem.Text = "Execute";
            // 
            // killToolStripMenuItem
            // 
            this.killToolStripMenuItem.Name = "killToolStripMenuItem";
            this.killToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.killToolStripMenuItem.Text = "Kill";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 564);
            this.Controls.Add(this.toolStripContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lsboxNames;
        private System.Windows.Forms.Label lbExe;
        private System.Windows.Forms.Label lbWrkDir;
        private System.Windows.Forms.TextBox tboxExe;
        private System.Windows.Forms.TextBox tboxWrkDir;
        private System.Windows.Forms.Label lbStdInEncoding;
        private System.Windows.Forms.Label lbEnvVar;
        private System.Windows.Forms.Label lbArgs;
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
        private System.Windows.Forms.ToolStripMenuItem copySTDINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySTDOUTToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem pasteToSTDINToolStripMenuItem;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkWriteStdIn;
        private System.Windows.Forms.CheckBox chkHideWindow;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
    }
}
