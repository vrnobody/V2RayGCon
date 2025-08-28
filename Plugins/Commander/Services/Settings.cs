using System.Collections.Generic;
using Commander.Resources.Langs;

namespace Commander.Services
{
    public class Settings
    {
        VgcApis.Interfaces.Services.ISettingsService vgcSetting;
        VgcApis.Interfaces.Services.IServersService vgcServers;
        readonly string pluginName = Properties.Resources.Name;
        Models.Data.UserSettings userSettings;

        public Settings() { }

        public void Run(VgcApis.Interfaces.Services.IApiService vgcApi)
        {
            vgcServers = vgcApi.GetServersService();
            vgcSetting = vgcApi.GetSettingService();
            userSettings = LoadUserSettings();
        }

        #region properties

        #endregion

        #region public methods
        public string Chain(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            string orgServerUid,
            string packageName
        )
        {
            return vgcServers.PackServersV4Ui(
                servList,
                orgServerUid,
                packageName,
                string.Empty,
                string.Empty,
                VgcApis.Models.Datas.Enums.BalancerStrategies.Random,
                VgcApis.Models.Datas.Enums.PackageTypes.Chain
            );
        }

        public string Pack(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            string orgServerUid,
            string packageName,
            string interval,
            string url,
            VgcApis.Models.Datas.Enums.BalancerStrategies strategy
        )
        {
            return vgcServers.PackServersV4Ui(
                servList,
                orgServerUid,
                packageName,
                interval,
                url,
                strategy,
                VgcApis.Models.Datas.Enums.PackageTypes.Balancer
            );
        }

        public IReadOnlyCollection<VgcApis.Interfaces.ICoreServCtrl> GetAllServersList() =>
            vgcServers.GetAllServersOrderByIndex();

        public List<Models.Data.ProcParam> GetProcParams()
        {
            return userSettings.procParams;
        }

        public Models.Data.ProcParam GetProcParamByIndex(int index)
        {
            var max = userSettings.procParams.Count;
            if (max <= 0)
            {
                return new Models.Data.ProcParam();
            }

            index = VgcApis.Misc.Utils.Clamp(index, 0, max);
            return userSettings.procParams[index];
        }

        public void RemoveProcParamByName(string name)
        {
            var num = userSettings.procParams.RemoveAll(p => p.name == name);
            SaveUserSettings();
            if (num <= 0)
            {
                var msg = string.Format(I18N.RemoveFailed, name);
                VgcApis.Misc.UI.MsgBox(msg);
            }
        }

        public bool SavePackage(Models.Data.ProcParam procParam)
        {
            if (procParam == null)
            {
                return false;
            }

            var pps = userSettings.procParams;
            pps.RemoveAll(pp => pp.name == procParam.name);
            pps.Add(procParam);
            SaveUserSettings();
            return true;
        }

        public void SaveUserSettings()
        {
            try
            {
                var content = VgcApis.Misc.Utils.SerializeObject(userSettings);
                vgcSetting.SavePluginsSetting(pluginName, content);
            }
            catch { }
        }

        public void Cleanup() { }
        #endregion

        #region private methods
        Models.Data.UserSettings LoadUserSettings()
        {
            var empty = new Models.Data.UserSettings();
            var userSettingString = vgcSetting.GetPluginsSetting(pluginName);
            if (string.IsNullOrEmpty(userSettingString))
            {
                return empty;
            }

            try
            {
                var result = VgcApis.Misc.Utils.DeserializeObject<Models.Data.UserSettings>(
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
