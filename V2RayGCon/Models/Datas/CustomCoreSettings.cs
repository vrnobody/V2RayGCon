using System.Collections.Generic;
using System.Text;

namespace V2RayGCon.Models.Datas
{
    public class CustomCoreSettings
    {
        public double index = 0;

        public string name = "";
        public string dir = "";
        public bool setWorkingDir = false;
        public string exe = "";
        public string args = "";
        public string envs = "";

        public string stdOutEncoding = "";
        public string stdInEncoding = "";

        public string configFile = "";
        public bool useFile = false;
        public bool useStdin = true;

        public string speedtestConfigTemplateName = "";

        public CustomCoreSettings() { }

        #region properties

        #endregion

        #region public methods
        static readonly List<string> encodings = new List<string>()
        {
            "utf8",
            "unicode",
            "ascii",
            "cp936",
        };

        public static List<string> GetEncodings() => encodings;

        public Encoding GetStdInEncoding()
        {
            return string.IsNullOrEmpty(stdInEncoding)
                ? Encoding.Default
                : VgcApis.Misc.Utils.TranslateEncoding(stdInEncoding);
        }

        public Encoding GetStdOutEncoding()
        {
            return string.IsNullOrEmpty(stdInEncoding)
                ? Encoding.UTF8
                : VgcApis.Misc.Utils.TranslateEncoding(stdOutEncoding);
        }

        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
