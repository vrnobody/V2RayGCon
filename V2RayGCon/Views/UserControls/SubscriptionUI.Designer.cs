namespace V2RayGCon.Views.UserControls
{
    partial class SubscriptionUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubscriptionUI));
            this.lbIndex = new System.Windows.Forms.Label();
            this.tboxAlias = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tboxUrl = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.chkIsUse = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkIsSetMark = new System.Windows.Forms.CheckBox();
            this.lbTotal = new System.Windows.Forms.Label();
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
            this.tboxAlias.TextChanged += new System.EventHandler(this.tboxAlias_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.label3.Name = "label3";
            // 
            // tboxUrl
            // 
            resources.ApplyResources(this.tboxUrl, "tboxUrl");
            this.tboxUrl.Name = "tboxUrl";
            this.tboxUrl.TextChanged += new System.EventHandler(this.tboxUrl_TextChanged);
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // chkIsUse
            // 
            resources.ApplyResources(this.chkIsUse, "chkIsUse");
            this.chkIsUse.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkIsUse.Name = "chkIsUse";
            this.toolTip1.SetToolTip(this.chkIsUse, resources.GetString("chkIsUse.ToolTip"));
            this.chkIsUse.UseVisualStyleBackColor = true;
            // 
            // chkIsSetMark
            // 
            resources.ApplyResources(this.chkIsSetMark, "chkIsSetMark");
            this.chkIsSetMark.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkIsSetMark.Name = "chkIsSetMark";
            this.toolTip1.SetToolTip(this.chkIsSetMark, resources.GetString("chkIsSetMark.ToolTip"));
            this.chkIsSetMark.UseVisualStyleBackColor = true;
            // 
            // lbTotal
            // 
            resources.ApplyResources(this.lbTotal, "lbTotal");
            this.lbTotal.ForeColor = System.Drawing.Color.Red;
            this.lbTotal.Name = "lbTotal";
            this.toolTip1.SetToolTip(this.lbTotal, resources.GetString("lbTotal.ToolTip"));
            this.lbTotal.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTotal_MouseDown);
            // 
            // SubscriptionUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lbTotal);
            this.Controls.Add(this.chkIsSetMark);
            this.Controls.Add(this.chkIsUse);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.tboxUrl);
            this.Controls.Add(this.tboxAlias);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbIndex);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Name = "SubscriptionUI";
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tboxUrl;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.CheckBox chkIsUse;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkIsSetMark;
        private System.Windows.Forms.Label lbTotal;
    }
}
