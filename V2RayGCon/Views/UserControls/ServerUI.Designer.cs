namespace V2RayGCon.Views.UserControls
{
    partial class ServerUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerUI));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkSelected = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnMenu = new System.Windows.Forms.Button();
            this.rlbIsRunning = new VgcApis.UserControls.RoundLabel();
            this.rlbSetting = new VgcApis.UserControls.RoundLabel();
            this.rlbMark = new VgcApis.UserControls.RoundLabel();
            this.rlbRemark = new VgcApis.UserControls.RoundLabel();
            this.rlbLastModify = new VgcApis.UserControls.RoundLabel();
            this.rlbSpeedtest = new VgcApis.UserControls.RoundLabel();
            this.rlbInboundMode = new VgcApis.UserControls.RoundLabel();
            this.ctxMenuStripMore = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiboxingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vmessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.v2cfgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.showSettingWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.logOfThisServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runSpeedTestToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rtboxServerTitle = new VgcApis.UserControls.ExRichTextBox();
            this.ctxMenuStripMore.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkSelected
            // 
            resources.ApplyResources(this.chkSelected, "chkSelected");
            this.chkSelected.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSelected.Name = "chkSelected";
            this.toolTip1.SetToolTip(this.chkSelected, resources.GetString("chkSelected.ToolTip"));
            this.chkSelected.UseVisualStyleBackColor = true;
            this.chkSelected.CheckedChanged += new System.EventHandler(this.chkSelected_CheckedChanged);
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnStart.ForeColor = System.Drawing.Color.DimGray;
            this.btnStart.Name = "btnStart";
            this.toolTip1.SetToolTip(this.btnStart, resources.GetString("btnStart.ToolTip"));
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            resources.ApplyResources(this.btnStop, "btnStop");
            this.btnStop.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnStop.ForeColor = System.Drawing.Color.Maroon;
            this.btnStop.Name = "btnStop";
            this.toolTip1.SetToolTip(this.btnStop, resources.GetString("btnStop.ToolTip"));
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnMenu
            // 
            resources.ApplyResources(this.btnMenu, "btnMenu");
            this.btnMenu.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnMenu.ForeColor = System.Drawing.Color.DimGray;
            this.btnMenu.Name = "btnMenu";
            this.toolTip1.SetToolTip(this.btnMenu, resources.GetString("btnMenu.ToolTip"));
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnShowPopupMenu_Click);
            // 
            // rlbIsRunning
            // 
            this.rlbIsRunning._BackColor = System.Drawing.Color.DarkOrange;
            this.rlbIsRunning.AutoEllipsis = true;
            this.rlbIsRunning.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.rlbIsRunning, "rlbIsRunning");
            this.rlbIsRunning.ForeColor = System.Drawing.Color.Ivory;
            this.rlbIsRunning.Name = "rlbIsRunning";
            this.toolTip1.SetToolTip(this.rlbIsRunning, resources.GetString("rlbIsRunning.ToolTip"));
            this.rlbIsRunning.UseCompatibleTextRendering = true;
            this.rlbIsRunning.Click += new System.EventHandler(this.rlbIsRunning_Click);
            // 
            // rlbSetting
            // 
            this.rlbSetting._BackColor = System.Drawing.Color.Silver;
            this.rlbSetting.AutoEllipsis = true;
            resources.ApplyResources(this.rlbSetting, "rlbSetting");
            this.rlbSetting.BackColor = System.Drawing.SystemColors.Control;
            this.rlbSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbSetting.ForeColor = System.Drawing.Color.DimGray;
            this.rlbSetting.Name = "rlbSetting";
            this.toolTip1.SetToolTip(this.rlbSetting, resources.GetString("rlbSetting.ToolTip"));
            this.rlbSetting.Click += new System.EventHandler(this.rlbSetting_Click);
            // 
            // rlbMark
            // 
            this.rlbMark._BackColor = System.Drawing.Color.LightGreen;
            this.rlbMark.AutoEllipsis = true;
            resources.ApplyResources(this.rlbMark, "rlbMark");
            this.rlbMark.BackColor = System.Drawing.SystemColors.Control;
            this.rlbMark.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbMark.ForeColor = System.Drawing.Color.DimGray;
            this.rlbMark.Name = "rlbMark";
            this.toolTip1.SetToolTip(this.rlbMark, resources.GetString("rlbMark.ToolTip"));
            this.rlbMark.Click += new System.EventHandler(this.rlbMark_Click);
            // 
            // rlbRemark
            // 
            this.rlbRemark._BackColor = System.Drawing.Color.LightSkyBlue;
            this.rlbRemark.AutoEllipsis = true;
            resources.ApplyResources(this.rlbRemark, "rlbRemark");
            this.rlbRemark.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbRemark.ForeColor = System.Drawing.Color.DimGray;
            this.rlbRemark.Name = "rlbRemark";
            this.toolTip1.SetToolTip(this.rlbRemark, resources.GetString("rlbRemark.ToolTip"));
            this.rlbRemark.Click += new System.EventHandler(this.rlbRemark_Click);
            // 
            // rlbLastModify
            // 
            this.rlbLastModify._BackColor = System.Drawing.Color.SandyBrown;
            this.rlbLastModify.AutoEllipsis = true;
            resources.ApplyResources(this.rlbLastModify, "rlbLastModify");
            this.rlbLastModify.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.rlbLastModify.ForeColor = System.Drawing.Color.DimGray;
            this.rlbLastModify.Name = "rlbLastModify";
            this.toolTip1.SetToolTip(this.rlbLastModify, resources.GetString("rlbLastModify.ToolTip"));
            this.rlbLastModify.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rlbLastModify_MouseDown);
            // 
            // rlbSpeedtest
            // 
            this.rlbSpeedtest._BackColor = System.Drawing.Color.Wheat;
            this.rlbSpeedtest.AutoEllipsis = true;
            resources.ApplyResources(this.rlbSpeedtest, "rlbSpeedtest");
            this.rlbSpeedtest.BackColor = System.Drawing.SystemColors.Control;
            this.rlbSpeedtest.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.rlbSpeedtest.ForeColor = System.Drawing.Color.Red;
            this.rlbSpeedtest.Name = "rlbSpeedtest";
            this.toolTip1.SetToolTip(this.rlbSpeedtest, resources.GetString("rlbSpeedtest.ToolTip"));
            this.rlbSpeedtest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rlbSpeedtest_MouseDown);
            // 
            // rlbInboundMode
            // 
            this.rlbInboundMode._BackColor = System.Drawing.Color.MediumTurquoise;
            this.rlbInboundMode.AutoEllipsis = true;
            resources.ApplyResources(this.rlbInboundMode, "rlbInboundMode");
            this.rlbInboundMode.BackColor = System.Drawing.SystemColors.Control;
            this.rlbInboundMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbInboundMode.ForeColor = System.Drawing.Color.DimGray;
            this.rlbInboundMode.Name = "rlbInboundMode";
            this.toolTip1.SetToolTip(this.rlbInboundMode, resources.GetString("rlbInboundMode.ToolTip"));
            this.rlbInboundMode.Click += new System.EventHandler(this.rlbInboundMode_Click);
            // 
            // ctxMenuStripMore
            // 
            this.ctxMenuStripMore.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxMenuStripMore.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem6,
            this.copyToolStripMenuItem,
            this.toolStripMenuItem1,
            this.showSettingWindowToolStripMenuItem,
            this.editToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem2,
            this.logOfThisServerToolStripMenuItem,
            this.runSpeedTestToolStripMenuItem1});
            this.ctxMenuStripMore.Name = "ctxMenuStripMore";
            resources.ApplyResources(this.ctxMenuStripMore, "ctxMenuStripMore");
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.multiboxingToolStripMenuItem1,
            this.stopToolStripMenuItem1});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            resources.ApplyResources(this.startToolStripMenuItem, "startToolStripMenuItem");
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // multiboxingToolStripMenuItem1
            // 
            this.multiboxingToolStripMenuItem1.Name = "multiboxingToolStripMenuItem1";
            resources.ApplyResources(this.multiboxingToolStripMenuItem1, "multiboxingToolStripMenuItem1");
            this.multiboxingToolStripMenuItem1.Click += new System.EventHandler(this.multiboxingToolStripMenuItem1_Click);
            // 
            // stopToolStripMenuItem1
            // 
            this.stopToolStripMenuItem1.Name = "stopToolStripMenuItem1";
            resources.ApplyResources(this.stopToolStripMenuItem1, "stopToolStripMenuItem1");
            this.stopToolStripMenuItem1.Click += new System.EventHandler(this.stopToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToTopToolStripMenuItem,
            this.moveToBottomToolStripMenuItem});
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            // 
            // moveToTopToolStripMenuItem
            // 
            this.moveToTopToolStripMenuItem.Name = "moveToTopToolStripMenuItem";
            resources.ApplyResources(this.moveToTopToolStripMenuItem, "moveToTopToolStripMenuItem");
            this.moveToTopToolStripMenuItem.Click += new System.EventHandler(this.moveToTopToolStripMenuItem_Click);
            // 
            // moveToBottomToolStripMenuItem
            // 
            this.moveToBottomToolStripMenuItem.Name = "moveToBottomToolStripMenuItem";
            resources.ApplyResources(this.moveToBottomToolStripMenuItem, "moveToBottomToolStripMenuItem");
            this.moveToBottomToolStripMenuItem.Click += new System.EventHandler(this.moveToBottomToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vToolStripMenuItem,
            this.vmessToolStripMenuItem,
            this.v2cfgToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            // 
            // vToolStripMenuItem
            // 
            this.vToolStripMenuItem.Name = "vToolStripMenuItem";
            resources.ApplyResources(this.vToolStripMenuItem, "vToolStripMenuItem");
            this.vToolStripMenuItem.Click += new System.EventHandler(this.vToolStripMenuItem_Click);
            // 
            // vmessToolStripMenuItem
            // 
            this.vmessToolStripMenuItem.Name = "vmessToolStripMenuItem";
            resources.ApplyResources(this.vmessToolStripMenuItem, "vmessToolStripMenuItem");
            this.vmessToolStripMenuItem.Click += new System.EventHandler(this.vmessToolStripMenuItem_Click);
            // 
            // v2cfgToolStripMenuItem
            // 
            this.v2cfgToolStripMenuItem.Name = "v2cfgToolStripMenuItem";
            resources.ApplyResources(this.v2cfgToolStripMenuItem, "v2cfgToolStripMenuItem");
            this.v2cfgToolStripMenuItem.Click += new System.EventHandler(this.v2cfgToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // showSettingWindowToolStripMenuItem
            // 
            this.showSettingWindowToolStripMenuItem.Name = "showSettingWindowToolStripMenuItem";
            resources.ApplyResources(this.showSettingWindowToolStripMenuItem, "showSettingWindowToolStripMenuItem");
            this.showSettingWindowToolStripMenuItem.Click += new System.EventHandler(this.showSettingsWindowToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            resources.ApplyResources(this.debugToolStripMenuItem, "debugToolStripMenuItem");
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // logOfThisServerToolStripMenuItem
            // 
            this.logOfThisServerToolStripMenuItem.Name = "logOfThisServerToolStripMenuItem";
            resources.ApplyResources(this.logOfThisServerToolStripMenuItem, "logOfThisServerToolStripMenuItem");
            this.logOfThisServerToolStripMenuItem.Click += new System.EventHandler(this.logOfThisServerToolStripMenuItem_Click);
            // 
            // runSpeedTestToolStripMenuItem1
            // 
            this.runSpeedTestToolStripMenuItem1.Name = "runSpeedTestToolStripMenuItem1";
            resources.ApplyResources(this.runSpeedTestToolStripMenuItem1, "runSpeedTestToolStripMenuItem1");
            this.runSpeedTestToolStripMenuItem1.Click += new System.EventHandler(this.runSpeedTestToolStripMenuItem1_Click);
            // 
            // rtboxServerTitle
            // 
            this.rtboxServerTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtboxServerTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rtboxServerTitle.DetectUrls = false;
            resources.ApplyResources(this.rtboxServerTitle, "rtboxServerTitle");
            this.rtboxServerTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rtboxServerTitle.Name = "rtboxServerTitle";
            this.rtboxServerTitle.ReadOnly = true;
            this.rtboxServerTitle.Click += new System.EventHandler(this.rtboxServerTitle_Click);
            // 
            // ServerUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnMenu);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.rlbIsRunning);
            this.Controls.Add(this.rlbSetting);
            this.Controls.Add(this.rlbMark);
            this.Controls.Add(this.rlbRemark);
            this.Controls.Add(this.rlbLastModify);
            this.Controls.Add(this.rlbSpeedtest);
            this.Controls.Add(this.rlbInboundMode);
            this.Controls.Add(this.chkSelected);
            this.Controls.Add(this.rtboxServerTitle);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Name = "ServerUI";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.ServerUI_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ServerListItem_MouseDown);
            this.ctxMenuStripMore.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkSelected;
        private System.Windows.Forms.ContextMenuStrip ctxMenuStripMore;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vmessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem v2cfgToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem logOfThisServerToolStripMenuItem;
        private VgcApis.UserControls.ExRichTextBox rtboxServerTitle;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiboxingToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showSettingWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runSpeedTestToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem moveToTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vToolStripMenuItem;
        private VgcApis.UserControls.RoundLabel rlbInboundMode;
        private VgcApis.UserControls.RoundLabel rlbSpeedtest;
        private VgcApis.UserControls.RoundLabel rlbLastModify;
        private VgcApis.UserControls.RoundLabel rlbMark;
        private VgcApis.UserControls.RoundLabel rlbSetting;
        private VgcApis.UserControls.RoundLabel rlbIsRunning;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnMenu;
        private VgcApis.UserControls.RoundLabel rlbRemark;
    }
}
