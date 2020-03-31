using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Controllers.OptionComponent;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class SubscriptionUI : UserControl
    {
        public delegate void OnDeleteHandler();

        Services.Servers servers;
        Services.Settings settings;
        VgcApis.Libs.Tasks.LazyGuy lazyCounter;
        private readonly Subscription subsCtrl;

        public SubscriptionUI(
            Subscription subsCtrl,
            Models.Datas.SubscriptionItem subscriptItem)
        {
            InitializeComponent();

            this.subsCtrl = subsCtrl;

            servers = Services.Servers.Instance;
            settings = Services.Settings.Instance;

            lazyCounter = new VgcApis.Libs.Tasks.LazyGuy(UpdateServerTotalNow, 1000);

            // tab page is lazy, do not call this in Load().
            InitControls(subscriptItem);

            BindEvent();
            lazyCounter.DoItLater();

            Disposed += (s, a) => Cleanup();
        }

        #region form thing

        private void InitControls(Models.Datas.SubscriptionItem subscriptItem)
        {
            lbIndex.Text = "";
            tboxUrl.Text = subscriptItem.url;
            tboxAlias.Text = subscriptItem.alias;
            chkIsUse.Checked = subscriptItem.isUse;
            chkIsSetMark.Checked = subscriptItem.isSetMark;
            SetBtnDeleteStat();
        }

        void SetBtnDeleteStat()
        {
            var isEnable = !IsEmpty();

            if (btnDelete.Enabled != isEnable)
            {
                btnDelete.Enabled = isEnable;
            }
        }
        #endregion

        #region public method
        public bool IsEmpty() =>
            string.IsNullOrWhiteSpace(tboxAlias.Text)
            && string.IsNullOrWhiteSpace(tboxUrl.Text);


        public void UpdateTextBoxColor(IEnumerable<string> alias, IEnumerable<string> urls)
        {
            UpdateTextBoxColorWorker(tboxUrl, urls);
            UpdateTextBoxColorWorker(tboxAlias, alias);
        }

        public bool IsUse() => chkIsUse.Checked;

        public void SetIsUse(bool val) => chkIsUse.Checked = val;

        public Models.Datas.SubscriptionItem GetValue()
        {
            return new Models.Datas.SubscriptionItem
            {
                isUse = chkIsUse.Checked,
                isSetMark = chkIsSetMark.Checked,
                alias = tboxAlias.Text,
                url = tboxUrl.Text,
            };
        }

        public void SetIndex(int index)
        {
            lbIndex.Text = index.ToString();
        }
        #endregion

        #region UI event
        private void tboxAlias_TextChanged(object sender, EventArgs e)
        {
            subsCtrl.MarkDuplicatedSubsInfo();
            SetBtnDeleteStat();
            subsCtrl.AutoAddEmptyUi();
            UpdateServerTotalLater();
        }

        private void tboxUrl_TextChanged(object sender, EventArgs e)
        {
            var url = tboxUrl.Text;

            if (settings.isAutoPatchSubsInfo
                && !string.IsNullOrEmpty(url)
                && VgcApis.Misc.Utils.TryPatchGitHubUrl(url, out var patched))
            {
                tboxUrl.Text = patched;
                return;
            }

            if (string.IsNullOrEmpty(tboxAlias.Text)
                && VgcApis.Misc.Utils.TryExtractAliasFromSubscriptionUrl(url, out var alias))
            {
                tboxAlias.Text = alias;
            }

            SetBtnDeleteStat();
            subsCtrl.AutoAddEmptyUi();
            subsCtrl.MarkDuplicatedSubsInfo();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!Misc.UI.Confirm(I18N.ConfirmDeleteControl))
            {
                return;
            }

            subsCtrl.RemoveSubsUi(this);
        }
        #endregion

        #region private method
        void UpdateTextBoxColorWorker(TextBox textBox, IEnumerable<string> texts)
        {
            var color = Color.White;
            var text = textBox.Text;

            if (!string.IsNullOrEmpty(text)
                && texts != null
                && texts.Where(t => t == text).Count() > 1)
            {
                color = VgcApis.Misc.UI.String2Color(text);
            }

            textBox.BackColor = color;
        }

        void OnCoreStateChangedHandler(object sender, EventArgs args) =>
            UpdateServerTotalLater();

        void UpdateServerTotalLater() => lazyCounter.DoItLater();

        void BindEvent()
        {
            servers.OnServerCountChange += OnCoreStateChangedHandler;
            servers.OnServerPropertyChange += OnCoreStateChangedHandler;
        }

        void ReleaseEvent()
        {
            servers.OnServerCountChange -= OnCoreStateChangedHandler;
            servers.OnServerPropertyChange -= OnCoreStateChangedHandler;
        }

        void UpdateServerTotalNow()
        {
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(
                lbTotal,
                () =>
                {
                    var alias = tboxAlias.Text;
                    var coreNum = servers.GetAllServersOrderByIndex()
                        .Select(s => s.GetCoreStates())
                        .Where(cfg => cfg.GetMark() == alias)
                        .Count();
                    lbTotal.Text = $"{I18N.TotalNum}{coreNum}";
                    lbTotal.ForeColor = coreNum == 0 ? Color.Red : Color.DarkGray;
                });
        }

        void Cleanup()
        {
            ReleaseEvent();
            lazyCounter.ForgetIt();
            lazyCounter.Quit();
        }

        private void UrlListItem_MouseDown(object sender, MouseEventArgs e) =>
            DoDragDrop(this, DragDropEffects.Move);

        private void lbTotal_MouseDown(object sender, MouseEventArgs e) =>
            DoDragDrop(this, DragDropEffects.Move);
        #endregion

        #region protected
        #endregion
    }
}
