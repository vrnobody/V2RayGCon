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
            this.rlbTag3 = new VgcApis.UserControls.RoundLabel();
            this.rlbCoreName = new VgcApis.UserControls.RoundLabel();
            this.rlbTag2 = new VgcApis.UserControls.RoundLabel();
            this.rlbTag1 = new VgcApis.UserControls.RoundLabel();
            this.rlbSpeedtest = new VgcApis.UserControls.RoundLabel();
            this.rlbIsRunning = new VgcApis.UserControls.RoundLabel();
            this.rlbSetting = new VgcApis.UserControls.RoundLabel();
            this.rlbMark = new VgcApis.UserControls.RoundLabel();
            this.rlbRemark = new VgcApis.UserControls.RoundLabel();
            this.rlbTotalNetFlow = new VgcApis.UserControls.RoundLabel();
            this.rlbLastModifyDate = new VgcApis.UserControls.RoundLabel();
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
            this.autoShareLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.v2cfgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.showSettingWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textEditortoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.logOfThisServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runSpeedTestToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rtboxServerTitle = new VgcApis.UserControls.ExRichTextBox();
            this.ctxMenuStripMore.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 7000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 96;
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
            // rlbTag3
            // 
            this.rlbTag3._BackColor = System.Drawing.Color.PowderBlue;
            resources.ApplyResources(this.rlbTag3, "rlbTag3");
            this.rlbTag3.AutoEllipsis = true;
            this.rlbTag3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbTag3.ForeColor = System.Drawing.Color.DimGray;
            this.rlbTag3.Name = "rlbTag3";
            this.toolTip1.SetToolTip(this.rlbTag3, resources.GetString("rlbTag3.ToolTip"));
            this.rlbTag3.Click += new System.EventHandler(this.rlbRemark_Click);
            // 
            // rlbCoreName
            // 
            this.rlbCoreName._BackColor = System.Drawing.Color.Lavender;
            resources.ApplyResources(this.rlbCoreName, "rlbCoreName");
            this.rlbCoreName.AutoEllipsis = true;
            this.rlbCoreName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbCoreName.ForeColor = System.Drawing.Color.DimGray;
            this.rlbCoreName.Name = "rlbCoreName";
            this.toolTip1.SetToolTip(this.rlbCoreName, resources.GetString("rlbCoreName.ToolTip"));
            this.rlbCoreName.Click += new System.EventHandler(this.rlbCoreName_Click);
            // 
            // rlbTag2
            // 
            this.rlbTag2._BackColor = System.Drawing.Color.PowderBlue;
            resources.ApplyResources(this.rlbTag2, "rlbTag2");
            this.rlbTag2.AutoEllipsis = true;
            this.rlbTag2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbTag2.ForeColor = System.Drawing.Color.DimGray;
            this.rlbTag2.Name = "rlbTag2";
            this.toolTip1.SetToolTip(this.rlbTag2, resources.GetString("rlbTag2.ToolTip"));
            this.rlbTag2.Click += new System.EventHandler(this.rlbRemark_Click);
            // 
            // rlbTag1
            // 
            this.rlbTag1._BackColor = System.Drawing.Color.PowderBlue;
            resources.ApplyResources(this.rlbTag1, "rlbTag1");
            this.rlbTag1.AutoEllipsis = true;
            this.rlbTag1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbTag1.ForeColor = System.Drawing.Color.DimGray;
            this.rlbTag1.Name = "rlbTag1";
            this.toolTip1.SetToolTip(this.rlbTag1, resources.GetString("rlbTag1.ToolTip"));
            this.rlbTag1.Click += new System.EventHandler(this.rlbRemark_Click);
            // 
            // rlbSpeedtest
            // 
            this.rlbSpeedtest._BackColor = System.Drawing.Color.Wheat;
            resources.ApplyResources(this.rlbSpeedtest, "rlbSpeedtest");
            this.rlbSpeedtest.AutoEllipsis = true;
            this.rlbSpeedtest.BackColor = System.Drawing.SystemColors.Control;
            this.rlbSpeedtest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbSpeedtest.ForeColor = System.Drawing.Color.Red;
            this.rlbSpeedtest.Name = "rlbSpeedtest";
            this.toolTip1.SetToolTip(this.rlbSpeedtest, resources.GetString("rlbSpeedtest.ToolTip"));
            this.rlbSpeedtest.Click += new System.EventHandler(this.rlbSpeedtest_Click);
            // 
            // rlbIsRunning
            // 
            this.rlbIsRunning._BackColor = System.Drawing.Color.DarkOrange;
            resources.ApplyResources(this.rlbIsRunning, "rlbIsRunning");
            this.rlbIsRunning.AutoEllipsis = true;
            this.rlbIsRunning.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbIsRunning.ForeColor = System.Drawing.Color.Ivory;
            this.rlbIsRunning.Name = "rlbIsRunning";
            this.toolTip1.SetToolTip(this.rlbIsRunning, resources.GetString("rlbIsRunning.ToolTip"));
            this.rlbIsRunning.UseCompatibleTextRendering = true;
            this.rlbIsRunning.Click += new System.EventHandler(this.rlbIsRunning_Click);
            // 
            // rlbSetting
            // 
            this.rlbSetting._BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.rlbSetting, "rlbSetting");
            this.rlbSetting.AutoEllipsis = true;
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
            resources.ApplyResources(this.rlbMark, "rlbMark");
            this.rlbMark.AutoEllipsis = true;
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
            resources.ApplyResources(this.rlbRemark, "rlbRemark");
            this.rlbRemark.AutoEllipsis = true;
            this.rlbRemark.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbRemark.ForeColor = System.Drawing.Color.DimGray;
            this.rlbRemark.Name = "rlbRemark";
            this.toolTip1.SetToolTip(this.rlbRemark, resources.GetString("rlbRemark.ToolTip"));
            this.rlbRemark.Click += new System.EventHandler(this.rlbRemark_Click);
            // 
            // rlbTotalNetFlow
            // 
            this.rlbTotalNetFlow._BackColor = System.Drawing.Color.LightSteelBlue;
            resources.ApplyResources(this.rlbTotalNetFlow, "rlbTotalNetFlow");
            this.rlbTotalNetFlow.AutoEllipsis = true;
            this.rlbTotalNetFlow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbTotalNetFlow.ForeColor = System.Drawing.Color.DimGray;
            this.rlbTotalNetFlow.Name = "rlbTotalNetFlow";
            this.toolTip1.SetToolTip(this.rlbTotalNetFlow, resources.GetString("rlbTotalNetFlow.ToolTip"));
            this.rlbTotalNetFlow.Click += new System.EventHandler(this.rlbTotalNetFlow_Click);
            // 
            // rlbLastModifyDate
            // 
            this.rlbLastModifyDate._BackColor = System.Drawing.Color.SandyBrown;
            resources.ApplyResources(this.rlbLastModifyDate, "rlbLastModifyDate");
            this.rlbLastModifyDate.AutoEllipsis = true;
            this.rlbLastModifyDate.BackColor = System.Drawing.SystemColors.Control;
            this.rlbLastModifyDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbLastModifyDate.ForeColor = System.Drawing.Color.DimGray;
            this.rlbLastModifyDate.Name = "rlbLastModifyDate";
            this.toolTip1.SetToolTip(this.rlbLastModifyDate, resources.GetString("rlbLastModifyDate.ToolTip"));
            this.rlbLastModifyDate.Click += new System.EventHandler(this.rlbLastModifyDate_Click);
            // 
            // rlbInboundMode
            // 
            this.rlbInboundMode._BackColor = System.Drawing.Color.MediumTurquoise;
            resources.ApplyResources(this.rlbInboundMode, "rlbInboundMode");
            this.rlbInboundMode.AutoEllipsis = true;
            this.rlbInboundMode.BackColor = System.Drawing.SystemColors.Control;
            this.rlbInboundMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rlbInboundMode.ForeColor = System.Drawing.Color.DimGray;
            this.rlbInboundMode.Name = "rlbInboundMode";
            this.toolTip1.SetToolTip(this.rlbInboundMode, resources.GetString("rlbInboundMode.ToolTip"));
            this.rlbInboundMode.Click += new System.EventHandler(this.rlbInboundMode_Click);
            // 
            // ctxMenuStripMore
            // 
            resources.ApplyResources(this.ctxMenuStripMore, "ctxMenuStripMore");
            this.ctxMenuStripMore.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxMenuStripMore.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem6,
            this.copyToolStripMenuItem,
            this.toolStripMenuItem1,
            this.showSettingWindowToolStripMenuItem,
            this.textEditortoolStripMenuItem,
            this.debugToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem2,
            this.logOfThisServerToolStripMenuItem,
            this.runSpeedTestToolStripMenuItem1});
            this.ctxMenuStripMore.Name = "ctxMenuStripMore";
            this.toolTip1.SetToolTip(this.ctxMenuStripMore, resources.GetString("ctxMenuStripMore.ToolTip"));
            // 
            // toolStripMenuItem3
            // 
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.multiboxingToolStripMenuItem1,
            this.stopToolStripMenuItem1});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            // 
            // startToolStripMenuItem
            // 
            resources.ApplyResources(this.startToolStripMenuItem, "startToolStripMenuItem");
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // multiboxingToolStripMenuItem1
            // 
            resources.ApplyResources(this.multiboxingToolStripMenuItem1, "multiboxingToolStripMenuItem1");
            this.multiboxingToolStripMenuItem1.Name = "multiboxingToolStripMenuItem1";
            this.multiboxingToolStripMenuItem1.Click += new System.EventHandler(this.multiboxingToolStripMenuItem1_Click);
            // 
            // stopToolStripMenuItem1
            // 
            resources.ApplyResources(this.stopToolStripMenuItem1, "stopToolStripMenuItem1");
            this.stopToolStripMenuItem1.Name = "stopToolStripMenuItem1";
            this.stopToolStripMenuItem1.Click += new System.EventHandler(this.stopToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem6
            // 
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToTopToolStripMenuItem,
            this.moveToBottomToolStripMenuItem});
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            // 
            // moveToTopToolStripMenuItem
            // 
            resources.ApplyResources(this.moveToTopToolStripMenuItem, "moveToTopToolStripMenuItem");
            this.moveToTopToolStripMenuItem.Name = "moveToTopToolStripMenuItem";
            this.moveToTopToolStripMenuItem.Click += new System.EventHandler(this.moveToTopToolStripMenuItem_Click);
            // 
            // moveToBottomToolStripMenuItem
            // 
            resources.ApplyResources(this.moveToBottomToolStripMenuItem, "moveToBottomToolStripMenuItem");
            this.moveToBottomToolStripMenuItem.Name = "moveToBottomToolStripMenuItem";
            this.moveToBottomToolStripMenuItem.Click += new System.EventHandler(this.moveToBottomToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoShareLinkToolStripMenuItem,
            this.v2cfgToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            // 
            // autoShareLinkToolStripMenuItem
            // 
            resources.ApplyResources(this.autoShareLinkToolStripMenuItem, "autoShareLinkToolStripMenuItem");
            this.autoShareLinkToolStripMenuItem.Name = "autoShareLinkToolStripMenuItem";
            this.autoShareLinkToolStripMenuItem.Click += new System.EventHandler(this.autoShareLinkToolStripMenuItem_Click);
            // 
            // v2cfgToolStripMenuItem
            // 
            resources.ApplyResources(this.v2cfgToolStripMenuItem, "v2cfgToolStripMenuItem");
            this.v2cfgToolStripMenuItem.Name = "v2cfgToolStripMenuItem";
            this.v2cfgToolStripMenuItem.Click += new System.EventHandler(this.v2cfgToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // showSettingWindowToolStripMenuItem
            // 
            resources.ApplyResources(this.showSettingWindowToolStripMenuItem, "showSettingWindowToolStripMenuItem");
            this.showSettingWindowToolStripMenuItem.Name = "showSettingWindowToolStripMenuItem";
            this.showSettingWindowToolStripMenuItem.Click += new System.EventHandler(this.showSettingsWindowToolStripMenuItem_Click);
            // 
            // textEditortoolStripMenuItem
            // 
            resources.ApplyResources(this.textEditortoolStripMenuItem, "textEditortoolStripMenuItem");
            this.textEditortoolStripMenuItem.Name = "textEditortoolStripMenuItem";
            this.textEditortoolStripMenuItem.Click += new System.EventHandler(this.textEditortoolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            resources.ApplyResources(this.debugToolStripMenuItem, "debugToolStripMenuItem");
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.showFinalConfigToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // logOfThisServerToolStripMenuItem
            // 
            resources.ApplyResources(this.logOfThisServerToolStripMenuItem, "logOfThisServerToolStripMenuItem");
            this.logOfThisServerToolStripMenuItem.Name = "logOfThisServerToolStripMenuItem";
            this.logOfThisServerToolStripMenuItem.Click += new System.EventHandler(this.logOfThisServerToolStripMenuItem_Click);
            // 
            // runSpeedTestToolStripMenuItem1
            // 
            resources.ApplyResources(this.runSpeedTestToolStripMenuItem1, "runSpeedTestToolStripMenuItem1");
            this.runSpeedTestToolStripMenuItem1.Name = "runSpeedTestToolStripMenuItem1";
            this.runSpeedTestToolStripMenuItem1.Click += new System.EventHandler(this.runSpeedTestToolStripMenuItem1_Click);
            // 
            // rtboxServerTitle
            // 
            resources.ApplyResources(this.rtboxServerTitle, "rtboxServerTitle");
            this.rtboxServerTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtboxServerTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.rtboxServerTitle.DetectUrls = false;
            this.rtboxServerTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rtboxServerTitle.Name = "rtboxServerTitle";
            this.rtboxServerTitle.ReadOnly = true;
            this.toolTip1.SetToolTip(this.rtboxServerTitle, resources.GetString("rtboxServerTitle.ToolTip"));
            this.rtboxServerTitle.Click += new System.EventHandler(this.rtboxServerTitle_Click);
            // 
            // ServerUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnMenu);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.rlbTag3);
            this.Controls.Add(this.rlbCoreName);
            this.Controls.Add(this.rlbTag2);
            this.Controls.Add(this.rlbTag1);
            this.Controls.Add(this.rlbSpeedtest);
            this.Controls.Add(this.rlbIsRunning);
            this.Controls.Add(this.rlbSetting);
            this.Controls.Add(this.rlbMark);
            this.Controls.Add(this.rlbRemark);
            this.Controls.Add(this.rlbTotalNetFlow);
            this.Controls.Add(this.rlbLastModifyDate);
            this.Controls.Add(this.rlbInboundMode);
            this.Controls.Add(this.chkSelected);
            this.Controls.Add(this.rtboxServerTitle);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "ServerUI";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ServerListItem_MouseDown);
            this.ctxMenuStripMore.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkSelected;
        private System.Windows.Forms.ContextMenuStrip ctxMenuStripMore;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoShareLinkToolStripMenuItem;
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
        private VgcApis.UserControls.RoundLabel rlbInboundMode;
        private VgcApis.UserControls.RoundLabel rlbSpeedtest;
        private VgcApis.UserControls.RoundLabel rlbTotalNetFlow;
        private VgcApis.UserControls.RoundLabel rlbMark;
        private VgcApis.UserControls.RoundLabel rlbSetting;
        private VgcApis.UserControls.RoundLabel rlbIsRunning;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnMenu;
        private VgcApis.UserControls.RoundLabel rlbRemark;
        private VgcApis.UserControls.RoundLabel rlbLastModifyDate;
        private VgcApis.UserControls.RoundLabel rlbTag1;
        private VgcApis.UserControls.RoundLabel rlbTag2;
        private VgcApis.UserControls.RoundLabel rlbTag3;
        private System.Windows.Forms.ToolStripMenuItem textEditortoolStripMenuItem;
        private VgcApis.UserControls.RoundLabel rlbCoreName;
    }
}
