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
            this.lbStatus = new System.Windows.Forms.Label();
            this.cboxInbound = new System.Windows.Forms.ComboBox();
            this.btnMenu = new System.Windows.Forms.Button();
            this.lbRunning = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkSelected = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.cboxMark = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnIsCollapse = new System.Windows.Forms.Button();
            this.lbIsAutorun = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnMultiboxing = new System.Windows.Forms.Button();
            this.lbLastModify = new System.Windows.Forms.Label();
            this.cboxInboundAddr = new System.Windows.Forms.ComboBox();
            this.ctxMenuStripMore = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiboxingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.autorunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.globalImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skipCNWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.untrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vmessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.v2cfgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.logOfThisServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runSpeedTestToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rtboxServerTitle = new System.Windows.Forms.RichTextBox();
            this.ctxMenuStripMore.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbStatus
            // 
            this.lbStatus.AutoEllipsis = true;
            this.lbStatus.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.lbStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.lbStatus, "lbStatus");
            this.lbStatus.Name = "lbStatus";
            this.toolTip1.SetToolTip(this.lbStatus, resources.GetString("lbStatus.ToolTip"));
            this.lbStatus.UseCompatibleTextRendering = true;
            this.lbStatus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbStatus_MouseDown);
            // 
            // cboxInbound
            // 
            this.cboxInbound.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboxInbound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxInbound.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cboxInbound.FormattingEnabled = true;
            this.cboxInbound.Items.AddRange(new object[] {
            resources.GetString("cboxInbound.Items"),
            resources.GetString("cboxInbound.Items1"),
            resources.GetString("cboxInbound.Items2")});
            resources.ApplyResources(this.cboxInbound, "cboxInbound");
            this.cboxInbound.Name = "cboxInbound";
            this.toolTip1.SetToolTip(this.cboxInbound, resources.GetString("cboxInbound.ToolTip"));
            this.cboxInbound.SelectedIndexChanged += new System.EventHandler(this.cboxInbound_SelectedIndexChanged);
            // 
            // btnMenu
            // 
            resources.ApplyResources(this.btnMenu, "btnMenu");
            this.btnMenu.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnMenu.Name = "btnMenu";
            this.toolTip1.SetToolTip(this.btnMenu, resources.GetString("btnMenu.ToolTip"));
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnAction_Click);
            // 
            // lbRunning
            // 
            this.lbRunning.Cursor = System.Windows.Forms.Cursors.SizeAll;
            resources.ApplyResources(this.lbRunning, "lbRunning");
            this.lbRunning.ForeColor = System.Drawing.Color.Green;
            this.lbRunning.Name = "lbRunning";
            this.toolTip1.SetToolTip(this.lbRunning, resources.GetString("lbRunning.ToolTip"));
            this.lbRunning.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbRunning_MouseDown);
            // 
            // chkSelected
            // 
            resources.ApplyResources(this.chkSelected, "chkSelected");
            this.chkSelected.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkSelected.Name = "chkSelected";
            this.toolTip1.SetToolTip(this.chkSelected, resources.GetString("chkSelected.ToolTip"));
            this.chkSelected.UseVisualStyleBackColor = true;
            this.chkSelected.CheckedChanged += new System.EventHandler(this.chkSelected_CheckedChanged);
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnStart.Name = "btnStart";
            this.toolTip1.SetToolTip(this.btnStart, resources.GetString("btnStart.ToolTip"));
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // cboxMark
            // 
            this.cboxMark.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboxMark.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cboxMark.FormattingEnabled = true;
            resources.ApplyResources(this.cboxMark, "cboxMark");
            this.cboxMark.Name = "cboxMark";
            this.toolTip1.SetToolTip(this.cboxMark, resources.GetString("cboxMark.ToolTip"));
            this.cboxMark.DropDown += new System.EventHandler(this.cboxMark_DropDown);
            this.cboxMark.TextChanged += new System.EventHandler(this.cboxMark_TextChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            this.label4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label4_MouseDown);
            // 
            // btnIsCollapse
            // 
            resources.ApplyResources(this.btnIsCollapse, "btnIsCollapse");
            this.btnIsCollapse.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnIsCollapse.Name = "btnIsCollapse";
            this.toolTip1.SetToolTip(this.btnIsCollapse, resources.GetString("btnIsCollapse.ToolTip"));
            this.btnIsCollapse.UseVisualStyleBackColor = true;
            this.btnIsCollapse.Click += new System.EventHandler(this.btnIsCollapse_Click);
            // 
            // lbIsAutorun
            // 
            this.lbIsAutorun.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.lbIsAutorun.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            resources.ApplyResources(this.lbIsAutorun, "lbIsAutorun");
            this.lbIsAutorun.Name = "lbIsAutorun";
            this.toolTip1.SetToolTip(this.lbIsAutorun, resources.GetString("lbIsAutorun.ToolTip"));
            this.lbIsAutorun.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbIsAutorun_MouseDown);
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
            // btnMultiboxing
            // 
            resources.ApplyResources(this.btnMultiboxing, "btnMultiboxing");
            this.btnMultiboxing.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnMultiboxing.Name = "btnMultiboxing";
            this.toolTip1.SetToolTip(this.btnMultiboxing, resources.GetString("btnMultiboxing.ToolTip"));
            this.btnMultiboxing.UseVisualStyleBackColor = true;
            this.btnMultiboxing.Click += new System.EventHandler(this.btnMultiboxing_Click);
            // 
            // lbLastModify
            // 
            this.lbLastModify.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            resources.ApplyResources(this.lbLastModify, "lbLastModify");
            this.lbLastModify.Name = "lbLastModify";
            this.toolTip1.SetToolTip(this.lbLastModify, resources.GetString("lbLastModify.ToolTip"));
            this.lbLastModify.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LbAddTimestamp_MouseDown);
            // 
            // cboxInboundAddr
            // 
            this.cboxInboundAddr.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboxInboundAddr.FormattingEnabled = true;
            this.cboxInboundAddr.Items.AddRange(new object[] {
            resources.GetString("cboxInboundAddr.Items"),
            resources.GetString("cboxInboundAddr.Items1"),
            resources.GetString("cboxInboundAddr.Items2"),
            resources.GetString("cboxInboundAddr.Items3")});
            resources.ApplyResources(this.cboxInboundAddr, "cboxInboundAddr");
            this.cboxInboundAddr.Name = "cboxInboundAddr";
            this.toolTip1.SetToolTip(this.cboxInboundAddr, resources.GetString("cboxInboundAddr.ToolTip"));
            this.cboxInboundAddr.TextChanged += new System.EventHandler(this.cboxInboundAddr_TextChanged);
            // 
            // ctxMenuStripMore
            // 
            this.ctxMenuStripMore.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxMenuStripMore.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem6,
            this.toolStripMenuItem4,
            this.toolStripMenuItem1,
            this.copyToolStripMenuItem,
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
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autorunToolStripMenuItem,
            this.globalImportToolStripMenuItem,
            this.skipCNWebsiteToolStripMenuItem,
            this.untrackToolStripMenuItem});
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // autorunToolStripMenuItem
            // 
            this.autorunToolStripMenuItem.Name = "autorunToolStripMenuItem";
            resources.ApplyResources(this.autorunToolStripMenuItem, "autorunToolStripMenuItem");
            this.autorunToolStripMenuItem.Click += new System.EventHandler(this.autorunToolStripMenuItem_Click);
            // 
            // globalImportToolStripMenuItem
            // 
            this.globalImportToolStripMenuItem.Name = "globalImportToolStripMenuItem";
            resources.ApplyResources(this.globalImportToolStripMenuItem, "globalImportToolStripMenuItem");
            this.globalImportToolStripMenuItem.Click += new System.EventHandler(this.globalImportToolStripMenuItem_Click);
            // 
            // skipCNWebsiteToolStripMenuItem
            // 
            this.skipCNWebsiteToolStripMenuItem.Name = "skipCNWebsiteToolStripMenuItem";
            resources.ApplyResources(this.skipCNWebsiteToolStripMenuItem, "skipCNWebsiteToolStripMenuItem");
            this.skipCNWebsiteToolStripMenuItem.Click += new System.EventHandler(this.skipCNWebsiteToolStripMenuItem_Click);
            // 
            // untrackToolStripMenuItem
            // 
            this.untrackToolStripMenuItem.Name = "untrackToolStripMenuItem";
            resources.ApplyResources(this.untrackToolStripMenuItem, "untrackToolStripMenuItem");
            this.untrackToolStripMenuItem.Click += new System.EventHandler(this.untrackToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
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
            this.rtboxServerTitle.Cursor = System.Windows.Forms.Cursors.Default;
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
            this.Controls.Add(this.cboxInboundAddr);
            this.Controls.Add(this.cboxMark);
            this.Controls.Add(this.btnMultiboxing);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnIsCollapse);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.chkSelected);
            this.Controls.Add(this.btnMenu);
            this.Controls.Add(this.cboxInbound);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbRunning);
            this.Controls.Add(this.rtboxServerTitle);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.lbLastModify);
            this.Controls.Add(this.lbIsAutorun);
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
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.ComboBox cboxInbound;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Label lbRunning;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkSelected;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ContextMenuStrip ctxMenuStripMore;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vmessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem v2cfgToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem logOfThisServerToolStripMenuItem;
        private System.Windows.Forms.ComboBox cboxMark;
        private System.Windows.Forms.Button btnIsCollapse;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbIsAutorun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnMultiboxing;
        private System.Windows.Forms.RichTextBox rtboxServerTitle;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiboxingToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem autorunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem globalImportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skipCNWebsiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem untrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runSpeedTestToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem moveToTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vToolStripMenuItem;
        private System.Windows.Forms.Label lbLastModify;
        private System.Windows.Forms.ComboBox cboxInboundAddr;
    }
}
