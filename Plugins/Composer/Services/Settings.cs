using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using Composer.Resources.Langs;

namespace Composer.Services
{
    public class Settings
    {
        VgcApis.Interfaces.Services.ISettingsService vgcSetting;
        VgcApis.Interfaces.Services.IServersService vgcServer;
        readonly string pluginName = Properties.Resources.Name;
        Models.UserSettings userSettings;

        public Settings() { }

        public void Run(VgcApis.Interfaces.Services.IApiService vgcApi)
        {
            vgcSetting = vgcApi.GetSettingService();
            vgcServer = vgcApi.GetServersService();

            userSettings = LoadUserSettings();
            userSettings.Normalize();
        }

        #region public methods
        public string ComposeServers(Models.PackageItem pkgItem)
        {
            var upkg = userSettings.packages.FirstOrDefault(el => el.name == pkgItem.name);
            var options = ToComposOptions(pkgItem);

            var cfg = "";
            try
            {
                cfg = vgcServer.ComposeServersToString(options, upkg?.uid);
            }
            catch (Exception ex)
            {
                VgcApis.Misc.UI.MsgBox($"{ex.GetType().Name}: {ex.Message}");
                return "";
            }

            if (string.IsNullOrEmpty(cfg))
            {
                VgcApis.Misc.UI.MsgBox(I18N.PackServFailed);
                return "";
            }

            if (vgcServer.GetServerByConfig(cfg) != null)
            {
                VgcApis.Misc.UI.MsgBox(I18N.ErrSameConfigExists);
                return "";
            }

            var uid = vgcServer.ReplaceOrAddNewServer(upkg?.uid, pkgItem.name, cfg, "Composer");
            if (upkg != null && upkg.uid != uid)
            {
                upkg.uid = uid;
                SaveUserSettings();
            }
            var title = vgcServer.GetServerByUid(uid)?.GetCoreStates().GetTitle();
            VgcApis.Misc.UI.MsgBox($"{I18N.PackServSuccess}\n{title}");
            return uid ?? "";
        }

        public List<Models.ServerInfoItem> RemoveNonExistServers(
            IEnumerable<Models.ServerInfoItem> servInfos
        )
        {
            var uids = servInfos.Select(el => el.uid).ToList();
            var filtered = vgcServer
                .GetServersByUids(uids)
                .Select(el => el.GetCoreStates().GetUid())
                .ToList();
            return servInfos.Where(el => filtered.Contains(el.uid)).ToList();
        }

        public IReadOnlyCollection<VgcApis.Interfaces.ICoreServCtrl> GetServers(string filter)
        {
            return vgcServer.GetFilteredServers(filter);
        }

        public List<VgcApis.Interfaces.ICoreServCtrl> GetServersSelected()
        {
            return vgcServer.GetSelectedServers();
        }

        public void SaveCurPackageName(string name)
        {
            this.userSettings.curPackageName = name;
            SaveUserSettings();
        }

        public string GetCurPackageName() => userSettings.curPackageName;

        public List<Models.PackageItem> GetPackageItems() =>
            VgcApis.Misc.Utils.Clone(userSettings.packages);

        public void SavePackageItems(List<Models.PackageItem> packageItems)
        {
            this.userSettings.packages = VgcApis.Misc.Utils.Clone(packageItems);
            SaveUserSettings();
        }

        public void Cleanup() { }
        #endregion

        #region private methods
        VgcApis.Models.Composer.Options ToComposOptions(Models.PackageItem pkgItem)
        {
            var options = new VgcApis.Models.Composer.Options()
            {
                isAppend = pkgItem.isAppend,
                skelecton = pkgItem.skelecton,
            };

            foreach (var selector in pkgItem.selectors)
            {
                var s = new VgcApis.Models.Composer.Selector()
                {
                    tag = selector.tag,
                    filter = selector.filter,
                };
                foreach (var servInfo in selector.servInfos)
                {
                    s.uids.Add(servInfo.uid);
                }
                options.selectors.Add(s);
            }
            return options;
        }

        void SaveUserSettings()
        {
            try
            {
                var content = VgcApis.Misc.Utils.SerializeObject(userSettings);
                vgcSetting.SavePluginsSetting(pluginName, content);
            }
            catch { }
        }

        Models.UserSettings LoadUserSettings()
        {
            var empty = new Models.UserSettings();
            var userSettingString = vgcSetting.GetPluginsSetting(pluginName);
            if (string.IsNullOrEmpty(userSettingString))
            {
                return empty;
            }

            try
            {
                var result = VgcApis.Misc.Utils.DeserializeObject<Models.UserSettings>(
                    userSettingString
                );
                return result ?? empty;
            }
            catch { }

            return empty;
        }
        #endregion
    }
}
