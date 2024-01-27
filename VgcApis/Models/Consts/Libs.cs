namespace VgcApis.Models.Consts
{
    public static class Libs
    {
        public static int DefaultBufferSize = 4 * 1024;

        #region system
        public static string UiThreadName = @"VgcUiThread";
        #endregion

        #region VgcApis.Libs.Sys.CacheLogger
        public const int MaxCacheLoggerLineNumber = 500;
        public const int MinCacheLoggerLineNumber = MaxCacheLoggerLineNumber * 2 / 5;
        #endregion
    }
}
