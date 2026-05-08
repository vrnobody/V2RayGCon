namespace Composer.Views.WinForms
{
    partial class FormServerSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormServerSelector));
            this.label1 = new System.Windows.Forms.Label();
            this.tboxTag = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboxFilterKw = new VgcApis.UserControls.AcmComboBox();
            this.btnPullServers = new VgcApis.UserControls.RoundButton();
            this.btnRefreshTotal = new VgcApis.UserControls.RoundButton();
            this.btnCancel = new VgcApis.UserControls.RoundButton();
            this.flyCustomServers = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new VgcApis.UserControls.RoundButton();
            this.lbTotal = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panelFilterKeywords = new System.Windows.Forms.Panel();
            this.panelFilterKeywords.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // tboxTag
            // 
            resources.ApplyResources(this.tboxTag, "tboxTag");
            this.tboxTag.Name = "tboxTag";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // cboxFilterKw
            // 
            resources.ApplyResources(this.cboxFilterKw, "cboxFilterKw");
            this.cboxFilterKw.Items.AddRange(new object[] {
            resources.GetString("cboxFilterKw.Items"),
            resources.GetString("cboxFilterKw.Items1"),
            resources.GetString("cboxFilterKw.Items2")});
            this.cboxFilterKw.Name = "cboxFilterKw";
            this.cboxFilterKw.ReadOnly = false;
            this.cboxFilterKw.TextChanged += new System.EventHandler(this.tboxFilterKw_TextChanged);
            // 
            // btnPullServers
            // 
            this.btnPullServers.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.btnPullServers, "btnPullServers");
            this.btnPullServers.Name = "btnPullServers";
            this.toolTip1.SetToolTip(this.btnPullServers, resources.GetString("btnPullServers.ToolTip"));
            this.btnPullServers.UseVisualStyleBackColor = false;
            this.btnPullServers.Click += new System.EventHandler(this.btnPullServers_Click);
            // 
            // btnRefreshTotal
            // 
            resources.ApplyResources(this.btnRefreshTotal, "btnRefreshTotal");
            this.btnRefreshTotal.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnRefreshTotal.Name = "btnRefreshTotal";
            this.toolTip1.SetToolTip(this.btnRefreshTotal, resources.GetString("btnRefreshTotal.ToolTip"));
            this.btnRefreshTotal.UseVisualStyleBackColor = false;
            this.btnRefreshTotal.Click += new System.EventHandler(this.btnRefreshTotal_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // flyCustomServers
            // 
            this.flyCustomServers.AllowDrop = true;
            resources.ApplyResources(this.flyCustomServers, "flyCustomServers");
            this.flyCustomServers.BackColor = System.Drawing.SystemColors.Control;
            this.flyCustomServers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flyCustomServers.Name = "flyCustomServers";
            this.flyCustomServers.DragDrop += new System.Windows.Forms.DragEventHandler(this.flyCustomServers_DragDrop);
            this.flyCustomServers.DragEnter += new System.Windows.Forms.DragEventHandler(this.flyCustomServers_DragEnter);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lbTotal
            // 
            resources.ApplyResources(this.lbTotal, "lbTotal");
            this.lbTotal.Name = "lbTotal";
            // 
            // panelFilterKeywords
            // 
            resources.ApplyResources(this.panelFilterKeywords, "panelFilterKeywords");
            this.panelFilterKeywords.Controls.Add(this.cboxFilterKw);
            this.panelFilterKeywords.Name = "panelFilterKeywords";
            // 
            // FormServerSelector
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelFilterKeywords);
            this.Controls.Add(this.lbTotal);
            this.Controls.Add(this.flyCustomServers);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnRefreshTotal);
            this.Controls.Add(this.btnPullServers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tboxTag);
            this.Controls.Add(this.label1);
            this.Name = "FormServerSelector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormServerSelector_FormClosing);
            this.panelFilterKeywords.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboxTag;
        private System.Windows.Forms.Label label2;
        private VgcApis.UserControls.AcmComboBox cboxFilterKw;
        private System.Windows.Forms.FlowLayoutPanel flyCustomServers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panelFilterKeywords;
        private VgcApis.UserControls.RoundButton btnPullServers;
        private VgcApis.UserControls.RoundButton btnRefreshTotal;
        private VgcApis.UserControls.RoundButton btnCancel;
        private VgcApis.UserControls.RoundButton btnSave;
    }
}
