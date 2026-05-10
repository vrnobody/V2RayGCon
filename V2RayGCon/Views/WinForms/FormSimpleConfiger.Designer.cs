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
            this.btnClient = new VgcApis.UserControls.RoundButton();
            this.btnServer = new VgcApis.UserControls.RoundButton();
            this.btnExit = new VgcApis.UserControls.RoundButton();
            this.panelVeeImporter = new System.Windows.Forms.Panel();
            this.SimpleConfigerUI1 = new V2RayGCon.Views.UserControls.SimpleConfigerUI();
            this.panelVeeImporter.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClient
            // 
            resources.ApplyResources(this.btnClient, "btnClient");
            this.btnClient.BackColor = System.Drawing.SystemColors.Control;
            this.btnClient.Name = "btnClient";
            this.toolTip1.SetToolTip(this.btnClient, resources.GetString("btnClient.ToolTip"));
            this.btnClient.UseVisualStyleBackColor = false;
            this.btnClient.Click += new System.EventHandler(this.btnClient_Click);
            // 
            // btnServer
            // 
            resources.ApplyResources(this.btnServer, "btnServer");
            this.btnServer.BackColor = System.Drawing.SystemColors.Control;
            this.btnServer.Name = "btnServer";
            this.toolTip1.SetToolTip(this.btnServer, resources.GetString("btnServer.ToolTip"));
            this.btnServer.UseVisualStyleBackColor = false;
            this.btnServer.Click += new System.EventHandler(this.btnServer_Click);
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.BackColor = System.Drawing.SystemColors.Control;
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // panelVeeImporter
            // 
            this.panelVeeImporter.Controls.Add(this.SimpleConfigerUI1);
            resources.ApplyResources(this.panelVeeImporter, "panelVeeImporter");
            this.panelVeeImporter.Name = "panelVeeImporter";
            // 
            // SimpleConfigerUI1
            // 
            resources.ApplyResources(this.SimpleConfigerUI1, "SimpleConfigerUI1");
            this.SimpleConfigerUI1.Name = "SimpleConfigerUI1";
            // 
            // FormSimpleConfiger
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelVeeImporter);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnServer);
            this.Controls.Add(this.btnClient);
            this.Name = "FormSimpleConfiger";
            this.Load += new System.EventHandler(this.FormSimpleEditor_Load);
            this.panelVeeImporter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panelVeeImporter;
        private UserControls.SimpleConfigerUI SimpleConfigerUI1;
        private VgcApis.UserControls.RoundButton btnClient;
        private VgcApis.UserControls.RoundButton btnExit;
        private VgcApis.UserControls.RoundButton btnServer;
    }
}
