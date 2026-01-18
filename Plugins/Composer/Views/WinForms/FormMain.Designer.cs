namespace Composer.Views.WinForms
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
            this.tableLayoutPanelBackGroundLeftRight = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flyPkgNames = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel2UpDown = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3LeftRight = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboxSkelectonTpl = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rtboxSkelecton = new VgcApis.UserControls.ExRichTextBox();
            this.btnSkFormat = new System.Windows.Forms.Button();
            this.btnLoadSkTpl = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cboxNodeInsertPos = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.flySelectors = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOpenFormServerSelector = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tboxPkgName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChain = new System.Windows.Forms.Button();
            this.btnPack = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanelBackGroundLeftRight.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2UpDown.SuspendLayout();
            this.tableLayoutPanel3LeftRight.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelBackGroundLeftRight
            // 
            resources.ApplyResources(this.tableLayoutPanelBackGroundLeftRight, "tableLayoutPanelBackGroundLeftRight");
            this.tableLayoutPanelBackGroundLeftRight.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanelBackGroundLeftRight.Controls.Add(this.tableLayoutPanel2UpDown, 1, 0);
            this.tableLayoutPanelBackGroundLeftRight.Name = "tableLayoutPanelBackGroundLeftRight";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flyPkgNames);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // flyPkgNames
            // 
            this.flyPkgNames.AllowDrop = true;
            resources.ApplyResources(this.flyPkgNames, "flyPkgNames");
            this.flyPkgNames.BackColor = System.Drawing.SystemColors.Control;
            this.flyPkgNames.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flyPkgNames.Name = "flyPkgNames";
            this.flyPkgNames.DragDrop += new System.Windows.Forms.DragEventHandler(this.flyPkgNames_DragDrop);
            this.flyPkgNames.DragEnter += new System.Windows.Forms.DragEventHandler(this.flyPkgNames_DragEnter);
            // 
            // tableLayoutPanel2UpDown
            // 
            resources.ApplyResources(this.tableLayoutPanel2UpDown, "tableLayoutPanel2UpDown");
            this.tableLayoutPanel2UpDown.Controls.Add(this.tableLayoutPanel3LeftRight, 0, 1);
            this.tableLayoutPanel2UpDown.Controls.Add(this.groupBox4, 0, 0);
            this.tableLayoutPanel2UpDown.Name = "tableLayoutPanel2UpDown";
            // 
            // tableLayoutPanel3LeftRight
            // 
            resources.ApplyResources(this.tableLayoutPanel3LeftRight, "tableLayoutPanel3LeftRight");
            this.tableLayoutPanel3LeftRight.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel3LeftRight.Controls.Add(this.groupBox3, 1, 0);
            this.tableLayoutPanel3LeftRight.Name = "tableLayoutPanel3LeftRight";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cboxSkelectonTpl);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.rtboxSkelecton);
            this.groupBox2.Controls.Add(this.btnSkFormat);
            this.groupBox2.Controls.Add(this.btnLoadSkTpl);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // cboxSkelectonTpl
            // 
            resources.ApplyResources(this.cboxSkelectonTpl, "cboxSkelectonTpl");
            this.cboxSkelectonTpl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxSkelectonTpl.FormattingEnabled = true;
            this.cboxSkelectonTpl.Name = "cboxSkelectonTpl";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // rtboxSkelecton
            // 
            this.rtboxSkelecton.AcceptsTab = true;
            resources.ApplyResources(this.rtboxSkelecton, "rtboxSkelecton");
            this.rtboxSkelecton.Name = "rtboxSkelecton";
            this.rtboxSkelecton.TabStop = false;
            // 
            // btnSkFormat
            // 
            resources.ApplyResources(this.btnSkFormat, "btnSkFormat");
            this.btnSkFormat.Name = "btnSkFormat";
            this.toolTip1.SetToolTip(this.btnSkFormat, resources.GetString("btnSkFormat.ToolTip"));
            this.btnSkFormat.UseVisualStyleBackColor = true;
            this.btnSkFormat.Click += new System.EventHandler(this.btnSkFormat_Click);
            // 
            // btnLoadSkTpl
            // 
            resources.ApplyResources(this.btnLoadSkTpl, "btnLoadSkTpl");
            this.btnLoadSkTpl.Name = "btnLoadSkTpl";
            this.toolTip1.SetToolTip(this.btnLoadSkTpl, resources.GetString("btnLoadSkTpl.ToolTip"));
            this.btnLoadSkTpl.UseVisualStyleBackColor = true;
            this.btnLoadSkTpl.Click += new System.EventHandler(this.btnLoadSkTpl_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cboxNodeInsertPos);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.flySelectors);
            this.groupBox3.Controls.Add(this.btnOpenFormServerSelector);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // cboxNodeInsertPos
            // 
            resources.ApplyResources(this.cboxNodeInsertPos, "cboxNodeInsertPos");
            this.cboxNodeInsertPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxNodeInsertPos.FormattingEnabled = true;
            this.cboxNodeInsertPos.Items.AddRange(new object[] {
            resources.GetString("cboxNodeInsertPos.Items"),
            resources.GetString("cboxNodeInsertPos.Items1")});
            this.cboxNodeInsertPos.Name = "cboxNodeInsertPos";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // flySelectors
            // 
            this.flySelectors.AllowDrop = true;
            resources.ApplyResources(this.flySelectors, "flySelectors");
            this.flySelectors.BackColor = System.Drawing.SystemColors.Control;
            this.flySelectors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flySelectors.Name = "flySelectors";
            this.flySelectors.DragDrop += new System.Windows.Forms.DragEventHandler(this.flySelectors_DragDrop);
            this.flySelectors.DragEnter += new System.Windows.Forms.DragEventHandler(this.flySelectors_DragEnter);
            // 
            // btnOpenFormServerSelector
            // 
            resources.ApplyResources(this.btnOpenFormServerSelector, "btnOpenFormServerSelector");
            this.btnOpenFormServerSelector.Name = "btnOpenFormServerSelector";
            this.toolTip1.SetToolTip(this.btnOpenFormServerSelector, resources.GetString("btnOpenFormServerSelector.ToolTip"));
            this.btnOpenFormServerSelector.UseVisualStyleBackColor = true;
            this.btnOpenFormServerSelector.Click += new System.EventHandler(this.btnNewNodeFilter_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tboxPkgName);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.btnChain);
            this.groupBox4.Controls.Add(this.btnPack);
            this.groupBox4.Controls.Add(this.btnSave);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // tboxPkgName
            // 
            resources.ApplyResources(this.tboxPkgName, "tboxPkgName");
            this.tboxPkgName.Name = "tboxPkgName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // btnChain
            // 
            resources.ApplyResources(this.btnChain, "btnChain");
            this.btnChain.Name = "btnChain";
            this.toolTip1.SetToolTip(this.btnChain, resources.GetString("btnChain.ToolTip"));
            this.btnChain.UseVisualStyleBackColor = true;
            this.btnChain.Click += new System.EventHandler(this.btnChain_Click);
            // 
            // btnPack
            // 
            resources.ApplyResources(this.btnPack, "btnPack");
            this.btnPack.Name = "btnPack";
            this.toolTip1.SetToolTip(this.btnPack, resources.GetString("btnPack.ToolTip"));
            this.btnPack.UseVisualStyleBackColor = true;
            this.btnPack.Click += new System.EventHandler(this.btnPack_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelBackGroundLeftRight);
            this.Name = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanelBackGroundLeftRight.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2UpDown.ResumeLayout(false);
            this.tableLayoutPanel3LeftRight.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBackGroundLeftRight;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flyPkgNames;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2UpDown;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3LeftRight;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private VgcApis.UserControls.ExRichTextBox rtboxSkelecton;
        private System.Windows.Forms.FlowLayoutPanel flySelectors;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox tboxPkgName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpenFormServerSelector;
        private System.Windows.Forms.ComboBox cboxSkelectonTpl;
        private System.Windows.Forms.Button btnLoadSkTpl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboxNodeInsertPos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPack;
        private System.Windows.Forms.Button btnSkFormat;
        private System.Windows.Forms.Button btnChain;
    }
}
