using System.Collections.Generic;
using System.Text;

namespace V2RayGCon.Models.Datas
{
    public class CustomCoreSettings
    {
        public double index = 0;

        public string name = "";
        public string dir = "";
        public string exe = "";
        public string args = "";

        public string stdOutEncoding = "";
        public string stdInEncoding = "";

        public string configFilename = "";
        public bool useConfigFile = false;
        public bool useStdInToPassConfig = true;

        public bool isBindToShareLinkProtocol = false;
        public bool isBindToConfigProtocol = false;
        public string protocols = "";

        public CustomCoreSettings() { }

        #region properties

        #endregion

        #region public methods
        static readonly List<string> encodings = new List<string>()
        {
            "utf8",
            "unicode",
            "ascii",
            "cp936"
        };

        public static List<string> GetEncodings() => encodings;

        public static Encoding TranslateEncoding(string encoding)
        {
            var enc = encoding?.ToLower() ?? "";
            if (string.IsNullOrEmpty(enc) || enc == "utf8")
            {
                return Encoding.UTF8;
            }

            if (enc.StartsWith("cp") && enc.Length > 2)
            {
                if (int.TryParse(enc.Substring(2), out var num))
                {
                    try
                    {
                        return Encoding.GetEncoding(num);
                    }
                    catch { }
                }
            }

            switch (encoding)
            {
                case "unicode":
                    return Encoding.Unicode;
                case "ascii":
                    return Encoding.ASCII;
                default:
                    return Encoding.UTF8;
            }
        }
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
