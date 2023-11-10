using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    public class CustomConfigTemplate
    {
        public double index = 0;
        public string name = "";
        public string jsonArrMergeOption = "";
        public string template = "";
        public bool isSocks5Inbound = false;

        public CustomConfigTemplate() { }

        #region static
        static readonly string extJsonArrMergeOption = "ByTag";

        public static List<string> GetJsonArrayMergeOptions()
        {
            var r = VgcApis.Misc.Utils.EnumToList<MergeArrayHandling>();
            r.Add(extJsonArrMergeOption);
            return r;
        }
        #endregion

        #region public
        public string GetJsonArrMergeOption()
        {
            if (jsonArrMergeOption == extJsonArrMergeOption)
            {
                return extJsonArrMergeOption;
            }
            if (
                VgcApis.Misc.Utils.TryParseEnum<MergeArrayHandling>(jsonArrMergeOption, out var mah)
            )
            {
                return mah.ToString();
            }
            return MergeArrayHandling.Replace.ToString();
        }

        public string MergeToConfig(string config, int port)
        {
            return MergeToConfig(config, VgcApis.Models.Consts.Webs.LoopBackIP, port);
        }

        public string MergeToConfig(string config, string host, int port)
        {
            if (string.IsNullOrEmpty(template))
            {
                return config;
            }

            var ty = VgcApis.Misc.Utils.DetectConfigType(config);
            string r = "";
            switch (ty)
            {
                case VgcApis.Models.Datas.Enums.ConfigType.yaml:
                    r = MergeToYaml(config, host, port);
                    break;
                case VgcApis.Models.Datas.Enums.ConfigType.json:
                    var json = VgcApis.Misc.Utils.ParseJObject(config);
                    if (json != null && MergeToJObject(ref json, host, port))
                    {
                        r = VgcApis.Misc.Utils.FormatConfig(json);
                    }
                    break;
                default:
                    r = MergeToText(config, host, port);
                    break;
            }
            return r ?? "";
        }

        public bool MergeToJObject(ref JObject config, string host, int port)
        {
            if (string.IsNullOrEmpty(template))
            {
                return true;
            }

            if (config == null)
            {
                return false;
            }

            try
            {
                var tpl = GetFormatedTemplate(host, port);
                var mixin = JObject.Parse(tpl);
                MergeJObject(ref config, mixin);
                return true;
            }
            catch { }
            return false;
        }

        public string GetFormatedTemplate(string host, int port)
        {
            try
            {
                return template?.Replace($"%host%", host ?? "")?.Replace($"%port%", port.ToString())
                    ?? "";
            }
            catch { }
            return template ?? "";
        }
        #endregion

        #region private
        string MergeToYaml(string config, string host, int port)
        {
            var tpl = GetFormatedTemplate(host, port);
            return VgcApis.Misc.Utils.MergeYaml(config, tpl);
        }

        string MergeToText(string config, string host, int port)
        {
            var tpl = GetFormatedTemplate(host, port);
            if (string.IsNullOrEmpty(tpl))
            {
                return config;
            }
            return string.Join("\n", new List<string>() { tpl, config });
        }

        void MergeJObject(ref JObject body, JObject mixin)
        {
            if (mixin == null)
            {
                return;
            }

            if (jsonArrMergeOption == extJsonArrMergeOption)
            {
                Misc.Utils.CombineConfigWithRoutingInFront(ref body, mixin);
                return;
            }

            if (
                !VgcApis.Misc.Utils.TryParseEnum<MergeArrayHandling>(
                    jsonArrMergeOption,
                    out var mah
                )
            )
            {
                mah = MergeArrayHandling.Replace;
            }

            body?.Merge(
                mixin,
                new JsonMergeSettings
                {
                    MergeArrayHandling = mah,
                    MergeNullValueHandling = MergeNullValueHandling.Ignore,
                }
            );
        }
        #endregion
    }
}
