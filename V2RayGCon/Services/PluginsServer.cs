using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace V2RayGCon.Services
{
    class PluginsServer : BaseClasses.SingletonService<PluginsServer>
    {
        Settings setting;
        Notifier notifier;

        public event EventHandler OnRequireMenuUpdate;

        Libs.Lua.Apis vgcApis = new Libs.Lua.Apis();

        readonly Dictionary<string, VgcApis.Interfaces.IPlugin> plugins =
             new Dictionary<string, VgcApis.Interfaces.IPlugin>();

        PluginsServer()
        {
            plugins = LoadAllPlugins();
        }

        public void Run(
            Settings setting,
            Servers servers,
            ConfigMgr configMgr,
            ShareLinkMgr slinkMgr,
            Notifier notifier)
        {
            this.setting = setting;
            this.notifier = notifier;
            vgcApis.Run(setting, servers, configMgr, slinkMgr, notifier);
        }

        #region properties
        #endregion

        #region public methods
        public ReadOnlyCollection<VgcApis.Interfaces.IPlugin> GetAllEnabledPlugins()
        {
            var result = new List<VgcApis.Interfaces.IPlugin>();
            var enabledList = GetCurEnabledPluginFileNames();
            foreach (var filename in enabledList)
            {
                if (!plugins.ContainsKey(filename))
                {
                    continue;
                }
                result.Add(plugins[filename]);
            }
            return result.AsReadOnly();
        }

        public void RestartAllPlugins()
        {
            var enabledList = GetCurEnabledPluginFileNames();

            foreach (var p in plugins)
            {
                if (enabledList.Contains(p.Key))
                {
                    p.Value.Run(vgcApis);
                }
                else
                {
                    p.Value.Cleanup();
                }
            }

            UpdateNotifierMenu(enabledList);
            InvokeEventOnRequireMenuUpdateIgnoreError();
        }

        public void StopAllPlugins()
        {
            foreach (var p in plugins)
            {
                p.Value.Cleanup();
            }
            UpdateNotifierMenu(null);
        }

        public List<Models.Datas.PluginInfoItem> GetterAllPluginsInfo()
        {
            return GetPluginInfoFrom(plugins);
        }

        #endregion

        #region private methods

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

        /// <summary>
        /// Update plugin menu item.
        /// </summary>
        /// <param name="enabledList">nullable</param>
        void UpdateNotifierMenu(List<string> enabledList)
        {
            if (enabledList == null || enabledList.Count() <= 0)
            {
                notifier.UpdatePluginMenu(null);
                return;
            }

            var children = new List<ToolStripMenuItem>();
            foreach (var fileName in enabledList)
            {
                if (!plugins.ContainsKey(fileName))
                {
                    continue;
                }

                var plugin = plugins[fileName];
                var mi = plugin.GetMenu();
                mi.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                mi.ToolTipText = plugin.Description;
                children.Add(mi);
            }

            notifier.UpdatePluginMenu(children);
        }

        public Dictionary<string, VgcApis.Interfaces.IPlugin> LoadAllPlugins()
        {
            // Original design of plug-in system would load dll files from hard drive.
            // That is why loading logic looks so complex.
            var pluginList = new Dictionary<string, VgcApis.Interfaces.IPlugin>();
            var plugins = new List<VgcApis.Interfaces.IPlugin>();

            plugins.Add(new Luna.Luna());
            plugins.Add(new Pacman.Pacman());

            // Many thanks to windows defender
            plugins.Add(new ProxySetter.ProxySetter());

            plugins.Add(new Statistics.Statistics());

            foreach (var plugin in plugins)
            {
                pluginList.Add(plugin.Name, plugin);
            }
            return pluginList;
        }

        void CleanupPlugins(List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                if (plugins.ContainsKey(fileName))
                {
                    plugins[fileName].Cleanup();
                }
            }
        }

        List<string> GetCurEnabledPluginFileNames()
        {
            var list = setting.GetPluginInfoItems();
            return list
                .Where(p => p.isUse)
                .Select(p => p.filename)
                .ToList();
        }

        List<Models.Datas.PluginInfoItem> GetPluginInfoFrom(
            Dictionary<string, VgcApis.Interfaces.IPlugin> pluginList)
        {
            if (pluginList.Count <= 0)
            {
                return new List<Models.Datas.PluginInfoItem>();
            }

            var enabledList = GetCurEnabledPluginFileNames();
            var infos = new List<Models.Datas.PluginInfoItem>();
            foreach (var item in pluginList)
            {
                var plugin = item.Value;
                var filename = item.Key;
                var pluginInfo = new Models.Datas.PluginInfoItem
                {
                    filename = filename,
                    name = plugin.Name,
                    version = plugin.Version,
                    description = plugin.Description,
                    isUse = enabledList.Contains(filename),
                };
                infos.Add(pluginInfo);
            }
            return infos;
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            VgcApis.Libs.Sys.FileLogger.Info("PluginsServer.Cleanup() begin");
            CleanupPlugins(plugins.Keys.ToList());
            plugins.Clear();
            vgcApis.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info("PluginsServer.Cleanup() done");
        }
        #endregion
    }
}
