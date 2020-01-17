namespace Pacman.Views.UserControls
{
    partial class BeanUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeanUI));
            this.chkTitle = new System.Windows.Forms.CheckBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkTitle
            // 
            this.chkTitle.AutoEllipsis = true;
            this.chkTitle.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.chkTitle, "chkTitle");
            this.chkTitle.Name = "chkTitle";
            this.chkTitle.UseVisualStyleBackColor = true;
            this.chkTitle.CheckedChanged += new System.EventHandler(this.chkTitle_CheckedChanged);
            // 
            // lbStatus
            // 
            this.lbStatus.AutoEllipsis = true;
            this.lbStatus.ForeColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.lbStatus, "lbStatus");
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbStatus_MouseDown);
            // 
            // BeanUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.chkTitle);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Name = "BeanUI";
            this.Load += new System.EventHandler(this.BeanUI_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BeanUI_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkTitle;
        private System.Windows.Forms.Label lbStatus;
    }
}
