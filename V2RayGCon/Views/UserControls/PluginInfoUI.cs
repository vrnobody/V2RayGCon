using System.Windows.Forms;

namespace V2RayGCon.Views.UserControls
{
    public partial class PluginInfoUI : UserControl
    {
        readonly Models.Datas.PluginInfoItem curInfo;

        public PluginInfoUI(Models.Datas.PluginInfoItem pluginInfo)
        {
            curInfo = pluginInfo;
            InitializeComponent();
            lbFilename.Text = pluginInfo.name + " v" + pluginInfo.version;
            lbDescription.Text = pluginInfo.description;
            chkIsUse.Checked = pluginInfo.isUse;
        }

        public Models.Datas.PluginInfoItem GetValue()
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
