using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace VgcApis.Models.Interfaces
{
    public interface IShareLinkDecoder :
        IDisposable
    {
        /// <summary>
        /// Return empty list if fail.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        List<string> ExtractLinksFromText(string text);

        /// <summary>
        /// Return null if decode fail!
        /// </summary>
        Tuple<JObject, JToken> Decode(string shareLink);

        /// <summary>
        /// Return null if encode fail!
        /// </summary>
        string Encode(string config);
    }
}
