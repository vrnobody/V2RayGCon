using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        return MergeYamlConfig(config, tpl);
                    default:
                        return string.Join("\n", new List<string>() { tpl, config });
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

        string MergeYamlConfig(string config, string inbound)
        {
            VgcApis.Misc.UI.MsgBox("YAML is not supported yet!");
            return null;
        }

        string MergeJsonConfig(string config, string inbound)
        {
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
