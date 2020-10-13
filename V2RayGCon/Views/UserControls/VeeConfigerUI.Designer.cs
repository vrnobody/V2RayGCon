namespace V2RayGCon.Views.UserControls
{
    partial class VeeConfigerUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VeeConfigerUI));
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
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tboxDescription = new System.Windows.Forms.TextBox();
            this.chkTlsCertSelfSign = new System.Windows.Forms.CheckBox();
            this.lbStreamParam3 = new System.Windows.Forms.Label();
            this.lbStreamParam2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.tboxStreamParam2 = new System.Windows.Forms.TextBox();
            this.tboxStreamParam3 = new System.Windows.Forms.TextBox();
            this.tboxPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboxTlsType = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tboxTlsServName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            resources.GetString("cboxTlsType.Items2")});
            this.cboxTlsType.Name = "cboxTlsType";
            this.cboxTlsType.SelectedValueChanged += new System.EventHandler(this.cboxTlsType_SelectedValueChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.cboxAuth2);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.lbAuth2);
            this.groupBox2.Controls.Add(this.lbAuth1);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.tboxPort);
            this.groupBox2.Controls.Add(this.cboxProtocol);
            this.groupBox2.Controls.Add(this.tboxAuth1);
            this.groupBox2.Controls.Add(this.tboxHost);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.tboxTlsServName);
            this.groupBox3.Controls.Add(this.cboxTlsType);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.chkTlsCertSelfSign);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
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
            // VeeConfigerUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tboxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tboxDescription);
            this.Name = "VeeConfigerUI";
            this.Load += new System.EventHandler(this.VeeImporter_Load);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tboxDescription;
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
    }
}
