using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class ServerUI
        : UserControl,
            BaseClasses.IFormMainFlyPanelComponent,
            VgcApis.Interfaces.IDropableControl
    {
        readonly Services.Servers servers;
        readonly Services.Settings settings;
        readonly Services.ShareLinkMgr slinkMgr;

        VgcApis.Interfaces.ICoreServCtrl coreServCtrl;

        VgcApis.Libs.Infr.Highlighter highlighter = new VgcApis.Libs.Infr.Highlighter();
        readonly VgcApis.Libs.Tasks.LazyGuy lazyUiUpdater,
            lazyHighlighter;

        static readonly Bitmap[] btnBgCaches = new Bitmap[4];
        readonly List<Control> roundLables;
        Label bottomLine = null;

        public ServerUI()
        {
            // this.Size = 476, 50

            servers = Services.Servers.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;
            settings = Services.Settings.Instance;

            InitializeComponent();
            InitBottomLine();

            roundLables = new List<Control>
            {
                rlbSetting,
                rlbInboundMode,
                rlbLastModifyDate,
                rlbCoreName,
                rlbRemark,
                rlbTotalNetFlow,
                rlbMark,
                rlbSpeedtest,
                rlbTag1,
                rlbTag2,
                rlbTag3,
            };

            lazyUiUpdater = new VgcApis.Libs.Tasks.LazyGuy(RefreshUiWorker, 400, 3000)
            {
                Name = "ServerUi.RefreshPanel",
            };

            lazyHighlighter = new VgcApis.Libs.Tasks.LazyGuy(HighlightWorker, 400, 1000)
            {
                Name = "ServerUi.HighLight",
            };

            InitButtonBackgroundImage();

            ResetControls();

            this.MouseEnter += ShowCtrlBtn;
            foreach (Control item in this.Controls)
            {
                item.MouseEnter += ShowCtrlBtn;
            }
        }

        #region auto hide control buttons
        public static ServerUI currentServerUI = null;

        static readonly AutoResetEvent areHideButtons = new AutoResetEvent(true);

        void ShowCtrlBtn(object sender, EventArgs args)
        {
            var that = this;
            currentServerUI = that;

            if (!IsButtonsVisible())
            {
                SetCtrlButtonsVisiblity(true);
            }

            if (!areHideButtons.WaitOne(0))
            {
                return;
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                if (!(that.Parent is FlowLayoutPanel parent))
                {
                    areHideButtons.Set();
                    return;
                }

                VgcApis.Misc.Utils.Sleep(500);
                try
                {
                    that.Invoke(
                        (MethodInvoker)
                            delegate
                            {
                                var children = parent.Controls.OfType<ServerUI>();
                                foreach (var child in children)
                                {
                                    if (
                                        child != currentServerUI
                                        && !child.IsDisposed
                                        && child.IsButtonsVisible()
                                    )
                                    {
                                        child.SetCtrlButtonsVisiblity(false);
                                    }
                                }
                            }
                    );
                }
                catch { }
                areHideButtons.Set();
            });
        }
        #endregion

        #region interface VgcApis.Models.IDropableControl
        public string GetTitle() => coreServCtrl.GetCoreStates().GetTitle();

        public string GetUid() => coreServCtrl.GetCoreStates().GetUid();

        public string GetStatus() => coreServCtrl.GetCoreStates().GetStatus();
        #endregion

        #region private method


        public void InitBottomLine()
        {
            bottomLine = new Label()
            {
                Height = 1,
                Dock = DockStyle.Bottom,
                BackColor = Color.LightGray,
            };
            Controls.Add(bottomLine);
        }

        private void ResetControls()
        {
            rtboxServerTitle.BackColor = BackColor;
            SetCtrlButtonsVisiblity(false);
        }

        private void InitButtonBackgroundImage()
        {
            CreateBgImgForButton(btnStart, ButtonTypes.Start);
            CreateBgImgForButton(btnRestart, ButtonTypes.Restart);
            CreateBgImgForButton(btnStop, ButtonTypes.Stop);
            CreateBgImgForButton(btnMenu, ButtonTypes.Menu);
        }

        private void ReleaseCoreCtrlEvents(VgcApis.Interfaces.ICoreServCtrl cs)
        {
            if (cs == null)
            {
                return;
            }
            cs.OnCoreStart -= OnCorePropertyChangesHandler;
            cs.OnCoreStop -= OnCorePropertyChangesHandler;
            cs.OnPropertyChanged -= OnCorePropertyChangesHandler;
        }

        private void BindCoreCtrlEvents(VgcApis.Interfaces.ICoreServCtrl cs)
        {
            if (cs == null)
            {
                return;
            }
            cs.OnCoreStart += OnCorePropertyChangesHandler;
            cs.OnCoreStop += OnCorePropertyChangesHandler;
            cs.OnPropertyChanged += OnCorePropertyChangesHandler;
        }

        void ShowModifyConfigsWinForm() => WinForms.FormModifyServerSettings.ShowForm(coreServCtrl);

        void HighlightWorker()
        {
            VgcApis.Misc.UI.Invoke(() =>
            {
                this.highlighter.Highlight(rtboxServerTitle, GetIndex());
            });
        }

        void StartThisServerOnlyThen(Action done = null)
        {
            var server = this.coreServCtrl;
            servers.StopAllServersThen(() => server.GetCoreCtrl().RestartCoreThen(done));
        }

        void RefreshUiLater() => lazyUiUpdater?.Deadline();

        void RefreshUiWorker(Action done)
        {
            var csc = coreServCtrl;
            if (csc == null)
            {
                return;
            }

            var cs = csc.GetCoreStates();
            var cc = csc.GetCoreCtrl();

            void first()
            {
                var flyPanel = this.Parent;
                if (flyPanel == null || flyPanel.IsDisposed)
                {
                    return;
                }

                // must update background first
                var isSelected = cs.IsSelected();

                // first line
                UpdateOnOffLabel(cc.IsCoreRunning());
                UpdateSelectCheckboxState(isSelected);
                UpdateTitleTextBox(cs);
            }

            void second()
            {
                // second line
                UpdateSettingsLable(cs);
                UpdateInboundModeLabel(cs);
                UpdateLastModifiedLable(cs.GetLastModifiedUtcTicks());
                UpdateCoreNameLabel(cc);
                UpdateMarkLabel(cs.GetMark());
                UpdateTag1Label(cs.GetTag1());
                UpdateTag2Label(cs.GetTag2());
                UpdateTag3Label(cs.GetTag3());
                UpdateRemarkLabel(cs.GetRemark());
                UpdateStatusLable(cs);
                UpdateNetworkFlowLable(cs);
                CompactRoundLables();
            }

            void third()
            {
                HighlightLater();
                done?.Invoke();
            }

            VgcApis.Misc.UI.InvokeAllAsync(10, first, second, third);
        }

        void HighlightLater()
        {
            lazyHighlighter?.Postpone();
        }

        void UpdateCoreNameLabel(VgcApis.Interfaces.CoreCtrlComponents.ICoreCtrl cc)
        {
            var coreName = cc.GetCustomCoreName();
            UpdateControlTextAndTooltip(
                rlbCoreName,
                coreName,
                $"{I18N.CustomCoreName}: {coreName}"
            );
        }

        void UpdateNetworkFlowLable(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreState)
        {
            var text = "";
            var tooltip = "";
            if (settings.isEnableStatistics)
            {
                var up = coreState.GetUplinkTotalInBytes();
                var down = coreState.GetDownlinkTotalInBytes();
                if (up > 0 || down > 0)
                {
                    const long mib = 1024 * 1024;
                    var dm = down / mib;
                    text = $"⇵ {dm}M";
                    tooltip = string.Format(I18N.NetFlowToolTipTpl, up / mib, dm);
                }
            }
            UpdateControlTextAndTooltip(rlbTotalNetFlow, text, tooltip);
        }

        void OnCorePropertyChangesHandler(object sender, EventArgs args) => RefreshUiLater();

        void UpdateControlTextAndTooltip(Control control, string text, string tooltip)
        {
            UpdateControlTextOndemand(control, text);

            if (control.Visible && toolTip1.GetToolTip(control) != tooltip)
            {
                toolTip1.SetToolTip(control, tooltip);
            }
        }

        void CompactRoundLables()
        {
            var left = chkSelected.Left;
            var margin = chkSelected.Left;

            foreach (var control in roundLables)
            {
                if (!control.Visible)
                {
                    continue;
                }

                if (control.Left != left)
                {
                    control.Left = left;
                }
                left = control.Right + margin;
            }
        }

        enum ButtonTypes
        {
            Start = 0,
            Restart = 1,
            Stop = 2,
            Menu = 3,
        }

        readonly object drawImageLocker = new object();

        void CreateBgImgForButton(Button btn, ButtonTypes btnType)
        {
            var idx = (int)btnType;
            btn.Text = string.Empty;
            btn.FlatAppearance.BorderSize = 0;

            var ico = btnBgCaches[idx];
            var size = btn.ClientSize;
            if (ico == null || ico.Size != size)
            {
                ico = CreateBgCache(size, btnType);
                btnBgCaches[idx] = ico;
            }

            Bitmap clone;
            lock (drawImageLocker)
            {
                clone = ico.Clone() as Bitmap;
            }
            btn.BackgroundImage = clone;
        }

        Bitmap CreateBgCache(Size size, ButtonTypes btnType)
        {
            var bmp = new Bitmap(size.Width, size.Height);

            var r = Math.Min(bmp.Width, bmp.Height) * 0.6f / 2f;
            var cx = bmp.Width / 2f;
            var cy = bmp.Height / 2f;
            var penWeight = r * 0.4f;
            var pc = Color.FromArgb(45, 45, 45);
            using (var g = Graphics.FromImage(bmp))
            using (var pen = new Pen(pc, penWeight))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                switch (btnType)
                {
                    case ButtonTypes.Stop:
                        DrawRect(g, cx, cy, r);
                        break;
                    case ButtonTypes.Restart:
                        DrawRefresh(g, pen, penWeight, cx, cy, r);
                        break;
                    case ButtonTypes.Start:
                        DrawTriangle(g, pen, penWeight, cx, cy, r);
                        break;
                    case ButtonTypes.Menu:
                    default:
                        DrawHamburg(g, pen, penWeight, cx, cy, r);
                        break;
                }
            }
            return bmp;
        }

        void DrawRefresh(Graphics g, Pen pen, float pw, float cx, float cy, float r)
        {
            // C
            var circle = new RectangleF(cx - r, cy - r, r * 2, r * 2);
            g.DrawArc(pen, circle, 15, 360 - 45 - 15);

            // >
            var d = (float)(r * Math.Sqrt(2) / 2);
            var len = r / 2;
            var points = new PointF[]
            {
                new PointF(cx + d - len, cy - d + pw / 2),
                new PointF(cx + d, cy - d + pw / 2),
                new PointF(cx + d, cy - d - len),
            };
            g.DrawLines(pen, points);
        }

        void DrawHamburg(Graphics g, Pen pen, float pw, float cx, float cy, float r)
        {
            var xs = cx - r;
            var xe = cx + r;
            var h = r - pw / 2f;

            g.DrawLine(pen, xs, cy - h, xe, cy - h);
            g.DrawLine(pen, xs, cy, xe, cy);
            g.DrawLine(pen, xs, cy + h, xe, cy + h);
        }

        void DrawTriangle(Graphics g, Pen pen, float pw, float cx, float cy, float r)
        {
            var ml = r * 3 / 4;
            var mr = r * 2 / 4;

            var points = new PointF[]
            {
                new PointF(cx - ml, cy - r + pw / 2),
                new PointF(cx + mr, cy),
                new PointF(cx - ml, cy + r - pw / 2),
            };
            g.DrawLines(pen, points);
        }

        void DrawRect(Graphics g, float cx, float cy, float r)
        {
            g.FillRectangle(Brushes.Brown, cx - r, cy - r, r * 2, r * 2);
        }

        void UpdateInboundModeLabel(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreState)
        {
            var lines = new List<string>();
            var tpl = coreState.GetInboundName();
            var tplName = string.IsNullOrEmpty(tpl) ? $"{I18N.Empty}" : tpl;
            lines.Add($"{I18N.Template}: {tplName}");

            var inbs = coreServCtrl.GetConfiger().GetFormattedInboundsInfoFromCache();
            for (int i = 0; i < inbs.Count(); i++)
            {
                var info = inbs[i];
                lines.Add($"inbound[{i}] -> {inbs[i]}");
            }

            var name = coreState.GetInboundName();
            var tooltip = lines.Count() > 0 ? string.Join(Environment.NewLine, lines) : name;

            UpdateControlTextAndTooltip(rlbInboundMode, name, tooltip);
        }

        void UpdateControlTextOndemand(Control control, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (control.Visible)
                {
                    control.Visible = false;
                }
                return;
            }

            if (control.Text != text)
            {
                control.Text = text;
            }

            if (!control.Visible)
            {
                control.Visible = true;
            }
        }

        private void UpdateTitleTextBox(
            VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreStates
        )
        {
            var cs = coreStates;
            var summary = VgcApis.Misc.Utils.AutoEllipsis(
                cs.GetSummary(),
                VgcApis.Models.Consts.AutoEllipsis.ServerSummaryMaxLength
            );
            var tip = $"{I18N.NameColon}{cs.GetLongName()}\n{I18N.SummaryColon}{summary}";
            UpdateControlTextAndTooltip(rtboxServerTitle, cs.GetTitle(), tip);
        }

        void UpdateMarkLabel(string mark)
        {
            var m = VgcApis.Misc.Utils.AutoEllipsis(
                mark,
                VgcApis.Models.Consts.AutoEllipsis.MarkLabelTextMaxLength
            );
            var tooltip = $"{I18N.Mark}{m}";
            UpdateControlTextAndTooltip(rlbMark, mark, tooltip);
        }

        void UpdateTag1Label(string tag)
        {
            var m = VgcApis.Misc.Utils.AutoEllipsis(
                tag,
                VgcApis.Models.Consts.AutoEllipsis.MarkLabelTextMaxLength
            );
            var tooltip = $"{I18N.Tag1}{m}";
            UpdateControlTextAndTooltip(rlbTag1, tag, tooltip);
        }

        void UpdateTag2Label(string tag)
        {
            var m = VgcApis.Misc.Utils.AutoEllipsis(
                tag,
                VgcApis.Models.Consts.AutoEllipsis.MarkLabelTextMaxLength
            );
            var tooltip = $"{I18N.Tag2}{m}";
            UpdateControlTextAndTooltip(rlbTag2, tag, tooltip);
        }

        void UpdateTag3Label(string tag)
        {
            var m = VgcApis.Misc.Utils.AutoEllipsis(
                tag,
                VgcApis.Models.Consts.AutoEllipsis.MarkLabelTextMaxLength
            );
            var tooltip = $"{I18N.Tag3}{m}";
            UpdateControlTextAndTooltip(rlbTag3, tag, tooltip);
        }

        void UpdateRemarkLabel(string remark)
        {
            var m = VgcApis.Misc.Utils.AutoEllipsis(
                remark,
                VgcApis.Models.Consts.AutoEllipsis.MarkLabelTextMaxLength
            );
            var tooltip = $"{I18N.Remark}{m}";
            UpdateControlTextAndTooltip(rlbRemark, remark, tooltip);
        }

        void UpdateStatusLable(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates cs)
        {
            var r = cs.GetSpeedTestResult();
            var isTimeout = r == long.MaxValue;
            var color = isTimeout ? Color.OrangeRed : Color.DimGray;
            var status = cs.GetStatus();
            var tooltip = Ticks2Tooltip(cs.GetLastSpeedTestUtcTicks());

            UpdateControlTextAndTooltip(rlbSpeedtest, status, tooltip);
            if (rlbSpeedtest.ForeColor != color)
            {
                rlbSpeedtest.ForeColor = color;
            }
        }

        void UpdateLastModifiedLable(long utcTicks)
        {
            var date = new DateTime(utcTicks, DateTimeKind.Utc).ToLocalTime();
            var text = date.ToString(I18N.MMdd);
            var tooltip = Ticks2Tooltip(utcTicks);
            UpdateControlTextAndTooltip(rlbLastModifyDate, text, tooltip);
        }

        string Ticks2Tooltip(long utcTicks)
        {
            var date = new DateTime(utcTicks, DateTimeKind.Utc).ToLocalTime();
            return I18N.LastModified + date.ToLongDateString() + date.ToLongTimeString();
        }

        private void UpdateSettingsLable(
            VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreStates
        )
        {
            var text =
                (coreStates.IsAutoRun() ? "A" : "")
                + (!string.IsNullOrEmpty(coreStates.GetCustomTemplateNames()) ? "C" : "")
                + (coreStates.IsIgnoreSendThrough() ? "I" : "")
                + (coreStates.IsAcceptInjection() ? "" : "N")
                + (coreStates.IsUntrack() ? "U" : "");

            UpdateControlTextOndemand(rlbSetting, text);
        }

        void UpdateSelectCheckboxState(bool isSelected)
        {
            if (isSelected == chkSelected.Checked)
            {
                return;
            }

            chkSelected.Checked = isSelected;
            SetServerTitleLabelFontStyle(isSelected);
        }

        void SetServerTitleLabelFontStyle(bool selected)
        {
            var fontStyle = new Font(
                rtboxServerTitle.Font,
                selected ? FontStyle.Bold : FontStyle.Regular
            );
            rtboxServerTitle.Font = fontStyle;
        }

        private void UpdateOnOffLabel(bool isServerOn)
        {
            if (rlbIsRunning.Visible != isServerOn)
            {
                rlbIsRunning.Visible = isServerOn;
            }
        }

        void UserMouseDown()
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        void ShowPopupMenu(Control control)
        {
            Point ctrlPos = new Point(control.Left, control.Top + control.Height);
            var scrBounds = Screen.FromControl(control).Bounds;
            var ctrlScreenPos = this.PointToScreen(ctrlPos);

            var menuBounds = ctxMenuStripMore.Size;

            var left =
                Math.Min(scrBounds.X + scrBounds.Width - 10, ctrlScreenPos.X + menuBounds.Width)
                - menuBounds.Width;
            var top =
                Math.Min(scrBounds.Y + scrBounds.Height - 10, ctrlScreenPos.Y + menuBounds.Height)
                - menuBounds.Height;

            ctxMenuStripMore.Show(new Point(left, top));

            var dir =
                left - scrBounds.X > scrBounds.Width / 2
                    ? ToolStripDropDownDirection.Left
                    : ToolStripDropDownDirection.Right;
            var items = ctxMenuStripMore.Items;
            foreach (var item in items)
            {
                switch (item)
                {
                    case ToolStripMenuItem tmi:
                        tmi.DropDownDirection = dir;
                        break;
                    default:
                        continue;
                }
            }
        }

        #endregion

        #region properties
        public bool isSelected
        {
            get { return coreServCtrl.GetCoreStates().IsSelected(); }
            private set { }
        }
        #endregion

        #region internal
        internal VgcApis.Interfaces.ICoreServCtrl GetInnerCoreServCtrl() => this.coreServCtrl;
        #endregion

        #region public method


        public void SetBorderStyle(bool isFullBorder)
        {
            if (!isFullBorder && bottomLine.Visible == true)
            {
                return;
            }
            bottomLine.Visible = !isFullBorder;
            var top = isFullBorder ? 3 : 0;
            this.Margin = new Padding(3, top, 3, top);
            this.BorderStyle = isFullBorder ? BorderStyle.FixedSingle : BorderStyle.None;
        }

        public bool IsButtonsVisible() => btnStart.Visible;

        public void SetCtrlButtonsVisiblity(bool isVisable)
        {
            try
            {
                if (btnStart.Visible != isVisable)
                {
                    btnStart.Visible = isVisable;
                    btnRestart.Visible = isVisable;
                    btnStop.Visible = isVisable;
                    btnMenu.Visible = isVisable;
                }
            }
            catch { }
        }

        public void Rebind(VgcApis.Interfaces.ICoreServCtrl coreServCtrl)
        {
            ReleaseCoreCtrlEvents(this.coreServCtrl);
            this.coreServCtrl = coreServCtrl;
            ResetControls();
            BindCoreCtrlEvents(coreServCtrl);
            RefreshUiLater();
        }

        public void ChangeHighlighter(VgcApis.Libs.Infr.Highlighter hi)
        {
            this.highlighter =
                hi ?? throw new ArgumentNullException("forget some thing?", nameof(hi));
            HighlightLater();
        }

        public string GetConfig() => coreServCtrl?.GetConfiger()?.GetConfig() ?? "";

        public double GetIndex() => coreServCtrl?.GetCoreStates().GetIndex() ?? double.MinValue;

        public void SetIndex(double index)
        {
            coreServCtrl.GetCoreStates().SetIndex(index);
        }

        public void Cleanup()
        {
            ReleaseCoreCtrlEvents(this.coreServCtrl);
            lazyUiUpdater?.Dispose();
            lazyHighlighter?.Dispose();

            this.MouseEnter -= ShowCtrlBtn;
            foreach (Control item in this.Controls)
            {
                item.MouseEnter -= ShowCtrlBtn;
            }

            // causing System.NullReferenceException
            // this.coreServCtrl = null;
        }
        #endregion

        #region UI event
        private void ServerListItem_MouseDown(object sender, MouseEventArgs e)
        {
            // this effect is ugly and useless
            // Cursor.Current = Misc.UI.CreateCursorIconFromUserControl(this);
            UserMouseDown();
        }

        private void chkSelected_CheckedChanged(object sender, EventArgs e)
        {
            var selected = chkSelected.Checked;
            if (selected == coreServCtrl.GetCoreStates().IsSelected())
            {
                return;
            }
            coreServCtrl.GetCoreStates().SetIsSelected(selected);
            SetServerTitleLabelFontStyle(chkSelected.Checked);
        }

        private void mobToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = slinkMgr.EncodeConfigToShareLink(
                coreServCtrl.GetCoreStates().GetName(),
                GetConfig(),
                VgcApis.Models.Datas.Enums.LinkTypes.mob
            );

            Misc.Utils.CopyToClipboardAndPrompt(content);
        }

        private void autoShareLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var url = coreServCtrl?.GetConfiger()?.GetShareLink();
            Misc.Utils.CopyToClipboardAndPrompt(url);
        }

        private void v2cfgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = slinkMgr.EncodeConfigToShareLink(
                coreServCtrl.GetCoreStates().GetName(),
                GetConfig(),
                VgcApis.Models.Datas.Enums.LinkTypes.v2cfg
            );

            Misc.Utils.CopyToClipboardAndPrompt(content);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var title = this.coreServCtrl.GetCoreStates().GetTitle();
            var msg = $"{title}{Environment.NewLine}{I18N.ConfirmDeletion}";
            if (!VgcApis.Misc.UI.Confirm(msg))
            {
                return;
            }

            var csc = this.coreServCtrl;
            // causing System.NullReferenceException
            // this.coreServCtrl = null;

            ReleaseCoreCtrlEvents(csc);

            var uid = csc?.GetCoreStates().GetUid();
            if (!servers.DeleteServerByUid(uid))
            {
                VgcApis.Misc.UI.MsgBox(I18N.CantFindOrgServDelFail);
            }
        }

        private void logOfThisServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetLogger().ShowFormLog();
        }

        private void rtboxServerTitle_Click(object sender, EventArgs e)
        {
            chkSelected.Checked = !chkSelected.Checked;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartThisServerOnlyThen();
        }

        private void multiboxingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().RestartCoreThen();
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().StopCoreThen();
        }

        private void runSpeedTestToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            settings.isSpeedtestCancelled = false;
            coreServCtrl.GetCoreCtrl().RunSpeedTestThen();
        }

        private void moveToTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreStates().SetIndex(0);
            servers.ResetIndexQuiet();
            servers.RequireFormMainReload();
        }

        private void moveToBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreStates().SetIndex(double.MaxValue);
            servers.ResetIndexQuiet();
            servers.RequireFormMainReload();
        }

        private void showFinalConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var title = coreServCtrl.GetCoreStates().GetTitle();
            var config = coreServCtrl.GetConfiger().GetFinalConfig();
            WinForms.FormTextConfigEditor.ShowConfig(title, config, true);
        }

        private void rlbIsRunning_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().StopCoreThen();
        }

        private void showSettingsWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
        }

        private void lbLastModifyDate_MouseDown(object sender, MouseEventArgs e)
        {
            UserMouseDown();
        }

        private void btnShowPopupMenu_Click(object sender, EventArgs e)
        {
            ShowPopupMenu(sender as Control);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartThisServerOnlyThen();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().StopCoreThen();
        }

        private void rlbSetting_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
        }

        private void rlbInboundMode_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
        }

        private void rlbMark_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
        }

        private void rlbTotalNetFlow_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmClearStat))
            {
                var s = coreServCtrl.GetCoreStates();
                s.SetDownlinkTotal(0);
                s.SetUplinkTotal(0);
            }
        }

        private void rlbSpeedtest_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmClearSpeedTestResults))
            {
                coreServCtrl.GetCoreStates().SetSpeedTestResult(0);
            }
        }

        private void textEditortoolStripMenuItem_Click(object sender, EventArgs e)
        {
            var uid = coreServCtrl.GetCoreStates().GetUid();
            WinForms.FormTextConfigEditor.ShowServer(uid);
        }

        private void rlbCoreName_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
        }

        private void rlbLastModifyDate_Click(object sender, EventArgs e)
        {
            var uid = coreServCtrl.GetCoreStates().GetUid();
            WinForms.FormTextConfigEditor.ShowServer(uid);
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().RestartCoreThen();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().RestartCoreThen();
        }

        private void toCustomIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var curIdx = coreServCtrl.GetCoreStates().GetIndex();
            VgcApis.Misc.UI.GetUserInput(
                I18N.DestIndex,
                str =>
                {
                    if (!double.TryParse(str, out var index))
                    {
                        VgcApis.Misc.UI.MsgBox(I18N.ParseNumberFailed);
                        return;
                    }

                    if (VgcApis.Misc.Utils.AreEqual(curIdx, index))
                    {
                        return;
                    }
                    var delta = index > curIdx ? 0.05 : -0.05;
                    coreServCtrl.GetCoreStates().SetIndex(index + delta);
                    servers.ResetIndexQuiet();
                    servers.RequireFormMainReload();
                }
            );
        }

        private void rlbRemark_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
        }
        #endregion
    }
}
