namespace VgcApis.WinForms
{
    partial class FormMultiLineInput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMultiLineInput));
            this.lbTitle = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rtboxContent = new VgcApis.UserControls.ExRichTextBox();
            this.btnCancel = new VgcApis.UserControls.RoundButton();
            this.btnOk = new VgcApis.UserControls.RoundButton();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            resources.ApplyResources(this.lbTitle, "lbTitle");
            this.lbTitle.Name = "lbTitle";
            // 
            // rtboxContent
            // 
            this.rtboxContent.AcceptsTab = true;
            resources.ApplyResources(this.rtboxContent, "rtboxContent");
            this.rtboxContent.DetectUrls = false;
            this.rtboxContent.Name = "rtboxContent";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnOk.Name = "btnOk";
            this.toolTip1.SetToolTip(this.btnOk, resources.GetString("btnOk.ToolTip"));
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // FormMultiLineInput
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.rtboxContent);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.KeyPreview = true;
            this.Name = "FormMultiLineInput";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMultiLineInput_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private UserControls.ExRichTextBox rtboxContent;
        private System.Windows.Forms.Label lbTitle;
        private UserControls.RoundButton btnOk;
        private UserControls.RoundButton btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
