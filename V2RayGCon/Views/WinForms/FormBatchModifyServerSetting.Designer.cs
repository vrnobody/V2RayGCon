namespace V2RayGCon.Views.WinForms
{
    partial class FormBatchModifyServerSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBatchModifyServerSetting));
            this.chkInIP = new System.Windows.Forms.CheckBox();
            this.tboxInPort = new System.Windows.Forms.TextBox();
            this.chkInPort = new System.Windows.Forms.CheckBox();
            this.tboxInIP = new System.Windows.Forms.TextBox();
            this.chkMark = new System.Windows.Forms.CheckBox();
            this.chkInMode = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkIncrement = new System.Windows.Forms.CheckBox();
            this.chkShareOverLAN = new System.Windows.Forms.CheckBox();
            this.cboxInMode = new System.Windows.Forms.ComboBox();
            this.cboxMark = new System.Windows.Forms.ComboBox();
            this.btnModify = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboxIsInjectSkipCNSite = new System.Windows.Forms.ComboBox();
            this.chkIsInjectSkipCNSite = new System.Windows.Forms.CheckBox();
            this.cboxImport = new System.Windows.Forms.ComboBox();
            this.cboxAutorun = new System.Windows.Forms.ComboBox();
            this.chkImport = new System.Windows.Forms.CheckBox();
            this.chkAutorun = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkInIP
            // 
            resources.ApplyResources(this.chkInIP, "chkInIP");
            this.chkInIP.Name = "chkInIP";
            this.toolTip1.SetToolTip(this.chkInIP, resources.GetString("chkInIP.ToolTip"));
            this.chkInIP.UseVisualStyleBackColor = true;
            // 
            // tboxInPort
            // 
            resources.ApplyResources(this.tboxInPort, "tboxInPort");
            this.tboxInPort.Name = "tboxInPort";
            this.toolTip1.SetToolTip(this.tboxInPort, resources.GetString("tboxInPort.ToolTip"));
            // 
            // chkInPort
            // 
            resources.ApplyResources(this.chkInPort, "chkInPort");
            this.chkInPort.Name = "chkInPort";
            this.toolTip1.SetToolTip(this.chkInPort, resources.GetString("chkInPort.ToolTip"));
            this.chkInPort.UseVisualStyleBackColor = true;
            // 
            // tboxInIP
            // 
            resources.ApplyResources(this.tboxInIP, "tboxInIP");
            this.tboxInIP.Name = "tboxInIP";
            this.toolTip1.SetToolTip(this.tboxInIP, resources.GetString("tboxInIP.ToolTip"));
            // 
            // chkMark
            // 
            resources.ApplyResources(this.chkMark, "chkMark");
            this.chkMark.Name = "chkMark";
            this.toolTip1.SetToolTip(this.chkMark, resources.GetString("chkMark.ToolTip"));
            this.chkMark.UseVisualStyleBackColor = true;
            // 
            // chkInMode
            // 
            resources.ApplyResources(this.chkInMode, "chkInMode");
            this.chkInMode.Name = "chkInMode";
            this.toolTip1.SetToolTip(this.chkInMode, resources.GetString("chkInMode.ToolTip"));
            this.chkInMode.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.chkIncrement);
            this.groupBox1.Controls.Add(this.chkShareOverLAN);
            this.groupBox1.Controls.Add(this.cboxInMode);
            this.groupBox1.Controls.Add(this.chkInMode);
            this.groupBox1.Controls.Add(this.tboxInPort);
            this.groupBox1.Controls.Add(this.tboxInIP);
            this.groupBox1.Controls.Add(this.chkInPort);
            this.groupBox1.Controls.Add(this.chkInIP);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // chkIncrement
            // 
            resources.ApplyResources(this.chkIncrement, "chkIncrement");
            this.chkIncrement.Name = "chkIncrement";
            this.toolTip1.SetToolTip(this.chkIncrement, resources.GetString("chkIncrement.ToolTip"));
            this.chkIncrement.UseVisualStyleBackColor = true;
            // 
            // chkShareOverLAN
            // 
            resources.ApplyResources(this.chkShareOverLAN, "chkShareOverLAN");
            this.chkShareOverLAN.Name = "chkShareOverLAN";
            this.toolTip1.SetToolTip(this.chkShareOverLAN, resources.GetString("chkShareOverLAN.ToolTip"));
            this.chkShareOverLAN.UseVisualStyleBackColor = true;
            this.chkShareOverLAN.CheckedChanged += new System.EventHandler(this.chkShareOverLAN_CheckedChanged);
            // 
            // cboxInMode
            // 
            resources.ApplyResources(this.cboxInMode, "cboxInMode");
            this.cboxInMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxInMode.FormattingEnabled = true;
            this.cboxInMode.Items.AddRange(new object[] {
            resources.GetString("cboxInMode.Items"),
            resources.GetString("cboxInMode.Items1"),
            resources.GetString("cboxInMode.Items2")});
            this.cboxInMode.Name = "cboxInMode";
            this.toolTip1.SetToolTip(this.cboxInMode, resources.GetString("cboxInMode.ToolTip"));
            // 
            // cboxMark
            // 
            resources.ApplyResources(this.cboxMark, "cboxMark");
            this.cboxMark.FormattingEnabled = true;
            this.cboxMark.Name = "cboxMark";
            this.toolTip1.SetToolTip(this.cboxMark, resources.GetString("cboxMark.ToolTip"));
            // 
            // btnModify
            // 
            resources.ApplyResources(this.btnModify, "btnModify");
            this.btnModify.Name = "btnModify";
            this.toolTip1.SetToolTip(this.btnModify, resources.GetString("btnModify.ToolTip"));
            this.btnModify.UseVisualStyleBackColor = true;
            this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.toolTip1.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.cboxIsInjectSkipCNSite);
            this.groupBox2.Controls.Add(this.chkIsInjectSkipCNSite);
            this.groupBox2.Controls.Add(this.cboxImport);
            this.groupBox2.Controls.Add(this.cboxAutorun);
            this.groupBox2.Controls.Add(this.chkImport);
            this.groupBox2.Controls.Add(this.chkAutorun);
            this.groupBox2.Controls.Add(this.chkMark);
            this.groupBox2.Controls.Add(this.cboxMark);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // cboxIsInjectSkipCNSite
            // 
            resources.ApplyResources(this.cboxIsInjectSkipCNSite, "cboxIsInjectSkipCNSite");
            this.cboxIsInjectSkipCNSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxIsInjectSkipCNSite.FormattingEnabled = true;
            this.cboxIsInjectSkipCNSite.Items.AddRange(new object[] {
            resources.GetString("cboxIsInjectSkipCNSite.Items"),
            resources.GetString("cboxIsInjectSkipCNSite.Items1")});
            this.cboxIsInjectSkipCNSite.Name = "cboxIsInjectSkipCNSite";
            this.toolTip1.SetToolTip(this.cboxIsInjectSkipCNSite, resources.GetString("cboxIsInjectSkipCNSite.ToolTip"));
            // 
            // chkIsInjectSkipCNSite
            // 
            resources.ApplyResources(this.chkIsInjectSkipCNSite, "chkIsInjectSkipCNSite");
            this.chkIsInjectSkipCNSite.Name = "chkIsInjectSkipCNSite";
            this.toolTip1.SetToolTip(this.chkIsInjectSkipCNSite, resources.GetString("chkIsInjectSkipCNSite.ToolTip"));
            this.chkIsInjectSkipCNSite.UseVisualStyleBackColor = true;
            // 
            // cboxImport
            // 
            resources.ApplyResources(this.cboxImport, "cboxImport");
            this.cboxImport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxImport.FormattingEnabled = true;
            this.cboxImport.Items.AddRange(new object[] {
            resources.GetString("cboxImport.Items"),
            resources.GetString("cboxImport.Items1")});
            this.cboxImport.Name = "cboxImport";
            this.toolTip1.SetToolTip(this.cboxImport, resources.GetString("cboxImport.ToolTip"));
            // 
            // cboxAutorun
            // 
            resources.ApplyResources(this.cboxAutorun, "cboxAutorun");
            this.cboxAutorun.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxAutorun.FormattingEnabled = true;
            this.cboxAutorun.Items.AddRange(new object[] {
            resources.GetString("cboxAutorun.Items"),
            resources.GetString("cboxAutorun.Items1")});
            this.cboxAutorun.Name = "cboxAutorun";
            this.toolTip1.SetToolTip(this.cboxAutorun, resources.GetString("cboxAutorun.ToolTip"));
            // 
            // chkImport
            // 
            resources.ApplyResources(this.chkImport, "chkImport");
            this.chkImport.Name = "chkImport";
            this.toolTip1.SetToolTip(this.chkImport, resources.GetString("chkImport.ToolTip"));
            this.chkImport.UseVisualStyleBackColor = true;
            // 
            // chkAutorun
            // 
            resources.ApplyResources(this.chkAutorun, "chkAutorun");
            this.chkAutorun.Name = "chkAutorun";
            this.toolTip1.SetToolTip(this.chkAutorun, resources.GetString("chkAutorun.ToolTip"));
            this.chkAutorun.UseVisualStyleBackColor = true;
            // 
            // FormBatchModifyServerSetting
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnModify);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBatchModifyServerSetting";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Shown += new System.EventHandler(this.FormBatchModifyServerInfo_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkInIP;
        private System.Windows.Forms.TextBox tboxInPort;
        private System.Windows.Forms.CheckBox chkInPort;
        private System.Windows.Forms.TextBox tboxInIP;
        private System.Windows.Forms.CheckBox chkMark;
        private System.Windows.Forms.CheckBox chkInMode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboxInMode;
        private System.Windows.Forms.ComboBox cboxMark;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboxImport;
        private System.Windows.Forms.ComboBox cboxAutorun;
        private System.Windows.Forms.CheckBox chkImport;
        private System.Windows.Forms.CheckBox chkAutorun;
        private System.Windows.Forms.ComboBox cboxIsInjectSkipCNSite;
        private System.Windows.Forms.CheckBox chkIsInjectSkipCNSite;
        private System.Windows.Forms.CheckBox chkShareOverLAN;
        private System.Windows.Forms.CheckBox chkIncrement;
    }
}
