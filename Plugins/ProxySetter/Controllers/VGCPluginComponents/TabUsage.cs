using System.Windows.Forms;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    class TabUsage : ComponentCtrl
    {
        public TabUsage(TextBox tboxReadme)
        {
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
