namespace V2RayGCon.Views.WinForms
{
    partial class FormModifyServerSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModifyServerSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.tboxServerName = new System.Windows.Forms.TextBox();
            this.cboxInboundAddress = new System.Windows.Forms.ComboBox();
            this.chkAutoRun = new System.Windows.Forms.CheckBox();
            this.chkGlobalImport = new System.Windows.Forms.CheckBox();
            this.chkBypassCnSite = new System.Windows.Forms.CheckBox();
            this.cboxInboundMode = new System.Windows.Forms.ComboBox();
            this.cboxMark = new System.Windows.Forms.ComboBox();
            this.chkUntrack = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tboxDescription = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tboxRemark = new System.Windows.Forms.TextBox();
            this.pboxQrcode = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClearStat = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cboxZoomMode = new System.Windows.Forms.ComboBox();
            this.cboxShareLinkType = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnSaveQrcode = new System.Windows.Forms.Button();
            this.btnCopyShareLink = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tboxShareLink = new System.Windows.Forms.TextBox();
            this.lbServerTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pboxQrcode)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tboxServerName
            // 
            resources.ApplyResources(this.tboxServerName, "tboxServerName");
            this.tboxServerName.Name = "tboxServerName";
            // 
            // cboxInboundAddress
            // 
            this.cboxInboundAddress.FormattingEnabled = true;
            this.cboxInboundAddress.Items.AddRange(new object[] {
            resources.GetString("cboxInboundAddress.Items"),
            resources.GetString("cboxInboundAddress.Items1"),
            resources.GetString("cboxInboundAddress.Items2"),
            resources.GetString("cboxInboundAddress.Items3")});
            resources.ApplyResources(this.cboxInboundAddress, "cboxInboundAddress");
            this.cboxInboundAddress.Name = "cboxInboundAddress";
            this.cboxInboundAddress.TextChanged += new System.EventHandler(this.cboxInboundAddress_TextChanged);
            // 
            // chkAutoRun
            // 
            resources.ApplyResources(this.chkAutoRun, "chkAutoRun");
            this.chkAutoRun.Name = "chkAutoRun";
            this.toolTip1.SetToolTip(this.chkAutoRun, resources.GetString("chkAutoRun.ToolTip"));
            this.chkAutoRun.UseVisualStyleBackColor = true;
            // 
            // chkGlobalImport
            // 
            resources.ApplyResources(this.chkGlobalImport, "chkGlobalImport");
            this.chkGlobalImport.Name = "chkGlobalImport";
            this.toolTip1.SetToolTip(this.chkGlobalImport, resources.GetString("chkGlobalImport.ToolTip"));
            this.chkGlobalImport.UseVisualStyleBackColor = true;
            // 
            // chkBypassCnSite
            // 
            resources.ApplyResources(this.chkBypassCnSite, "chkBypassCnSite");
            this.chkBypassCnSite.Name = "chkBypassCnSite";
            this.toolTip1.SetToolTip(this.chkBypassCnSite, resources.GetString("chkBypassCnSite.ToolTip"));
            this.chkBypassCnSite.UseVisualStyleBackColor = true;
            // 
            // cboxInboundMode
            // 
            this.cboxInboundMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxInboundMode.FormattingEnabled = true;
            this.cboxInboundMode.Items.AddRange(new object[] {
            resources.GetString("cboxInboundMode.Items"),
            resources.GetString("cboxInboundMode.Items1"),
            resources.GetString("cboxInboundMode.Items2"),
            resources.GetString("cboxInboundMode.Items3")});
            resources.ApplyResources(this.cboxInboundMode, "cboxInboundMode");
            this.cboxInboundMode.Name = "cboxInboundMode";
            this.cboxInboundMode.SelectedIndexChanged += new System.EventHandler(this.cboxInboundMode_SelectedIndexChanged);
            // 
            // cboxMark
            // 
            this.cboxMark.FormattingEnabled = true;
            resources.ApplyResources(this.cboxMark, "cboxMark");
            this.cboxMark.Name = "cboxMark";
            // 
            // chkUntrack
            // 
            resources.ApplyResources(this.chkUntrack, "chkUntrack");
            this.chkUntrack.Name = "chkUntrack";
            this.toolTip1.SetToolTip(this.chkUntrack, resources.GetString("chkUntrack.ToolTip"));
            this.chkUntrack.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // tboxDescription
            // 
            resources.ApplyResources(this.tboxDescription, "tboxDescription");
            this.tboxDescription.Name = "tboxDescription";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // tboxRemark
            // 
            resources.ApplyResources(this.tboxRemark, "tboxRemark");
            this.tboxRemark.Name = "tboxRemark";
            // 
            // pboxQrcode
            // 
            this.pboxQrcode.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.pboxQrcode, "pboxQrcode");
            this.pboxQrcode.Name = "pboxQrcode";
            this.pboxQrcode.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboxInboundAddress);
            this.groupBox1.Controls.Add(this.cboxInboundMode);
            this.groupBox1.Controls.Add(this.btnClearStat);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.chkGlobalImport);
            this.groupBox1.Controls.Add(this.chkBypassCnSite);
            this.groupBox1.Controls.Add(this.tboxServerName);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.chkUntrack);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkAutoRun);
            this.groupBox1.Controls.Add(this.tboxRemark);
            this.groupBox1.Controls.Add(this.tboxDescription);
            this.groupBox1.Controls.Add(this.cboxMark);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnClearStat
            // 
            resources.ApplyResources(this.btnClearStat, "btnClearStat");
            this.btnClearStat.Name = "btnClearStat";
            this.toolTip1.SetToolTip(this.btnClearStat, resources.GetString("btnClearStat.ToolTip"));
            this.btnClearStat.UseVisualStyleBackColor = true;
            this.btnClearStat.Click += new System.EventHandler(this.btnClearStat_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cboxZoomMode
            // 
            this.cboxZoomMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxZoomMode.FormattingEnabled = true;
            this.cboxZoomMode.Items.AddRange(new object[] {
            resources.GetString("cboxZoomMode.Items"),
            resources.GetString("cboxZoomMode.Items1")});
            resources.ApplyResources(this.cboxZoomMode, "cboxZoomMode");
            this.cboxZoomMode.Name = "cboxZoomMode";
            this.cboxZoomMode.SelectedValueChanged += new System.EventHandler(this.cboxZoomMode_SelectedValueChanged);
            // 
            // cboxShareLinkType
            // 
            this.cboxShareLinkType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxShareLinkType.FormattingEnabled = true;
            this.cboxShareLinkType.Items.AddRange(new object[] {
            resources.GetString("cboxShareLinkType.Items"),
            resources.GetString("cboxShareLinkType.Items1")});
            resources.ApplyResources(this.cboxShareLinkType, "cboxShareLinkType");
            this.cboxShareLinkType.Name = "cboxShareLinkType";
            this.cboxShareLinkType.SelectedValueChanged += new System.EventHandler(this.cboxShareLinkType_SelectedValueChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnSaveQrcode);
            this.groupBox4.Controls.Add(this.btnCopyShareLink);
            this.groupBox4.Controls.Add(this.cboxShareLinkType);
            this.groupBox4.Controls.Add(this.cboxZoomMode);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.pboxQrcode);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.tboxShareLink);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // btnSaveQrcode
            // 
            resources.ApplyResources(this.btnSaveQrcode, "btnSaveQrcode");
            this.btnSaveQrcode.Name = "btnSaveQrcode";
            this.btnSaveQrcode.UseVisualStyleBackColor = true;
            this.btnSaveQrcode.Click += new System.EventHandler(this.btnSaveQrcode_Click);
            // 
            // btnCopyShareLink
            // 
            resources.ApplyResources(this.btnCopyShareLink, "btnCopyShareLink");
            this.btnCopyShareLink.Name = "btnCopyShareLink";
            this.btnCopyShareLink.UseVisualStyleBackColor = true;
            this.btnCopyShareLink.Click += new System.EventHandler(this.btnCopyShareLink_Click);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // tboxShareLink
            // 
            resources.ApplyResources(this.tboxShareLink, "tboxShareLink");
            this.tboxShareLink.Name = "tboxShareLink";
            this.tboxShareLink.TextChanged += new System.EventHandler(this.tboxShareLink_TextChanged);
            // 
            // lbServerTitle
            // 
            resources.ApplyResources(this.lbServerTitle, "lbServerTitle");
            this.lbServerTitle.Name = "lbServerTitle";
            // 
            // FormModifyServerSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbServerTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormModifyServerSettings";
            this.Load += new System.EventHandler(this.FormModifyServerSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pboxQrcode)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboxServerName;
        private System.Windows.Forms.ComboBox cboxInboundAddress;
        private System.Windows.Forms.CheckBox chkAutoRun;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkGlobalImport;
        private System.Windows.Forms.CheckBox chkBypassCnSite;
        private System.Windows.Forms.ComboBox cboxInboundMode;
        private System.Windows.Forms.ComboBox cboxMark;
        private System.Windows.Forms.CheckBox chkUntrack;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tboxDescription;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tboxRemark;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pboxQrcode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboxShareLinkType;
        private System.Windows.Forms.ComboBox cboxZoomMode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSaveQrcode;
        private System.Windows.Forms.Button btnCopyShareLink;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tboxShareLink;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lbServerTitle;
        private System.Windows.Forms.Button btnClearStat;
    }
}
