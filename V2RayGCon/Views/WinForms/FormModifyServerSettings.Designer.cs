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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkOutbOTA = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOpenInEditor = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tboxDescription = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tboxRemark = new System.Windows.Forms.TextBox();
            this.pboxQrcode = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboxZoomMode = new System.Windows.Forms.ComboBox();
            this.cboxShareLinkType = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkOutbStreamUseTls = new System.Windows.Forms.CheckBox();
            this.cboxOutbMethod = new System.Windows.Forms.ComboBox();
            this.cboxOutbStreamParma = new System.Windows.Forms.ComboBox();
            this.cboxOutbProto = new System.Windows.Forms.ComboBox();
            this.cboxOutbStreamType = new System.Windows.Forms.ComboBox();
            this.tboxOutbAddr = new System.Windows.Forms.TextBox();
            this.tboxOutbAuth2 = new System.Windows.Forms.TextBox();
            this.tboxOutbAuth1 = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.lbOutbAuth2 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lbOutbAuth1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnSaveQrcode = new System.Windows.Forms.Button();
            this.btnCopyShareLink = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tboxShareLink = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbServerTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pboxQrcode)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // tboxServerName
            // 
            resources.ApplyResources(this.tboxServerName, "tboxServerName");
            this.tboxServerName.Name = "tboxServerName";
            this.toolTip1.SetToolTip(this.tboxServerName, resources.GetString("tboxServerName.ToolTip"));
            // 
            // cboxInboundAddress
            // 
            resources.ApplyResources(this.cboxInboundAddress, "cboxInboundAddress");
            this.cboxInboundAddress.FormattingEnabled = true;
            this.cboxInboundAddress.Items.AddRange(new object[] {
            resources.GetString("cboxInboundAddress.Items"),
            resources.GetString("cboxInboundAddress.Items1"),
            resources.GetString("cboxInboundAddress.Items2"),
            resources.GetString("cboxInboundAddress.Items3")});
            this.cboxInboundAddress.Name = "cboxInboundAddress";
            this.toolTip1.SetToolTip(this.cboxInboundAddress, resources.GetString("cboxInboundAddress.ToolTip"));
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
            resources.ApplyResources(this.cboxInboundMode, "cboxInboundMode");
            this.cboxInboundMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxInboundMode.FormattingEnabled = true;
            this.cboxInboundMode.Items.AddRange(new object[] {
            resources.GetString("cboxInboundMode.Items"),
            resources.GetString("cboxInboundMode.Items1"),
            resources.GetString("cboxInboundMode.Items2"),
            resources.GetString("cboxInboundMode.Items3")});
            this.cboxInboundMode.Name = "cboxInboundMode";
            this.toolTip1.SetToolTip(this.cboxInboundMode, resources.GetString("cboxInboundMode.ToolTip"));
            this.cboxInboundMode.SelectedIndexChanged += new System.EventHandler(this.cboxInboundMode_SelectedIndexChanged);
            // 
            // cboxMark
            // 
            resources.ApplyResources(this.cboxMark, "cboxMark");
            this.cboxMark.FormattingEnabled = true;
            this.cboxMark.Name = "cboxMark";
            this.toolTip1.SetToolTip(this.cboxMark, resources.GetString("cboxMark.ToolTip"));
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
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // chkOutbOTA
            // 
            resources.ApplyResources(this.chkOutbOTA, "chkOutbOTA");
            this.chkOutbOTA.Name = "chkOutbOTA";
            this.toolTip1.SetToolTip(this.chkOutbOTA, resources.GetString("chkOutbOTA.ToolTip"));
            this.chkOutbOTA.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // btnOpenInEditor
            // 
            resources.ApplyResources(this.btnOpenInEditor, "btnOpenInEditor");
            this.btnOpenInEditor.Name = "btnOpenInEditor";
            this.toolTip1.SetToolTip(this.btnOpenInEditor, resources.GetString("btnOpenInEditor.ToolTip"));
            this.btnOpenInEditor.UseVisualStyleBackColor = true;
            this.btnOpenInEditor.Click += new System.EventHandler(this.btnOpenInEditor_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // tboxDescription
            // 
            resources.ApplyResources(this.tboxDescription, "tboxDescription");
            this.tboxDescription.Name = "tboxDescription";
            this.toolTip1.SetToolTip(this.tboxDescription, resources.GetString("tboxDescription.ToolTip"));
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.toolTip1.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // tboxRemark
            // 
            resources.ApplyResources(this.tboxRemark, "tboxRemark");
            this.tboxRemark.Name = "tboxRemark";
            this.toolTip1.SetToolTip(this.tboxRemark, resources.GetString("tboxRemark.ToolTip"));
            // 
            // pboxQrcode
            // 
            resources.ApplyResources(this.pboxQrcode, "pboxQrcode");
            this.pboxQrcode.BackColor = System.Drawing.Color.White;
            this.pboxQrcode.Name = "pboxQrcode";
            this.pboxQrcode.TabStop = false;
            this.toolTip1.SetToolTip(this.pboxQrcode, resources.GetString("pboxQrcode.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.cboxInboundAddress);
            this.groupBox1.Controls.Add(this.cboxInboundMode);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.chkGlobalImport);
            this.groupBox1.Controls.Add(this.chkBypassCnSite);
            this.groupBox1.Controls.Add(this.tboxServerName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.chkUntrack);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkAutoRun);
            this.groupBox1.Controls.Add(this.tboxRemark);
            this.groupBox1.Controls.Add(this.tboxDescription);
            this.groupBox1.Controls.Add(this.cboxMark);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // cboxZoomMode
            // 
            resources.ApplyResources(this.cboxZoomMode, "cboxZoomMode");
            this.cboxZoomMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxZoomMode.FormattingEnabled = true;
            this.cboxZoomMode.Items.AddRange(new object[] {
            resources.GetString("cboxZoomMode.Items"),
            resources.GetString("cboxZoomMode.Items1")});
            this.cboxZoomMode.Name = "cboxZoomMode";
            this.toolTip1.SetToolTip(this.cboxZoomMode, resources.GetString("cboxZoomMode.ToolTip"));
            this.cboxZoomMode.SelectedValueChanged += new System.EventHandler(this.cboxZoomMode_SelectedValueChanged);
            // 
            // cboxShareLinkType
            // 
            resources.ApplyResources(this.cboxShareLinkType, "cboxShareLinkType");
            this.cboxShareLinkType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxShareLinkType.FormattingEnabled = true;
            this.cboxShareLinkType.Items.AddRange(new object[] {
            resources.GetString("cboxShareLinkType.Items"),
            resources.GetString("cboxShareLinkType.Items1")});
            this.cboxShareLinkType.Name = "cboxShareLinkType";
            this.toolTip1.SetToolTip(this.cboxShareLinkType, resources.GetString("cboxShareLinkType.ToolTip"));
            this.cboxShareLinkType.SelectedValueChanged += new System.EventHandler(this.cboxShareLinkType_SelectedValueChanged);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.chkOutbOTA);
            this.groupBox3.Controls.Add(this.chkOutbStreamUseTls);
            this.groupBox3.Controls.Add(this.cboxOutbMethod);
            this.groupBox3.Controls.Add(this.cboxOutbStreamParma);
            this.groupBox3.Controls.Add(this.cboxOutbProto);
            this.groupBox3.Controls.Add(this.cboxOutbStreamType);
            this.groupBox3.Controls.Add(this.tboxOutbAddr);
            this.groupBox3.Controls.Add(this.tboxOutbAuth2);
            this.groupBox3.Controls.Add(this.tboxOutbAuth1);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.lbOutbAuth2);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.lbOutbAuth1);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // chkOutbStreamUseTls
            // 
            resources.ApplyResources(this.chkOutbStreamUseTls, "chkOutbStreamUseTls");
            this.chkOutbStreamUseTls.Name = "chkOutbStreamUseTls";
            this.toolTip1.SetToolTip(this.chkOutbStreamUseTls, resources.GetString("chkOutbStreamUseTls.ToolTip"));
            this.chkOutbStreamUseTls.UseVisualStyleBackColor = true;
            // 
            // cboxOutbMethod
            // 
            resources.ApplyResources(this.cboxOutbMethod, "cboxOutbMethod");
            this.cboxOutbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxOutbMethod.FormattingEnabled = true;
            this.cboxOutbMethod.Name = "cboxOutbMethod";
            this.toolTip1.SetToolTip(this.cboxOutbMethod, resources.GetString("cboxOutbMethod.ToolTip"));
            // 
            // cboxOutbStreamParma
            // 
            resources.ApplyResources(this.cboxOutbStreamParma, "cboxOutbStreamParma");
            this.cboxOutbStreamParma.FormattingEnabled = true;
            this.cboxOutbStreamParma.Name = "cboxOutbStreamParma";
            this.toolTip1.SetToolTip(this.cboxOutbStreamParma, resources.GetString("cboxOutbStreamParma.ToolTip"));
            // 
            // cboxOutbProto
            // 
            resources.ApplyResources(this.cboxOutbProto, "cboxOutbProto");
            this.cboxOutbProto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxOutbProto.FormattingEnabled = true;
            this.cboxOutbProto.Items.AddRange(new object[] {
            resources.GetString("cboxOutbProto.Items"),
            resources.GetString("cboxOutbProto.Items1"),
            resources.GetString("cboxOutbProto.Items2"),
            resources.GetString("cboxOutbProto.Items3"),
            resources.GetString("cboxOutbProto.Items4")});
            this.cboxOutbProto.Name = "cboxOutbProto";
            this.toolTip1.SetToolTip(this.cboxOutbProto, resources.GetString("cboxOutbProto.ToolTip"));
            this.cboxOutbProto.SelectedValueChanged += new System.EventHandler(this.cboxOutbProto_SelectedValueChanged);
            // 
            // cboxOutbStreamType
            // 
            resources.ApplyResources(this.cboxOutbStreamType, "cboxOutbStreamType");
            this.cboxOutbStreamType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxOutbStreamType.FormattingEnabled = true;
            this.cboxOutbStreamType.Name = "cboxOutbStreamType";
            this.toolTip1.SetToolTip(this.cboxOutbStreamType, resources.GetString("cboxOutbStreamType.ToolTip"));
            this.cboxOutbStreamType.SelectedIndexChanged += new System.EventHandler(this.cboxOutbStreamType_SelectedIndexChanged);
            // 
            // tboxOutbAddr
            // 
            resources.ApplyResources(this.tboxOutbAddr, "tboxOutbAddr");
            this.tboxOutbAddr.Name = "tboxOutbAddr";
            this.toolTip1.SetToolTip(this.tboxOutbAddr, resources.GetString("tboxOutbAddr.ToolTip"));
            this.tboxOutbAddr.TextChanged += new System.EventHandler(this.tboxOutbAddr_TextChanged);
            // 
            // tboxOutbAuth2
            // 
            resources.ApplyResources(this.tboxOutbAuth2, "tboxOutbAuth2");
            this.tboxOutbAuth2.Name = "tboxOutbAuth2";
            this.toolTip1.SetToolTip(this.tboxOutbAuth2, resources.GetString("tboxOutbAuth2.ToolTip"));
            // 
            // tboxOutbAuth1
            // 
            resources.ApplyResources(this.tboxOutbAuth1, "tboxOutbAuth1");
            this.tboxOutbAuth1.Name = "tboxOutbAuth1";
            this.toolTip1.SetToolTip(this.tboxOutbAuth1, resources.GetString("tboxOutbAuth1.ToolTip"));
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            this.toolTip1.SetToolTip(this.label16, resources.GetString("label16.ToolTip"));
            // 
            // lbOutbAuth2
            // 
            resources.ApplyResources(this.lbOutbAuth2, "lbOutbAuth2");
            this.lbOutbAuth2.Name = "lbOutbAuth2";
            this.toolTip1.SetToolTip(this.lbOutbAuth2, resources.GetString("lbOutbAuth2.ToolTip"));
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            this.toolTip1.SetToolTip(this.label14, resources.GetString("label14.ToolTip"));
            // 
            // lbOutbAuth1
            // 
            resources.ApplyResources(this.lbOutbAuth1, "lbOutbAuth1");
            this.lbOutbAuth1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbOutbAuth1.Name = "lbOutbAuth1";
            this.toolTip1.SetToolTip(this.lbOutbAuth1, resources.GetString("lbOutbAuth1.ToolTip"));
            this.lbOutbAuth1.Click += new System.EventHandler(this.lbOutbAuth1_Click);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            this.toolTip1.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.toolTip1.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.btnSaveQrcode);
            this.groupBox4.Controls.Add(this.btnCopyShareLink);
            this.groupBox4.Controls.Add(this.cboxShareLinkType);
            this.groupBox4.Controls.Add(this.cboxZoomMode);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.pboxQrcode);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.tboxShareLink);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox4, resources.GetString("groupBox4.ToolTip"));
            // 
            // btnSaveQrcode
            // 
            resources.ApplyResources(this.btnSaveQrcode, "btnSaveQrcode");
            this.btnSaveQrcode.Name = "btnSaveQrcode";
            this.toolTip1.SetToolTip(this.btnSaveQrcode, resources.GetString("btnSaveQrcode.ToolTip"));
            this.btnSaveQrcode.UseVisualStyleBackColor = true;
            this.btnSaveQrcode.Click += new System.EventHandler(this.btnSaveQrcode_Click);
            // 
            // btnCopyShareLink
            // 
            resources.ApplyResources(this.btnCopyShareLink, "btnCopyShareLink");
            this.btnCopyShareLink.Name = "btnCopyShareLink";
            this.toolTip1.SetToolTip(this.btnCopyShareLink, resources.GetString("btnCopyShareLink.ToolTip"));
            this.btnCopyShareLink.UseVisualStyleBackColor = true;
            this.btnCopyShareLink.Click += new System.EventHandler(this.btnCopyShareLink_Click);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // tboxShareLink
            // 
            resources.ApplyResources(this.tboxShareLink, "tboxShareLink");
            this.tboxShareLink.Name = "tboxShareLink";
            this.toolTip1.SetToolTip(this.tboxShareLink, resources.GetString("tboxShareLink.ToolTip"));
            this.tboxShareLink.TextChanged += new System.EventHandler(this.tboxShareLink_TextChanged);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.toolTip1.SetToolTip(this.btnOK, resources.GetString("btnOK.ToolTip"));
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.toolTip1.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbServerTitle
            // 
            resources.ApplyResources(this.lbServerTitle, "lbServerTitle");
            this.lbServerTitle.Name = "lbServerTitle";
            this.toolTip1.SetToolTip(this.lbServerTitle, resources.GetString("lbServerTitle.ToolTip"));
            // 
            // FormModifyServerSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOpenInEditor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lbServerTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormModifyServerSettings";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.FormModifyServerSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pboxQrcode)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tboxRemark;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pboxQrcode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboxShareLinkType;
        private System.Windows.Forms.ComboBox cboxZoomMode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkOutbOTA;
        private System.Windows.Forms.CheckBox chkOutbStreamUseTls;
        private System.Windows.Forms.ComboBox cboxOutbMethod;
        private System.Windows.Forms.ComboBox cboxOutbStreamParma;
        private System.Windows.Forms.ComboBox cboxOutbProto;
        private System.Windows.Forms.ComboBox cboxOutbStreamType;
        private System.Windows.Forms.TextBox tboxOutbAddr;
        private System.Windows.Forms.TextBox tboxOutbAuth1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lbOutbAuth2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lbOutbAuth1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSaveQrcode;
        private System.Windows.Forms.Button btnCopyShareLink;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tboxShareLink;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbServerTitle;
        private System.Windows.Forms.TextBox tboxOutbAuth2;
        private System.Windows.Forms.Button btnOpenInEditor;
    }
}
