using System.Windows.Forms;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    class TabUsage : ComponentCtrl
    {
        public TabUsage(
            LinkLabel linkLableTxthinkingPac,
            TextBox tboxReadme)
        {
            linkLableTxthinkingPac.Click += (s, a) => VgcApis.Libs.UI.VisitUrl(
                 Resources.Langs.I18N.VisitTxthinkingGithub,
                 Resources.Langs.StrConst.TxthinkingPacProjectUrl);

            tboxReadme.Text = Resources.Langs.I18N.UsageReadme;
        }

        #region public method

        public override void Cleanup()
        {
            // do nothing
        }

        public override bool IsOptionsChanged() => false;

        public override bool SaveOptions() => true;
        #endregion
    }
}
