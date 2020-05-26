namespace Luna.Views.UserControls
{
    partial class LuaUI
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LuaUI));
            this.btnRun = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lbRunningState = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.rlbOptions = new VgcApis.UserControls.RoundLabel();
            this.btnMenuMore = new System.Windows.Forms.Button();
            this.contextMenuStripMore = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terminateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripMore.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            resources.ApplyResources(this.btnRun, "btnRun");
            this.btnRun.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnRun.Name = "btnRun";
            this.toolTip1.SetToolTip(this.btnRun, resources.GetString("btnRun.ToolTip"));
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStop
            // 
            resources.ApplyResources(this.btnStop, "btnStop");
            this.btnStop.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnStop.Name = "btnStop";
            this.toolTip1.SetToolTip(this.btnStop, resources.GetString("btnStop.ToolTip"));
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lbRunningState
            // 
            this.lbRunningState.Cursor = System.Windows.Forms.Cursors.SizeAll;
            resources.ApplyResources(this.lbRunningState, "lbRunningState");
            this.lbRunningState.ForeColor = System.Drawing.Color.Green;
            this.lbRunningState.Name = "lbRunningState";
            this.toolTip1.SetToolTip(this.lbRunningState, resources.GetString("lbRunningState.ToolTip"));
            this.lbRunningState.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbRunningState_MouseDown);
            // 
            // lbName
            // 
            this.lbName.AutoEllipsis = true;
            this.lbName.Cursor = System.Windows.Forms.Cursors.SizeAll;
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.Name = "lbName";
            this.toolTip1.SetToolTip(this.lbName, resources.GetString("lbName.ToolTip"));
            this.lbName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbName_MouseDown);
            // 
            // rlbOptions
            // 
            this.rlbOptions._BackColor = System.Drawing.Color.Wheat;
            this.rlbOptions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbOptions.ForeColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.rlbOptions, "rlbOptions");
            this.rlbOptions.Name = "rlbOptions";
            this.toolTip1.SetToolTip(this.rlbOptions, resources.GetString("rlbOptions.ToolTip"));
            this.rlbOptions.Click += new System.EventHandler(this.rlbOptions_Click);
            // 
            // btnMenuMore
            // 
            resources.ApplyResources(this.btnMenuMore, "btnMenuMore");
            this.btnMenuMore.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnMenuMore.Name = "btnMenuMore";
            this.toolTip1.SetToolTip(this.btnMenuMore, resources.GetString("btnMenuMore.ToolTip"));
            this.btnMenuMore.UseVisualStyleBackColor = true;
            this.btnMenuMore.Click += new System.EventHandler(this.btnMenuMore_Click);
            // 
            // contextMenuStripMore
            // 
            this.contextMenuStripMore.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionToolStripMenuItem,
            this.toolStripMenuItem1,
            this.editToolStripMenuItem,
            this.optionToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuStripMore.Name = "contextMenuStripMore";
            resources.ApplyResources(this.contextMenuStripMore, "contextMenuStripMore");
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restartToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.terminateToolStripMenuItem});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            resources.ApplyResources(this.actionToolStripMenuItem, "actionToolStripMenuItem");
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            resources.ApplyResources(this.restartToolStripMenuItem, "restartToolStripMenuItem");
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // terminateToolStripMenuItem
            // 
            this.terminateToolStripMenuItem.Name = "terminateToolStripMenuItem";
            resources.ApplyResources(this.terminateToolStripMenuItem, "terminateToolStripMenuItem");
            this.terminateToolStripMenuItem.Click += new System.EventHandler(this.terminateToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            resources.ApplyResources(this.optionToolStripMenuItem, "optionToolStripMenuItem");
            this.optionToolStripMenuItem.Click += new System.EventHandler(this.optionToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // LuaUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.rlbOptions);
            this.Controls.Add(this.btnMenuMore);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.lbRunningState);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnRun);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Name = "LuaUI";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.LuaUI_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LuaUI_MouseDown);
            this.contextMenuStripMore.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lbRunningState;
        private System.Windows.Forms.Label lbName;
        private VgcApis.UserControls.RoundLabel rlbOptions;
        private System.Windows.Forms.Button btnMenuMore;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMore;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem terminateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
    }
}
