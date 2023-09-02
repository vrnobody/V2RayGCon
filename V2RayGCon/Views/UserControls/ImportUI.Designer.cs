namespace V2RayGCon.Views.UserControls
{
    partial class ImportUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportUI));
            this.lbIndex = new System.Windows.Forms.Label();
            this.tboxAlias = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tboxUrl = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.chkMergeWhenStart = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkMergeWhenSpeedTest = new System.Windows.Forms.CheckBox();
            this.chkMergeWhenPacking = new System.Windows.Forms.CheckBox();
            this.btnBrowseLocalFile = new System.Windows.Forms.Button();
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
            // label2
            // 
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tboxUrl
            // 
            resources.ApplyResources(this.tboxUrl, "tboxUrl");
            this.tboxUrl.Name = "tboxUrl";
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // chkMergeWhenStart
            // 
            resources.ApplyResources(this.chkMergeWhenStart, "chkMergeWhenStart");
            this.chkMergeWhenStart.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkMergeWhenStart.Name = "chkMergeWhenStart";
            this.toolTip1.SetToolTip(this.chkMergeWhenStart, resources.GetString("chkMergeWhenStart.ToolTip"));
            this.chkMergeWhenStart.UseVisualStyleBackColor = true;
            // 
            // chkMergeWhenSpeedTest
            // 
            resources.ApplyResources(this.chkMergeWhenSpeedTest, "chkMergeWhenSpeedTest");
            this.chkMergeWhenSpeedTest.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkMergeWhenSpeedTest.Name = "chkMergeWhenSpeedTest";
            this.toolTip1.SetToolTip(this.chkMergeWhenSpeedTest, resources.GetString("chkMergeWhenSpeedTest.ToolTip"));
            this.chkMergeWhenSpeedTest.UseVisualStyleBackColor = true;
            // 
            // chkMergeWhenPacking
            // 
            resources.ApplyResources(this.chkMergeWhenPacking, "chkMergeWhenPacking");
            this.chkMergeWhenPacking.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkMergeWhenPacking.Name = "chkMergeWhenPacking";
            this.toolTip1.SetToolTip(this.chkMergeWhenPacking, resources.GetString("chkMergeWhenPacking.ToolTip"));
            this.chkMergeWhenPacking.UseVisualStyleBackColor = true;
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
            // ImportUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnBrowseLocalFile);
            this.Controls.Add(this.chkMergeWhenPacking);
            this.Controls.Add(this.chkMergeWhenSpeedTest);
            this.Controls.Add(this.chkMergeWhenStart);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.tboxUrl);
            this.Controls.Add(this.tboxAlias);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbIndex);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Name = "ImportUI";
            this.Tag = "";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UrlListItem_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbIndex;
        private System.Windows.Forms.TextBox tboxAlias;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tboxUrl;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.CheckBox chkMergeWhenStart;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkMergeWhenSpeedTest;
        private System.Windows.Forms.CheckBox chkMergeWhenPacking;
        private System.Windows.Forms.Button btnBrowseLocalFile;
    }
}
