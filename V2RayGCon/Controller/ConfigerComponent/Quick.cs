using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace V2RayGCon.Controller.ConfigerComponet
{
    class Quick : ConfigerComponentController
    {
        Service.Cache cache;
        Service.Servers servers;
        Service.ConfigMgr configMgr;

        bool isUseV4;

        public Quick(
            Button skipCN,
            Button mtProto,
            CheckBox chkUseV4)
        {
            cache = Service.Cache.Instance;
            servers = Service.Servers.Instance;
            configMgr = Service.ConfigMgr.Instance;

            isUseV4 = chkUseV4.Checked;
            chkUseV4.CheckedChanged += (s, a) =>
            {
                isUseV4 = chkUseV4.Checked;
            };

            skipCN.Click += (s, a) =>
            {
                container.InjectConfigHelper(
                    () => configMgr
                        .InjectSkipCnSiteSettingsIntoConfig(
                            ref container.config,
                            isUseV4)
                );
            };

            mtProto.Click += (s, a) =>
            {
                container.InjectConfigHelper(() =>
                {
                    InsertMTProto(ref container.config);
                });
            };
        }

        #region public method
        public override void Update(JObject config)
        {
        }
        #endregion

        #region private method
        void InsertMTProto(ref JObject config)
        {
            var hasInbDtr = Lib.Utils.GetKey(config, "inboundDetour") != null;
            var hasInbounds = Lib.Utils.GetKey(config, "inbounds") != null;
            var hasOutbDtr = Lib.Utils.GetKey(config, "outboundDetour") != null;
            var hasOutbounds = Lib.Utils.GetKey(config, "outbounds") != null;

            var inbTag = "inboundDetour";
            var outbTag = "outboundDetour";

            if (!hasInbDtr && (isUseV4 || hasInbounds))
            {
                inbTag = "inbounds";
            }

            if (!hasOutbDtr && (isUseV4 || hasOutbounds))
            {
                outbTag = "outbounds";
            }

            var mtproto = JObject.Parse(@"{}");
            mtproto[inbTag] = cache.tpl.LoadTemplate("inbDtrMtProto");
            mtproto[outbTag] = cache.tpl.LoadTemplate("outbDtrMtProto");
            mtproto["routing"] = cache.tpl.LoadTemplate("routingMtProto");

            string dbgInfoMtproto;
            foreach (string key in new string[] { inbTag, outbTag, "routing" })
            {
                if (!Lib.Utils.TryExtractJObjectPart(mtproto, key, out JObject part))
                {
                    continue;
                }

                if (!Lib.Utils.Contains(config, part))
                {
                    continue;
                }

                try
                {
                    Lib.Utils.RemoveKeyFromJObject(mtproto, key);
                }
                catch (KeyNotFoundException) { }
                dbgInfoMtproto = mtproto.ToString();
            }

            var user = Lib.Utils.GetKey(mtproto, $"{inbTag}.0.settings.users.0");
            if (user != null && user is JObject)
            {
                user["secret"] = Lib.Utils.RandomHex(32);
            }

            Lib.Utils.CombineConfigWithRoutingInFront(ref mtproto, config);
            config = mtproto;
        }
        #endregion
    }
}
