namespace VgcApis.Models.Consts
{
    public static class Import
    {
        public static readonly int DecodeCacheSize = 10;

        public static readonly int HtmlCacheSize = 30;

        public static readonly int ParseImportTimeout = 30 * 1000;

        public static readonly int ParseImportDepth = 5;

        public static readonly int ParseImportRetry = 3;

        public static readonly string MarkImportSuccess = @"√";

        public static readonly string MarkImportFail = @"×";
    }
}
