using System.Collections.Generic;

namespace Luna.Models.Apis.Components
{
    public sealed class Misc :
        VgcApis.Models.BaseClasses.ComponentOf<LuaApis>,
        VgcApis.Models.Interfaces.Lua.ILuaMisc
    {
        Services.Settings settings;
        VgcApis.Models.IServices.IUtilsService vgcUtils;
        VgcApis.Models.IServices.IShareLinkMgrService vgcSlinkMgr;

        public Misc(
            Services.Settings settings,
            VgcApis.Models.IServices.IApiService api)
        {
            this.settings = settings;
            vgcUtils = api.GetUtilsService();
            vgcSlinkMgr = api.GetShareLinkMgrService();
        }

        #region ILuaMisc.ImportLinks     

        public int ImportLinks(string links, string mark)
        {
            var pair = new string[] { links, mark ?? "" };
            var linkList = new List<string[]> { pair };
            var decoders = vgcSlinkMgr.GenDecoderList(false);
            var results = vgcSlinkMgr.ImportLinksBatchMode(linkList, decoders);

            container
                .GetComponent<VgcApis.Models.Interfaces.Lua.ILuaServer>()
                .UpdateAllSummary();

            var count = 0;
            foreach (var result in results)
            {
                if (VgcApis.Libs.Utils.IsImportResultSuccess(result))
                {
                    count++;
                }
            }
            return count;
        }
        #endregion

        #region ILuaMisc thinggy
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
                config, VgcApis.Models.Datas.Enum.LinkTypes.v);

        public string Config2VmessLink(string config) =>
            vgcSlinkMgr.EncodeConfigToShareLink(
                config, VgcApis.Models.Datas.Enum.LinkTypes.vmess);

        public string Config2V2cfg(string config) =>
            vgcSlinkMgr.EncodeConfigToShareLink(
                config, VgcApis.Models.Datas.Enum.LinkTypes.v2cfg);

        public string ShareLink2ConfigString(string shareLink) =>
          vgcSlinkMgr.DecodeShareLinkToConfig(shareLink) ?? @"";

        public string AddVeePrefix(string b64Str) =>
           vgcUtils.AddLinkPrefix(b64Str, VgcApis.Models.Datas.Enum.LinkTypes.v);

        public string AddVmessPrefix(string b64Str) =>
           vgcUtils.AddLinkPrefix(b64Str, VgcApis.Models.Datas.Enum.LinkTypes.vmess);

        public string AddV2cfgPrefix(string b64Str) =>
           vgcUtils.AddLinkPrefix(b64Str, VgcApis.Models.Datas.Enum.LinkTypes.v2cfg);

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

        public string GetAppDir() => VgcApis.Libs.Utils.GetAppDir();

        public void Sleep(int milliseconds) =>
          System.Threading.Thread.Sleep(milliseconds);

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
            container.SendLog(text);
        }
        #endregion
    }
}
