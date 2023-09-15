
namespace V2RayGCon.Views.WinForms
{
    partial class FormCustomCoreSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCustomCoreSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.tboxName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.chkUseStdin = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkUseFile = new System.Windows.Forms.CheckBox();
            this.btnDir = new System.Windows.Forms.Button();
            this.chkSetWorkingDir = new System.Windows.Forms.CheckBox();
            this.tboxDir = new System.Windows.Forms.TextBox();
            this.tboxExe = new System.Windows.Forms.TextBox();
            this.cboxStdinEncoding = new System.Windows.Forms.ComboBox();
            this.cboxStdoutEncoding = new System.Windows.Forms.ComboBox();
            this.cboxArgs = new System.Windows.Forms.ComboBox();
            this.cboxConfigFilename = new System.Windows.Forms.ComboBox();
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
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // chkUseStdin
            // 
            resources.ApplyResources(this.chkUseStdin, "chkUseStdin");
            this.chkUseStdin.Name = "chkUseStdin";
            this.toolTip1.SetToolTip(this.chkUseStdin, resources.GetString("chkUseStdin.ToolTip"));
            this.chkUseStdin.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkUseFile
            // 
            resources.ApplyResources(this.chkUseFile, "chkUseFile");
            this.chkUseFile.Name = "chkUseFile";
            this.toolTip1.SetToolTip(this.chkUseFile, resources.GetString("chkUseFile.ToolTip"));
            this.chkUseFile.UseVisualStyleBackColor = true;
            // 
            // btnDir
            // 
            resources.ApplyResources(this.btnDir, "btnDir");
            this.btnDir.Name = "btnDir";
            this.toolTip1.SetToolTip(this.btnDir, resources.GetString("btnDir.ToolTip"));
            this.btnDir.UseVisualStyleBackColor = true;
            this.btnDir.Click += new System.EventHandler(this.btnDir_Click);
            // 
            // chkSetWorkingDir
            // 
            resources.ApplyResources(this.chkSetWorkingDir, "chkSetWorkingDir");
            this.chkSetWorkingDir.Name = "chkSetWorkingDir";
            this.toolTip1.SetToolTip(this.chkSetWorkingDir, resources.GetString("chkSetWorkingDir.ToolTip"));
            this.chkSetWorkingDir.UseVisualStyleBackColor = true;
            // 
            // tboxDir
            // 
            resources.ApplyResources(this.tboxDir, "tboxDir");
            this.tboxDir.Name = "tboxDir";
            // 
            // tboxExe
            // 
            resources.ApplyResources(this.tboxExe, "tboxExe");
            this.tboxExe.Name = "tboxExe";
            // 
            // cboxStdinEncoding
            // 
            resources.ApplyResources(this.cboxStdinEncoding, "cboxStdinEncoding");
            this.cboxStdinEncoding.FormattingEnabled = true;
            this.cboxStdinEncoding.Name = "cboxStdinEncoding";
            // 
            // cboxStdoutEncoding
            // 
            resources.ApplyResources(this.cboxStdoutEncoding, "cboxStdoutEncoding");
            this.cboxStdoutEncoding.FormattingEnabled = true;
            this.cboxStdoutEncoding.Name = "cboxStdoutEncoding";
            // 
            // cboxArgs
            // 
            resources.ApplyResources(this.cboxArgs, "cboxArgs");
            this.cboxArgs.FormattingEnabled = true;
            this.cboxArgs.Items.AddRange(new object[] {
            resources.GetString("cboxArgs.Items"),
            resources.GetString("cboxArgs.Items1"),
            resources.GetString("cboxArgs.Items2")});
            this.cboxArgs.Name = "cboxArgs";
            // 
            // cboxConfigFilename
            // 
            resources.ApplyResources(this.cboxConfigFilename, "cboxConfigFilename");
            this.cboxConfigFilename.FormattingEnabled = true;
            this.cboxConfigFilename.Items.AddRange(new object[] {
            resources.GetString("cboxConfigFilename.Items"),
            resources.GetString("cboxConfigFilename.Items1")});
            this.cboxConfigFilename.Name = "cboxConfigFilename";
            // 
            // FormCustomCoreSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboxConfigFilename);
            this.Controls.Add(this.btnDir);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkUseFile);
            this.Controls.Add(this.chkSetWorkingDir);
            this.Controls.Add(this.chkUseStdin);
            this.Controls.Add(this.cboxStdoutEncoding);
            this.Controls.Add(this.cboxArgs);
            this.Controls.Add(this.cboxStdinEncoding);
            this.Controls.Add(this.tboxExe);
            this.Controls.Add(this.tboxDir);
            this.Controls.Add(this.tboxName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormCustomCoreSettings";
            this.Load += new System.EventHandler(this.FormCustomCoreSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboxName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkUseStdin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox tboxDir;
        private System.Windows.Forms.TextBox tboxExe;
        private System.Windows.Forms.ComboBox cboxStdinEncoding;
        private System.Windows.Forms.ComboBox cboxStdoutEncoding;
        private System.Windows.Forms.CheckBox chkUseFile;
        private System.Windows.Forms.Button btnDir;
        private System.Windows.Forms.CheckBox chkSetWorkingDir;
        private System.Windows.Forms.ComboBox cboxArgs;
        private System.Windows.Forms.ComboBox cboxConfigFilename;
    }
}