using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class ServerUI :
        UserControl,
        BaseClasses.IFormMainFlyPanelComponent,
        VgcApis.Interfaces.IDropableControl
    {
        Services.Servers servers;
        Services.Settings settings;
        Services.ShareLinkMgr slinkMgr;
        VgcApis.Interfaces.ICoreServCtrl coreServCtrl;

        string keyword = null;

        VgcApis.Libs.Tasks.Bar uiUpdateLock = new VgcApis.Libs.Tasks.Bar();
        VgcApis.Libs.Tasks.LazyGuy lazyUiUpdater;

        static readonly Bitmap[] btnBgCaches = new Bitmap[3];

        public ServerUI(VgcApis.Interfaces.ICoreServCtrl serverItem)
        {
            servers = Services.Servers.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;
            settings = Services.Settings.Instance;

            this.coreServCtrl = serverItem;
            InitializeComponent();

            lazyUiUpdater = new VgcApis.Libs.Tasks.LazyGuy(RefreshUiLater, 100);
        }

        private void ServerUI_Load(object sender, EventArgs e)
        {
            rtboxServerTitle.BackColor = BackColor;
            rlbSpeedtest.Text = @"";
            rlbSpeedtest.Visible = false;

            InitButtonBackgroundImage();
            BindCoreCtrlEvents();
            RefreshUiLater();
        }

        private void InitButtonBackgroundImage()
        {
            CreateBgImgForButton(btnStart, ButtonTypes.Start);
            CreateBgImgForButton(btnStop, ButtonTypes.Stop);
            CreateBgImgForButton(btnMenu, ButtonTypes.Menu);
        }

        private void ReleaseCoreCtrlEvents()
        {
            coreServCtrl.OnCoreStart -= OnCorePropertyChangesHandler;
            coreServCtrl.OnCoreStop -= OnCorePropertyChangesHandler;
            coreServCtrl.OnPropertyChanged -= OnCorePropertyChangesHandler;
        }

        private void BindCoreCtrlEvents()
        {
            coreServCtrl.OnCoreStart += OnCorePropertyChangesHandler;
            coreServCtrl.OnCoreStop += OnCorePropertyChangesHandler;
            coreServCtrl.OnPropertyChanged += OnCorePropertyChangesHandler;
        }

        #region interface VgcApis.Models.IDropableControl
        public string GetTitle() => coreServCtrl.GetCoreStates().GetTitle();

        public string GetUid() => coreServCtrl.GetCoreStates().GetUid();

        public string GetStatus() => coreServCtrl.GetCoreStates().GetStatus();
        #endregion

        #region private method
        void ShowModifyConfigsWinForm() => WinForms.FormModifyServerSettings.ShowForm(coreServCtrl);

        private void HighLightServerTitleWithKeywords()
        {
            VgcApis.Misc.UI.RunInUiThread(rtboxServerTitle, () =>
            {
                var box = rtboxServerTitle;
                var title = box.Text.ToLower();

                if (string.IsNullOrEmpty(keyword)
                    || !VgcApis.Misc.Utils.PartialMatchCi(title, keyword))
                {
                    return;
                }

                int idxTitle = 0, idxKeyword = 0;
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
            });
        }

        void StartThisServerOnlyThen(Action done = null)
        {
            var server = this.coreServCtrl;
            servers.StopAllServersThen(() => server.GetCoreCtrl().RestartCoreThen(done));
        }

        void RefreshUiLater()
        {
            if (!uiUpdateLock.Install())
            {
                lazyUiUpdater.DoItLater();
                return;
            }

            RefreshUiThen(() => uiUpdateLock.Remove());
        }

        void OnCorePropertyChangesHandler(object sender, EventArgs args) =>
            RefreshUiLater();

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
            var rleft = rlbIsRunning.Left;
            var margin = rleft / 2;

            rlbSetting.Left = rleft + (rlbIsRunning.Width - rlbSetting.Width) / 2;

            var end = rlbInboundMode.Right;

            var controls = new List<Control>
            {
                rlbLastModify,
                rlbMark,
                rlbSpeedtest,
                rlbRemark,
            };

            foreach (var control in controls)
            {
                if (!control.Visible)
                {
                    continue;
                }

                var start = end + margin;
                if (control.Left != start)
                {
                    control.Left = end + margin;
                }
                end = control.Right;
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

            Bitmap clone;
            lock (drawImageLocker)
            {
                var size = btn.ClientSize;
                if (btnBgCaches[idx] == null || btnBgCaches[idx].Size != size)
                {
                    btnBgCaches[idx] = CreateBgCache(size, btnType);
                }
                clone = btnBgCaches[idx].Clone() as Bitmap;
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

            var points = new PointF[] {
                    new PointF(cx - ml, cy - r+pw/2),
                    new PointF(cx + mr, cy),
                    new PointF(cx - ml, cy + r-pw/2),
                };
            g.DrawLines(pen, points);
        }

        void DrawRect(Graphics g, float cx, float cy, float r)
        {
            g.FillRectangle(Brushes.Brown, cx - r, cy - r, r * 2, r * 2);
        }

        void UpdateInboundModeLabel(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreState)
        {
            var text = @"Config";
            var tooltip = I18N.InbModeConfigToolTip;
            var inbModeIdx = coreState.GetInboundType();
            var lower = coreState.GetInboundIp().Equals(VgcApis.Models.Consts.Webs.LoopBackIP);

            switch (inbModeIdx)
            {
                case (int)VgcApis.Models.Datas.Enums.ProxyTypes.HTTP:
                    text = (lower ? @"h" : @"H") + coreState.GetInboundPort();
                    tooltip = @"inbound -> http://" + coreState.GetInboundAddr();
                    break;
                case (int)VgcApis.Models.Datas.Enums.ProxyTypes.SOCKS:
                    text = (lower ? @"s" : @"S") + coreState.GetInboundPort();
                    tooltip = @"inbound -> socks://" + coreState.GetInboundAddr();
                    break;
            }
            UpdateControlTextAndTooltip(rlbInboundMode, text, tooltip);
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

        void RefreshUiThen(Action done)
        {
            VgcApis.Misc.UI.RunInUiThread(rtboxServerTitle, () =>
            {
                try
                {
                    var cs = coreServCtrl.GetCoreStates();
                    var cc = coreServCtrl.GetCoreCtrl();

                    // must update background first
                    var isSelected = cs.IsSelected();

                    // first line
                    UpdateOnOffLabel(cc.IsCoreRunning());
                    UpdateSelectCheckboxState(isSelected);
                    UpdateTitleTextBox(cs);

                    // second line
                    UpdateInboundModeLabel(cs);
                    UpdateLastModifiedLable(cs.GetLastModifiedUtcTicks());
                    UpdateMarkLabel(cs.GetMark());
                    UpdateRemarkLabel(cs.GetRemark());
                    UpdateStatusLable(cs);
                    UpdateSettingsLable(cs);
                    CompactRoundLables();
                }
                catch { }
                finally
                {
                    done?.Invoke();
                }
            });
        }

        private void UpdateTitleTextBox(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreStates)
        {
            var cs = coreStates;
            var summary = VgcApis.Misc.Utils.AutoEllipsis(cs.GetSummary(), VgcApis.Models.Consts.AutoEllipsis.ServerSummaryMaxLength);
            var tip = $"{I18N.NameColon}{cs.GetLongName()}\n{I18N.SummaryColon}{summary}";
            UpdateControlTextAndTooltip(rtboxServerTitle, cs.GetTitle(), tip);
        }

        void UpdateMarkLabel(string mark)
        {
            var m = VgcApis.Misc.Utils.AutoEllipsis(mark, VgcApis.Models.Consts.AutoEllipsis.MarkLabelTextMaxLength);
            var tooltip = $"{I18N.Mark}{m}";
            UpdateControlTextAndTooltip(rlbMark, mark, tooltip);
        }

        void UpdateRemarkLabel(string remark)
        {
            var m = VgcApis.Misc.Utils.AutoEllipsis(remark, VgcApis.Models.Consts.AutoEllipsis.MarkLabelTextMaxLength);
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
            UpdateControlTextAndTooltip(rlbLastModify, text, tooltip);
        }

        string Ticks2Tooltip(long utcTicks)
        {
            var date = new DateTime(utcTicks, DateTimeKind.Utc).ToLocalTime();
            return I18N.LastModified + date.ToLongDateString() + date.ToLongTimeString();
        }

        private void UpdateSettingsLable(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreStates)
        {
            var text = (coreStates.IsAutoRun() ? "A" : "")
                + (coreStates.IsInjectSkipCnSite() ? "C" : "")
                + (coreStates.IsInjectGlobalImport() ? "I" : "")
                + (coreStates.IsUntrack() ? "U" : "");

            if (text.Length > 0 && text.Length <= 2)
            {
                text = $" {text} ";
            }

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
            var fontStyle = new Font(rtboxServerTitle.Font, selected ? FontStyle.Bold : FontStyle.Regular);
            rtboxServerTitle.Font = fontStyle;
        }

        private void UpdateOnOffLabel(bool isServerOn)
        {
            var text = isServerOn ? "ON" : "OFF";

            if (rlbIsRunning.Text != text)
            {
                rlbIsRunning.Text = text;
            }
            var bc = isServerOn ? Color.DarkOrange : BackColor;
            var fc = isServerOn ? Color.White : Color.ForestGreen;

            if (rlbIsRunning._BackColor != bc)
            {
                rlbIsRunning._BackColor = bc;
            }

            if (rlbIsRunning.ForeColor != fc)
            {
                rlbIsRunning.ForeColor = fc;
            }
        }

        void UserMouseDown()
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        void ShowPopupMenu(Control control)
        {
            Point pos = new Point(control.Left, control.Top + control.Height);
            ctxMenuStripMore.Show(this, pos);
        }

        #endregion

        #region properties
        public bool isSelected
        {
            get
            {
                return coreServCtrl.GetCoreStates().IsSelected();
            }
            private set { }
        }
        #endregion

        #region public method
        public void SetKeywords(string keywords)
        {
            this.keyword = keywords?.Replace(@" ", "")?.ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                return;
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                // control may be desposed, the sun may explode while this function is running
                try
                {
                    HighLightServerTitleWithKeywords();
                }
                catch { }
            });
        }

        public string GetConfig() => coreServCtrl.GetConfiger().GetConfig();

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
            lazyUiUpdater?.Quit();

            ReleaseCoreCtrlEvents();
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

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var config = coreServCtrl.GetConfiger().GetConfig();
            new Views.WinForms.FormConfiger(config);
        }

        private void vmessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var vmessLink = slinkMgr.EncodeConfigToShareLink(
                GetConfig(),
                VgcApis.Models.Datas.Enums.LinkTypes.vmess);

            Misc.Utils.CopyToClipboardAndPrompt(vmessLink);
        }

        private void v2cfgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = slinkMgr.EncodeConfigToShareLink(
                GetConfig(),
                VgcApis.Models.Datas.Enums.LinkTypes.v2cfg);

            Misc.Utils.CopyToClipboardAndPrompt(content);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Misc.UI.Confirm(I18N.ConfirmDeleteControl))
            {
                return;
            }
            Cleanup();
            servers.DeleteServerByConfig(GetConfig());
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
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                settings.isSpeedtestCancelled = false;
                coreServCtrl.GetCoreCtrl().RunSpeedTest();
            });
        }

        private void moveToTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreStates().SetIndex(0);
            servers.RequireFormMainReload();
        }

        private void moveToBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreStates().SetIndex(double.MaxValue);
            servers.RequireFormMainReload();
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var finalConfig = coreServCtrl.GetConfiger().GetFinalConfig();
            new WinForms.FormConfiger(finalConfig.ToString(Formatting.Indented));
        }

        private void vToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var vee = slinkMgr.EncodeConfigToShareLink(
                GetConfig(),
                VgcApis.Models.Datas.Enums.LinkTypes.v);
            Misc.Utils.CopyToClipboardAndPrompt(vee);
        }


        bool isRlbIsRunningDisabled = false;
        private void rlbIsRunning_Click(object sender, EventArgs e)
        {

            if (isRlbIsRunningDisabled)
            {
                return;
            }

            var cc = coreServCtrl.GetCoreCtrl();
            rlbIsRunning._BackColor = Color.DarkGray;
            isRlbIsRunningDisabled = true;

            Action done = () =>
            {
                isRlbIsRunningDisabled = false;
            };

            if (cc.IsCoreRunning())
            {
                cc.StopCoreThen(done);
            }
            else
            {
                cc.RestartCoreThen(done);
            }
        }

        private void showSettingsWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
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

        private void rlbLastModify_MouseDown(object sender, MouseEventArgs e)
        {
            UserMouseDown();
        }

        private void rlbSpeedtest_MouseDown(object sender, MouseEventArgs e)
        {
            UserMouseDown();
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

        private void rlbRemark_Click(object sender, EventArgs e)
        {
            ShowModifyConfigsWinForm();
        }
        #endregion
    }
}
