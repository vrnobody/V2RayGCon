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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.tboxDescription = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
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
            resources.GetString("cboxInboundMode.Items2")});
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
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            // FormModifyServerSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkBypassCnSite);
            this.Controls.Add(this.chkUntrack);
            this.Controls.Add(this.chkGlobalImport);
            this.Controls.Add(this.chkAutoRun);
            this.Controls.Add(this.cboxInboundMode);
            this.Controls.Add(this.cboxMark);
            this.Controls.Add(this.cboxInboundAddress);
            this.Controls.Add(this.tboxDescription);
            this.Controls.Add(this.tboxServerName);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormModifyServerSettings";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.FormModifyServerSettings_Load);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tboxDescription;
        private System.Windows.Forms.Label label10;
    }
}