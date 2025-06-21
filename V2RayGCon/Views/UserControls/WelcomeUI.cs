﻿using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class WelcomeUI : UserControl, BaseClasses.IFormMainFlyPanelComponent
    {
        readonly Services.Settings setting;
        readonly Services.ShareLinkMgr slinkMgr;
        readonly int marginBottom;

        public WelcomeUI()
        {
            setting = Services.Settings.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;

            InitializeComponent();
            marginBottom = this.Height - pnlBasicUsage.Top;
        }

        #region public method
        public void Cleanup() { }
        #endregion

        private void WelcomeFlyPanelComponent_Load(object sender, System.EventArgs e)
        {
            using (var core = new Libs.V2Ray.Core(setting))
            {
                if (!core.IsV2RayExecutableExist())
                {
                    return;
                }
            }

            pnlBasicUsage.Top = pnlDownloadV2RayCore.Top;
            pnlDownloadV2RayCore.Visible = false;
            this.Height = pnlBasicUsage.Top + marginBottom;
        }

        private void lbDownloadV2rayCore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WinForms.FormDownloadCore.ShowForm();
        }

        private void lbV2rayCoreGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var url = VgcApis.Models.Consts.Core.GetSourceUrlByIndex(1);
            VgcApis.Misc.UI.VisitUrl(I18N.VisitV2rayCoreReleasePage, url);
        }

        private void lbWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VgcApis.Misc.UI.VisitUrl(I18N.VistWikiPage, Properties.Resources.WikiLink);
        }

        private void lbIssue_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VgcApis.Misc.UI.VisitUrl(I18N.VisitVGCIssuePage, Properties.Resources.IssueLink);
        }

        private void lbCopyFromClipboard_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string text = VgcApis.Misc.Utils.ReadFromClipboard();
            slinkMgr.ImportLinkWithOutV2cfgLinks(text);
        }

        private void lbScanQRCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            void Success(string text)
            {
                var msg = VgcApis.Misc.Utils.AutoEllipsis(
                    text,
                    VgcApis.Models.Consts.AutoEllipsis.QrcodeTextMaxLength
                );
                VgcApis.Misc.Logger.Log($"QRCode: {msg}");
                slinkMgr.ImportLinkWithOutV2cfgLinks(text);
            }

            void Fail()
            {
                VgcApis.Misc.UI.MsgBox(I18N.NoQRCode);
            }

            Libs.QRCode.QRCode.ScanQRCode(Success, Fail);
        }

        private void lbConfigEditor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WinForms.FormTextConfigEditor.ShowEmptyConfig();
        }

        private void linkLabelAddSubs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WinForms.FormOption.ShowForm();
        }
    }
}
