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
            this.chkWin7 = new System.Windows.Forms.CheckBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.pgBarDownload = new System.Windows.Forms.ProgressBar();
            this.cboxArch = new System.Windows.Forms.ComboBox();
            this.labelCoreVersion = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboxDownloadSource = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboxVer
            // 
            this.cboxVer.FormattingEnabled = true;
            resources.ApplyResources(this.cboxVer, "cboxVer");
            this.cboxVer.Name = "cboxVer";
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
            // chkWin7
            // 
            resources.ApplyResources(this.chkWin7, "chkWin7");
            this.chkWin7.Name = "chkWin7";
            this.toolTip1.SetToolTip(this.chkWin7, resources.GetString("chkWin7.ToolTip"));
            this.chkWin7.UseVisualStyleBackColor = true;
            this.chkWin7.CheckedChanged += new System.EventHandler(this.chkWin7_CheckedChanged);
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // pgBarDownload
            // 
            resources.ApplyResources(this.pgBarDownload, "pgBarDownload");
            this.pgBarDownload.Name = "pgBarDownload";
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
            this.cboxArch.SelectedIndexChanged += new System.EventHandler(this.cboxArch_SelectedIndexChanged);
            // 
            // labelCoreVersion
            // 
            resources.ApplyResources(this.labelCoreVersion, "labelCoreVersion");
            this.labelCoreVersion.Name = "labelCoreVersion";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCheckVersion);
            this.groupBox1.Controls.Add(this.labelCoreVersion);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkWin7);
            this.groupBox2.Controls.Add(this.chkUseProxy);
            this.groupBox2.Controls.Add(this.btnExit);
            this.groupBox2.Controls.Add(this.btnDownload);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.cboxDownloadSource);
            this.groupBox2.Controls.Add(this.cboxArch);
            this.groupBox2.Controls.Add(this.pgBarDownload);
            this.groupBox2.Controls.Add(this.btnRefreshVer);
            this.groupBox2.Controls.Add(this.cboxVer);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // cboxDownloadSource
            // 
            this.cboxDownloadSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxDownloadSource.FormattingEnabled = true;
            this.cboxDownloadSource.Items.AddRange(new object[] {
            resources.GetString("cboxDownloadSource.Items"),
            resources.GetString("cboxDownloadSource.Items1"),
            resources.GetString("cboxDownloadSource.Items2")});
            resources.ApplyResources(this.cboxDownloadSource, "cboxDownloadSource");
            this.cboxDownloadSource.Name = "cboxDownloadSource";
            this.cboxDownloadSource.SelectedIndexChanged += new System.EventHandler(this.cboxDownloadSource_SelectedIndexChanged);
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
        private System.Windows.Forms.ComboBox cboxArch;
        private System.Windows.Forms.Label labelCoreVersion;
        private System.Windows.Forms.Button btnCheckVersion;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.CheckBox chkUseProxy;
        private System.Windows.Forms.ComboBox cboxDownloadSource;
        private System.Windows.Forms.CheckBox chkWin7;
    }
}