using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Service
{
    class PluginsServer : Model.BaseClass.SingletonService<PluginsServer>
    {
        Setting setting;
        Notifier notifier;

        public event EventHandler OnRequireMenuUpdate;

        Lib.Lua.Apis vgcApis = new Lib.Lua.Apis();

        Dictionary<string, VgcApis.Models.Interfaces.IPlugin> plugins =
            new Dictionary<string, VgcApis.Models.Interfaces.IPlugin>();

        PluginsServer() { }

        public void Run(
            Setting setting,
            Servers servers,
            ConfigMgr configMgr,
            ShareLinkMgr slinkMgr,
            Notifier notifier)
        {
            this.setting = setting;
            this.notifier = notifier;

            vgcApis.Run(setting, servers, configMgr, slinkMgr, notifier);
            plugins = LoadAllPlugins();
            RestartAllPlugins();
        }

        #region properties
        #endregion

        #region public methods
        public ReadOnlyCollection<VgcApis.Models.Interfaces.IPlugin> GetAllEnabledPlugins()
        {
            var result = new List<VgcApis.Models.Interfaces.IPlugin>();
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

        public List<Model.Data.PluginInfoItem> GetterAllPluginsInfo()
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

        public Dictionary<string, VgcApis.Models.Interfaces.IPlugin> LoadAllPlugins()
        {
            // Original design of plugins would load dll files from file system.
            // That is why loading logic looks so complex.
            var pluginList = new Dictionary<string, VgcApis.Models.Interfaces.IPlugin>();
            var plugins = new List<VgcApis.Models.Interfaces.IPlugin>();

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

        List<Model.Data.PluginInfoItem> GetPluginInfoFrom(
            Dictionary<string, VgcApis.Models.Interfaces.IPlugin> pluginList)
        {
            if (pluginList.Count <= 0)
            {
                return new List<Model.Data.PluginInfoItem>();
            }

            var enabledList = GetCurEnabledPluginFileNames();
            var infos = new List<Model.Data.PluginInfoItem>();
            foreach (var item in pluginList)
            {
                var plugin = item.Value;
                var filename = item.Key;
                var pluginInfo = new Model.Data.PluginInfoItem
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
            CleanupPlugins(plugins.Keys.ToList());
            plugins = new Dictionary<string, VgcApis.Models.Interfaces.IPlugin>();
            vgcApis.Dispose();
        }
        #endregion
    }
}
