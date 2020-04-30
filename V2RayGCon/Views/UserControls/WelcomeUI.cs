using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class WelcomeUI :
        UserControl,
        BaseClasses.IFormMainFlyPanelComponent
    {
        Services.Settings setting;
        Services.ShareLinkMgr slinkMgr;

        int marginBottom;

        public WelcomeUI()
        {
            setting = Services.Settings.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;

            InitializeComponent();
            marginBottom = this.Height - pnlBasicUsage.Top;
        }

        #region public method
        public void Cleanup()
        {
        }
        #endregion

        private void WelcomeFlyPanelComponent_Load(object sender, System.EventArgs e)
        {
            var core = new V2RayGCon.Libs.V2Ray.Core(setting);
            if (!core.IsExecutableExist())
            {
                return;
            }

            pnlBasicUsage.Top = pnlDownloadV2RayCore.Top;
            pnlDownloadV2RayCore.Visible = false;
            this.Height = pnlBasicUsage.Top + marginBottom;
        }

        private void lbDownloadV2rayCore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Views.WinForms.FormDownloadCore.ShowForm();
        }

        private void lbV2rayCoreGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Misc.UI.VisitUrl(I18N.VisitV2rayCoreReleasePage, StrConst.V2rayCoreReleasePageUrl);
        }

        private void lbWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Misc.UI.VisitUrl(I18N.VistWikiPage, Properties.Resources.WikiLink);
        }

        private void lbIssue_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Misc.UI.VisitUrl(I18N.VisitVGCIssuePage, Properties.Resources.IssueLink);
        }

        private void lbCopyFromClipboard_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string text = Misc.Utils.GetClipboardText();
            slinkMgr.ImportLinkWithOutV2cfgLinks(text);
        }

        private void lbScanQRCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            void Success(string text)
            {
                var msg = VgcApis.Misc.Utils.AutoEllipsis(text, VgcApis.Models.Consts.AutoEllipsis.QrcodeTextMaxLength);
                setting.SendLog($"QRCode: {msg}");
                slinkMgr.ImportLinkWithOutV2cfgLinks(text);
            }

            void Fail()
            {
                MessageBox.Show(I18N.NoQRCode);
            }

            Libs.QRCode.QRCode.ScanQRCode(Success, Fail);
        }

        private void lbSimAddVmessWin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Views.WinForms.FormSimAddVmessClient.GetForm();
        }

        private void lbConfigEditor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Views.WinForms.FormConfiger.ShowConfig();
        }
    }
}
