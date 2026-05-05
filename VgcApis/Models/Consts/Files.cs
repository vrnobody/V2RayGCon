using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VgcApis.Models.Consts
{
    public static class Files
    {
        public static readonly int StreamDefaultTimeout = 10 * 60 * 1000;

        public static readonly string LibsDir = "libs";

        public static readonly string PluginsDir = "3rd\\plugins";

        public static readonly string CoreFolderNameInside3rd = "3rd\\core";

        public static readonly string CoreFolderName = "core";

        public static readonly string AllExt = @"All File|*.*";

        public static string GenExtWithAllFile(params string[] exts)
        {
            return GenExtStr(true, exts);
        }

        public static string GenExtStr(bool appendAllFile, params string[] exts)
        {
            var list = new List<string>();
            foreach (var ext in exts)
            {
                var l = ext.ToLower();
                var e = $"{l} file|*.{l}";
                list.Add(e);
            }
            if (appendAllFile)
            {
                list.Add(AllExt);
            }
            return string.Join("|", list);
        }

        public static readonly string TlsCertExts = GenExtWithAllFile("pem", "crt");

        public static readonly string PngExt = GenExtWithAllFile("png");
        public static readonly string JsExt = GenExtWithAllFile("js");
        public static readonly string JsonExt = GenExtWithAllFile("json");
        public static readonly string PacExt = GenExtWithAllFile("pac");
        public static readonly string LuaExt = GenExtWithAllFile("lua");
        public static readonly string ExeExt = GenExtWithAllFile("exe");
        public static readonly string TxtExt = GenExtWithAllFile("txt");
        public static readonly string CsvExt = GenExtWithAllFile("csv");

        #region helper functions

        #endregion
    }
}
