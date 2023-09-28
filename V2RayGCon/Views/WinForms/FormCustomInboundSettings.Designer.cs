
namespace V2RayGCon.Views.WinForms
{
    partial class FormCustomInboundSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCustomInboundSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.tboxName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnFormat = new System.Windows.Forms.Button();
            this.rtboxTemplate = new VgcApis.UserControls.ExRichTextBox();
            this.btnTest = new System.Windows.Forms.Button();
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
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.toolTip1.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
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
            // rtboxTemplate
            // 
            resources.ApplyResources(this.rtboxTemplate, "rtboxTemplate");
            this.rtboxTemplate.Name = "rtboxTemplate";
            this.toolTip1.SetToolTip(this.rtboxTemplate, resources.GetString("rtboxTemplate.ToolTip"));
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.toolTip1.SetToolTip(this.btnTest, resources.GetString("btnTest.ToolTip"));
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // FormCustomInboundSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnFormat);
            this.Controls.Add(this.rtboxTemplate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tboxName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormCustomInboundSettings";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
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
    }
}