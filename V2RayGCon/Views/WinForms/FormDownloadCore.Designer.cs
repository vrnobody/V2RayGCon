namespace V2RayGCon.Views.WinForms
{
    partial class FormDownloadCore
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDownloadCore));
            this.cboxVer = new System.Windows.Forms.ComboBox();
            this.btnRefreshVer = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCheckVersion = new System.Windows.Forms.Button();
            this.chkUseProxy = new System.Windows.Forms.CheckBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.pgBarDownload = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.cboxArch = new System.Windows.Forms.ComboBox();
            this.labelCoreVersion = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboxVer
            // 
            resources.ApplyResources(this.cboxVer, "cboxVer");
            this.cboxVer.FormattingEnabled = true;
            this.cboxVer.Name = "cboxVer";
            this.toolTip1.SetToolTip(this.cboxVer, resources.GetString("cboxVer.ToolTip"));
            // 
            // btnRefreshVer
            // 
            resources.ApplyResources(this.btnRefreshVer, "btnRefreshVer");
            this.btnRefreshVer.Name = "btnRefreshVer";
            this.toolTip1.SetToolTip(this.btnRefreshVer, resources.GetString("btnRefreshVer.ToolTip"));
            this.btnRefreshVer.UseVisualStyleBackColor = true;
            this.btnRefreshVer.Click += new System.EventHandler(this.BtnRefreshVer_Click);
            // 
            // btnDownload
            // 
            resources.ApplyResources(this.btnDownload, "btnDownload");
            this.btnDownload.Name = "btnDownload";
            this.toolTip1.SetToolTip(this.btnDownload, resources.GetString("btnDownload.ToolTip"));
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.toolTip1.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnCheckVersion
            // 
            resources.ApplyResources(this.btnCheckVersion, "btnCheckVersion");
            this.btnCheckVersion.Name = "btnCheckVersion";
            this.toolTip1.SetToolTip(this.btnCheckVersion, resources.GetString("btnCheckVersion.ToolTip"));
            this.btnCheckVersion.UseVisualStyleBackColor = true;
            this.btnCheckVersion.Click += new System.EventHandler(this.BtnCheckVersion_Click);
            // 
            // chkUseProxy
            // 
            resources.ApplyResources(this.chkUseProxy, "chkUseProxy");
            this.chkUseProxy.Name = "chkUseProxy";
            this.toolTip1.SetToolTip(this.chkUseProxy, resources.GetString("chkUseProxy.ToolTip"));
            this.chkUseProxy.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.toolTip1.SetToolTip(this.btnExit, resources.GetString("btnExit.ToolTip"));
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // pgBarDownload
            // 
            resources.ApplyResources(this.pgBarDownload, "pgBarDownload");
            this.pgBarDownload.Name = "pgBarDownload";
            this.toolTip1.SetToolTip(this.pgBarDownload, resources.GetString("pgBarDownload.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // cboxArch
            // 
            resources.ApplyResources(this.cboxArch, "cboxArch");
            this.cboxArch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxArch.FormattingEnabled = true;
            this.cboxArch.Items.AddRange(new object[] {
            resources.GetString("cboxArch.Items"),
            resources.GetString("cboxArch.Items1")});
            this.cboxArch.Name = "cboxArch";
            this.toolTip1.SetToolTip(this.cboxArch, resources.GetString("cboxArch.ToolTip"));
            this.cboxArch.SelectedIndexChanged += new System.EventHandler(this.cboxArch_SelectedIndexChanged);
            // 
            // labelCoreVersion
            // 
            resources.ApplyResources(this.labelCoreVersion, "labelCoreVersion");
            this.labelCoreVersion.Name = "labelCoreVersion";
            this.toolTip1.SetToolTip(this.labelCoreVersion, resources.GetString("labelCoreVersion.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.btnCheckVersion);
            this.groupBox1.Controls.Add(this.labelCoreVersion);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.chkUseProxy);
            this.groupBox2.Controls.Add(this.btnExit);
            this.groupBox2.Controls.Add(this.btnDownload);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.cboxArch);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.pgBarDownload);
            this.groupBox2.Controls.Add(this.btnRefreshVer);
            this.groupBox2.Controls.Add(this.cboxVer);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // FormDownloadCore
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDownloadCore";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Shown += new System.EventHandler(this.FormDownloadCore_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboxVer;
        private System.Windows.Forms.Button btnRefreshVer;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ProgressBar pgBarDownload;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboxArch;
        private System.Windows.Forms.Label labelCoreVersion;
        private System.Windows.Forms.Button btnCheckVersion;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.CheckBox chkUseProxy;
    }
}