namespace Luna.Views.WinForms
{
    partial class FormLuaCoreSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLuaCoreSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.tboxName = new System.Windows.Forms.TextBox();
            this.chkAutorun = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkHidden = new System.Windows.Forms.CheckBox();
            this.chkClrSupports = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tboxName
            // 
            resources.ApplyResources(this.tboxName, "tboxName");
            this.tboxName.Name = "tboxName";
            // 
            // chkAutorun
            // 
            resources.ApplyResources(this.chkAutorun, "chkAutorun");
            this.chkAutorun.Name = "chkAutorun";
            this.chkAutorun.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkHidden
            // 
            resources.ApplyResources(this.chkHidden, "chkHidden");
            this.chkHidden.Name = "chkHidden";
            this.chkHidden.UseVisualStyleBackColor = true;
            // 
            // chkClrSupports
            // 
            resources.ApplyResources(this.chkClrSupports, "chkClrSupports");
            this.chkClrSupports.Name = "chkClrSupports";
            this.chkClrSupports.UseVisualStyleBackColor = true;
            // 
            // FormLuaCoreSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkClrSupports);
            this.Controls.Add(this.chkHidden);
            this.Controls.Add(this.chkAutorun);
            this.Controls.Add(this.tboxName);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLuaCoreSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboxName;
        private System.Windows.Forms.CheckBox chkAutorun;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkHidden;
        private System.Windows.Forms.CheckBox chkClrSupports;
    }
}