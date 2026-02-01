namespace V2RayGCon.Views.UserControls
{
    partial class SimpleConfigerUI
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleConfigerUI));
            this.cboxAuth2 = new System.Windows.Forms.ComboBox();
            this.cboxStreamParma1 = new System.Windows.Forms.ComboBox();
            this.cboxProtocol = new System.Windows.Forms.ComboBox();
            this.cboxStreamType = new System.Windows.Forms.ComboBox();
            this.tboxHost = new System.Windows.Forms.TextBox();
            this.tboxAuth1 = new System.Windows.Forms.TextBox();
            this.lbStreamParam1 = new System.Windows.Forms.Label();
            this.lbAuth2 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lbAuth1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tboxName = new System.Windows.Forms.TextBox();
            this.lbName = new System.Windows.Forms.Label();
            this.chkTlsCertSelfSign = new System.Windows.Forms.CheckBox();
            this.lbStreamParam3 = new System.Windows.Forms.Label();
            this.lbStreamParam2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbTlsParam1 = new System.Windows.Forms.Label();
            this.lbTlsParam2 = new System.Windows.Forms.Label();
            this.lbTlsParam3 = new System.Windows.Forms.Label();
            this.lbTlsParam4 = new System.Windows.Forms.Label();
            this.tboxStreamParam2 = new System.Windows.Forms.TextBox();
            this.tboxStreamParam3 = new System.Windows.Forms.TextBox();
            this.tboxPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboxTlsType = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbAuth3 = new System.Windows.Forms.Label();
            this.tboxAuth3 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tboxTlsParam2 = new System.Windows.Forms.TextBox();
            this.cboxTlsFingerprint = new System.Windows.Forms.ComboBox();
            this.tboxTlsParam4 = new System.Windows.Forms.TextBox();
            this.tboxTlsParam1 = new System.Windows.Forms.TextBox();
            this.tboxTlsAlpn = new System.Windows.Forms.TextBox();
            this.tboxTlsServName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tboxTlsParam3 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboxAuth2
            // 
            resources.ApplyResources(this.cboxAuth2, "cboxAuth2");
            this.cboxAuth2.FormattingEnabled = true;
            this.cboxAuth2.Name = "cboxAuth2";
            // 
            // cboxStreamParma1
            // 
            resources.ApplyResources(this.cboxStreamParma1, "cboxStreamParma1");
            this.cboxStreamParma1.FormattingEnabled = true;
            this.cboxStreamParma1.Name = "cboxStreamParma1";
            this.cboxStreamParma1.SelectedValueChanged += new System.EventHandler(this.cboxStreamParma1_SelectedValueChanged);
            // 
            // cboxProtocol
            // 
            resources.ApplyResources(this.cboxProtocol, "cboxProtocol");
            this.cboxProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxProtocol.FormattingEnabled = true;
            this.cboxProtocol.Items.AddRange(new object[] {
            resources.GetString("cboxProtocol.Items"),
            resources.GetString("cboxProtocol.Items1"),
            resources.GetString("cboxProtocol.Items2"),
            resources.GetString("cboxProtocol.Items3"),
            resources.GetString("cboxProtocol.Items4"),
            resources.GetString("cboxProtocol.Items5")});
            this.cboxProtocol.Name = "cboxProtocol";
            this.cboxProtocol.SelectedValueChanged += new System.EventHandler(this.cboxProtocol_SelectedValueChanged);
            // 
            // cboxStreamType
            // 
            resources.ApplyResources(this.cboxStreamType, "cboxStreamType");
            this.cboxStreamType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxStreamType.FormattingEnabled = true;
            this.cboxStreamType.Name = "cboxStreamType";
            this.cboxStreamType.SelectedValueChanged += new System.EventHandler(this.cboxStreamType_SelectedValueChanged);
            // 
            // tboxHost
            // 
            resources.ApplyResources(this.tboxHost, "tboxHost");
            this.tboxHost.Name = "tboxHost";
            // 
            // tboxAuth1
            // 
            resources.ApplyResources(this.tboxAuth1, "tboxAuth1");
            this.tboxAuth1.Name = "tboxAuth1";
            this.tboxAuth1.TextChanged += new System.EventHandler(this.tboxAuth1_TextChanged);
            // 
            // lbStreamParam1
            // 
            resources.ApplyResources(this.lbStreamParam1, "lbStreamParam1");
            this.lbStreamParam1.Name = "lbStreamParam1";
            this.toolTip1.SetToolTip(this.lbStreamParam1, resources.GetString("lbStreamParam1.ToolTip"));
            // 
            // lbAuth2
            // 
            resources.ApplyResources(this.lbAuth2, "lbAuth2");
            this.lbAuth2.Name = "lbAuth2";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // lbAuth1
            // 
            resources.ApplyResources(this.lbAuth1, "lbAuth1");
            this.lbAuth1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbAuth1.Name = "lbAuth1";
            this.lbAuth1.Click += new System.EventHandler(this.lbAuth1_Click);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // tboxName
            // 
            resources.ApplyResources(this.tboxName, "tboxName");
            this.tboxName.Name = "tboxName";
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.AutoEllipsis = true;
            this.lbName.Name = "lbName";
            // 
            // chkTlsCertSelfSign
            // 
            resources.ApplyResources(this.chkTlsCertSelfSign, "chkTlsCertSelfSign");
            this.chkTlsCertSelfSign.Name = "chkTlsCertSelfSign";
            this.toolTip1.SetToolTip(this.chkTlsCertSelfSign, resources.GetString("chkTlsCertSelfSign.ToolTip"));
            this.chkTlsCertSelfSign.UseVisualStyleBackColor = true;
            // 
            // lbStreamParam3
            // 
            resources.ApplyResources(this.lbStreamParam3, "lbStreamParam3");
            this.lbStreamParam3.Name = "lbStreamParam3";
            // 
            // lbStreamParam2
            // 
            resources.ApplyResources(this.lbStreamParam2, "lbStreamParam2");
            this.lbStreamParam2.Name = "lbStreamParam2";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
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
            // lbTlsParam1
            // 
            resources.ApplyResources(this.lbTlsParam1, "lbTlsParam1");
            this.lbTlsParam1.Name = "lbTlsParam1";
            this.toolTip1.SetToolTip(this.lbTlsParam1, resources.GetString("lbTlsParam1.ToolTip"));
            // 
            // lbTlsParam2
            // 
            resources.ApplyResources(this.lbTlsParam2, "lbTlsParam2");
            this.lbTlsParam2.Name = "lbTlsParam2";
            this.toolTip1.SetToolTip(this.lbTlsParam2, resources.GetString("lbTlsParam2.ToolTip"));
            // 
            // lbTlsParam3
            // 
            resources.ApplyResources(this.lbTlsParam3, "lbTlsParam3");
            this.lbTlsParam3.Name = "lbTlsParam3";
            this.toolTip1.SetToolTip(this.lbTlsParam3, resources.GetString("lbTlsParam3.ToolTip"));
            // 
            // lbTlsParam4
            // 
            resources.ApplyResources(this.lbTlsParam4, "lbTlsParam4");
            this.lbTlsParam4.Name = "lbTlsParam4";
            this.toolTip1.SetToolTip(this.lbTlsParam4, resources.GetString("lbTlsParam4.ToolTip"));
            // 
            // tboxStreamParam2
            // 
            resources.ApplyResources(this.tboxStreamParam2, "tboxStreamParam2");
            this.tboxStreamParam2.Name = "tboxStreamParam2";
            // 
            // tboxStreamParam3
            // 
            resources.ApplyResources(this.tboxStreamParam3, "tboxStreamParam3");
            this.tboxStreamParam3.Name = "tboxStreamParam3";
            // 
            // tboxPort
            // 
            resources.ApplyResources(this.tboxPort, "tboxPort");
            this.tboxPort.Name = "tboxPort";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.cboxStreamType);
            this.groupBox1.Controls.Add(this.lbStreamParam1);
            this.groupBox1.Controls.Add(this.lbStreamParam3);
            this.groupBox1.Controls.Add(this.lbStreamParam2);
            this.groupBox1.Controls.Add(this.tboxStreamParam2);
            this.groupBox1.Controls.Add(this.tboxStreamParam3);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.cboxStreamParma1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cboxTlsType
            // 
            resources.ApplyResources(this.cboxTlsType, "cboxTlsType");
            this.cboxTlsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxTlsType.FormattingEnabled = true;
            this.cboxTlsType.Items.AddRange(new object[] {
            resources.GetString("cboxTlsType.Items"),
            resources.GetString("cboxTlsType.Items1"),
            resources.GetString("cboxTlsType.Items2"),
            resources.GetString("cboxTlsType.Items3")});
            this.cboxTlsType.Name = "cboxTlsType";
            this.cboxTlsType.SelectedValueChanged += new System.EventHandler(this.cboxTlsType_SelectedValueChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.cboxAuth2);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.lbAuth3);
            this.groupBox2.Controls.Add(this.lbAuth2);
            this.groupBox2.Controls.Add(this.lbAuth1);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.tboxPort);
            this.groupBox2.Controls.Add(this.cboxProtocol);
            this.groupBox2.Controls.Add(this.tboxAuth3);
            this.groupBox2.Controls.Add(this.tboxAuth1);
            this.groupBox2.Controls.Add(this.tboxHost);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // lbAuth3
            // 
            resources.ApplyResources(this.lbAuth3, "lbAuth3");
            this.lbAuth3.Name = "lbAuth3";
            // 
            // tboxAuth3
            // 
            resources.ApplyResources(this.tboxAuth3, "tboxAuth3");
            this.tboxAuth3.Name = "tboxAuth3";
            this.tboxAuth3.TextChanged += new System.EventHandler(this.tboxAuth1_TextChanged);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.tboxTlsParam2);
            this.groupBox3.Controls.Add(this.cboxTlsFingerprint);
            this.groupBox3.Controls.Add(this.tboxTlsParam4);
            this.groupBox3.Controls.Add(this.tboxTlsParam1);
            this.groupBox3.Controls.Add(this.lbTlsParam2);
            this.groupBox3.Controls.Add(this.lbTlsParam4);
            this.groupBox3.Controls.Add(this.tboxTlsAlpn);
            this.groupBox3.Controls.Add(this.lbTlsParam1);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.tboxTlsServName);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.cboxTlsType);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.chkTlsCertSelfSign);
            this.groupBox3.Controls.Add(this.tboxTlsParam3);
            this.groupBox3.Controls.Add(this.lbTlsParam3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // tboxTlsParam2
            // 
            resources.ApplyResources(this.tboxTlsParam2, "tboxTlsParam2");
            this.tboxTlsParam2.Name = "tboxTlsParam2";
            // 
            // cboxTlsFingerprint
            // 
            resources.ApplyResources(this.cboxTlsFingerprint, "cboxTlsFingerprint");
            this.cboxTlsFingerprint.FormattingEnabled = true;
            this.cboxTlsFingerprint.Items.AddRange(new object[] {
            resources.GetString("cboxTlsFingerprint.Items"),
            resources.GetString("cboxTlsFingerprint.Items1"),
            resources.GetString("cboxTlsFingerprint.Items2"),
            resources.GetString("cboxTlsFingerprint.Items3"),
            resources.GetString("cboxTlsFingerprint.Items4"),
            resources.GetString("cboxTlsFingerprint.Items5"),
            resources.GetString("cboxTlsFingerprint.Items6"),
            resources.GetString("cboxTlsFingerprint.Items7"),
            resources.GetString("cboxTlsFingerprint.Items8"),
            resources.GetString("cboxTlsFingerprint.Items9"),
            resources.GetString("cboxTlsFingerprint.Items10")});
            this.cboxTlsFingerprint.Name = "cboxTlsFingerprint";
            // 
            // tboxTlsParam4
            // 
            resources.ApplyResources(this.tboxTlsParam4, "tboxTlsParam4");
            this.tboxTlsParam4.Name = "tboxTlsParam4";
            // 
            // tboxTlsParam1
            // 
            resources.ApplyResources(this.tboxTlsParam1, "tboxTlsParam1");
            this.tboxTlsParam1.Name = "tboxTlsParam1";
            // 
            // tboxTlsAlpn
            // 
            resources.ApplyResources(this.tboxTlsAlpn, "tboxTlsAlpn");
            this.tboxTlsAlpn.Name = "tboxTlsAlpn";
            // 
            // tboxTlsServName
            // 
            resources.ApplyResources(this.tboxTlsServName, "tboxTlsServName");
            this.tboxTlsServName.Name = "tboxTlsServName";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tboxTlsParam3
            // 
            resources.ApplyResources(this.tboxTlsParam3, "tboxTlsParam3");
            this.tboxTlsParam3.Name = "tboxTlsParam3";
            // 
            // SimpleConfigerUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tboxName);
            this.Controls.Add(this.lbName);
            this.Name = "SimpleConfigerUI";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cboxAuth2;
        private System.Windows.Forms.ComboBox cboxStreamParma1;
        private System.Windows.Forms.ComboBox cboxProtocol;
        private System.Windows.Forms.ComboBox cboxStreamType;
        private System.Windows.Forms.TextBox tboxHost;
        private System.Windows.Forms.TextBox tboxAuth1;
        private System.Windows.Forms.Label lbStreamParam1;
        private System.Windows.Forms.Label lbAuth2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lbAuth1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tboxName;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.CheckBox chkTlsCertSelfSign;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lbStreamParam3;
        private System.Windows.Forms.Label lbStreamParam2;
        private System.Windows.Forms.TextBox tboxStreamParam2;
        private System.Windows.Forms.TextBox tboxStreamParam3;
        private System.Windows.Forms.TextBox tboxPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboxTlsType;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tboxTlsServName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboxTlsFingerprint;
        private System.Windows.Forms.TextBox tboxTlsParam3;
        private System.Windows.Forms.TextBox tboxTlsParam2;
        private System.Windows.Forms.Label lbTlsParam3;
        private System.Windows.Forms.TextBox tboxTlsParam1;
        private System.Windows.Forms.Label lbTlsParam2;
        private System.Windows.Forms.TextBox tboxTlsAlpn;
        private System.Windows.Forms.Label lbTlsParam1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tboxTlsParam4;
        private System.Windows.Forms.Label lbTlsParam4;
        private System.Windows.Forms.Label lbAuth3;
        private System.Windows.Forms.TextBox tboxAuth3;
    }
}
