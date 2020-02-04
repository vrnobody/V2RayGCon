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
        public event OnDeleteHandler OnDelete; // add empty delegate

        Services.Servers servers;
        VgcApis.Libs.Tasks.LazyGuy lazyCounter;
        private readonly Subscription subsCtrl;

        public SubscriptionUI(
            Subscription subsCtrl,
            Models.Datas.SubscriptionItem subscriptItem)
        {
            InitializeComponent();

            this.subsCtrl = subsCtrl;
            servers = Services.Servers.Instance;
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
        }
        #endregion

        #region public method


        public void UpdateTextBoxColor(
            IEnumerable<string> alias,
            IEnumerable<string> urls)
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
            UpdateServerTotalLater();
        }

        private void tboxUrl_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tboxAlias.Text)
                && VgcApis.Misc.Utils.TryExtractAliasFromSubscriptionUrl(tboxUrl.Text, out var alias))
            {
                tboxAlias.Text = alias;
            }

            subsCtrl.MarkDuplicatedSubsInfo();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!Misc.UI.Confirm(I18N.ConfirmDeleteControl))
            {
                return;
            }

            var flyPanel = this.Parent as FlowLayoutPanel;
            flyPanel.Controls.Remove(this);

            try
            {
                OnDelete?.Invoke();
            }
            catch { }
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
