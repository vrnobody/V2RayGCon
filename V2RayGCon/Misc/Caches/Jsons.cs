using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Misc.Caches
{
    public static class Jsons
    {
        static readonly JObject template,
            package,
            example;

        static Jsons()
        {
            template = JObject.Parse(StrConst.config_tpl);
            package = JObject.Parse(StrConst.config_pkg);
            example = JObject.Parse(StrConst.config_example);
        }

        #region public method
        static public string LoadExample(string key)
        {
            var node = LoadJObjectPart(example, key);
            return node.ToString();
        }

        public static JObject LoadPackage(string key)
        {
            var node = LoadJObjectPart(package, key);
            return JObject.Parse(node.ToString());
        }

        public static JToken LoadTemplate(string key)
        {
            var node = LoadJObjectPart(template, key);
            return JToken.Parse(node.ToString());
        }

        public static JObject LoadMinConfig()
        {
            return JObject.Parse(StrConst.config_min);
        }
        #endregion

        #region private method
        static JToken LoadJObjectPart(JObject source, string path)
        {
            var result = VgcApis.Misc.Utils.GetKey(source, path);
            if (result == null)
            {
                throw new JsonReaderException();
            }
            return result;
        }
        #endregion
    }
}
