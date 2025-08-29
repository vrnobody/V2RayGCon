﻿using System.Collections.Generic;
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
        public Models.Data.CmderParam GetFirstCmderParam()
        {
            var first = userSettings.cmderParams.OrderBy(p => p.name).FirstOrDefault();
            return VgcApis.Misc.Utils.Clone(first) ?? new Models.Data.CmderParam();
        }

        public List<string> GetCmderParamNames()
        {
            var pps = userSettings.cmderParams;
            var names = pps.Select(p => p.name).OrderBy(name => name).ToList();
            return names;
        }

        public Models.Data.CmderParam GetCmderParamByName(string name)
        {
            var r = userSettings.cmderParams.FirstOrDefault(p => p.name == name);
            return VgcApis.Misc.Utils.Clone(r);
        }

        public bool RemoveCmderParamByName(string name)
        {
            var src = userSettings.cmderParams;
            var trimed = src.Where(p => p.name != name).ToList();
            var ok = src.Count > trimed.Count;
            if (ok)
            {
                userSettings.cmderParams = trimed;
                SaveUserSettings();
            }
            return ok;
        }

        public void SaveCmderParam(Models.Data.CmderParam cmderParam)
        {
            if (cmderParam == null)
            {
                return;
            }

            var list = userSettings.cmderParams.Where(p => p.name != cmderParam.name).ToList();
            var clone = VgcApis.Misc.Utils.Clone(cmderParam);
            list.Add(clone);
            userSettings.cmderParams = list;
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
