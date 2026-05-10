
namespace VgcApis.WinForms
{
    partial class FormInput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInput));
            this.tboxContent = new System.Windows.Forms.TextBox();
            this.btnOk = new VgcApis.UserControls.RoundButton();
            this.btnCancel = new VgcApis.UserControls.RoundButton();
            this.lbTitle = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // tboxContent
            // 
            resources.ApplyResources(this.tboxContent, "tboxContent");
            this.tboxContent.Name = "tboxContent";
            this.tboxContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tboxContent_KeyDown);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbTitle
            // 
            resources.ApplyResources(this.lbTitle, "lbTitle");
            this.lbTitle.Name = "lbTitle";
            // 
            // FormInput
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tboxContent);
            this.Name = "FormInput";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tboxContent;
        private VgcApis.UserControls.RoundButton btnOk;
        private VgcApis.UserControls.RoundButton btnCancel;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
