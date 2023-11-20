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
            this.cboxInName = new System.Windows.Forms.ComboBox();
            this.cboxMark = new System.Windows.Forms.ComboBox();
            this.btnModify = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnTemplates = new System.Windows.Forms.Button();
            this.cboxUntrack = new System.Windows.Forms.ComboBox();
            this.cboxCustomCoreName = new System.Windows.Forms.ComboBox();
            this.cboxSendThrough = new System.Windows.Forms.ComboBox();
            this.cboxInject = new System.Windows.Forms.ComboBox();
            this.cboxAutorun = new System.Windows.Forms.ComboBox();
            this.tboxTemplates = new System.Windows.Forms.TextBox();
            this.tboxRemark = new System.Windows.Forms.TextBox();
            this.chkUntrack = new System.Windows.Forms.CheckBox();
            this.chkTemplates = new System.Windows.Forms.CheckBox();
            this.chkSendThough = new System.Windows.Forms.CheckBox();
            this.chkRemark = new System.Windows.Forms.CheckBox();
            this.chkInject = new System.Windows.Forms.CheckBox();
            this.chkCustomCore = new System.Windows.Forms.CheckBox();
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
            this.groupBox1.Controls.Add(this.cboxInName);
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
            // cboxInName
            // 
            resources.ApplyResources(this.cboxInName, "cboxInName");
            this.cboxInName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxInName.FormattingEnabled = true;
            this.cboxInName.Items.AddRange(new object[] {
            resources.GetString("cboxInName.Items"),
            resources.GetString("cboxInName.Items1"),
            resources.GetString("cboxInName.Items2"),
            resources.GetString("cboxInName.Items3")});
            this.cboxInName.Name = "cboxInName";
            this.toolTip1.SetToolTip(this.cboxInName, resources.GetString("cboxInName.ToolTip"));
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
            this.groupBox2.Controls.Add(this.btnTemplates);
            this.groupBox2.Controls.Add(this.cboxUntrack);
            this.groupBox2.Controls.Add(this.cboxCustomCoreName);
            this.groupBox2.Controls.Add(this.cboxSendThrough);
            this.groupBox2.Controls.Add(this.cboxInject);
            this.groupBox2.Controls.Add(this.cboxAutorun);
            this.groupBox2.Controls.Add(this.tboxTemplates);
            this.groupBox2.Controls.Add(this.tboxRemark);
            this.groupBox2.Controls.Add(this.chkUntrack);
            this.groupBox2.Controls.Add(this.chkTemplates);
            this.groupBox2.Controls.Add(this.chkSendThough);
            this.groupBox2.Controls.Add(this.chkRemark);
            this.groupBox2.Controls.Add(this.chkInject);
            this.groupBox2.Controls.Add(this.chkCustomCore);
            this.groupBox2.Controls.Add(this.chkAutorun);
            this.groupBox2.Controls.Add(this.chkMark);
            this.groupBox2.Controls.Add(this.cboxMark);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // btnTemplates
            // 
            resources.ApplyResources(this.btnTemplates, "btnTemplates");
            this.btnTemplates.Name = "btnTemplates";
            this.toolTip1.SetToolTip(this.btnTemplates, resources.GetString("btnTemplates.ToolTip"));
            this.btnTemplates.UseVisualStyleBackColor = true;
            this.btnTemplates.Click += new System.EventHandler(this.btnTemplates_Click);
            // 
            // cboxUntrack
            // 
            resources.ApplyResources(this.cboxUntrack, "cboxUntrack");
            this.cboxUntrack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxUntrack.FormattingEnabled = true;
            this.cboxUntrack.Items.AddRange(new object[] {
            resources.GetString("cboxUntrack.Items"),
            resources.GetString("cboxUntrack.Items1")});
            this.cboxUntrack.Name = "cboxUntrack";
            this.toolTip1.SetToolTip(this.cboxUntrack, resources.GetString("cboxUntrack.ToolTip"));
            // 
            // cboxCustomCoreName
            // 
            resources.ApplyResources(this.cboxCustomCoreName, "cboxCustomCoreName");
            this.cboxCustomCoreName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxCustomCoreName.FormattingEnabled = true;
            this.cboxCustomCoreName.Items.AddRange(new object[] {
            resources.GetString("cboxCustomCoreName.Items"),
            resources.GetString("cboxCustomCoreName.Items1")});
            this.cboxCustomCoreName.Name = "cboxCustomCoreName";
            this.toolTip1.SetToolTip(this.cboxCustomCoreName, resources.GetString("cboxCustomCoreName.ToolTip"));
            // 
            // cboxSendThrough
            // 
            resources.ApplyResources(this.cboxSendThrough, "cboxSendThrough");
            this.cboxSendThrough.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxSendThrough.FormattingEnabled = true;
            this.cboxSendThrough.Items.AddRange(new object[] {
            resources.GetString("cboxSendThrough.Items"),
            resources.GetString("cboxSendThrough.Items1")});
            this.cboxSendThrough.Name = "cboxSendThrough";
            this.toolTip1.SetToolTip(this.cboxSendThrough, resources.GetString("cboxSendThrough.ToolTip"));
            // 
            // cboxInject
            // 
            resources.ApplyResources(this.cboxInject, "cboxInject");
            this.cboxInject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxInject.FormattingEnabled = true;
            this.cboxInject.Items.AddRange(new object[] {
            resources.GetString("cboxInject.Items"),
            resources.GetString("cboxInject.Items1")});
            this.cboxInject.Name = "cboxInject";
            this.toolTip1.SetToolTip(this.cboxInject, resources.GetString("cboxInject.ToolTip"));
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
            // tboxTemplates
            // 
            resources.ApplyResources(this.tboxTemplates, "tboxTemplates");
            this.tboxTemplates.Name = "tboxTemplates";
            this.toolTip1.SetToolTip(this.tboxTemplates, resources.GetString("tboxTemplates.ToolTip"));
            // 
            // tboxRemark
            // 
            resources.ApplyResources(this.tboxRemark, "tboxRemark");
            this.tboxRemark.Name = "tboxRemark";
            this.toolTip1.SetToolTip(this.tboxRemark, resources.GetString("tboxRemark.ToolTip"));
            // 
            // chkUntrack
            // 
            resources.ApplyResources(this.chkUntrack, "chkUntrack");
            this.chkUntrack.Name = "chkUntrack";
            this.toolTip1.SetToolTip(this.chkUntrack, resources.GetString("chkUntrack.ToolTip"));
            this.chkUntrack.UseVisualStyleBackColor = true;
            // 
            // chkTemplates
            // 
            resources.ApplyResources(this.chkTemplates, "chkTemplates");
            this.chkTemplates.Name = "chkTemplates";
            this.toolTip1.SetToolTip(this.chkTemplates, resources.GetString("chkTemplates.ToolTip"));
            this.chkTemplates.UseVisualStyleBackColor = true;
            // 
            // chkSendThough
            // 
            resources.ApplyResources(this.chkSendThough, "chkSendThough");
            this.chkSendThough.Name = "chkSendThough";
            this.toolTip1.SetToolTip(this.chkSendThough, resources.GetString("chkSendThough.ToolTip"));
            this.chkSendThough.UseVisualStyleBackColor = true;
            // 
            // chkRemark
            // 
            resources.ApplyResources(this.chkRemark, "chkRemark");
            this.chkRemark.Name = "chkRemark";
            this.toolTip1.SetToolTip(this.chkRemark, resources.GetString("chkRemark.ToolTip"));
            this.chkRemark.UseVisualStyleBackColor = true;
            // 
            // chkInject
            // 
            resources.ApplyResources(this.chkInject, "chkInject");
            this.chkInject.Name = "chkInject";
            this.toolTip1.SetToolTip(this.chkInject, resources.GetString("chkInject.ToolTip"));
            this.chkInject.UseVisualStyleBackColor = true;
            // 
            // chkCustomCore
            // 
            resources.ApplyResources(this.chkCustomCore, "chkCustomCore");
            this.chkCustomCore.Name = "chkCustomCore";
            this.toolTip1.SetToolTip(this.chkCustomCore, resources.GetString("chkCustomCore.ToolTip"));
            this.chkCustomCore.UseVisualStyleBackColor = true;
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
        private System.Windows.Forms.ComboBox cboxInName;
        private System.Windows.Forms.ComboBox cboxMark;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboxUntrack;
        private System.Windows.Forms.ComboBox cboxAutorun;
        private System.Windows.Forms.CheckBox chkUntrack;
        private System.Windows.Forms.CheckBox chkAutorun;
        private System.Windows.Forms.CheckBox chkShareOverLAN;
        private System.Windows.Forms.CheckBox chkIncrement;
        private System.Windows.Forms.TextBox tboxRemark;
        private System.Windows.Forms.CheckBox chkRemark;
        private System.Windows.Forms.ComboBox cboxCustomCoreName;
        private System.Windows.Forms.CheckBox chkCustomCore;
        private System.Windows.Forms.Button btnTemplates;
        private System.Windows.Forms.ComboBox cboxSendThrough;
        private System.Windows.Forms.ComboBox cboxInject;
        private System.Windows.Forms.TextBox tboxTemplates;
        private System.Windows.Forms.CheckBox chkTemplates;
        private System.Windows.Forms.CheckBox chkSendThough;
        private System.Windows.Forms.CheckBox chkInject;
    }
}
