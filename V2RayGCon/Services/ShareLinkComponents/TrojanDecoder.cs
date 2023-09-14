using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace V2RayGCon.Services.ShareLinkComponents
{
    internal sealed class TrojanDecoder
        : VgcApis.BaseClasses.ComponentOf<Codecs>,
            VgcApis.Interfaces.IShareLinkDecoder
    {
        public TrojanDecoder() { }

        #region properties

        #endregion

        #region public methods
        public string Decode(string shareLink)
        {
            /*
             * trojan://password@remote_host:remote_port
             * in which the password is url-encoded in case it contains illegal characters.
             */

            try
            {
                var vc = Comm.ParseNonStandarUriShareLink("trojan", shareLink);
                return GetParent()?.TrojanToConfig(vc);
            }
            catch { }
            return null;
        }

        public string Encode(string config)
        {
            return Comm.EncodeUriShareLink("trojan", config);
        }

        public List<string> ExtractLinksFromText(string text) =>
            Misc.Utils.ExtractLinks(text, VgcApis.Models.Datas.Enums.LinkTypes.trojan);
        #endregion

        #region private methods
        #endregion

        #region protected methods

        #endregion
    }
}
