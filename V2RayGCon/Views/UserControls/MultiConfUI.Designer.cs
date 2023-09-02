namespace V2RayGCon.Views.UserControls
{
    partial class MultiConfUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiConfUI));
            this.lbIndex = new System.Windows.Forms.Label();
            this.tboxAlias = new System.Windows.Forms.TextBox();
            this.tboxUrl = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnBrowseLocalFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbIndex
            // 
            resources.ApplyResources(this.lbIndex, "lbIndex");
            this.lbIndex.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbIndex.Name = "lbIndex";
            // 
            // tboxAlias
            // 
            resources.ApplyResources(this.tboxAlias, "tboxAlias");
            this.tboxAlias.Name = "tboxAlias";
            // 
            // tboxUrl
            // 
            resources.ApplyResources(this.tboxUrl, "tboxUrl");
            this.tboxUrl.Name = "tboxUrl";
            this.toolTip1.SetToolTip(this.tboxUrl, resources.GetString("tboxUrl.ToolTip"));
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnBrowseLocalFile
            // 
            this.btnBrowseLocalFile.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.btnBrowseLocalFile, "btnBrowseLocalFile");
            this.btnBrowseLocalFile.Name = "btnBrowseLocalFile";
            this.toolTip1.SetToolTip(this.btnBrowseLocalFile, resources.GetString("btnBrowseLocalFile.ToolTip"));
            this.btnBrowseLocalFile.UseVisualStyleBackColor = true;
            this.btnBrowseLocalFile.Click += new System.EventHandler(this.btnBrowseLocalFile_Click);
            // 
            // label1
            // 
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // MultiConfUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnBrowseLocalFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.tboxUrl);
            this.Controls.Add(this.tboxAlias);
            this.Controls.Add(this.lbIndex);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Name = "MultiConfUI";
            this.Tag = "";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UrlListItem_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbIndex;
        private System.Windows.Forms.TextBox tboxAlias;
        private System.Windows.Forms.TextBox tboxUrl;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnBrowseLocalFile;
        private System.Windows.Forms.Label label1;
    }
}
