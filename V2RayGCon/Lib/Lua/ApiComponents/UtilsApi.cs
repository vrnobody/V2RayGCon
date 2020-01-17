using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace V2RayGCon.Lib.Lua.ApiComponents
{
    public class UtilsApi :
        VgcApis.Models.BaseClasses.Disposable,
        VgcApis.Models.IServices.IUtilsService
    {
        #region json
        public void SetValue<T>(JToken json, string path, T value) =>
            Utils.SetValue<T>(json, path, value);

        public string GetString(JToken json, string path) =>
            Utils.GetValue<string>(json, path);

        public JToken GetKey(JToken json, string path) =>
            Utils.GetKey(json, path);

        public string GetProtocol(JObject config) =>
            Utils.GetProtocolFromConfig(config);

        public string JTokenToString(JToken jtoken) =>
            jtoken.ToString();

        public void Replace(JToken node, JToken value) =>
            node.Replace(value);

        public void Union(JObject body, JObject mixin) =>
            Lib.Utils.UnionJson(ref body, mixin);

        public void Merge(JObject body, JObject mixin) =>
            Utils.MergeJson(ref body, mixin);

        public void CombineWithRoutingInTheEnd(JObject body, JObject mixin) =>
            Utils.CombineConfigWithRoutingInTheEnd(ref body, mixin);

        public void CombineWithRoutingInFront(JObject body, JObject mixin) =>
            Utils.CombineConfigWithRoutingInFront(ref body, mixin);

        public JObject ToJObject(JToken jtoken) =>
            jtoken as JObject;

        public JArray ToJArray(JToken jtoken) =>
            jtoken as JArray;

        public JToken ParseJToken(string json)
        {
            try
            {
                return JToken.Parse(json);
            }
            catch { }
            return null;
        }

        public JArray ParseJArray(string json) =>
            ParseJToken(json) as JArray;

        public JObject ParseJObject(string json) =>
            ParseJToken(json) as JObject;

        #endregion

        #region misc
        public string AddLinkPrefix(string linkBody, VgcApis.Models.Datas.Enum.LinkTypes type) =>
            Utils.AddLinkPrefix(linkBody, type);

        public string Base64Encode(string plainText) =>
            Utils.Base64Encode(plainText);

        public string Base64Decode(string b64String) =>
            Utils.Base64Decode(b64String);

        public string GetLinkBody(string link) =>
            Utils.GetLinkBody(link);

        public string ScanQrcode()
        {
            var shareLink = @"";
            AutoResetEvent are = new AutoResetEvent(false);

            void Success(string result)
            {
                shareLink = result;
                are.Set();
            }

            void Fail()
            {
                are.Set();
            }

            Lib.QRCode.QRCode.ScanQRCode(Success, Fail);
            are.WaitOne();
            return shareLink;
        }

        public void ExecuteInParallel<TParam>(
            IEnumerable<TParam> source, Action<TParam> worker) =>
            Lib.Utils.ExecuteInParallel(source, worker);

        public void ExecuteInParallel<TParam, TResult>(
            IEnumerable<TParam> source, Func<TParam, TResult> worker) =>
            Lib.Utils.ExecuteInParallel(source, worker);
        #endregion

    }
}
