using Luna.Resources.Langs;
using System.Drawing;
using System.Windows.Forms;

namespace Luna
{
    // Using lunar not lua to void naming conflicts.
    public class Luna : VgcApis.Models.BaseClasses.Plugin
    {
        Services.Settings settings;
        Services.LuaServer luaServer;
        Services.FormMgr formMgr;
        Services.MenuUpdater menuUpdater;

        ToolStripMenuItem miRoot = null, miShowWindow;
        public Luna()
        {
            miShowWindow = new ToolStripMenuItem(I18N.ShowWindow, null, (s, a) => Show());
        }

        #region properties
        public override string Name => Properties.Resources.Name;
        public override string Version => Properties.Resources.Version;
        public override string Description => I18N.Description;

        // icon from http://lua-users.org/wiki/LuaLogo
        public override Image Icon => Properties.Resources.Lua_Logo_32x32;
        #endregion

        #region public overrides
        public override ToolStripMenuItem GetMenu()
        {
            if (miRoot == null)
            {
                miRoot = new ToolStripMenuItem(this.Name, this.Icon);
            }

            return miRoot;
        }
        #endregion

        #region protected overrides
        protected override void Popup()
        {
            formMgr.ShowOrCreateFirstForm();
        }

        protected override void Start(VgcApis.Models.IServices.IApiService api)
        {
            var vgcSettings = api.GetSettingService();
            var vgcNotifier = api.GetNotifierService();

            miRoot = GetMenu(); // make sure miRoot is not null

            settings = new Services.Settings();
            luaServer = new Services.LuaServer();
            formMgr = new Services.FormMgr();
            menuUpdater = new Services.MenuUpdater(vgcNotifier);

            settings.Run(vgcSettings);
            luaServer.Run(settings, api);
            formMgr.Run(settings, luaServer, api);
            menuUpdater.Run(luaServer, miRoot, miShowWindow);
        }

        protected override void Stop()
        {
            settings?.SetIsDisposing(true);
            menuUpdater?.Dispose();
            formMgr?.Dispose();
            luaServer?.Dispose();
            settings?.Dispose();
        }
        #endregion
    }
}
