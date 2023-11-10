
namespace V2RayGCon.Views.UserControls
{
    partial class ConfigTemplateUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigTemplateUI));
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rlbBinding = new VgcApis.UserControls.RoundLabel();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            resources.ApplyResources(this.lbTitle, "lbTitle");
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
            // 
            // btnEdit
            // 
            this.btnEdit.BackgroundImage = global::V2RayGCon.Properties.Resources.EditWindow_16x;
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.Name = "btnEdit";
            this.toolTip1.SetToolTip(this.btnEdit, resources.GetString("btnEdit.ToolTip"));
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.toolTip1.SetToolTip(this.btnDelete, resources.GetString("btnDelete.ToolTip"));
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // rlbBinding
            // 
            this.rlbBinding._BackColor = System.Drawing.Color.Wheat;
            resources.ApplyResources(this.rlbBinding, "rlbBinding");
            this.rlbBinding.Name = "rlbBinding";
            this.toolTip1.SetToolTip(this.rlbBinding, resources.GetString("rlbBinding.ToolTip"));
            this.rlbBinding.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rlbBinding_MouseDown);
            // 
            // ConfigTemplateUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.rlbBinding);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.lbTitle);
            this.Name = "ConfigTemplateUI";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CoreSettingUI_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ToolTip toolTip1;
        private VgcApis.UserControls.RoundLabel rlbBinding;
    }
}
