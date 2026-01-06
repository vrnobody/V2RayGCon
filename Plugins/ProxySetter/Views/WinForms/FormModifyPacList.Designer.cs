namespace ProxySetter.Views.WinForms
{
    partial class FormModifyPacList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModifyPacList));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtboxWhiteList = new System.Windows.Forms.RichTextBox();
            this.btnSortWhiteList = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rtboxBlackList = new System.Windows.Forms.RichTextBox();
            this.btnSortBlackList = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtboxWhiteList);
            this.groupBox1.Controls.Add(this.btnSortWhiteList);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // rtboxWhiteList
            // 
            resources.ApplyResources(this.rtboxWhiteList, "rtboxWhiteList");
            this.rtboxWhiteList.Name = "rtboxWhiteList";
            // 
            // btnSortWhiteList
            // 
            resources.ApplyResources(this.btnSortWhiteList, "btnSortWhiteList");
            this.btnSortWhiteList.Name = "btnSortWhiteList";
            this.toolTip1.SetToolTip(this.btnSortWhiteList, resources.GetString("btnSortWhiteList.ToolTip"));
            this.btnSortWhiteList.UseVisualStyleBackColor = true;
            this.btnSortWhiteList.Click += new System.EventHandler(this.btnSortWhiteList_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rtboxBlackList);
            this.groupBox2.Controls.Add(this.btnSortBlackList);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // rtboxBlackList
            // 
            resources.ApplyResources(this.rtboxBlackList, "rtboxBlackList");
            this.rtboxBlackList.Name = "rtboxBlackList";
            // 
            // btnSortBlackList
            // 
            resources.ApplyResources(this.btnSortBlackList, "btnSortBlackList");
            this.btnSortBlackList.Name = "btnSortBlackList";
            this.toolTip1.SetToolTip(this.btnSortBlackList, resources.GetString("btnSortBlackList.ToolTip"));
            this.btnSortBlackList.UseVisualStyleBackColor = true;
            this.btnSortBlackList.Click += new System.EventHandler(this.btnSortBlackList_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormModifyPacList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormModifyPacList";
            this.Load += new System.EventHandler(this.FormModifyPacList_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSortWhiteList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox rtboxWhiteList;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RichTextBox rtboxBlackList;
        private System.Windows.Forms.Button btnSortBlackList;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}