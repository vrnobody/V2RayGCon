using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace V2RayGCon.Controllers.FormJsonConfigEditorComponet
{
    class Quick : ConfigerComponentController
    {
        Services.Cache cache;
        Services.ConfigMgr configMgr;

        bool isUseV4;

        public Quick(Button skipCN, Button mtProto, CheckBox chkUseV4)
        {
            cache = Services.Cache.Instance;
            configMgr = Services.ConfigMgr.Instance;

            isUseV4 = chkUseV4.Checked;
            chkUseV4.CheckedChanged += (s, a) =>
            {
                isUseV4 = chkUseV4.Checked;
            };

            skipCN.Click += (s, a) =>
            {
                container.InjectConfigHelper(
                    () =>
                        configMgr.InjectSkipCnSiteSettingsIntoConfig(ref container.config, isUseV4)
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
        public override void Update(JObject config) { }
        #endregion

        #region private method
        void InsertMTProto(ref JObject config)
        {
            var hasInbDtr = Misc.Utils.GetKey(config, "inboundDetour") != null;
            var hasInbounds = Misc.Utils.GetKey(config, "inbounds") != null;
            var hasOutbDtr = Misc.Utils.GetKey(config, "outboundDetour") != null;
            var hasOutbounds = Misc.Utils.GetKey(config, "outbounds") != null;

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
                if (!Misc.Utils.TryExtractJObjectPart(mtproto, key, out JObject part))
                {
                    continue;
                }

                if (!Misc.Utils.Contains(config, part))
                {
                    continue;
                }

                try
                {
                    Misc.Utils.RemoveKeyFromJObject(mtproto, key);
                }
                catch (KeyNotFoundException) { }
                dbgInfoMtproto = mtproto.ToString();
            }

            var user = Misc.Utils.GetKey(mtproto, $"{inbTag}.0.settings.users.0");
            if (user != null && user is JObject)
            {
                user["secret"] = VgcApis.Misc.Utils.RandomHex(32);
            }

            Misc.Utils.CombineConfigWithRoutingInFront(ref mtproto, config);
            config = mtproto;
        }
        #endregion
    }
}
