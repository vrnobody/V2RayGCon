using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    public class CustomInboundSettings
    {
        public double index = 0;
        public string name = "";
        public string template = "";
        public string format = "json"; // json, yaml, text

        public CustomInboundSettings() { }

        #region public
        public string MergeToConfig(string config, string host, int port)
        {
            var tpl = GetFormatedTemplate(host, port);
            if (string.IsNullOrEmpty(tpl))
            {
                return config;
            }

            try
            {
                switch (format)
                {
                    case "json":
                        return MergeJsonConfig(config, tpl);
                    case "yaml":
                        // 没找到不需要先定义class的YAML库，只好用土制的查找替换了。
                        return VgcApis.Misc.Utils.MergeYamlInboundIntoConfig(config, tpl);
                    default:
                        return MergeTextConfig(config, tpl);
                }
            }
            catch { }
            return config;
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

        string MergeTextConfig(string config, string inbound)
        {
            return string.Join("\n", new List<string>() { inbound, config });
        }

        string MergeJsonConfig(string config, string inbound)
        {
            if (!VgcApis.Misc.Utils.IsJson(config) || !VgcApis.Misc.Utils.IsJson(inbound))
            {
                return config;
            }

            var body = JObject.Parse(config);
            var mixin = JObject.Parse(inbound);
            body.Merge(
                mixin,
                new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Replace,
                    MergeNullValueHandling = MergeNullValueHandling.Ignore,
                }
            );
            return VgcApis.Misc.Utils.FormatConfig(body);
        }
        #endregion
    }
}
