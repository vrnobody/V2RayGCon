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
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.panelBtnContainer = new System.Windows.Forms.Panel();
            this.btnExportToFile = new System.Windows.Forms.Button();
            this.btnImportFromFile = new System.Windows.Forms.Button();
            this.btnDeleteAllScripts = new System.Windows.Forms.Button();
            this.btnStopAllScript = new System.Windows.Forms.Button();
            this.btnOpenEditor = new System.Windows.Forms.Button();
            this.btnKillAllScript = new System.Windows.Forms.Button();
            this.panelFlyContainer = new System.Windows.Forms.Panel();
            this.flyScriptUIContainer = new System.Windows.Forms.FlowLayoutPanel();
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
            this.panelMainContainer.SuspendLayout();
            this.panelBtnContainer.SuspendLayout();
            this.panelFlyContainer.SuspendLayout();
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
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panelMainContainer);
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
            // panelMainContainer
            // 
            resources.ApplyResources(this.panelMainContainer, "panelMainContainer");
            this.panelMainContainer.Controls.Add(this.panelBtnContainer);
            this.panelMainContainer.Controls.Add(this.panelFlyContainer);
            this.panelMainContainer.Name = "panelMainContainer";
            this.toolTip1.SetToolTip(this.panelMainContainer, resources.GetString("panelMainContainer.ToolTip"));
            // 
            // panelBtnContainer
            // 
            resources.ApplyResources(this.panelBtnContainer, "panelBtnContainer");
            this.panelBtnContainer.Controls.Add(this.btnExportToFile);
            this.panelBtnContainer.Controls.Add(this.btnImportFromFile);
            this.panelBtnContainer.Controls.Add(this.btnDeleteAllScripts);
            this.panelBtnContainer.Controls.Add(this.btnStopAllScript);
            this.panelBtnContainer.Controls.Add(this.btnOpenEditor);
            this.panelBtnContainer.Controls.Add(this.btnKillAllScript);
            this.panelBtnContainer.Name = "panelBtnContainer";
            this.toolTip1.SetToolTip(this.panelBtnContainer, resources.GetString("panelBtnContainer.ToolTip"));
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
            // btnOpenEditor
            // 
            resources.ApplyResources(this.btnOpenEditor, "btnOpenEditor");
            this.btnOpenEditor.Name = "btnOpenEditor";
            this.toolTip1.SetToolTip(this.btnOpenEditor, resources.GetString("btnOpenEditor.ToolTip"));
            this.btnOpenEditor.UseVisualStyleBackColor = true;
            this.btnOpenEditor.Click += new System.EventHandler(this.btnOpenEditor_Click);
            // 
            // btnKillAllScript
            // 
            resources.ApplyResources(this.btnKillAllScript, "btnKillAllScript");
            this.btnKillAllScript.Name = "btnKillAllScript";
            this.toolTip1.SetToolTip(this.btnKillAllScript, resources.GetString("btnKillAllScript.ToolTip"));
            this.btnKillAllScript.UseVisualStyleBackColor = true;
            // 
            // panelFlyContainer
            // 
            resources.ApplyResources(this.panelFlyContainer, "panelFlyContainer");
            this.panelFlyContainer.Controls.Add(this.flyScriptUIContainer);
            this.panelFlyContainer.Name = "panelFlyContainer";
            this.toolTip1.SetToolTip(this.panelFlyContainer, resources.GetString("panelFlyContainer.ToolTip"));
            // 
            // flyScriptUIContainer
            // 
            resources.ApplyResources(this.flyScriptUIContainer, "flyScriptUIContainer");
            this.flyScriptUIContainer.AllowDrop = true;
            this.flyScriptUIContainer.BackColor = System.Drawing.SystemColors.Window;
            this.flyScriptUIContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flyScriptUIContainer.Name = "flyScriptUIContainer";
            this.toolTip1.SetToolTip(this.flyScriptUIContainer, resources.GetString("flyScriptUIContainer.ToolTip"));
            this.flyScriptUIContainer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.flyScriptUIContainer_Scroll);
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
            this.panelMainContainer.ResumeLayout(false);
            this.panelBtnContainer.ResumeLayout(false);
            this.panelFlyContainer.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbStatusBarMsg;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Button btnKillAllScript;
        private System.Windows.Forms.Button btnStopAllScript;
        private System.Windows.Forms.FlowLayoutPanel flyScriptUIContainer;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panelBtnContainer;
        private System.Windows.Forms.Button btnExportToFile;
        private System.Windows.Forms.Button btnImportFromFile;
        private System.Windows.Forms.Button btnDeleteAllScripts;
        private System.Windows.Forms.Panel panelFlyContainer;
        private System.Windows.Forms.Panel panelMainContainer;
        private System.Windows.Forms.Button btnOpenEditor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    }
}
