using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services.Caches
{
    public class Template
    {
        readonly JObject template,
            package;

        public Template()
        {
            template = JObject.Parse(StrConst.config_tpl);
            package = JObject.Parse(StrConst.config_pkg);
        }

        #region public method
        public JObject LoadPackage(string key)
        {
            var node = LoadJObjectPart(package, key);
            return JObject.Parse(node.ToString());
        }

        public JToken LoadTemplate(string key)
        {
            var node = LoadJObjectPart(template, key);
            return JToken.Parse(node.ToString());
        }

        public JObject LoadMinConfig()
        {
            return JObject.Parse(StrConst.config_min);
        }
        #endregion

        #region private method
        JToken LoadJObjectPart(JObject source, string path)
        {
            var result = Misc.Utils.GetKey(source, path);
            if (result == null)
            {
                throw new JsonReaderException();
            }
            return result;
        }
        #endregion
    }
}
