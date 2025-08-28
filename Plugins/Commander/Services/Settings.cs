using System.Collections.Generic;
using System.Linq;
using Commander.Resources.Langs;
using Newtonsoft.Json;

namespace Commander.Services
{
    public class Settings
    {
        VgcApis.Interfaces.Services.ISettingsService vgcSetting;
        readonly string pluginName = Properties.Resources.Name;
        Models.Data.UserSettings userSettings;

        public Settings() { }

        public void Run(VgcApis.Interfaces.Services.IApiService vgcApi)
        {
            vgcSetting = vgcApi.GetSettingService();
            userSettings = LoadUserSettings();
        }

        #region public methods
        public Models.Data.CmderParam GetCurrentCmderParam()
        {
            var empty = new Models.Data.CmderParam();
            var names = GetCmderParamNames();
            if (names.Count < 1)
            {
                return empty;
            }
            var name = names.First();
            var r = userSettings.cmderParams.FirstOrDefault(p => p.name == name);
            if (r == null)
            {
                return empty;
            }
            return VgcApis.Misc.Utils.Clone(r);
        }

        public List<string> GetCmderParamNames()
        {
            var pps = userSettings.cmderParams;
            var names = pps.Select(p => p.name).OrderBy(name => name).ToList();
            return names;
        }

        public Models.Data.CmderParam GetCmderParamByName(string name)
        {
            var empty = new Models.Data.CmderParam();
            var r = userSettings.cmderParams.FirstOrDefault(p => p.name == name);
            if (r == null)
            {
                return empty;
            }
            return VgcApis.Misc.Utils.Clone(r);
        }

        public void RemoveCmderParamByName(string name)
        {
            var num = userSettings.cmderParams.RemoveAll(p => p.name == name);
            SaveUserSettings();
            if (num <= 0)
            {
                var msg = string.Format(I18N.RemoveFailed, name);
                VgcApis.Misc.UI.MsgBox(msg);
            }
        }

        public void SaveCmderParam(Models.Data.CmderParam cmderParam)
        {
            if (cmderParam == null)
            {
                return;
            }

            var pps = userSettings.cmderParams;
            pps.RemoveAll(pp => pp.name == cmderParam.name);
            var clone = VgcApis.Misc.Utils.Clone(cmderParam);
            pps.Add(clone);
            SaveUserSettings();
        }

        public void Cleanup() { }
        #endregion

        #region private methods
        void SaveUserSettings()
        {
            try
            {
                var content = VgcApis.Misc.Utils.SerializeObject(userSettings);
                vgcSetting.SavePluginsSetting(pluginName, content);
            }
            catch { }
        }

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
