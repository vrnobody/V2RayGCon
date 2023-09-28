namespace V2RayGCon.Views.WinForms
{
    partial class FormSimpleConfiger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSimpleConfiger));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.panelVeeImporter = new System.Windows.Forms.Panel();
            this.SimpleConfigerUI1 = new V2RayGCon.Views.UserControls.SimpleConfigerUI();
            this.panelVeeImporter.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.toolTip1.SetToolTip(this.btnOK, resources.GetString("btnOK.ToolTip"));
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.toolTip1.SetToolTip(this.btnExit, resources.GetString("btnExit.ToolTip"));
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // panelVeeImporter
            // 
            resources.ApplyResources(this.panelVeeImporter, "panelVeeImporter");
            this.panelVeeImporter.Controls.Add(this.SimpleConfigerUI1);
            this.panelVeeImporter.Name = "panelVeeImporter";
            this.toolTip1.SetToolTip(this.panelVeeImporter, resources.GetString("panelVeeImporter.ToolTip"));
            // 
            // SimpleConfigerUI1
            // 
            resources.ApplyResources(this.SimpleConfigerUI1, "SimpleConfigerUI1");
            this.SimpleConfigerUI1.Name = "SimpleConfigerUI1";
            this.toolTip1.SetToolTip(this.SimpleConfigerUI1, resources.GetString("SimpleConfigerUI1.ToolTip"));
            // 
            // FormSimpleConfiger
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelVeeImporter);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnOK);
            this.Name = "FormSimpleConfiger";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.FormSimpleEditor_Load);
            this.panelVeeImporter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panelVeeImporter;
        private UserControls.SimpleConfigerUI SimpleConfigerUI1;
    }
}
