
namespace V2RayGCon.Views.Updater
{
    partial class FormDownloader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDownloader));
            this.pgBar = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.lbSize = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbSpeed = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbSource = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pgBar
            // 
            resources.ApplyResources(this.pgBar, "pgBar");
            this.pgBar.Name = "pgBar";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lbSize
            // 
            resources.ApplyResources(this.lbSize, "lbSize");
            this.lbSize.Name = "lbSize";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lbSpeed
            // 
            resources.ApplyResources(this.lbSpeed, "lbSpeed");
            this.lbSpeed.Name = "lbSpeed";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbSource
            // 
            resources.ApplyResources(this.lbSource, "lbSource");
            this.lbSource.Name = "lbSource";
            // 
            // FormDownloader
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbSource);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbSpeed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pgBar);
            this.Name = "FormDownloader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pgBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbSpeed;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbSource;
    }
}