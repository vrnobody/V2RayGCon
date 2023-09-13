
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
            this.chkBindConfig = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.chkUseStdin = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkUseFile = new System.Windows.Forms.CheckBox();
            this.chkBindSharelink = new System.Windows.Forms.CheckBox();
            this.btnDir = new System.Windows.Forms.Button();
            this.chkSetWorkingDir = new System.Windows.Forms.CheckBox();
            this.tboxDir = new System.Windows.Forms.TextBox();
            this.tboxExe = new System.Windows.Forms.TextBox();
            this.tboxArgs = new System.Windows.Forms.TextBox();
            this.cboxStdinEncoding = new System.Windows.Forms.ComboBox();
            this.cboxStdoutEncoding = new System.Windows.Forms.ComboBox();
            this.tboxConfigFilename = new System.Windows.Forms.TextBox();
            this.tboxProtocols = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // tboxName
            // 
            resources.ApplyResources(this.tboxName, "tboxName");
            this.tboxName.Name = "tboxName";
            this.toolTip1.SetToolTip(this.tboxName, resources.GetString("tboxName.ToolTip"));
            // 
            // chkBindConfig
            // 
            resources.ApplyResources(this.chkBindConfig, "chkBindConfig");
            this.chkBindConfig.Name = "chkBindConfig";
            this.toolTip1.SetToolTip(this.chkBindConfig, resources.GetString("chkBindConfig.ToolTip"));
            this.chkBindConfig.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
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
            this.toolTip1.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
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
            // chkBindSharelink
            // 
            resources.ApplyResources(this.chkBindSharelink, "chkBindSharelink");
            this.chkBindSharelink.Name = "chkBindSharelink";
            this.toolTip1.SetToolTip(this.chkBindSharelink, resources.GetString("chkBindSharelink.ToolTip"));
            this.chkBindSharelink.UseVisualStyleBackColor = true;
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
            this.toolTip1.SetToolTip(this.tboxDir, resources.GetString("tboxDir.ToolTip"));
            // 
            // tboxExe
            // 
            resources.ApplyResources(this.tboxExe, "tboxExe");
            this.tboxExe.Name = "tboxExe";
            this.toolTip1.SetToolTip(this.tboxExe, resources.GetString("tboxExe.ToolTip"));
            // 
            // tboxArgs
            // 
            resources.ApplyResources(this.tboxArgs, "tboxArgs");
            this.tboxArgs.Name = "tboxArgs";
            this.toolTip1.SetToolTip(this.tboxArgs, resources.GetString("tboxArgs.ToolTip"));
            // 
            // cboxStdinEncoding
            // 
            resources.ApplyResources(this.cboxStdinEncoding, "cboxStdinEncoding");
            this.cboxStdinEncoding.FormattingEnabled = true;
            this.cboxStdinEncoding.Name = "cboxStdinEncoding";
            this.toolTip1.SetToolTip(this.cboxStdinEncoding, resources.GetString("cboxStdinEncoding.ToolTip"));
            // 
            // cboxStdoutEncoding
            // 
            resources.ApplyResources(this.cboxStdoutEncoding, "cboxStdoutEncoding");
            this.cboxStdoutEncoding.FormattingEnabled = true;
            this.cboxStdoutEncoding.Name = "cboxStdoutEncoding";
            this.toolTip1.SetToolTip(this.cboxStdoutEncoding, resources.GetString("cboxStdoutEncoding.ToolTip"));
            // 
            // tboxConfigFilename
            // 
            resources.ApplyResources(this.tboxConfigFilename, "tboxConfigFilename");
            this.tboxConfigFilename.Name = "tboxConfigFilename";
            this.toolTip1.SetToolTip(this.tboxConfigFilename, resources.GetString("tboxConfigFilename.ToolTip"));
            // 
            // tboxProtocols
            // 
            resources.ApplyResources(this.tboxProtocols, "tboxProtocols");
            this.tboxProtocols.Name = "tboxProtocols";
            this.toolTip1.SetToolTip(this.tboxProtocols, resources.GetString("tboxProtocols.ToolTip"));
            // 
            // FormCustomCoreSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDir);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkUseFile);
            this.Controls.Add(this.chkSetWorkingDir);
            this.Controls.Add(this.chkUseStdin);
            this.Controls.Add(this.chkBindSharelink);
            this.Controls.Add(this.chkBindConfig);
            this.Controls.Add(this.cboxStdoutEncoding);
            this.Controls.Add(this.cboxStdinEncoding);
            this.Controls.Add(this.tboxProtocols);
            this.Controls.Add(this.tboxConfigFilename);
            this.Controls.Add(this.tboxArgs);
            this.Controls.Add(this.tboxExe);
            this.Controls.Add(this.tboxDir);
            this.Controls.Add(this.tboxName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormCustomCoreSettings";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.FormCustomCoreSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboxName;
        private System.Windows.Forms.CheckBox chkBindConfig;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkUseStdin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox tboxDir;
        private System.Windows.Forms.TextBox tboxExe;
        private System.Windows.Forms.TextBox tboxArgs;
        private System.Windows.Forms.ComboBox cboxStdinEncoding;
        private System.Windows.Forms.ComboBox cboxStdoutEncoding;
        private System.Windows.Forms.CheckBox chkUseFile;
        private System.Windows.Forms.TextBox tboxConfigFilename;
        private System.Windows.Forms.CheckBox chkBindSharelink;
        private System.Windows.Forms.TextBox tboxProtocols;
        private System.Windows.Forms.Button btnDir;
        private System.Windows.Forms.CheckBox chkSetWorkingDir;
    }
}