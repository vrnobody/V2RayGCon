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
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tLSECHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tLSCertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x25519ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireGuardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mLDSA65ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.passwordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uUIDV4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alphabetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numAlphabetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numAlphabetSymbolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fragmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.routingCNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cNDirectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.fileToolStripMenuItem,
            this.generateToolStripMenuItem,
            this.passwordToolStripMenuItem,
            this.configToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.closeToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            resources.ApplyResources(this.closeToolStripMenuItem1, "closeToolStripMenuItem1");
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.closeToolStripMenuItem1_Click);
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tLSECHToolStripMenuItem,
            this.tLSCertToolStripMenuItem,
            this.x25519ToolStripMenuItem,
            this.wireGuardToolStripMenuItem,
            this.mLDSA65ToolStripMenuItem});
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
            // passwordToolStripMenuItem
            // 
            this.passwordToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uUIDV4ToolStripMenuItem,
            this.hexToolStripMenuItem,
            this.numberToolStripMenuItem,
            this.alphabetToolStripMenuItem,
            this.numAlphabetToolStripMenuItem,
            this.numAlphabetSymbolToolStripMenuItem});
            this.passwordToolStripMenuItem.Name = "passwordToolStripMenuItem";
            resources.ApplyResources(this.passwordToolStripMenuItem, "passwordToolStripMenuItem");
            // 
            // uUIDV4ToolStripMenuItem
            // 
            this.uUIDV4ToolStripMenuItem.Name = "uUIDV4ToolStripMenuItem";
            resources.ApplyResources(this.uUIDV4ToolStripMenuItem, "uUIDV4ToolStripMenuItem");
            this.uUIDV4ToolStripMenuItem.Click += new System.EventHandler(this.uUIDV4ToolStripMenuItem_Click_1);
            // 
            // hexToolStripMenuItem
            // 
            this.hexToolStripMenuItem.Name = "hexToolStripMenuItem";
            resources.ApplyResources(this.hexToolStripMenuItem, "hexToolStripMenuItem");
            this.hexToolStripMenuItem.Click += new System.EventHandler(this.hexToolStripMenuItem_Click);
            // 
            // numberToolStripMenuItem
            // 
            this.numberToolStripMenuItem.Name = "numberToolStripMenuItem";
            resources.ApplyResources(this.numberToolStripMenuItem, "numberToolStripMenuItem");
            this.numberToolStripMenuItem.Click += new System.EventHandler(this.numberToolStripMenuItem_Click);
            // 
            // alphabetToolStripMenuItem
            // 
            this.alphabetToolStripMenuItem.Name = "alphabetToolStripMenuItem";
            resources.ApplyResources(this.alphabetToolStripMenuItem, "alphabetToolStripMenuItem");
            this.alphabetToolStripMenuItem.Click += new System.EventHandler(this.alphabetToolStripMenuItem_Click);
            // 
            // numAlphabetToolStripMenuItem
            // 
            this.numAlphabetToolStripMenuItem.Name = "numAlphabetToolStripMenuItem";
            resources.ApplyResources(this.numAlphabetToolStripMenuItem, "numAlphabetToolStripMenuItem");
            this.numAlphabetToolStripMenuItem.Click += new System.EventHandler(this.numAlphabetToolStripMenuItem_Click);
            // 
            // numAlphabetSymbolToolStripMenuItem
            // 
            this.numAlphabetSymbolToolStripMenuItem.Name = "numAlphabetSymbolToolStripMenuItem";
            resources.ApplyResources(this.numAlphabetSymbolToolStripMenuItem, "numAlphabetSymbolToolStripMenuItem");
            this.numAlphabetSymbolToolStripMenuItem.Click += new System.EventHandler(this.numAlphabetSymbolToolStripMenuItem_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fragmentToolStripMenuItem,
            this.routingCNToolStripMenuItem});
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            resources.ApplyResources(this.configToolStripMenuItem, "configToolStripMenuItem");
            // 
            // fragmentToolStripMenuItem
            // 
            this.fragmentToolStripMenuItem.Name = "fragmentToolStripMenuItem";
            resources.ApplyResources(this.fragmentToolStripMenuItem, "fragmentToolStripMenuItem");
            this.fragmentToolStripMenuItem.Click += new System.EventHandler(this.fragmentToolStripMenuItem_Click);
            // 
            // routingCNToolStripMenuItem
            // 
            this.routingCNToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cNDirectToolStripMenuItem});
            this.routingCNToolStripMenuItem.Name = "routingCNToolStripMenuItem";
            resources.ApplyResources(this.routingCNToolStripMenuItem, "routingCNToolStripMenuItem");
            
            // 
            // cNDirectToolStripMenuItem
            // 
            this.cNDirectToolStripMenuItem.Name = "cNDirectToolStripMenuItem";
            resources.ApplyResources(this.cNDirectToolStripMenuItem, "cNDirectToolStripMenuItem");
            this.cNDirectToolStripMenuItem.Click += new System.EventHandler(this.cNDirectToolStripMenuItem_Click);
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
        private System.Windows.Forms.ToolStripMenuItem x25519ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireGuardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mLDSA65ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem passwordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uUIDV4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alphabetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numAlphabetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numAlphabetSymbolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fragmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem routingCNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cNDirectToolStripMenuItem;
    }
}
