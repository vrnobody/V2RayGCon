namespace VgcApis.Models.Consts
{
    public static class Files
    {
        public static readonly string CoreFolderNameInside3rd = "3rd\\core";

        public static readonly string CoreFolderName = "core";

        public static readonly string AllExt = @"All File|*.*";

        static string GenExtString(string extension, bool appendAllFile = true)
        {
            var l = extension.ToLower();
            var e = $"{l} file|*.{l}";
            return appendAllFile ? $"{e}|{AllExt}" : e;
        }

        public static readonly string PngExt = GenExtString("png");
        public static readonly string JsExt = GenExtString("js");
        public static readonly string JsonExt = GenExtString("json");
        public static readonly string PacExt = GenExtString("pac");
        public static readonly string LuaExt = GenExtString("lua");
        public static readonly string TxtExt = GenExtString("txt");
        public static readonly string CsvExt = GenExtString("csv");

        #region helper functions

        #endregion
    }
}
