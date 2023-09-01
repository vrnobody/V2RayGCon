using ProxySetter.Resources.Langs;
using System.Drawing;
using System.Windows.Forms;

namespace ProxySetter
{
    public class ProxySetter : VgcApis.BaseClasses.Plugin
    {
        Services.PsLuncher luncher;
        public ProxySetter() { }

        #region private methods

        #endregion

        #region properties
        ToolStripMenuItem menuItemCache = null;
        public override ToolStripMenuItem GetToolStripMenu()
        {
            if (menuItemCache != null)
            {
                return menuItemCache;
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                menuItemCache = new ToolStripMenuItem(Name, Icon);
                menuItemCache.ToolTipText = Description;

                var children = menuItemCache.DropDownItems;
                children.Add(new ToolStripMenuItem(
                    I18N.Options,
                    Properties.Resources.WebConfiguration_16x,
                    (s, a) => ShowMainForm()));
                children.Add(new ToolStripSeparator());
                children.AddRange(luncher?.GetSubMenu());
            });

            return menuItemCache;
        }

        public override string Name => Properties.Resources.Name;
        public override string Version => Properties.Resources.Version;
        public override string Description => I18N.Description;

        public override Image Icon => Properties.Resources.VBDynamicWeb_16x;
        #endregion

        #region public overrides
        public override void Run(VgcApis.Interfaces.Services.IApiService api)
        {
            if (!SetState(true))
            {
                return;
            }

            luncher = new Services.PsLuncher();
            luncher.Run(api);
            menuItemCache = null;
        }

        public override void ShowMainForm()
        {
            if (!GetState())
            {
                return;
            }

            VgcApis.Misc.UI.Invoke(() => luncher?.Show());
        }

        public override void Stop()
        {
            if (!SetState(false))
            {
                return;
            }
            luncher?.Cleanup();
        }
        #endregion
    }
}
