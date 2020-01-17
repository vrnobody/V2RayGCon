using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace VgcApis.Models.IServices
{
    public interface IUtilsService
    {
        #region json
        void SetValue<T>(JToken json, string path, T value);
        string GetString(JToken json, string path);

        void CombineWithRoutingInFront(JObject body, JObject mixin);
        void CombineWithRoutingInTheEnd(JObject body, JObject mixin);
        JToken GetKey(JToken json, string path);
        string GetProtocol(JObject config);
        string JTokenToString(JToken jtoken);
        void Merge(JObject body, JObject mixin);
        JToken ParseJToken(string json);
        JArray ParseJArray(string json);
        JObject ParseJObject(string json);
        void Replace(JToken node, JToken value);
        JArray ToJArray(JToken jtoken);
        JObject ToJObject(JToken jtoken);
        void Union(JObject body, JObject mixin);

        #endregion

        #region misc        
        string AddLinkPrefix(string linkBody, VgcApis.Models.Datas.Enum.LinkTypes type);
        string Base64Encode(string plainText);
        string Base64Decode(string b64String);
        string GetLinkBody(string link);

        void ExecuteInParallel<TParam>(
            IEnumerable<TParam> source, Action<TParam> worker);

        void ExecuteInParallel<TParam, TResult>(
            IEnumerable<TParam> source, Func<TParam, TResult> worker);

        string ScanQrcode();
        #endregion
    }
}
