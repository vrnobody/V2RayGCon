using Luna.Resources.Langs;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Luna
{
    // Using lunar not lua to void naming conflicts.
    public class Luna : VgcApis.BaseClasses.Plugin
    {
        Services.Settings settings;
        Services.LuaServer luaServer;
        Services.FormMgr formMgr;
        Services.MenuUpdater menuUpdater;

        readonly ToolStripMenuItem miRoot, miShowWindow;
        public Luna()
        {
            ToolStripMenuItem mr = null, msw = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                mr = new ToolStripMenuItem(this.Name, this.Icon);

                msw = new ToolStripMenuItem(
                    I18N.OpenScriptManger,
                    Properties.Resources.StoredProcedureScript_16x,
                    (s, a) => Show());
            });

            miRoot = mr;
            miShowWindow = msw;
        }

        #region properties
        public override string Name => Properties.Resources.Name;
        public override string Version => Properties.Resources.Version;
        public override string Description => I18N.Description;

        // icon from http://lua-users.org/wiki/LuaLogo
        public override Image Icon => Properties.Resources.Lua_Logo_32x32;
        #endregion

        #region public overrides
        public override ToolStripMenuItem GetMenu() => miRoot;

        #endregion

        #region protected overrides
        protected override void Popup()
        {
            formMgr.ShowOrCreateFirstForm();
        }

        protected override void Start(VgcApis.Interfaces.Services.IApiService api)
        {
            var vgcSettings = api.GetSettingService();
            var vgcNotifier = api.GetNotifierService();

            settings = new Services.Settings();
            luaServer = new Services.LuaServer();
            formMgr = new Services.FormMgr();
            menuUpdater = new Services.MenuUpdater(settings);

            settings.Run(vgcSettings, vgcNotifier);
            luaServer.Run(api, settings, formMgr);
            formMgr.Run(settings, luaServer, api);
            menuUpdater.Run(luaServer, miRoot, miShowWindow);

            luaServer.WakeUpAutoRunScripts(TimeSpan.FromSeconds(2));
        }

        protected override void Stop()
        {
            VgcApis.Libs.Sys.FileLogger.Info("Luna.Cleanup() begin");
            settings?.SetIsDisposing(true);
            menuUpdater?.Dispose();
            formMgr?.Dispose();
            luaServer?.Dispose();
            settings?.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info("Luna.Cleanup() end");
        }
        #endregion
    }
}
