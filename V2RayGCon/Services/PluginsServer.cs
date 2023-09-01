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
        Settings setting;
        Notifier notifier;

        public event EventHandler OnRequireMenuUpdate;

        Libs.Lua.Apis vgcApis = new Libs.Lua.Apis();

        readonly object pluginsLocker = new object();

        Dictionary<string, VgcApis.Interfaces.IPlugin> pluginList =
             new Dictionary<string, VgcApis.Interfaces.IPlugin>();

        List<string> internalPluginNames = new List<string>();

        PluginsServer()
        {

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

            LoadAllPlugins();
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
                if (!pluginList.ContainsKey(filename))
                {
                    continue;
                }
                result.Add(pluginList[filename]);
            }
            return result.AsReadOnly();
        }

        public void RestartAllPlugins()
        {
            var enabledList = GetCurEnabledPluginFileNames();

            foreach (var kv in pluginList)
            {
                var name = kv.Key;
                var p = kv.Value;
                if (enabledList.Contains(name))
                {
                    p.Run(vgcApis);
                }
                else
                {
                    p.Stop();
                }
            }

            UpdateNotifierMenu(enabledList);
            InvokeEventOnRequireMenuUpdateIgnoreError();
        }

        public List<Models.Datas.PluginInfoItem> GetterAllPluginsInfo()
        {
            return GetPluginInfoFrom(pluginList);
        }

        public void RefreshPluginList()
        {
            var extPlugins = setting.isLoad3rdPartyPlugins ?
                LoadAllPluginFromDir() :
                new List<VgcApis.Interfaces.IPlugin>();

            var extNames = extPlugins.Select(p => p.Name).ToList();
            var curNames = pluginList.Keys.Where(k => !internalPluginNames.Contains(k)).ToList();
            RemoveMissingPlugins(extNames, curNames);
            lock (pluginsLocker)
            {
                foreach (var ep in extPlugins)
                {
                    var name = ep.Name;
                    if (!pluginList.ContainsKey(name))
                    {
                        pluginList.Add(name, ep);
                    }
                }
            }

            var enabledList = GetCurEnabledPluginFileNames();
            UpdateNotifierMenu(enabledList);
        }
        #endregion

        #region private methods
        void RemoveMissingPlugins(List<string> extNames, List<string> curNames)
        {
            var names = curNames.Where(n => !extNames.Contains(n)).ToList();

            var ps = pluginList.Where(kv => names.Contains(kv.Key))
                .Select(kv => kv.Value)
                .ToList();

            lock (pluginsLocker)
            {
                foreach (var name in names)
                {
                    pluginList.Remove(name);
                }
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                foreach (var p in ps)
                {
                    p.Dispose();
                }
            });
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
                if (!pluginList.ContainsKey(fileName))
                {
                    continue;
                }

                var plugin = pluginList[fileName];
                var mi = plugin.GetToolStripMenu();
                mi.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                mi.ToolTipText = plugin.Description;
                children.Add(mi);
            }

            notifier.UpdatePluginMenu(children);
        }

        VgcApis.Interfaces.IPlugin LoadPluginFromFile(string dllFile)
        {
            var iName = nameof(VgcApis.Interfaces.IPlugin);
            try
            {
                // 不要用File.ReadAllBytes()然后Assembly.Load(bytes)
                // 多次加载相同插件时，对象名称后面会多个"_2"
                var assembly = Assembly.LoadFrom(dllFile);
                foreach (var ty in assembly.GetExportedTypes())
                {
                    if (ty.GetInterface(iName, false) != null)
                    {
                        return Activator.CreateInstance(ty) as VgcApis.Interfaces.IPlugin;
                    }
                }
            }
            catch { }
            return null;
        }

        List<VgcApis.Interfaces.IPlugin> LoadAllPluginFromDir()
        {
            var dir = VgcApis.Models.Consts.Files.PluginsDir;
            var r = new List<VgcApis.Interfaces.IPlugin>();
            try
            {
                foreach (string file in Directory.GetFiles(dir, @"*.dll", SearchOption.AllDirectories))
                {
                    var p = LoadPluginFromFile(file);
                    if (p != null)
                    {
                        r.Add(p);
                    }
                }
            }
            catch { }
            return r;
        }

        void LoadAllPlugins()
        {
            var ps = new List<VgcApis.Interfaces.IPlugin>() {
                new NeoLuna.NeoLuna(),
                new Pacman.Pacman(),
                new ProxySetter.ProxySetter(),
            };

            internalPluginNames = ps.Select(p => p.Name).ToList();

            if (setting.isLoad3rdPartyPlugins)
            {

                ps.AddRange(LoadAllPluginFromDir());
            }

            pluginList = ps.ToDictionary(p => p.Name);
        }

        void DisposePlugins(List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                if (pluginList.ContainsKey(fileName))
                {
                    pluginList[fileName].Dispose();
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
            DisposePlugins(pluginList.Keys.ToList());
            pluginList.Clear();
            vgcApis.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info("PluginsServer.Cleanup() done");
        }
        #endregion
    }
}
