using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace V2RayGCon.Services
{
    class PluginsServer : BaseClasses.SingletonService<PluginsServer>
    {
        Settings settings;
        Notifier notifier;

        public event EventHandler OnRequireMenuUpdate;

        Libs.Lua.Apis apis = new Libs.Lua.Apis();

        PluginsComponents.PluginMgr pluginsMgr = null;

        PluginsServer() { }

        public void Run(
            Settings setting,
            Servers servers,
            ConfigMgr configMgr,
            ShareLinkMgr slinkMgr,
            Notifier notifier
        )
        {
            this.settings = setting;
            this.notifier = notifier;

            apis.Run(setting, servers, configMgr, slinkMgr, notifier);

            pluginsMgr = new PluginsComponents.PluginMgr(settings, apis);
            pluginsMgr.Init();
        }

        #region properties
        #endregion

        #region public methods
        public ReadOnlyCollection<VgcApis.Interfaces.IPlugin> GetAllEnabledPlugins() =>
            pluginsMgr.GetAllEnabledPlugins();

        public void RestartAllPlugins()
        {
            pluginsMgr.RestartAllPlugins();
            RefreshMenuItems();
        }

        public List<Models.Datas.PluginInfoItem> GatherAllPluginInfos() =>
            pluginsMgr.GatherAllPluginInfos();
        #endregion

        #region private methods
        void RefreshMenuItems()
        {
            var menu = pluginsMgr.GetEnabledPluginMenus();
            notifier.UpdatePluginMenu(menu);
            InvokeEventOnRequireMenuUpdateIgnoreError();
        }

        void InvokeEventOnRequireMenuUpdateIgnoreError()
        {
            try
            {
                OnRequireMenuUpdate?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                // do not hurt me.
            }
        }

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            VgcApis.Libs.Sys.FileLogger.Info("PluginsServer.Cleanup() begin");
            pluginsMgr.Dispose();
            apis.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info("PluginsServer.Cleanup() done");
        }
        #endregion
    }
}
