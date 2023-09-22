using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
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

        string keyword = null;
        readonly VgcApis.Libs.Tasks.LazyGuy lazyUiUpdater,
            lazyHighlighter;

        static readonly Bitmap[] btnBgCaches = new Bitmap[3];
        readonly List<Control> roundLables;

        public ServerUI()
        {
            // this.Size = 476, 50

            servers = Services.Servers.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;
            settings = Services.Settings.Instance;

            InitializeComponent();
            InitButtomLine();

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

            lazyHighlighter = new VgcApis.Libs.Tasks.LazyGuy(HighLightKeywordsThen, 400, 1000)
            {
                Name = "ServerUi.HighLight",
            };

            InitButtonBackgroundImage();

            ResetControls();

            foreach (Control item in this.Controls)
            {
                item.MouseEnter += ShowCtrlBtn;
            }
        }

        private void InitButtomLine()
        {
            Controls.Add(
                new Label()
                {
                    Height = 1,
                    Dock = DockStyle.Bottom,
                    BackColor = Color.LightGray
                }
            );
        }

        private void ResetControls()
        {
            rtboxServerTitle.BackColor = BackColor;
            SetCtrlButtonsVisiblity(false);
        }

        private void InitButtonBackgroundImage()
        {
            CreateBgImgForButton(btnStart, ButtonTypes.Start);
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

        async void ShowCtrlBtn(object sender, EventArgs args)
        {
            await SetCtrlButtonsVisiblityLater(true);
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

        #region interface VgcApis.Models.IDropableControl
        public string GetTitle() => coreServCtrl.GetCoreStates().GetTitle();

        public string GetUid() => coreServCtrl.GetCoreStates().GetUid();

        public string GetStatus() => coreServCtrl.GetCoreStates().GetStatus();
        #endregion

        #region private method
        bool isCtrlBtnVisable = false;

        async Task SetCtrlButtonsVisiblityLater(bool isVisable)
        {
            isCtrlBtnVisable = isVisable;
            await Task.Delay(200);
            SetCtrlButtonsVisiblity(isCtrlBtnVisable);
        }

        void SetCtrlButtonsVisiblity(bool isVisable)
        {
            try
            {
                if (btnStart.Visible != isVisable)
                {
                    btnStart.Visible = isVisable;
                    btnStop.Visible = isVisable;
                    btnMenu.Visible = isVisable;
                }
            }
            catch { }
        }

        void ShowModifyConfigsWinForm() => WinForms.FormModifyServerSettings.ShowForm(coreServCtrl);

        void HighLightIndex()
        {
            if ($"#{(int)GetIndex()}" != keyword)
            {
                return;
            }

            rtboxServerTitle.SelectionStart = 0;
            rtboxServerTitle.SelectionLength = keyword.Length - 1;
            rtboxServerTitle.SelectionBackColor = Color.Yellow;
        }

        void HighLightKeyWords()
        {
            var box = rtboxServerTitle;
            var title = box.Text.ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                return;
            }

            if (keyword.StartsWith("#"))
            {
                HighLightIndex();
                return;
            }

            if (!VgcApis.Misc.Utils.PartialMatchCi(title, keyword))
            {
                return;
            }

            int idxTitle = 0,
                idxKeyword = 0;
            while (idxTitle < title.Length && idxKeyword < keyword.Length)
            {
                if (title[idxTitle].CompareTo(keyword[idxKeyword]) == 0)
                {
                    box.SelectionStart = idxTitle;
                    box.SelectionLength = 1;
                    box.SelectionBackColor = Color.Yellow;
                    idxKeyword++;
                }
                idxTitle++;
            }
            box.SelectionStart = 0;
            box.SelectionLength = 0;
            box.DeselectAll();
        }

        void HighLightKeywordsThen()
        {
            VgcApis.Misc.UI.Invoke(HighLightKeyWords);
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

            void worker()
            {
                var flyPanel = this.Parent;
                if (flyPanel == null || flyPanel.IsDisposed)
                {
                    return;
                }

                var cs = csc.GetCoreStates();
                var cc = csc.GetCoreCtrl();

                // must update background first
                var isSelected = cs.IsSelected();

                // first line
                UpdateOnOffLabel(cc.IsCoreRunning());
                UpdateSelectCheckboxState(isSelected);
                UpdateTitleTextBox(cs);

                // second line
                UpdateInboundModeLabel(cs);
                UpdateLastModifiedLable(cs.GetLastModifiedUtcTicks());
                UpdateCoreNameLabel(cc);
                UpdateMarkLabel(cs.GetMark());
                UpdateTag1Label(cs.GetTag1());
                UpdateTag2Label(cs.GetTag2());
                UpdateTag3Label(cs.GetTag3());
                UpdateRemarkLabel(cs.GetRemark());
                UpdateStatusLable(cs);
                UpdateSettingsLable(cs);
                UpdateNetworkFlowLable(cs);

                CompactRoundLables();
            }

            void next()
            {
                lazyHighlighter?.Postpone();
                done?.Invoke();
            }

            VgcApis.Misc.UI.InvokeThen(worker, next);
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
            Start,
            Stop,
            Menu,
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
            var pw = r * 0.4f;
            var pc = Color.FromArgb(45, 45, 45);
            using (var g = Graphics.FromImage(bmp))
            using (var pen = new Pen(pc, pw))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                switch (btnType)
                {
                    case ButtonTypes.Stop:
                        DrawRect(g, cx, cy, r);
                        break;
                    case ButtonTypes.Start:
                        DrawTriangle(g, pen, pw, cx, cy, r);
                        break;
                    case ButtonTypes.Menu:
                    default:
                        DrawHamburg(g, pen, pw, cx, cy, r);
                        break;
                }
            }
            return bmp;
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
            var name = coreState.GetInboundName();
            var tooltip = name;

            var info = coreServCtrl.GetConfiger().GetInboundInfo();
            if (info != null)
            {
                tooltip = $"inbound -> {info.protocol}://{info.host}:{info.port}";
            }

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
            var text = (coreStates.IsAutoRun() ? "A" : "") + (coreStates.IsUntrack() ? "U" : "");
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

        #region public method
        public void Rebind(VgcApis.Interfaces.ICoreServCtrl coreServCtrl)
        {
            ReleaseCoreCtrlEvents(this.coreServCtrl);
            this.coreServCtrl = coreServCtrl;
            ResetControls();
            BindCoreCtrlEvents(coreServCtrl);
            RefreshUiLater();
        }

        public void SetKeywords(string keywords)
        {
            this.keyword = keywords?.Replace(@" ", "")?.ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                return;
            }

            lazyHighlighter?.Deadline();
        }

        public string GetConfig() => coreServCtrl?.GetConfiger()?.GetConfig() ?? "";

        public void SetSelected(bool selected)
        {
            coreServCtrl.GetCoreStates().SetIsSelected(selected);
        }

        public double GetIndex() => coreServCtrl.GetCoreStates().GetIndex();

        public void SetIndex(double index)
        {
            coreServCtrl.GetCoreStates().SetIndex(index);
        }

        public void Cleanup()
        {
            ReleaseCoreCtrlEvents(this.coreServCtrl);
            lazyUiUpdater?.Dispose();
            lazyHighlighter?.Dispose();
            foreach (Control item in this.Controls)
            {
                item.MouseEnter -= ShowCtrlBtn;
            }

            this.coreServCtrl = null;
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
            if (!Misc.UI.Confirm(I18N.ConfirmDeleteControl))
            {
                return;
            }

            var csc = this.coreServCtrl;
            this.coreServCtrl = null;

            ReleaseCoreCtrlEvents(csc);

            var uid = csc?.GetCoreStates().GetUid();
            if (!servers.DeleteServerByUid(uid))
            {
                MessageBox.Show(I18N.CantFindOrgServDelFail);
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

        private async void ServerUI_MouseEnter(object sender, EventArgs e)
        {
            await SetCtrlButtonsVisiblityLater(true);
        }

        private async void ServerUI_MouseLeave(object sender, EventArgs e)
        {
            await SetCtrlButtonsVisiblityLater(false);
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
            ShowModifyConfigsWinForm();
        }

        private void rlbSpeedtest_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
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

        private void rlbRemark_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
        }
        #endregion
    }
}
