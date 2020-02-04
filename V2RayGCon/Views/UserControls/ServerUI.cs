using Newtonsoft.Json;
using System;
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
        Services.ShareLinkMgr slinkMgr;
        VgcApis.Interfaces.ICoreServCtrl coreServCtrl;

        int[] formHeight;
        Bitmap[] foldingButtonIcons;
        string keyword = null;

        string lazyMarkSaverCache = @"";

        VgcApis.Libs.Tasks.Bar uiUpdateLock = new VgcApis.Libs.Tasks.Bar();
        VgcApis.Libs.Tasks.LazyGuy lazyMarkSaver, lazyUiUpdater;
        VgcApis.Libs.Tasks.CancelableTimeout lazyAddrSetter = null;

        public ServerUI(
            VgcApis.Interfaces.ICoreServCtrl serverItem)
        {
            servers = Services.Servers.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;

            this.coreServCtrl = serverItem;
            InitializeComponent();

            this.foldingButtonIcons = new Bitmap[] {
                Properties.Resources.StepBackArrow_16x,
                Properties.Resources.StepOverArrow_16x,
            };

            this.formHeight = new int[] {
                this.Height,  // collapseLevel= 0
                this.cboxInbound.Top,
            };

            lazyMarkSaver = new VgcApis.Libs.Tasks.LazyGuy(
                () => coreServCtrl.GetCoreStates().SetMark(lazyMarkSaverCache), 2000);
            lazyUiUpdater = new VgcApis.Libs.Tasks.LazyGuy(
                RefreshUiLater, 100);
        }

        private void ServerUI_Load(object sender, EventArgs e)
        {
            rtboxServerTitle.BackColor = this.BackColor;
            InitStatusLabel();
            BindCoreCtrlEvents();
            RefreshUiLater();
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
        public string GetTitle() =>
            coreServCtrl.GetCoreStates().GetTitle();

        public string GetUid() =>
            coreServCtrl.GetCoreStates().GetUid();

        public string GetStatus() =>
            coreServCtrl.GetCoreStates().GetStatus();
        #endregion

        #region private method
        void InitStatusLabel()
        {
            VgcApis.Misc.UI.RunInUiThread(lbStatus, () =>
            {
                try
                {
                    Misc.UI.UpdateControlOnDemand(lbStatus, @"");
                }
                catch { }
            });
        }

        private void HighLightTitleWithKeywords()
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

        void RestartServer()
        {
            var server = this.coreServCtrl;
            lazyAddrSetter?.Timeout();
            servers.StopAllServersThen(
                () => server.GetCoreCtrl().RestartCoreThen());
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

        void RefreshUiThen(Action done)
        {
            VgcApis.Misc.UI.RunInUiThread(rtboxServerTitle, () =>
            {
                try
                {
                    var cs = coreServCtrl.GetCoreStates();
                    var cc = coreServCtrl.GetCoreCtrl();
                    Misc.UI.UpdateControlOnDemand(cboxInbound, cs.GetInboundType());
                    Misc.UI.UpdateControlOnDemand(rtboxServerTitle, cs.GetTitle());
                    Misc.UI.UpdateControlOnDemand(lbStatus, cs.GetStatus());
                    UpdateServerOptionTickStat(cs);
                    Misc.UI.UpdateControlOnDemand(cboxInboundAddr, cs.GetInboundAddr());
                    UpdateMarkLable(cs);
                    UpdateSelectedTickStat(cs);
                    UpdateOnOffLabel(cc.IsCoreRunning());
                    Misc.UI.UpdateControlOnDemand(cboxMark, cs.GetMark());
                    UpdateBorderFoldingStat(cs.GetFoldingState());
                    UpdateToolsTip();
                    UpdateLastModifiedLable(cs.GetLastModifiedUtcTicks());
                }
                catch { }
                finally
                {
                    done?.Invoke();
                }
            });
        }

        void UpdateLastModifiedLable(long utcTicks)
        {
            var date = new DateTime(utcTicks, DateTimeKind.Utc).ToLocalTime();

            Misc.UI.UpdateControlOnDemand(lbLastModify, date.ToString(I18N.MMdd));
            var tooltip = I18N.LastModified + date.ToLongDateString() + date.ToLongTimeString();
            if (toolTip1.GetToolTip(lbLastModify) != tooltip)
            {
                toolTip1.SetToolTip(lbLastModify, tooltip);
            }
        }

        private void UpdateServerOptionTickStat(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreStates)
        {
            Misc.UI.UpdateControlOnDemand(globalImportToolStripMenuItem, coreStates.IsInjectGlobalImport());
            Misc.UI.UpdateControlOnDemand(skipCNWebsiteToolStripMenuItem, coreStates.IsInjectSkipCnSite());
            Misc.UI.UpdateControlOnDemand(autorunToolStripMenuItem, coreStates.IsAutoRun());
            Misc.UI.UpdateControlOnDemand(untrackToolStripMenuItem, coreStates.IsUntrack());
        }

        private void UpdateToolsTip()
        {
            var title = rtboxServerTitle.Text;
            if (toolTip1.GetToolTip(rtboxServerTitle) != title)
            {
                toolTip1.SetToolTip(rtboxServerTitle, title);
            }
        }

        private void UpdateMarkLable(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreStates)
        {
            var text = (coreStates.IsAutoRun() ? "A" : "")
                + (coreStates.IsInjectSkipCnSite() ? "C" : "")
                + (coreStates.IsInjectGlobalImport() ? "I" : "")
                + (coreStates.IsUntrack() ? "U" : "");

            if (lbIsAutorun.Text != text)
            {
                lbIsAutorun.Text = text;
            }
        }

        void UpdateBorderFoldingStat(int foldingState)
        {
            var level = Misc.Utils.Clamp(foldingState, 0, foldingButtonIcons.Length);

            if (btnIsCollapse.BackgroundImage != foldingButtonIcons[level])
            {
                btnIsCollapse.BackgroundImage = foldingButtonIcons[level];
            }

            var newHeight = this.formHeight[level];
            if (this.Height != newHeight)
            {
                this.Height = newHeight;
            }
        }

        void UpdateSelectedTickStat(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates coreStates)
        {
            if (coreStates.IsSelected() == chkSelected.Checked)
            {
                return;
            }

            chkSelected.Checked = coreStates.IsSelected();
            HighlightSelectedServerItem(chkSelected.Checked);
        }

        void HighlightSelectedServerItem(bool selected)
        {
            var fontStyle = new Font(rtboxServerTitle.Font, selected ? FontStyle.Bold : FontStyle.Regular);
            var colorRed = selected ? Color.OrangeRed : Color.Black;
            rtboxServerTitle.Font = fontStyle;
            lbStatus.Font = fontStyle;
            lbStatus.ForeColor = colorRed;
        }

        private void UpdateOnOffLabel(bool isServerOn)
        {
            var text = isServerOn ? "ON" : "OFF";

            if (cboxInboundAddr.Enabled == isServerOn)
            {
                cboxInboundAddr.Enabled = !isServerOn;
            }

            if (lbRunning.Text != text)
            {
                lbRunning.Text = text;
                lbRunning.ForeColor = isServerOn ? Color.DarkOrange : Color.Green;
            }
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
                    HighLightTitleWithKeywords();
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
            lazyAddrSetter?.Timeout();

            lazyMarkSaver?.DoItNow();
            lazyMarkSaver?.Quit();
            lazyUiUpdater?.Quit();

            ReleaseCoreCtrlEvents();
        }


        #endregion

        #region UI event
        private void ServerListItem_MouseDown(object sender, MouseEventArgs e)
        {
            // this effect is ugly and useless
            // Cursor.Current = Misc.UI.CreateCursorIconFromUserControl(this);
            DoDragDrop((ServerUI)sender, DragDropEffects.Move);
        }

        private void btnAction_Click(object sender, System.EventArgs e)
        {
            Button btnSender = sender as Button;
            Point pos = new Point(btnSender.Left, btnSender.Top + btnSender.Height);
            ctxMenuStripMore.Show(this, pos);
            //menu.Show(this, pos);
        }

        private void cboxInbound_SelectedIndexChanged(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreStates().SetInboundType(cboxInbound.SelectedIndex);
        }

        private void chkSelected_CheckedChanged(object sender, EventArgs e)
        {
            var selected = chkSelected.Checked;
            if (selected == coreServCtrl.GetCoreStates().IsSelected())
            {
                return;
            }
            coreServCtrl.GetCoreStates().SetIsSelected(selected);
            HighlightSelectedServerItem(chkSelected.Checked);
        }

        void SetInboundAddrLater(string address)
        {
            lazyAddrSetter?.Cancel();
            lazyAddrSetter = new VgcApis.Libs.Tasks.CancelableTimeout(
                () =>
                {
                    if (VgcApis.Misc.Utils.TryParseIPAddr(address, out var ip, out var port))
                    {
                        coreServCtrl.GetCoreStates().SetInboundAddr(ip, port);
                    }
                }, 2000);
            lazyAddrSetter.Start();
        }

        private void cboxInboundAddr_TextChanged(object sender, EventArgs e)
        {
            VgcApis.Misc.UI.MarkInvalidAddressWithColorRed(cboxInboundAddr);

            var addr = cboxInboundAddr.Text;
            SetInboundAddrLater(addr);
        }

        private void lbSummary_Click(object sender, EventArgs e)
        {
            chkSelected.Checked = !chkSelected.Checked;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            RestartServer();
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

        private void cboxMark_TextChanged(object sender, EventArgs e)
        {
            lazyMarkSaverCache = cboxMark.Text;
            lazyMarkSaver?.DoItLater();
        }

        private void cboxMark_DropDown(object sender, EventArgs e)
        {
            var servers = Services.Servers.Instance;
            cboxMark.Items.Clear();
            foreach (var item in servers.GetMarkList())
            {
                cboxMark.Items.Add(item);
            }
            Misc.UI.ResetComboBoxDropdownMenuWidth(cboxMark);
        }

        private void Label1_MouseDown(object sender, MouseEventArgs e)
        {
            ServerListItem_MouseDown(this, e);
        }

        private void LbAddTimestamp_MouseDown(object sender, MouseEventArgs e)
        {
            ServerListItem_MouseDown(this, e);
        }

        private void lbStatus_MouseDown(object sender, MouseEventArgs e)
        {
            ServerListItem_MouseDown(this, e);
        }

        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            ServerListItem_MouseDown(this, e);
        }

        private void lbRunning_MouseDown(object sender, MouseEventArgs e)
        {
            ServerListItem_MouseDown(this, e);
        }

        private void btnMultiboxing_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().RestartCoreThen();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().StopCoreThen();
        }

        private void btnIsCollapse_Click(object sender, EventArgs e)
        {
            var level = (coreServCtrl.GetCoreStates().GetFoldingState() + 1) % 2;
            coreServCtrl.GetCoreStates().SetFoldingState(level);
        }

        private void lbIsAutorun_MouseDown(object sender, MouseEventArgs e)
        {
            ServerListItem_MouseDown(this, e);
        }

        private void rtboxServerTitle_Click(object sender, EventArgs e)
        {
            chkSelected.Checked = !chkSelected.Checked;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestartServer();
        }

        private void multiboxingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().RestartCoreThen();
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreCtrl().StopCoreThen();
        }

        private void untrackToolStripMenuItem_Click(object sender, EventArgs e) =>
            coreServCtrl.GetCoreStates().ToggleIsUntrack();

        private void autorunToolStripMenuItem_Click(object sender, EventArgs e) =>
            coreServCtrl.GetCoreStates().ToggleIsAutoRun();


        private void globalImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            coreServCtrl.GetCoreStates().ToggleIsInjectImport();
        }

        private void skipCNWebsiteToolStripMenuItem_Click(object sender, EventArgs e) =>
            coreServCtrl.GetCoreStates().ToggleIsInjectSkipCnSite();

        private void runSpeedTestToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            VgcApis.Misc.Utils.RunInBackground(() => coreServCtrl.GetCoreCtrl().RunSpeedTest());
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


        #endregion


    }
}
