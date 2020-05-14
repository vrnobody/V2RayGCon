namespace Luna.Views.WinForms
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbStatusBarMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnExportToFile = new System.Windows.Forms.Button();
            this.btnImportFromFile = new System.Windows.Forms.Button();
            this.btnDeleteAllScripts = new System.Windows.Forms.Button();
            this.btnStopAllScript = new System.Windows.Forms.Button();
            this.btnKillAllScript = new System.Windows.Forms.Button();
            this.flyScriptUIContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.tabOption = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkEnableCodeAnalyze = new System.Windows.Forms.CheckBox();
            this.chkLoadClrLib = new System.Windows.Forms.CheckBox();
            this.btnSaveOptions = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabOption.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.BottomToolStripPanel, "toolStripContainer1.BottomToolStripPanel");
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            this.toolTip1.SetToolTip(this.toolStripContainer1.BottomToolStripPanel, resources.GetString("toolStripContainer1.BottomToolStripPanel.ToolTip"));
            // 
            // toolStripContainer1.ContentPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tabControl1);
            this.toolTip1.SetToolTip(this.toolStripContainer1.ContentPanel, resources.GetString("toolStripContainer1.ContentPanel.ToolTip"));
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.LeftToolStripPanel, "toolStripContainer1.LeftToolStripPanel");
            this.toolTip1.SetToolTip(this.toolStripContainer1.LeftToolStripPanel, resources.GetString("toolStripContainer1.LeftToolStripPanel.ToolTip"));
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.RightToolStripPanel, "toolStripContainer1.RightToolStripPanel");
            this.toolTip1.SetToolTip(this.toolStripContainer1.RightToolStripPanel, resources.GetString("toolStripContainer1.RightToolStripPanel.ToolTip"));
            this.toolTip1.SetToolTip(this.toolStripContainer1, resources.GetString("toolStripContainer1.ToolTip"));
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.TopToolStripPanel, "toolStripContainer1.TopToolStripPanel");
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolTip1.SetToolTip(this.toolStripContainer1.TopToolStripPanel, resources.GetString("toolStripContainer1.TopToolStripPanel.ToolTip"));
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbStatusBarMsg});
            this.statusStrip1.Name = "statusStrip1";
            this.toolTip1.SetToolTip(this.statusStrip1, resources.GetString("statusStrip1.ToolTip"));
            // 
            // lbStatusBarMsg
            // 
            resources.ApplyResources(this.lbStatusBarMsg, "lbStatusBarMsg");
            this.lbStatusBarMsg.Name = "lbStatusBarMsg";
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabOption);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.toolTip1.SetToolTip(this.tabControl1, resources.GetString("tabControl1.ToolTip"));
            // 
            // tabGeneral
            // 
            resources.ApplyResources(this.tabGeneral, "tabGeneral");
            this.tabGeneral.Controls.Add(this.panel2);
            this.tabGeneral.Controls.Add(this.flyScriptUIContainer);
            this.tabGeneral.Name = "tabGeneral";
            this.toolTip1.SetToolTip(this.tabGeneral, resources.GetString("tabGeneral.ToolTip"));
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.btnExportToFile);
            this.panel2.Controls.Add(this.btnImportFromFile);
            this.panel2.Controls.Add(this.btnDeleteAllScripts);
            this.panel2.Controls.Add(this.btnStopAllScript);
            this.panel2.Controls.Add(this.btnKillAllScript);
            this.panel2.Name = "panel2";
            this.toolTip1.SetToolTip(this.panel2, resources.GetString("panel2.ToolTip"));
            // 
            // btnExportToFile
            // 
            resources.ApplyResources(this.btnExportToFile, "btnExportToFile");
            this.btnExportToFile.Name = "btnExportToFile";
            this.toolTip1.SetToolTip(this.btnExportToFile, resources.GetString("btnExportToFile.ToolTip"));
            this.btnExportToFile.UseVisualStyleBackColor = true;
            // 
            // btnImportFromFile
            // 
            resources.ApplyResources(this.btnImportFromFile, "btnImportFromFile");
            this.btnImportFromFile.Name = "btnImportFromFile";
            this.toolTip1.SetToolTip(this.btnImportFromFile, resources.GetString("btnImportFromFile.ToolTip"));
            this.btnImportFromFile.UseVisualStyleBackColor = true;
            // 
            // btnDeleteAllScripts
            // 
            resources.ApplyResources(this.btnDeleteAllScripts, "btnDeleteAllScripts");
            this.btnDeleteAllScripts.Name = "btnDeleteAllScripts";
            this.toolTip1.SetToolTip(this.btnDeleteAllScripts, resources.GetString("btnDeleteAllScripts.ToolTip"));
            this.btnDeleteAllScripts.UseVisualStyleBackColor = true;
            // 
            // btnStopAllScript
            // 
            resources.ApplyResources(this.btnStopAllScript, "btnStopAllScript");
            this.btnStopAllScript.Name = "btnStopAllScript";
            this.toolTip1.SetToolTip(this.btnStopAllScript, resources.GetString("btnStopAllScript.ToolTip"));
            this.btnStopAllScript.UseVisualStyleBackColor = true;
            // 
            // btnKillAllScript
            // 
            resources.ApplyResources(this.btnKillAllScript, "btnKillAllScript");
            this.btnKillAllScript.Name = "btnKillAllScript";
            this.toolTip1.SetToolTip(this.btnKillAllScript, resources.GetString("btnKillAllScript.ToolTip"));
            this.btnKillAllScript.UseVisualStyleBackColor = true;
            // 
            // flyScriptUIContainer
            // 
            resources.ApplyResources(this.flyScriptUIContainer, "flyScriptUIContainer");
            this.flyScriptUIContainer.AllowDrop = true;
            this.flyScriptUIContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flyScriptUIContainer.Name = "flyScriptUIContainer";
            this.toolTip1.SetToolTip(this.flyScriptUIContainer, resources.GetString("flyScriptUIContainer.ToolTip"));
            this.flyScriptUIContainer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.flyScriptUIContainer_Scroll);
            // 
            // tabOption
            // 
            resources.ApplyResources(this.tabOption, "tabOption");
            this.tabOption.Controls.Add(this.panel1);
            this.tabOption.Name = "tabOption";
            this.toolTip1.SetToolTip(this.tabOption, resources.GetString("tabOption.ToolTip"));
            this.tabOption.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.btnSaveOptions);
            this.panel1.Name = "panel1";
            this.toolTip1.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.chkEnableCodeAnalyze);
            this.groupBox1.Controls.Add(this.chkLoadClrLib);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // chkEnableCodeAnalyze
            // 
            resources.ApplyResources(this.chkEnableCodeAnalyze, "chkEnableCodeAnalyze");
            this.chkEnableCodeAnalyze.Name = "chkEnableCodeAnalyze";
            this.toolTip1.SetToolTip(this.chkEnableCodeAnalyze, resources.GetString("chkEnableCodeAnalyze.ToolTip"));
            this.chkEnableCodeAnalyze.UseVisualStyleBackColor = true;
            // 
            // chkLoadClrLib
            // 
            resources.ApplyResources(this.chkLoadClrLib, "chkLoadClrLib");
            this.chkLoadClrLib.Name = "chkLoadClrLib";
            this.toolTip1.SetToolTip(this.chkLoadClrLib, resources.GetString("chkLoadClrLib.ToolTip"));
            this.chkLoadClrLib.UseVisualStyleBackColor = true;
            // 
            // btnSaveOptions
            // 
            resources.ApplyResources(this.btnSaveOptions, "btnSaveOptions");
            this.btnSaveOptions.Name = "btnSaveOptions";
            this.toolTip1.SetToolTip(this.btnSaveOptions, resources.GetString("btnSaveOptions.ToolTip"));
            this.btnSaveOptions.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            this.toolTip1.SetToolTip(this.menuStrip1, resources.GetString("menuStrip1.ToolTip"));
            // 
            // windowToolStripMenuItem
            // 
            resources.ApplyResources(this.windowToolStripMenuItem, "windowToolStripMenuItem");
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showEditorToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            // 
            // showEditorToolStripMenuItem
            // 
            resources.ApplyResources(this.showEditorToolStripMenuItem, "showEditorToolStripMenuItem");
            this.showEditorToolStripMenuItem.Name = "showEditorToolStripMenuItem";
            this.showEditorToolStripMenuItem.Click += new System.EventHandler(this.showEditorToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // closeToolStripMenuItem
            // 
            resources.ApplyResources(this.closeToolStripMenuItem, "closeToolStripMenuItem");
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabOption.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbStatusBarMsg;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.Button btnKillAllScript;
        private System.Windows.Forms.Button btnStopAllScript;
        private System.Windows.Forms.FlowLayoutPanel flyScriptUIContainer;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnExportToFile;
        private System.Windows.Forms.Button btnImportFromFile;
        private System.Windows.Forms.Button btnDeleteAllScripts;
        private System.Windows.Forms.TabPage tabOption;
        private System.Windows.Forms.Button btnSaveOptions;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkEnableCodeAnalyze;
        private System.Windows.Forms.CheckBox chkLoadClrLib;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    }
}
