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
            this.btnKill = new System.Windows.Forms.Button();
            this.chkIsAutoRun = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lbRunningState = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
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
            // btnKill
            // 
            resources.ApplyResources(this.btnKill, "btnKill");
            this.btnKill.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnKill.Name = "btnKill";
            this.toolTip1.SetToolTip(this.btnKill, resources.GetString("btnKill.ToolTip"));
            this.btnKill.UseVisualStyleBackColor = true;
            this.btnKill.Click += new System.EventHandler(this.btnKill_Click);
            // 
            // chkIsAutoRun
            // 
            resources.ApplyResources(this.chkIsAutoRun, "chkIsAutoRun");
            this.chkIsAutoRun.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkIsAutoRun.Name = "chkIsAutoRun";
            this.toolTip1.SetToolTip(this.chkIsAutoRun, resources.GetString("chkIsAutoRun.ToolTip"));
            this.chkIsAutoRun.UseVisualStyleBackColor = true;
            this.chkIsAutoRun.CheckedChanged += new System.EventHandler(this.chkIsAutoRun_CheckedChanged);
            // 
            // lbRunningState
            // 
            this.lbRunningState.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.lbRunningState, "lbRunningState");
            this.lbRunningState.ForeColor = System.Drawing.Color.Green;
            this.lbRunningState.Name = "lbRunningState";
            this.toolTip1.SetToolTip(this.lbRunningState, resources.GetString("lbRunningState.ToolTip"));
            // 
            // lbName
            // 
            this.lbName.AutoEllipsis = true;
            this.lbName.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.Name = "lbName";
            this.toolTip1.SetToolTip(this.lbName, resources.GetString("lbName.ToolTip"));
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Name = "btnRemove";
            this.toolTip1.SetToolTip(this.btnRemove, resources.GetString("btnRemove.ToolTip"));
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // LuaUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.lbRunningState);
            this.Controls.Add(this.chkIsAutoRun);
            this.Controls.Add(this.btnKill);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnRun);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "LuaUI";
            this.Load += new System.EventHandler(this.LuaUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnKill;
        private System.Windows.Forms.CheckBox chkIsAutoRun;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lbRunningState;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Button btnRemove;
    }
}
