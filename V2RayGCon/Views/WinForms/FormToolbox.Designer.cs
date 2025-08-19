namespace V2RayGCon.Views.WinForms
{
    partial class FormToolbox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormToolbox));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.base64ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unicodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uRIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.base64ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.unicodeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uRIToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.scanQRCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rtBoxOutput = new VgcApis.UserControls.ExRichTextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.generateToolStripMenuItem,
            this.passwordToolStripMenuItem,
            this.toolToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.toolStripMenuItemPaste,
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
            // toolStripMenuItemPaste
            // 
            this.toolStripMenuItemPaste.Name = "toolStripMenuItemPaste";
            resources.ApplyResources(this.toolStripMenuItemPaste, "toolStripMenuItemPaste");
            this.toolStripMenuItemPaste.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
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
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decodeToolStripMenuItem,
            this.encodeToolStripMenuItem,
            this.scanQRCodeToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            resources.ApplyResources(this.toolToolStripMenuItem, "toolToolStripMenuItem");
            // 
            // decodeToolStripMenuItem
            // 
            this.decodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.base64ToolStripMenuItem,
            this.unicodeToolStripMenuItem,
            this.uRIToolStripMenuItem});
            this.decodeToolStripMenuItem.Name = "decodeToolStripMenuItem";
            resources.ApplyResources(this.decodeToolStripMenuItem, "decodeToolStripMenuItem");
            // 
            // base64ToolStripMenuItem
            // 
            this.base64ToolStripMenuItem.Name = "base64ToolStripMenuItem";
            resources.ApplyResources(this.base64ToolStripMenuItem, "base64ToolStripMenuItem");
            this.base64ToolStripMenuItem.Click += new System.EventHandler(this.decodeBase64ToolStripMenuItem_Click);
            // 
            // unicodeToolStripMenuItem
            // 
            this.unicodeToolStripMenuItem.Name = "unicodeToolStripMenuItem";
            resources.ApplyResources(this.unicodeToolStripMenuItem, "unicodeToolStripMenuItem");
            this.unicodeToolStripMenuItem.Click += new System.EventHandler(this.decodeUnicodeToolStripMenuItem_Click);
            // 
            // uRIToolStripMenuItem
            // 
            this.uRIToolStripMenuItem.Name = "uRIToolStripMenuItem";
            resources.ApplyResources(this.uRIToolStripMenuItem, "uRIToolStripMenuItem");
            this.uRIToolStripMenuItem.Click += new System.EventHandler(this.decodeUriToolStripMenuItem_Click);
            // 
            // encodeToolStripMenuItem
            // 
            this.encodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.base64ToolStripMenuItem1,
            this.unicodeToolStripMenuItem1,
            this.uRIToolStripMenuItem1});
            this.encodeToolStripMenuItem.Name = "encodeToolStripMenuItem";
            resources.ApplyResources(this.encodeToolStripMenuItem, "encodeToolStripMenuItem");
            // 
            // base64ToolStripMenuItem1
            // 
            this.base64ToolStripMenuItem1.Name = "base64ToolStripMenuItem1";
            resources.ApplyResources(this.base64ToolStripMenuItem1, "base64ToolStripMenuItem1");
            this.base64ToolStripMenuItem1.Click += new System.EventHandler(this.encodeBase64ToolStripMenuItem_Click);
            // 
            // unicodeToolStripMenuItem1
            // 
            this.unicodeToolStripMenuItem1.Name = "unicodeToolStripMenuItem1";
            resources.ApplyResources(this.unicodeToolStripMenuItem1, "unicodeToolStripMenuItem1");
            this.unicodeToolStripMenuItem1.Click += new System.EventHandler(this.encodeUnicodeToolStripMenuItem_Click);
            // 
            // uRIToolStripMenuItem1
            // 
            this.uRIToolStripMenuItem1.Name = "uRIToolStripMenuItem1";
            resources.ApplyResources(this.uRIToolStripMenuItem1, "uRIToolStripMenuItem1");
            this.uRIToolStripMenuItem1.Click += new System.EventHandler(this.encodeUriToolStripMenuItem_Click);
            // 
            // scanQRCodeToolStripMenuItem
            // 
            this.scanQRCodeToolStripMenuItem.Name = "scanQRCodeToolStripMenuItem";
            resources.ApplyResources(this.scanQRCodeToolStripMenuItem, "scanQRCodeToolStripMenuItem");
            this.scanQRCodeToolStripMenuItem.Click += new System.EventHandler(this.scanQRCodeToolStripMenuItem_Click);
            // 
            // rtBoxOutput
            // 
            resources.ApplyResources(this.rtBoxOutput, "rtBoxOutput");
            this.rtBoxOutput.Name = "rtBoxOutput";
            // 
            // FormToolbox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtBoxOutput);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormToolbox";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
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
        private System.Windows.Forms.ToolStripMenuItem hexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanQRCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem base64ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unicodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uRIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem base64ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem unicodeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uRIToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPaste;
        private VgcApis.UserControls.ExRichTextBox rtBoxOutput;
    }
}
