namespace VgcApis.Models.Consts
{
    public static class Import
    {
        public const int DecodeCacheSize = 10;

        public const int ParseImportTimeout = 12 * 1000;

        public const int ParseImportDepth = 5;

        public const int ParseImportRetry = 3;

        public const string MarkImportSuccess = @"√";

        public const string MarkImportFail = @"×";

    }
}
