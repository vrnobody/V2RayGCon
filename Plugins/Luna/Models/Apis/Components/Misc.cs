using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Luna.Models.Apis.Components
{
    public sealed class Misc :
        VgcApis.BaseClasses.ComponentOf<LuaApis>,
        VgcApis.Interfaces.Lua.ILuaMisc
    {
        Services.Settings settings;
        VgcApis.Interfaces.Services.IUtilsService vgcUtils;
        VgcApis.Interfaces.Services.IShareLinkMgrService vgcSlinkMgr;
        VgcApis.Interfaces.Services.IServersService vgcServer;
        VgcApis.Interfaces.Services.ISettingsService vgcSettings;

        public Misc(
            Services.Settings settings,
            VgcApis.Interfaces.Services.IApiService api)
        {
            this.settings = settings;
            vgcUtils = api.GetUtilsService();
            vgcSlinkMgr = api.GetShareLinkMgrService();
            vgcServer = api.GetServersService();
            vgcSettings = api.GetSettingService();
        }

        #region ILuaMisc.ImportLinks     
        public int ImportLinks(string links, string mark)
        {
            var pair = new string[] { links, mark ?? "" };
            var linkList = new List<string[]> { pair };
            var decoders = vgcSlinkMgr.GenDecoderList(false);
            var results = vgcSlinkMgr.ImportLinksBatchMode(linkList, decoders);

            GetSibling<VgcApis.Interfaces.Lua.ILuaServer>()
                .UpdateAllSummary();

            var count = 0;
            foreach (var result in results)
            {
                if (VgcApis.Misc.Utils.IsImportResultSuccess(result))
                {
                    count++;
                }
            }
            return count;
        }
        #endregion

        #region ILuaMisc thinggy
        public int SetWallpaper(string filename) =>
            Libs.Sys.WinApis.SetWallpaper(filename);

        public string GetImageResolution(string filename) =>
            VgcApis.Misc.Utils.GetImageResolution(filename);

        public string PickRandomLine(string filename) =>
            VgcApis.Misc.Utils.PickRandomLine(filename);

        public string RandomHex(int len) => VgcApis.Misc.Utils.RandomHex(len);

        public string NewGuid() => Guid.NewGuid().ToString();

        public string GetSubscriptionConfig() => vgcSettings.GetSubscriptionConfig();

        public string Input(string title) => Input(title, 3);

        public string Input(string title, int lines)
        {
            using (var form = new Views.WinForms.FormInput(title, lines))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return form.result;
                }
            }
            return null;
        }

        public List<int> Choices(string title, NLua.LuaTable choices) =>
            Choices(title, choices, false);

        public List<int> Choices(string title, NLua.LuaTable choices, bool isShowKey)
        {
            var list = LuaTableToList(choices, isShowKey);
            return GetResultFromChoicesDialog(title, list.ToArray());
        }

        public List<int> Choices(string title, params string[] choices) =>
            GetResultFromChoicesDialog(title, choices);

        public int Choice(string title, NLua.LuaTable choices) =>
            Choice(title, choices, false);

        public int Choice(string title, NLua.LuaTable choices, bool isShowKey)
        {
            var list = LuaTableToList(choices, isShowKey);
            return GetResultFromChoiceDialog(title, list.ToArray());
        }

        public int Choice(string title, params string[] choices) =>
            GetResultFromChoiceDialog(title, choices);

        public bool Confirm(string content) =>
            VgcApis.Misc.UI.Confirm(content);

        public void Alert(string content) =>
            MessageBox.Show(content, VgcApis.Misc.Utils.GetAppName());


        public long GetTimeoutValue() => VgcApis.Models.Consts.Core.SpeedtestTimeout;

        public void RefreshFormMain() => vgcServer.RequireFormMainReload();

        public void WriteLocalStorage(string key, string value) =>
          settings.SetLuaShareMemory(key, value);

        public string ReadLocalStorage(string key) =>
            settings.GetLuaShareMemory(key);

        public bool RemoveLocalStorage(string key) =>
            settings.RemoveShareMemory(key);

        public List<string> LocalStorageKeys() =>
            settings.ShareMemoryKeys();

        public string Config2VeeLink(string config) =>
            vgcSlinkMgr.EncodeConfigToShareLink(
                config, VgcApis.Models.Datas.Enums.LinkTypes.v);

        public string Config2VmessLink(string config) =>
            vgcSlinkMgr.EncodeConfigToShareLink(
                config, VgcApis.Models.Datas.Enums.LinkTypes.vmess);

        public string Config2V2cfg(string config) =>
            vgcSlinkMgr.EncodeConfigToShareLink(
                config, VgcApis.Models.Datas.Enums.LinkTypes.v2cfg);

        public string ShareLink2ConfigString(string shareLink) =>
          vgcSlinkMgr.DecodeShareLinkToConfig(shareLink) ?? @"";

        public string AddVeePrefix(string b64Str) =>
           vgcUtils.AddLinkPrefix(b64Str, VgcApis.Models.Datas.Enums.LinkTypes.v);

        public string AddVmessPrefix(string b64Str) =>
           vgcUtils.AddLinkPrefix(b64Str, VgcApis.Models.Datas.Enums.LinkTypes.vmess);

        public string AddV2cfgPrefix(string b64Str) =>
           vgcUtils.AddLinkPrefix(b64Str, VgcApis.Models.Datas.Enums.LinkTypes.v2cfg);

        public string Base64Encode(string text) =>
            vgcUtils.Base64Encode(text);

        public string Base64Decode(string b64Str) =>
            vgcUtils.Base64Decode(b64Str);

        public string GetLinkBody(string link) =>
            vgcUtils.GetLinkBody(link);

        public string PredefinedFunctions() =>
            Resources.Files.Datas.LuaPredefinedFunctions;

        public string ScanQrcode() =>
            vgcUtils.ScanQrcode();

        public string GetAppDir() => VgcApis.Misc.Utils.GetAppDir();

        public void Sleep(int milliseconds) => Task.Delay(milliseconds).Wait();

        public void Print(params object[] contents)
        {
            var text = "";
            foreach (var content in contents)
            {
                if (content == null)
                {
                    text += @"nil";
                }
                else
                {
                    text += content.ToString();
                }
            }
            GetParent().SendLog(text);
        }
        #endregion

        #region private methods
        List<string> LuaTableToList(NLua.LuaTable table, bool isShowKey)
        {
            var r = new List<string>();
            foreach (KeyValuePair<object, object> de in table)
            {
                var v = de.Value.ToString();
                if (isShowKey)
                {
                    v = de.Key.ToString() + @"." + v;
                }
                r.Add(v);
            }
            return r;
        }

        private static List<int> GetResultFromChoicesDialog(string title, string[] choices)
        {
            using (var form = new Views.WinForms.FormChoices(title, choices))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return form.results;
                }
            }
            return new List<int>();
        }
        private static int GetResultFromChoiceDialog(string title, string[] choices)
        {
            using (var form = new Views.WinForms.FormChoice(title, choices))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return form.result;
                }
            }
            return -1;
        }

        #endregion
    }
}
