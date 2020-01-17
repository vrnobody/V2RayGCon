using Newtonsoft.Json.Linq;

namespace Luna.Models.Apis.Components

{
    public sealed class Json :
        VgcApis.Models.BaseClasses.ComponentOf<LuaApis>,
        VgcApis.Models.Interfaces.Lua.ILuaJson
    {
        VgcApis.Models.IServices.IUtilsService vgcUtils;

        public Json(
            VgcApis.Models.IServices.IApiService api)
        {
            this.vgcUtils = api.GetUtilsService();
        }

        #region ILuaJson thinggy
        public void SetIntValue(JToken json, string path, int value) =>
            vgcUtils.SetValue(json, path, value);

        public void SetBoolValue(JToken json, string path, bool value) =>
            vgcUtils.SetValue(json, path, value);

        public void SetStringValue(JToken json, string path, string value) =>
            vgcUtils.SetValue(json, path, value);

        public string GetString(JToken json, string path) =>
            vgcUtils.GetString(json, path);

        public JToken GetKey(JToken json, string path) =>
           vgcUtils.GetKey(json, path);

        public string GetProtocol(JObject config) =>
            vgcUtils.GetProtocol(config);

        public void CombineWithRoutingInFront(JObject body, JObject mixin) =>
            vgcUtils.CombineWithRoutingInFront(body, mixin);

        public void CombineWithRoutingInTheEnd(JObject body, JObject mixin) =>
            vgcUtils.CombineWithRoutingInTheEnd(body, mixin);

        public void Merge(JObject body, JObject mixin) =>
            vgcUtils.Merge(body, mixin);

        public JArray ParseJArray(string json) =>
            vgcUtils.ParseJArray(json);

        public JObject ParseJObject(string json) =>
            vgcUtils.ParseJObject(json);

        public JToken ParseJToken(string json) =>
            vgcUtils.ParseJToken(json);

        public void Replace(JToken node, JToken value) =>
            vgcUtils.Replace(node, value);

        public JArray ToJArray(JToken jtoken) =>
            vgcUtils.ToJArray(jtoken);

        public JObject ToJObject(JToken jtoken) =>
            vgcUtils.ToJObject(jtoken);

        public void Union(JObject body, JObject mixin) =>
            vgcUtils.Union(body, mixin);

        public string JTokenToString(JToken jtoken) =>
            vgcUtils.JTokenToString(jtoken);
        #endregion
    }
}
