
namespace V2RayGCon.Views.WinForms
{
    partial class FormShareLinkTypesSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormShareLinkTypesSelector));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flyPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.chkVless = new System.Windows.Forms.CheckBox();
            this.chkTrojan = new System.Windows.Forms.CheckBox();
            this.chkMob = new System.Windows.Forms.CheckBox();
            this.chkVmess = new System.Windows.Forms.CheckBox();
            this.chkShadowsocks = new System.Windows.Forms.CheckBox();
            this.chkSocks = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkHy2 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.flyPanel.SuspendLayout();
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
            this.flyPanel.Controls.Add(this.chkVless);
            this.flyPanel.Controls.Add(this.chkTrojan);
            this.flyPanel.Controls.Add(this.chkMob);
            this.flyPanel.Controls.Add(this.chkVmess);
            this.flyPanel.Controls.Add(this.chkShadowsocks);
            this.flyPanel.Controls.Add(this.chkSocks);
            this.flyPanel.Controls.Add(this.chkHy2);
            this.flyPanel.Name = "flyPanel";
            // 
            // chkVless
            // 
            resources.ApplyResources(this.chkVless, "chkVless");
            this.chkVless.Name = "chkVless";
            this.chkVless.UseVisualStyleBackColor = true;
            // 
            // chkTrojan
            // 
            resources.ApplyResources(this.chkTrojan, "chkTrojan");
            this.chkTrojan.Name = "chkTrojan";
            this.chkTrojan.UseVisualStyleBackColor = true;
            // 
            // chkMob
            // 
            resources.ApplyResources(this.chkMob, "chkMob");
            this.chkMob.Name = "chkMob";
            this.chkMob.UseVisualStyleBackColor = true;
            // 
            // chkVmess
            // 
            resources.ApplyResources(this.chkVmess, "chkVmess");
            this.chkVmess.Name = "chkVmess";
            this.chkVmess.UseVisualStyleBackColor = true;
            // 
            // chkShadowsocks
            // 
            resources.ApplyResources(this.chkShadowsocks, "chkShadowsocks");
            this.chkShadowsocks.Name = "chkShadowsocks";
            this.chkShadowsocks.UseVisualStyleBackColor = true;
            // 
            // chkSocks
            // 
            resources.ApplyResources(this.chkSocks, "chkSocks");
            this.chkSocks.Name = "chkSocks";
            this.chkSocks.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkHy2
            // 
            resources.ApplyResources(this.chkHy2, "chkHy2");
            this.chkHy2.Name = "chkHy2";
            this.chkHy2.UseVisualStyleBackColor = true;
            // 
            // FormShareLinkTypesSelector
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormShareLinkTypesSelector";
            this.groupBox1.ResumeLayout(false);
            this.flyPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.FlowLayoutPanel flyPanel;
        private System.Windows.Forms.CheckBox chkVless;
        private System.Windows.Forms.CheckBox chkTrojan;
        private System.Windows.Forms.CheckBox chkMob;
        private System.Windows.Forms.CheckBox chkVmess;
        private System.Windows.Forms.CheckBox chkShadowsocks;
        private System.Windows.Forms.CheckBox chkSocks;
        private System.Windows.Forms.CheckBox chkHy2;
    }
}
