using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    public class CustomInboundSettings
    {
        public double index = 0;
        public string name = "";
        public string template = "";

        public CustomInboundSettings() { }

        #region public
        public void MergeToJObject(ref JObject config, string host, int port)
        {
            if (config == null)
            {
                return;
            }

            try
            {
                var tpl = GetFormatedTemplate(host, port);
                var mixin = JObject.Parse(tpl);
                MergeJObject(ref config, mixin);
            }
            catch { }
        }

        public string MergeToYaml(string config, string host, int port)
        {
            var tpl = GetFormatedTemplate(host, port);
            return VgcApis.Misc.Utils.MergeYaml(config, tpl);
        }

        public string MergeToText(string config, string host, int port)
        {
            var tpl = GetFormatedTemplate(host, port);
            if (string.IsNullOrEmpty(tpl))
            {
                return config;
            }
            return string.Join("\n", new List<string>() { tpl, config });
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
        void MergeJObject(ref JObject body, JObject mixin)
        {
            if (mixin == null)
            {
                return;
            }
            body?.Merge(
                mixin,
                new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Replace,
                    MergeNullValueHandling = MergeNullValueHandling.Ignore,
                }
            );
        }
        #endregion
    }
}
