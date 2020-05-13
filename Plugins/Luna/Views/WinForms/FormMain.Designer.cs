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
            this.btnShowEditor = new System.Windows.Forms.Button();
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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSaveOptions = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabOption.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tabControl1);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbStatusBarMsg});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // lbStatusBarMsg
            // 
            this.lbStatusBarMsg.Name = "lbStatusBarMsg";
            resources.ApplyResources(this.lbStatusBarMsg, "lbStatusBarMsg");
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabOption);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.panel2);
            this.tabGeneral.Controls.Add(this.flyScriptUIContainer);
            resources.ApplyResources(this.tabGeneral, "tabGeneral");
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.btnShowEditor);
            this.panel2.Controls.Add(this.btnExportToFile);
            this.panel2.Controls.Add(this.btnImportFromFile);
            this.panel2.Controls.Add(this.btnDeleteAllScripts);
            this.panel2.Controls.Add(this.btnStopAllScript);
            this.panel2.Controls.Add(this.btnKillAllScript);
            this.panel2.Name = "panel2";
            // 
            // btnShowEditor
            // 
            resources.ApplyResources(this.btnShowEditor, "btnShowEditor");
            this.btnShowEditor.Name = "btnShowEditor";
            this.toolTip1.SetToolTip(this.btnShowEditor, resources.GetString("btnShowEditor.ToolTip"));
            this.btnShowEditor.UseVisualStyleBackColor = true;
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
            this.flyScriptUIContainer.AllowDrop = true;
            resources.ApplyResources(this.flyScriptUIContainer, "flyScriptUIContainer");
            this.flyScriptUIContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flyScriptUIContainer.Name = "flyScriptUIContainer";
            this.flyScriptUIContainer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.flyScriptUIContainer_Scroll);
            // 
            // tabOption
            // 
            this.tabOption.Controls.Add(this.panel1);
            resources.ApplyResources(this.tabOption, "tabOption");
            this.tabOption.Name = "tabOption";
            this.tabOption.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Controls.Add(this.btnSaveOptions);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.chkEnableCodeAnalyze);
            this.groupBox1.Controls.Add(this.chkLoadClrLib);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
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
            this.chkLoadClrLib.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // btnSaveOptions
            // 
            resources.ApplyResources(this.btnSaveOptions, "btnSaveOptions");
            this.btnSaveOptions.Name = "btnSaveOptions";
            this.btnSaveOptions.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSaveOptions;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkEnableCodeAnalyze;
        private System.Windows.Forms.CheckBox chkLoadClrLib;
        private System.Windows.Forms.Button btnShowEditor;
        private System.Windows.Forms.Panel panel1;
    }
}
