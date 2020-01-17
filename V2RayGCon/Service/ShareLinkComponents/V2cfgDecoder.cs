using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace V2RayGCon.Service.ShareLinkComponents
{
    internal sealed class V2cfgDecoder :
        VgcApis.Models.BaseClasses.ComponentOf<Codecs>,
        VgcApis.Models.Interfaces.IShareLinkDecoder
    {
        public V2cfgDecoder() { }

        #region properties

        #endregion

        #region public methods
        public Tuple<JObject, JToken> Decode(string shareLink)
        {
            try
            {
                var linkBody = Lib.Utils.GetLinkBody(shareLink);
                var plainText = Lib.Utils.Base64Decode(linkBody);
                var config = JObject.Parse(plainText);
                if (config != null)
                {
                    return new Tuple<JObject, JToken>(config, null);
                }
            }
            catch { }
            return null;
        }

        public string Encode(string config)
        {
            if (string.IsNullOrEmpty(config))
            {
                return null;
            }

            var body = Lib.Utils.Base64Encode(config);

            return Lib.Utils.AddLinkPrefix(
                body,
                VgcApis.Models.Datas.Enum.LinkTypes.v2cfg);
        }


        public List<string> ExtractLinksFromText(string text) =>
            Lib.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enum.LinkTypes.v2cfg);
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
