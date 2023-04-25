using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs
{
    internal interface IVeeDecoder
    {
        Tuple<JObject, JToken> Bytes2Config(byte[] bytes);
        byte[] Config2Bytes(JObject config);
        string GetSupportedVeeVersion();
        string GetSupportedEncodeProtocol();

        Models.Datas.VeeConfigsWithReality Bytes2VeeConfig(byte[] bytes);
        byte[] VeeConfig2Bytes(Models.Datas.VeeConfigsWithReality veeConfig);
    }
}
