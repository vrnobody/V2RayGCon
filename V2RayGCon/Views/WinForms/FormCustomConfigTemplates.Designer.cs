
namespace V2RayGCon.Views.WinForms
{
    partial class FormCustomConfigTemplates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCustomConfigTemplates));
            this.label1 = new System.Windows.Forms.Label();
            this.tboxName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnFormat = new System.Windows.Forms.Button();
            this.chkIsSocks5Inbound = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rtboxTemplate = new VgcApis.UserControls.ExRichTextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.cboxMergeOption = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tboxMergeParams = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
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
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnFormat
            // 
            resources.ApplyResources(this.btnFormat, "btnFormat");
            this.btnFormat.Name = "btnFormat";
            this.toolTip1.SetToolTip(this.btnFormat, resources.GetString("btnFormat.ToolTip"));
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // chkIsSocks5Inbound
            // 
            resources.ApplyResources(this.chkIsSocks5Inbound, "chkIsSocks5Inbound");
            this.chkIsSocks5Inbound.Name = "chkIsSocks5Inbound";
            this.toolTip1.SetToolTip(this.chkIsSocks5Inbound, resources.GetString("chkIsSocks5Inbound.ToolTip"));
            this.chkIsSocks5Inbound.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // rtboxTemplate
            // 
            this.rtboxTemplate.AcceptsTab = true;
            resources.ApplyResources(this.rtboxTemplate, "rtboxTemplate");
            this.rtboxTemplate.Name = "rtboxTemplate";
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // cboxMergeOption
            // 
            this.cboxMergeOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxMergeOption.FormattingEnabled = true;
            resources.ApplyResources(this.cboxMergeOption, "cboxMergeOption");
            this.cboxMergeOption.Name = "cboxMergeOption";
            this.cboxMergeOption.TextChanged += new System.EventHandler(this.cboxMergeOption_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tboxMergeParams
            // 
            resources.ApplyResources(this.tboxMergeParams, "tboxMergeParams");
            this.tboxMergeParams.Name = "tboxMergeParams";
            // 
            // FormCustomConfigTemplates
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tboxMergeParams);
            this.Controls.Add(this.chkIsSocks5Inbound);
            this.Controls.Add(this.cboxMergeOption);
            this.Controls.Add(this.btnFormat);
            this.Controls.Add(this.rtboxTemplate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tboxName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormCustomConfigTemplates";
            this.Load += new System.EventHandler(this.FormCustomCoreSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboxName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private VgcApis.UserControls.ExRichTextBox rtboxTemplate;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.ComboBox cboxMergeOption;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.CheckBox chkIsSocks5Inbound;
        private System.Windows.Forms.TextBox tboxMergeParams;
        private System.Windows.Forms.Label label4;
    }
}