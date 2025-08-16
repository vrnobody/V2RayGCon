namespace V2RayGCon.Views.WinForms
{
    partial class FormKeyGen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormKeyGen));
            this.rtBoxOutput = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tLSECHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tLSCertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uUIDV4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x25519ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireGuardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mLDSA65ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtBoxOutput
            // 
            resources.ApplyResources(this.rtBoxOutput, "rtBoxOutput");
            this.rtBoxOutput.Name = "rtBoxOutput";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tLSECHToolStripMenuItem,
            this.tLSCertToolStripMenuItem,
            this.uUIDV4ToolStripMenuItem,
            this.x25519ToolStripMenuItem,
            this.wireGuardToolStripMenuItem,
            this.mLDSA65ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
            this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            resources.ApplyResources(this.generateToolStripMenuItem, "generateToolStripMenuItem");
            // 
            // tLSECHToolStripMenuItem
            // 
            this.tLSECHToolStripMenuItem.Name = "tLSECHToolStripMenuItem";
            resources.ApplyResources(this.tLSECHToolStripMenuItem, "tLSECHToolStripMenuItem");
            this.tLSECHToolStripMenuItem.Click += new System.EventHandler(this.tLSECHToolStripMenuItem_Click);
            // 
            // tLSCertToolStripMenuItem
            // 
            this.tLSCertToolStripMenuItem.Name = "tLSCertToolStripMenuItem";
            resources.ApplyResources(this.tLSCertToolStripMenuItem, "tLSCertToolStripMenuItem");
            this.tLSCertToolStripMenuItem.Click += new System.EventHandler(this.tLSCertToolStripMenuItem_Click);
            // 
            // uUIDV4ToolStripMenuItem
            // 
            this.uUIDV4ToolStripMenuItem.Name = "uUIDV4ToolStripMenuItem";
            resources.ApplyResources(this.uUIDV4ToolStripMenuItem, "uUIDV4ToolStripMenuItem");
            this.uUIDV4ToolStripMenuItem.Click += new System.EventHandler(this.uUIDV4ToolStripMenuItem_Click);
            // 
            // x25519ToolStripMenuItem
            // 
            this.x25519ToolStripMenuItem.Name = "x25519ToolStripMenuItem";
            resources.ApplyResources(this.x25519ToolStripMenuItem, "x25519ToolStripMenuItem");
            this.x25519ToolStripMenuItem.Click += new System.EventHandler(this.x25519ToolStripMenuItem_Click);
            // 
            // wireGuardToolStripMenuItem
            // 
            this.wireGuardToolStripMenuItem.Name = "wireGuardToolStripMenuItem";
            resources.ApplyResources(this.wireGuardToolStripMenuItem, "wireGuardToolStripMenuItem");
            this.wireGuardToolStripMenuItem.Click += new System.EventHandler(this.wireGuardToolStripMenuItem_Click);
            // 
            // mLDSA65ToolStripMenuItem
            // 
            this.mLDSA65ToolStripMenuItem.Name = "mLDSA65ToolStripMenuItem";
            resources.ApplyResources(this.mLDSA65ToolStripMenuItem, "mLDSA65ToolStripMenuItem");
            this.mLDSA65ToolStripMenuItem.Click += new System.EventHandler(this.mLDSA65ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            resources.ApplyResources(this.closeToolStripMenuItem, "closeToolStripMenuItem");
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // FormKeyGen
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtBoxOutput);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormKeyGen";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtBoxOutput;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tLSECHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tLSCertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uUIDV4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x25519ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireGuardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mLDSA65ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    }
}