using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using V2RayGCon.Libs.Lua;

namespace V2RayGCon.Services.PluginsComponents
{
    internal class PluginMgr : IDisposable
    {
        private readonly Settings settings;
        private readonly Apis apis;

        readonly object locker = new object();
        Dictionary<string, VgcApis.Interfaces.IPlugin> cache =
            new Dictionary<string, VgcApis.Interfaces.IPlugin>();
        List<string> internalPluginNames = new List<string>();

        public PluginMgr(Settings settings, Apis apis)
        {
            this.settings = settings;
            this.apis = apis;
        }

        #region properties

        #endregion

        #region public methods
        public List<Models.Datas.PluginInfoItem> GatherAllPluginInfos()
        {
            var enabled = GetEnabledPluginFileNames();

            var r = new List<Models.Datas.PluginInfoItem>();
            foreach (var name in internalPluginNames)
            {
                var info = ToPluginInfoItem(cache[name], name, enabled.Contains(name));
                r.Add(info);
            }

            var extPlugins = settings.isLoad3rdPartyPlugins
                ? LoadAllPluginFromDir()
                : new Dictionary<string, VgcApis.Interfaces.IPlugin>();

            foreach (var kv in extPlugins)
            {
                var name = kv.Key;
                var p = kv.Value;
                var info = ToPluginInfoItem(p, name, enabled.Contains(name));
                r.Add(info);
            }

            return r;
        }

        public List<ToolStripMenuItem> GetEnabledPluginMenus()
        {
            var menu = new List<ToolStripMenuItem>();
            var enabledList = GetEnabledPluginFileNames();
            foreach (var fileName in enabledList)
            {
                if (cache.TryGetValue(fileName, out var plugin) && plugin != null)
                {
                    var mi = plugin.GetToolStripMenu();
                    mi.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                    mi.ToolTipText = plugin.Description;
                    menu.Add(mi);
                }
            }
            return menu;
        }

        public void RestartAllPlugins()
        {
            var enabledList = GetEnabledPluginFileNames();
            var disabled = cache.Keys.Where(k => !enabledList.Contains(k)).ToList();

            foreach (var filename in disabled)
            {
                if (cache.TryGetValue(filename, out var plugin) && plugin != null)
                {
                    plugin.Stop();
                }
            }

            LoadPlugins(enabledList);

            foreach (var filename in enabledList)
            {
                if (cache.TryGetValue(filename, out var plugin) && plugin != null)
                {
                    plugin.Run(apis);
                }
            }
        }

        public ReadOnlyCollection<VgcApis.Interfaces.IPlugin> GetAllEnabledPlugins()
        {
            var result = new List<VgcApis.Interfaces.IPlugin>();
            var enabledList = GetEnabledPluginFileNames();
            lock (locker)
            {
                foreach (var filename in enabledList)
                {
                    var p = GetPlugin(filename);
                    if (p != null)
                    {
                        result.Add(p);
                    }
                }
            }
            return result.AsReadOnly();
        }

        public void Init()
        {
            var ps = new List<VgcApis.Interfaces.IPlugin>()
            {
                new Commander.Commander(),
                new NeoLuna.NeoLuna(),
                new Pacman.Pacman(),
                new ProxySetter.ProxySetter(),
            };

            internalPluginNames = ps.Select(p => p.Name).ToList();
            cache = ps.ToDictionary(p => p.Name);

            if (settings.isLoad3rdPartyPlugins)
            {
                var filenames = GetEnabledPluginFileNames()
                    .Where(fn => !internalPluginNames.Contains(fn))
                    .ToList();

                LoadPlugins(filenames);
            }
        }
        #endregion

        #region private methods
        void LoadPlugins(IEnumerable<string> filenames)
        {
            lock (locker)
            {
                foreach (var filename in filenames)
                {
                    GetPlugin(filename);
                }
            }
        }

        string CompatibleWithLuna(string filename)
        {
            if (filename == "Luna")
            {
                // 不要搜索libs目录的Luna.dll，那个版本不兼容
                var file = Path.Combine(VgcApis.Models.Consts.Files.PluginsDir, "Luna.dll");
                if (File.Exists(file))
                {
                    return file;
                }
            }
            return filename;
        }

        VgcApis.Interfaces.IPlugin GetPlugin(string filename)
        {
            if (cache.TryGetValue(filename, out var plugin) && plugin != null)
            {
                return plugin;
            }

            var dll = CompatibleWithLuna(filename);
            var p = LoadPluginFromFile(dll);
            if (p != null)
            {
                cache.Add(filename, p);
            }
            return p;
        }

        VgcApis.Interfaces.IPlugin LoadPluginFromFile(string dllFile)
        {
            if (!File.Exists(dllFile))
            {
                return null;
            }

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

        Dictionary<string, VgcApis.Interfaces.IPlugin> LoadAllPluginFromDir()
        {
            var dir = VgcApis.Models.Consts.Files.PluginsDir;
            var r = new Dictionary<string, VgcApis.Interfaces.IPlugin>();
            try
            {
                foreach (
                    string file in Directory.GetFiles(dir, @"*.dll", SearchOption.AllDirectories)
                )
                {
                    var p = LoadPluginFromFile(file);
                    if (p != null)
                    {
                        r.Add(file, p);
                    }
                }
            }
            catch { }
            return r;
        }

        List<string> GetEnabledPluginFileNames()
        {
            return settings
                .GetPluginInfoItems()
                .Where(pi => pi.isUse)
                .Select(pi => pi.filename)
                .ToList();
        }

        void RemovePlugins(List<string> filenames)
        {
            List<VgcApis.Interfaces.IPlugin> ps = null;

            lock (locker)
            {
                ps = cache.Where(kv => filenames.Contains(kv.Key)).Select(kv => kv.Value).ToList();

                foreach (var name in filenames)
                {
                    cache.Remove(name);
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

        Models.Datas.PluginInfoItem ToPluginInfoItem(
            VgcApis.Interfaces.IPlugin plugin,
            string filename,
            bool isEanbled
        )
        {
            if (plugin == null)
            {
                return null;
            }

            return new Models.Datas.PluginInfoItem()
            {
                name = plugin.Name,
                filename = filename,
                isUse = isEanbled,
                version = plugin.Version,
                description = plugin.Description,
            };
        }

        void Cleanup()
        {
            foreach (var kv in cache)
            {
                kv.Value?.Dispose();
            }
        }
        #endregion


        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    Cleanup();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~PluginCache()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region protected methods

        #endregion
    }
}
