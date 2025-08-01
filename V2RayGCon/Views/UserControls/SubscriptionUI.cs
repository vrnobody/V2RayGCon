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

        readonly Services.Settings settings;
        private readonly Subscription subsCtrl;

        public SubscriptionUI(Subscription subsCtrl, Models.Datas.SubscriptionItem subscriptItem)
        {
            InitializeComponent();

            this.subsCtrl = subsCtrl;

            settings = Services.Settings.Instance;

            // tab page is lazy, do not call this in Load().
            InitControls(subscriptItem);
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

        public string GetAlias() => tboxAlias.Text;

        public void SetTotal(int total)
        {
            var color = total > 0 ? Color.DarkGray : Color.Red;
            var text = $"{I18N.TotalNum}{total}";
            VgcApis.Misc.UI.Invoke(() =>
            {
                lbTotal.ForeColor = color;
                lbTotal.Text = text;
            });
        }

        public bool IsEmpty() =>
            string.IsNullOrWhiteSpace(tboxAlias.Text) && string.IsNullOrWhiteSpace(tboxUrl.Text);

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
            subsCtrl.UpdateServUiTotal(this, EventArgs.Empty);
        }

        private void tboxUrl_TextChanged(object sender, EventArgs e)
        {
            var url = tboxUrl.Text;

            if (
                settings.isAutoPatchSubsInfo
                && !string.IsNullOrEmpty(url)
                && VgcApis.Misc.Utils.TryPatchGitHubUrl(url, out var patched)
            )
            {
                tboxUrl.Text = patched;
                return;
            }

            if (
                string.IsNullOrEmpty(tboxAlias.Text)
                && VgcApis.Misc.Utils.TryExtractAliasFromSubscriptionUrl(url, out var alias)
            )
            {
                tboxAlias.Text = alias;
            }

            SetBtnDeleteStat();
            subsCtrl.AutoAddEmptyUi();
            subsCtrl.MarkDuplicatedSubsInfo();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!VgcApis.Misc.UI.Confirm(I18N.ConfirmDeletion))
            {
                return;
            }

            subsCtrl.RemoveSubsUi(this);
        }

        private void lbAlias_Click(object sender, EventArgs e)
        {
            tboxAlias.Focus();
            var msg = VgcApis.Misc.Utils.CopyToClipboard(tboxAlias.Text)
                ? I18N.CopySuccess
                : I18N.CopyFail;
            VgcApis.Misc.UI.MsgBoxAsync(msg);
        }

        private void lbUrl_Click(object sender, EventArgs e)
        {
            var url = tboxUrl.Text;
            if (string.IsNullOrEmpty(url))
            {
                var clipboard = VgcApis.Misc.Utils.ReadFromClipboard();
                if (!string.IsNullOrEmpty(clipboard))
                {
                    tboxUrl.Text = clipboard;
                }
            }
            else
            {
                tboxUrl.Focus();
                var msg = VgcApis.Misc.Utils.CopyToClipboard(tboxUrl.Text)
                    ? I18N.CopySuccess
                    : I18N.CopyFail;
                VgcApis.Misc.UI.MsgBoxAsync(msg);
            }
        }
        #endregion

        #region private method
        void UpdateTextBoxColorWorker(TextBox textBox, IEnumerable<string> texts)
        {
            var color = Color.White;
            var text = textBox.Text;

            if (
                !string.IsNullOrEmpty(text)
                && texts != null
                && texts.Where(t => t == text).Count() > 1
            )
            {
                color = VgcApis.Misc.UI.String2Color(text);
            }

            textBox.BackColor = color;
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
