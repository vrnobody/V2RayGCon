namespace Luna.Views.WinForms
{
    partial class FormDataGrid
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDataGrid));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnCopy = new System.Windows.Forms.Button();
            this.cboxColumnIdx = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tboxFilter = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSelectedToCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllToCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autosizeByHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autosizeByContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAutosizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.lbTotal = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            resources.ApplyResources(this.lbTitle, "lbTitle");
            this.lbTitle.Name = "lbTitle";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // dgvData
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.dgvData.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.dgvData, "dgvData");
            this.dgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Name = "dgvData";
            this.dgvData.RowTemplate.Height = 23;
            // 
            // btnCopy
            // 
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.Name = "btnCopy";
            this.toolTip1.SetToolTip(this.btnCopy, resources.GetString("btnCopy.ToolTip"));
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // cboxColumnIdx
            // 
            this.cboxColumnIdx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxColumnIdx.FormattingEnabled = true;
            resources.ApplyResources(this.cboxColumnIdx, "cboxColumnIdx");
            this.cboxColumnIdx.Name = "cboxColumnIdx";
            this.cboxColumnIdx.SelectedIndexChanged += new System.EventHandler(this.cboxColumnIdx_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tboxFilter
            // 
            resources.ApplyResources(this.tboxFilter, "tboxFilter");
            this.tboxFilter.Name = "tboxFilter";
            this.tboxFilter.TextChanged += new System.EventHandler(this.tboxFilter_TextChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importFromCsvToolStripMenuItem,
            this.exportSelectedToCsvToolStripMenuItem,
            this.exportAllToCsvToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // importFromCsvToolStripMenuItem
            // 
            this.importFromCsvToolStripMenuItem.Name = "importFromCsvToolStripMenuItem";
            resources.ApplyResources(this.importFromCsvToolStripMenuItem, "importFromCsvToolStripMenuItem");
            this.importFromCsvToolStripMenuItem.Click += new System.EventHandler(this.importFromCsvToolStripMenuItem_Click);
            // 
            // exportSelectedToCsvToolStripMenuItem
            // 
            this.exportSelectedToCsvToolStripMenuItem.Name = "exportSelectedToCsvToolStripMenuItem";
            resources.ApplyResources(this.exportSelectedToCsvToolStripMenuItem, "exportSelectedToCsvToolStripMenuItem");
            this.exportSelectedToCsvToolStripMenuItem.Click += new System.EventHandler(this.exportSelectedToCsvToolStripMenuItem_Click);
            // 
            // exportAllToCsvToolStripMenuItem
            // 
            this.exportAllToCsvToolStripMenuItem.Name = "exportAllToCsvToolStripMenuItem";
            resources.ApplyResources(this.exportAllToCsvToolStripMenuItem, "exportAllToCsvToolStripMenuItem");
            this.exportAllToCsvToolStripMenuItem.Click += new System.EventHandler(this.exportAllToCsvToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autosizeByHeaderToolStripMenuItem,
            this.autosizeByContentToolStripMenuItem,
            this.disableAutosizeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // autosizeByHeaderToolStripMenuItem
            // 
            this.autosizeByHeaderToolStripMenuItem.Name = "autosizeByHeaderToolStripMenuItem";
            resources.ApplyResources(this.autosizeByHeaderToolStripMenuItem, "autosizeByHeaderToolStripMenuItem");
            this.autosizeByHeaderToolStripMenuItem.Click += new System.EventHandler(this.autosizeByHeaderToolStripMenuItem_Click);
            // 
            // autosizeByContentToolStripMenuItem
            // 
            this.autosizeByContentToolStripMenuItem.Name = "autosizeByContentToolStripMenuItem";
            resources.ApplyResources(this.autosizeByContentToolStripMenuItem, "autosizeByContentToolStripMenuItem");
            this.autosizeByContentToolStripMenuItem.Click += new System.EventHandler(this.autosizeByContentToolStripMenuItem_Click);
            // 
            // disableAutosizeToolStripMenuItem
            // 
            this.disableAutosizeToolStripMenuItem.Name = "disableAutosizeToolStripMenuItem";
            resources.ApplyResources(this.disableAutosizeToolStripMenuItem, "disableAutosizeToolStripMenuItem");
            this.disableAutosizeToolStripMenuItem.Click += new System.EventHandler(this.disableAutosizeToolStripMenuItem_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lbTotal
            // 
            resources.ApplyResources(this.lbTotal, "lbTotal");
            this.lbTotal.Name = "lbTotal";
            // 
            // FormDataGrid
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbTotal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tboxFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboxColumnIdx);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormDataGrid";
            this.Load += new System.EventHandler(this.FormDataGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cboxColumnIdx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboxFilter;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFromCsvToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSelectedToCsvToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllToCsvToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autosizeByHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autosizeByContentToolStripMenuItem;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ToolStripMenuItem disableAutosizeToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbTotal;
    }
}