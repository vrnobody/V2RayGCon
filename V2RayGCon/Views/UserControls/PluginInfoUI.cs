using System.Windows.Forms;

namespace V2RayGCon.Views.UserControls
{
    public partial class PluginInfoUI : UserControl
    {
        Model.Data.PluginInfoItem curInfo;

        public PluginInfoUI(Model.Data.PluginInfoItem pluginInfo)
        {
            curInfo = pluginInfo;
            InitializeComponent();
            lbFilename.Text = pluginInfo.name + " v" + pluginInfo.version;
            lbDescription.Text = pluginInfo.description;
            chkIsUse.Checked = pluginInfo.isUse;
        }

        public Model.Data.PluginInfoItem GetValue()
        {
            return curInfo;
        }

        private void chkIsUse_CheckedChanged(object sender, System.EventArgs e)
        {
            var use = chkIsUse.Checked;
            if (use != curInfo.isUse)
            {
                curInfo.isUse = use;
            }
        }
    }
}
