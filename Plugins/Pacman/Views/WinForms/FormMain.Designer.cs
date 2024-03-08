namespace Pacman.Views.WinForms
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnChain = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.tboxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboxObsUrl = new System.Windows.Forms.ComboBox();
            this.cboxObsInterval = new System.Windows.Forms.ComboBox();
            this.cboxBalancerStrategy = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPull = new System.Windows.Forms.Button();
            this.btnRefreshSelected = new System.Windows.Forms.Button();
            this.btnDeleteSelected = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectInvert = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.flyContents = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstBoxPackages = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox3, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.btnChain);
            this.groupBox2.Controls.Add(this.btnImport);
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.tboxName);
            this.groupBox2.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnChain
            // 
            resources.ApplyResources(this.btnChain, "btnChain");
            this.btnChain.Name = "btnChain";
            this.toolTip1.SetToolTip(this.btnChain, resources.GetString("btnChain.ToolTip"));
            this.btnChain.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            resources.ApplyResources(this.btnImport, "btnImport");
            this.btnImport.Name = "btnImport";
            this.toolTip1.SetToolTip(this.btnImport, resources.GetString("btnImport.ToolTip"));
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.toolTip1.SetToolTip(this.btnDelete, resources.GetString("btnDelete.ToolTip"));
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // tboxName
            // 
            resources.ApplyResources(this.tboxName, "tboxName");
            this.tboxName.Name = "tboxName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.tableLayoutPanel3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cboxObsUrl);
            this.panel1.Controls.Add(this.cboxObsInterval);
            this.panel1.Controls.Add(this.cboxBalancerStrategy);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnPull);
            this.panel1.Controls.Add(this.btnRefreshSelected);
            this.panel1.Controls.Add(this.btnDeleteSelected);
            this.panel1.Controls.Add(this.btnSelectNone);
            this.panel1.Controls.Add(this.btnSelectInvert);
            this.panel1.Controls.Add(this.btnSelectAll);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // cboxObsUrl
            // 
            resources.ApplyResources(this.cboxObsUrl, "cboxObsUrl");
            this.cboxObsUrl.FormattingEnabled = true;
            this.cboxObsUrl.Items.AddRange(new object[] {
            resources.GetString("cboxObsUrl.Items"),
            resources.GetString("cboxObsUrl.Items1"),
            resources.GetString("cboxObsUrl.Items2"),
            resources.GetString("cboxObsUrl.Items3"),
            resources.GetString("cboxObsUrl.Items4"),
            resources.GetString("cboxObsUrl.Items5"),
            resources.GetString("cboxObsUrl.Items6"),
            resources.GetString("cboxObsUrl.Items7")});
            this.cboxObsUrl.Name = "cboxObsUrl";
            // 
            // cboxObsInterval
            // 
            resources.ApplyResources(this.cboxObsInterval, "cboxObsInterval");
            this.cboxObsInterval.FormattingEnabled = true;
            this.cboxObsInterval.Items.AddRange(new object[] {
            resources.GetString("cboxObsInterval.Items"),
            resources.GetString("cboxObsInterval.Items1"),
            resources.GetString("cboxObsInterval.Items2"),
            resources.GetString("cboxObsInterval.Items3")});
            this.cboxObsInterval.Name = "cboxObsInterval";
            // 
            // cboxBalancerStrategy
            // 
            this.cboxBalancerStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxBalancerStrategy.FormattingEnabled = true;
            this.cboxBalancerStrategy.Items.AddRange(new object[] {
            resources.GetString("cboxBalancerStrategy.Items"),
            resources.GetString("cboxBalancerStrategy.Items1"),
            resources.GetString("cboxBalancerStrategy.Items2"),
            resources.GetString("cboxBalancerStrategy.Items3")});
            resources.ApplyResources(this.cboxBalancerStrategy, "cboxBalancerStrategy");
            this.cboxBalancerStrategy.Name = "cboxBalancerStrategy";
            this.cboxBalancerStrategy.SelectedIndexChanged += new System.EventHandler(this.cboxBalancerStrategy_SelectedIndexChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // btnPull
            // 
            resources.ApplyResources(this.btnPull, "btnPull");
            this.btnPull.Name = "btnPull";
            this.toolTip1.SetToolTip(this.btnPull, resources.GetString("btnPull.ToolTip"));
            this.btnPull.UseVisualStyleBackColor = true;
            // 
            // btnRefreshSelected
            // 
            resources.ApplyResources(this.btnRefreshSelected, "btnRefreshSelected");
            this.btnRefreshSelected.Name = "btnRefreshSelected";
            this.toolTip1.SetToolTip(this.btnRefreshSelected, resources.GetString("btnRefreshSelected.ToolTip"));
            this.btnRefreshSelected.UseVisualStyleBackColor = true;
            // 
            // btnDeleteSelected
            // 
            resources.ApplyResources(this.btnDeleteSelected, "btnDeleteSelected");
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.toolTip1.SetToolTip(this.btnDeleteSelected, resources.GetString("btnDeleteSelected.ToolTip"));
            this.btnDeleteSelected.UseVisualStyleBackColor = true;
            // 
            // btnSelectNone
            // 
            resources.ApplyResources(this.btnSelectNone, "btnSelectNone");
            this.btnSelectNone.Name = "btnSelectNone";
            this.toolTip1.SetToolTip(this.btnSelectNone, resources.GetString("btnSelectNone.ToolTip"));
            this.btnSelectNone.UseVisualStyleBackColor = true;
            // 
            // btnSelectInvert
            // 
            resources.ApplyResources(this.btnSelectInvert, "btnSelectInvert");
            this.btnSelectInvert.Name = "btnSelectInvert";
            this.toolTip1.SetToolTip(this.btnSelectInvert, resources.GetString("btnSelectInvert.ToolTip"));
            this.btnSelectInvert.UseVisualStyleBackColor = true;
            // 
            // btnSelectAll
            // 
            resources.ApplyResources(this.btnSelectAll, "btnSelectAll");
            this.btnSelectAll.Name = "btnSelectAll";
            this.toolTip1.SetToolTip(this.btnSelectAll, resources.GetString("btnSelectAll.ToolTip"));
            this.btnSelectAll.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.flyContents);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // flyContents
            // 
            this.flyContents.AllowDrop = true;
            resources.ApplyResources(this.flyContents, "flyContents");
            this.flyContents.BackColor = System.Drawing.SystemColors.Window;
            this.flyContents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flyContents.Name = "flyContents";
            this.flyContents.Scroll += new System.Windows.Forms.ScrollEventHandler(this.flyContents_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstBoxPackages);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lstBoxPackages
            // 
            resources.ApplyResources(this.lstBoxPackages, "lstBoxPackages");
            this.lstBoxPackages.FormattingEnabled = true;
            this.lstBoxPackages.Items.AddRange(new object[] {
            resources.GetString("lstBoxPackages.Items"),
            resources.GetString("lstBoxPackages.Items1"),
            resources.GetString("lstBoxPackages.Items2")});
            this.lstBoxPackages.Name = "lstBoxPackages";
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormMain";
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnPull;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox tboxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flyContents;
        private System.Windows.Forms.ListBox lstBoxPackages;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRefreshSelected;
        private System.Windows.Forms.Button btnDeleteSelected;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectInvert;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.ComboBox cboxBalancerStrategy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnChain;
        private System.Windows.Forms.ComboBox cboxObsUrl;
        private System.Windows.Forms.ComboBox cboxObsInterval;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}
