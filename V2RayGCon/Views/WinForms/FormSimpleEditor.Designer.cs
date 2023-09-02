namespace V2RayGCon.Views.WinForms
{
    partial class FormSimpleEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSimpleEditor));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.panelVeeImporter = new System.Windows.Forms.Panel();
            this.veeConfigerUI1 = new V2RayGCon.Views.UserControls.VeeConfigerUI();
            this.linkConfigEditor = new System.Windows.Forms.LinkLabel();
            this.lbTitle = new System.Windows.Forms.Label();
            this.panelVeeImporter.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // panelVeeImporter
            // 
            resources.ApplyResources(this.panelVeeImporter, "panelVeeImporter");
            this.panelVeeImporter.Controls.Add(this.veeConfigerUI1);
            this.panelVeeImporter.Name = "panelVeeImporter";
            // 
            // veeConfigerUI1
            // 
            resources.ApplyResources(this.veeConfigerUI1, "veeConfigerUI1");
            this.veeConfigerUI1.Name = "veeConfigerUI1";
            // 
            // linkConfigEditor
            // 
            resources.ApplyResources(this.linkConfigEditor, "linkConfigEditor");
            this.linkConfigEditor.Name = "linkConfigEditor";
            this.linkConfigEditor.TabStop = true;
            this.linkConfigEditor.UseCompatibleTextRendering = true;
            this.linkConfigEditor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkConfigEditor_LinkClicked);
            // 
            // lbTitle
            // 
            resources.ApplyResources(this.lbTitle, "lbTitle");
            this.lbTitle.Name = "lbTitle";
            // 
            // FormSimpleEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.panelVeeImporter);
            this.Controls.Add(this.linkConfigEditor);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnOK);
            this.Name = "FormSimpleEditor";
            this.Load += new System.EventHandler(this.FormSimpleEditor_Load);
            this.panelVeeImporter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panelVeeImporter;
        private System.Windows.Forms.LinkLabel linkConfigEditor;
        private System.Windows.Forms.Label lbTitle;
        private UserControls.VeeConfigerUI veeConfigerUI1;
    }
}
