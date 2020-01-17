namespace Statistics.Views.WinForms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lvStatsTable = new System.Windows.Forms.ListView();
            this.lvName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvCurDown = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvCurUp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvTotalDown = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvTotalUp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoResizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resizeByTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resizeByContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lvStatsTable);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // lvStatsTable
            // 
            this.lvStatsTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvName,
            this.lvCurDown,
            this.lvCurUp,
            this.lvTotalDown,
            this.lvTotalUp,
            this.columnHeader1});
            resources.ApplyResources(this.lvStatsTable, "lvStatsTable");
            this.lvStatsTable.GridLines = true;
            this.lvStatsTable.Name = "lvStatsTable";
            this.lvStatsTable.UseCompatibleStateImageBehavior = false;
            this.lvStatsTable.View = System.Windows.Forms.View.Details;
            // 
            // lvName
            // 
            resources.ApplyResources(this.lvName, "lvName");
            // 
            // lvCurDown
            // 
            resources.ApplyResources(this.lvCurDown, "lvCurDown");
            // 
            // lvCurUp
            // 
            resources.ApplyResources(this.lvCurUp, "lvCurUp");
            // 
            // lvTotalDown
            // 
            resources.ApplyResources(this.lvTotalDown, "lvTotalDown");
            // 
            // lvTotalUp
            // 
            resources.ApplyResources(this.lvTotalUp, "lvTotalUp");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.viewsToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            resources.ApplyResources(this.文件ToolStripMenuItem, "文件ToolStripMenuItem");
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            resources.ApplyResources(this.resetToolStripMenuItem, "resetToolStripMenuItem");
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewsToolStripMenuItem
            // 
            this.viewsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoResizeToolStripMenuItem});
            this.viewsToolStripMenuItem.Name = "viewsToolStripMenuItem";
            resources.ApplyResources(this.viewsToolStripMenuItem, "viewsToolStripMenuItem");
            // 
            // autoResizeToolStripMenuItem
            // 
            this.autoResizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resizeByTitleToolStripMenuItem,
            this.resizeByContentToolStripMenuItem});
            this.autoResizeToolStripMenuItem.Name = "autoResizeToolStripMenuItem";
            resources.ApplyResources(this.autoResizeToolStripMenuItem, "autoResizeToolStripMenuItem");
            // 
            // resizeByTitleToolStripMenuItem
            // 
            this.resizeByTitleToolStripMenuItem.Name = "resizeByTitleToolStripMenuItem";
            resources.ApplyResources(this.resizeByTitleToolStripMenuItem, "resizeByTitleToolStripMenuItem");
            // 
            // resizeByContentToolStripMenuItem
            // 
            this.resizeByContentToolStripMenuItem.Name = "resizeByContentToolStripMenuItem";
            resources.ApplyResources(this.resizeByContentToolStripMenuItem, "resizeByContentToolStripMenuItem");
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView lvStatsTable;
        private System.Windows.Forms.ColumnHeader lvName;
        private System.Windows.Forms.ColumnHeader lvCurDown;
        private System.Windows.Forms.ColumnHeader lvTotalDown;
        private System.Windows.Forms.ColumnHeader lvCurUp;
        private System.Windows.Forms.ColumnHeader lvTotalUp;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoResizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resizeByTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resizeByContentToolStripMenuItem;
    }
}