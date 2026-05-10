namespace NeoLuna.Views.WinForms
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
            this.btnOk = new VgcApis.UserControls.RoundButton();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnCancel = new VgcApis.UserControls.RoundButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rtboxInput = new VgcApis.UserControls.ExRichTextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.BackColor = System.Drawing.SystemColors.Control;
            this.btnOk.Name = "btnOk";
            this.toolTip1.SetToolTip(this.btnOk, resources.GetString("btnOk.ToolTip"));
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lbTitle
            // 
            resources.ApplyResources(this.lbTitle, "lbTitle");
            this.lbTitle.Name = "lbTitle";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rtboxInput
            // 
            this.rtboxInput.AcceptsTab = true;
            resources.ApplyResources(this.rtboxInput, "rtboxInput");
            this.rtboxInput.DetectUrls = false;
            this.rtboxInput.Name = "rtboxInput";
            // 
            // FormInput
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtboxInput);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.KeyPreview = true;
            this.Name = "FormInput";
            this.Load += new System.EventHandler(this.FormInput_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormInput_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbTitle;
        private VgcApis.UserControls.ExRichTextBox rtboxInput;
        private System.Windows.Forms.ToolTip toolTip1;
        private VgcApis.UserControls.RoundButton btnOk;
        private VgcApis.UserControls.RoundButton btnCancel;
    }
}
