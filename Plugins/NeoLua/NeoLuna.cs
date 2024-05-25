using System;
using System.Drawing;
using System.Windows.Forms;
using NeoLuna.Resources.Langs;

namespace NeoLuna
{
    // Using NeoLuna not lua to void naming conflicts.
    public class NeoLuna : VgcApis.BaseClasses.Plugin
    {
        Services.Settings settings;
        Services.LuaServer luaServer;
        Services.FormMgrSvc formMgr;
        Services.AstServer astServer;
        Services.MenuUpdater menuUpdater;

        readonly ToolStripMenuItem miRoot,
            miShowMgr,
            miShowEditor,
            miShowLog;

        public NeoLuna()
        {
            ToolStripMenuItem mr = null,
                msw = null,
                mse = null,
                mlog = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                mr = new ToolStripMenuItem(this.Name, this.Icon);

                msw = new ToolStripMenuItem(
                    I18N.OpenScriptManger,
                    Properties.Resources.StoredProcedureScript_16x,
                    (s, a) => ShowMainForm()
                );

                mse = new ToolStripMenuItem(
                    I18N.OpenScriptEditor,
                    Properties.Resources.EditWindow_16x,
                    (s, a) => formMgr?.ShowOrCreateFirstEditor()
                );

                mlog = new ToolStripMenuItem(
                    I18N.OpenLogWindow,
                    Properties.Resources.FSInteractiveWindow_16x,
                    (s, a) => formMgr?.ShowFormLog()
                );
            });

            miRoot = mr;
            miShowMgr = msw;
            miShowEditor = mse;
            miShowLog = mlog;
        }

        #region properties
        public override string Name => Properties.Resources.Name;
        public override string Version => Properties.Resources.Version;
        public override string Description => I18N.Description;

        // https://icons8.com/icon/1ovKzLWSCiN9/lua-is-a-lightweight%2C-multi-paradigm-programming-language
        public override Image Icon => Properties.Resources.lua_lang;
        #endregion

        #region public overrides
        public override ToolStripMenuItem GetToolStripMenu() => miRoot;

        public override void ShowMainForm()
        {
            if (!GetRunningState())
            {
                return;
            }
            // formMgr.ShowOrCreateFirstEditor();
            formMgr.ShowFormMain();
        }

        public override void Run(VgcApis.Interfaces.Services.IApiService api)
        {
            if (!SetRunningState(true))
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
            menuUpdater.Run(luaServer, miRoot, miShowMgr, miShowEditor, miShowLog);

            luaServer.WakeUpAutoRunScripts(TimeSpan.FromSeconds(2));
        }

        public override void Stop()
        {
            if (!SetRunningState(false))
            {
                return;
            }

            VgcApis.Libs.Sys.FileLogger.Info("NeoLuna.Cleanup() begin");
            settings?.SetIsDisposing(true);
            menuUpdater?.Dispose();
            formMgr?.Dispose();
            luaServer?.Dispose();
            settings?.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info("NeoLuna.Cleanup() end");
        }
        #endregion
    }
}
