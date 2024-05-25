namespace V2RayGCon.Views.UserControls
{
    partial class PluginInfoUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginInfoUI));
            this.chkIsUse = new System.Windows.Forms.CheckBox();
            this.lbDescription = new System.Windows.Forms.Label();
            this.lbFilename = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkIsUse
            // 
            resources.ApplyResources(this.chkIsUse, "chkIsUse");
            this.chkIsUse.Name = "chkIsUse";
            this.chkIsUse.UseVisualStyleBackColor = true;
            this.chkIsUse.CheckedChanged += new System.EventHandler(this.chkIsUse_CheckedChanged);
            // 
            // lbDescription
            // 
            this.lbDescription.AutoEllipsis = true;
            resources.ApplyResources(this.lbDescription, "lbDescription");
            this.lbDescription.Name = "lbDescription";
            // 
            // lbFilename
            // 
            this.lbFilename.AutoEllipsis = true;
            resources.ApplyResources(this.lbFilename, "lbFilename");
            this.lbFilename.Name = "lbFilename";
            // 
            // PluginInfoUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lbDescription);
            this.Controls.Add(this.lbFilename);
            this.Controls.Add(this.chkIsUse);
            this.Name = "PluginInfoUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkIsUse;
        private System.Windows.Forms.Label lbDescription;
        private System.Windows.Forms.Label lbFilename;
    }
}
