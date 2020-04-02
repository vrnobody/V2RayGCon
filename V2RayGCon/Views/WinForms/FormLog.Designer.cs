namespace V2RayGCon.Views.WinForms
{
    partial class FormLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLog));
            this.rtBoxLogger = new VgcApis.UserControls.ExRichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtBoxLogger
            // 
            this.rtBoxLogger.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtBoxLogger.DetectUrls = false;
            resources.ApplyResources(this.rtBoxLogger, "rtBoxLogger");
            this.rtBoxLogger.Name = "rtBoxLogger";
            this.rtBoxLogger.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rtBoxLogger);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // FormLog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "FormLog";
            this.Load += new System.EventHandler(this.FormLog_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private VgcApis.UserControls.ExRichTextBox rtBoxLogger;
        private System.Windows.Forms.Panel panel1;
    }
}
