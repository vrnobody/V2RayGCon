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
        Services.FormMgrSvc formMgr;
        Services.AstServer astServer;
        Services.MenuUpdater menuUpdater;

        readonly ToolStripMenuItem miRoot, miShowMgr, miShowEditor;
        public Luna()
        {
            ToolStripMenuItem mr = null, msw = null, mse = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                mr = new ToolStripMenuItem(this.Name, this.Icon);

                msw = new ToolStripMenuItem(
                    I18N.OpenScriptManger,
                    Properties.Resources.StoredProcedureScript_16x,
                    (s, a) => ShowMainForm());

                mse = new ToolStripMenuItem(
                    I18N.OpenScriptEditor,
                    Properties.Resources.EditWindow_16x,
                    (s, a) => formMgr?.ShowOrCreateFirstEditor());

            });

            miRoot = mr;
            miShowMgr = msw;
            miShowEditor = mse;
        }

        #region properties
        public override string Name => Properties.Resources.Name;
        public override string Version => Properties.Resources.Version;
        public override string Description => I18N.Description;

        // icon from http://lua-users.org/wiki/LuaLogo
        public override Image Icon => Properties.Resources.Lua_Logo_32x32;
        #endregion

        #region public overrides
        public override ToolStripMenuItem GetToolStripMenu() => miRoot;

        public override void ShowMainForm()
        {
            if (!GetState())
            {
                return;
            }
            // formMgr.ShowOrCreateFirstEditor();
            formMgr.ShowFormMain();
        }

        public override void Run(VgcApis.Interfaces.Services.IApiService api)
        {
            if (!SetState(true))
            {
                return;
            }

            var vgcSettings = api.GetSettingService();

            settings = new Services.Settings();
            settings.Run(vgcSettings);

            astServer = new Services.AstServer();
            astServer.Run();

            luaServer = new Services.LuaServer(settings);
            formMgr = new Services.FormMgrSvc(settings, luaServer, astServer, api);
            luaServer.Run(formMgr);

            menuUpdater = new Services.MenuUpdater();
            menuUpdater.Run(luaServer, miRoot, miShowMgr, miShowEditor);

            luaServer.WakeUpAutoRunScripts(TimeSpan.FromSeconds(2));
        }

        public override void Stop()
        {
            if (!SetState(false))
            {
                return;
            }

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
