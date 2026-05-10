
namespace V2RayGCon.Views.WinForms
{
    partial class FormTemplateNameSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTemplateNameSelector));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flyPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOk = new VgcApis.UserControls.RoundButton();
            this.btnCancel = new VgcApis.UserControls.RoundButton();
            this.tboxNames = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.flyPanel);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // flyPanel
            // 
            resources.ApplyResources(this.flyPanel, "flyPanel");
            this.flyPanel.BackColor = System.Drawing.SystemColors.Window;
            this.flyPanel.Name = "flyPanel";
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.BackColor = System.Drawing.SystemColors.Control;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tboxNames
            // 
            resources.ApplyResources(this.tboxNames, "tboxNames");
            this.tboxNames.Name = "tboxNames";
            this.tboxNames.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tboxNames_KeyDown);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FormTemplateNameSelector
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tboxNames);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormTemplateNameSelector";
            this.Load += new System.EventHandler(this.FormTemplateNameSelector_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flyPanel;
        private System.Windows.Forms.TextBox tboxNames;
        private System.Windows.Forms.Label label1;
        private VgcApis.UserControls.RoundButton btnOk;
        private VgcApis.UserControls.RoundButton btnCancel;
    }
}
