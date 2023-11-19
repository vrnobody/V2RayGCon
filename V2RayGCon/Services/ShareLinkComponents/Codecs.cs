using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    public sealed class Codecs : VgcApis.BaseClasses.ComponentOf<Codecs>
    {
        Settings setting;
        Cache cache;

        public Codecs() { }

        #region vless and trojan
        public string TrojanToConfig(Models.Datas.SharelinkMetadata vee)
        {
            if (vee == null)
            {
                return null;
            }

            var outbSs = cache.tpl.LoadTemplate("outbTrojan");

            outbSs["streamSettings"] = Comm.GenStreamSetting(cache, vee);
            var node = outbSs["settings"]["servers"][0];
            node["address"] = vee.host;
            node["port"] = vee.port;
            node["password"] = vee.auth1;
            node["flow"] = vee.auth2;

            var tpl = cache.tpl.LoadTemplate("tplLogWarn") as JObject;
            return GenerateJsonConfing(tpl, outbSs);
        }

        public string VlessToConfig(Models.Datas.SharelinkMetadata vee)
        {
            if (vee == null)
            {
                return null;
            }

            var outVmess = cache.tpl.LoadTemplate("outbVless");
            outVmess["streamSettings"] = Comm.GenStreamSetting(cache, vee);

            outVmess["protocol"] = "vless";
            var node = outVmess["settings"]["vnext"][0];
            node["address"] = vee.host;
            node["port"] = vee.port;
            node["users"][0]["id"] = vee.auth1;
            if (!string.IsNullOrEmpty(vee.auth2))
            {
                node["users"][0]["flow"] = vee.auth2;
            }
            node["users"][0]["encryption"] = "none";
            var tpl = cache.tpl.LoadTemplate("tplLogWarn") as JObject;
            return GenerateJsonConfing(tpl, outVmess);
        }
        #endregion

        #region public methods

        public string Encode<TDecoder>(string name, string config)
            where TDecoder : VgcApis.BaseClasses.ComponentOf<Codecs>,
                VgcApis.Interfaces.IShareLinkDecoder
        {
            try
            {
                return GetChild<TDecoder>()?.Encode(name, config);
            }
            catch { }
            return null;
        }

#pragma warning disable CA1822 // Mark members as static
        public VgcApis.Models.Datas.DecodeResult Decode(
            string shareLink,
            VgcApis.Interfaces.IShareLinkDecoder decoder
        )
#pragma warning restore CA1822 // Mark members as static
        {
            try
            {
                return decoder.Decode(shareLink);
            }
            catch { }
            return null;
        }

        public VgcApis.Models.Datas.DecodeResult Decode<TDecoder>(string shareLink)
            where TDecoder : VgcApis.BaseClasses.ComponentOf<Codecs>,
                VgcApis.Interfaces.IShareLinkDecoder
        {
            return GetChild<TDecoder>()?.Decode(shareLink);
        }

        public void Run(Cache cache, Settings setting)
        {
            this.setting = setting;
            this.cache = cache;

            var ssDecoder = new SsDecoder(cache);
            var v2cfgDecoder = new V2cfgDecoder();
            var vmessDecoder = new VmessDecoder(cache, setting);
            var trojanDecoder = new TrojanDecoder();
            var vlessDecoder = new VlessDecoder();
            var socksDecoder = new SocksDecoder(cache);

            AddChild(vlessDecoder);
            AddChild(trojanDecoder);
            AddChild(ssDecoder);
            AddChild(socksDecoder);
            AddChild(v2cfgDecoder);
            AddChild(vmessDecoder);
        }

        public List<VgcApis.Interfaces.IShareLinkDecoder> GetDecoders(bool isIncludeV2cfgDecoder)
        {
            var r = new List<VgcApis.Interfaces.IShareLinkDecoder>
            {
                GetChild<VlessDecoder>(),
                GetChild<VmessDecoder>(),
            };

            if (setting.CustomDefImportSocksShareLink)
            {
                r.Add(GetChild<SocksDecoder>());
            }

            if (setting.CustomDefImportTrojanShareLink)
            {
                r.Add(GetChild<TrojanDecoder>());
            }

            if (setting.CustomDefImportSsShareLink)
            {
                r.Add(GetChild<SsDecoder>());
            }

            if (isIncludeV2cfgDecoder)
            {
                r.Add(GetChild<V2cfgDecoder>());
            }

            return r;
        }
        #endregion

        #region private methods
        public string GenerateJsonConfing(JObject template, JToken outbound)
        {
            if (template == null || outbound == null)
            {
                return null;
            }

            var isV4 = setting.isUseV4;

            var inb = Misc.Utils.CreateJObject(
                (isV4 ? "inbounds.0" : "inbound"),
                cache.tpl.LoadTemplate("inbSimSock")
            );

            var outb = Misc.Utils.CreateJObject((isV4 ? "outbounds.0" : "outbound"), outbound);

            Misc.Utils.MergeJson(template, inb);
            Misc.Utils.MergeJson(template, outb);
            var key = VgcApis.Models.Consts.Config.SectionKeyV2rayGCon;
            template.Remove(key);
            return VgcApis.Misc.Utils.FormatConfig(template);
        }
        #endregion
    }
}
