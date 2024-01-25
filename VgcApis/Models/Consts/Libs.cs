namespace VgcApis.Models.Consts
{
    public static class Libs
    {
        #region system
        public static string UiThreadName = @"VgcUiThread";
        #endregion

        #region VgcApis.Libs.Sys.CacheLogger
        public const int MaxCacheLoggerLineNumber = 500;
        public const int MinCacheLoggerLineNumber = MaxCacheLoggerLineNumber * 2 / 5;
        #endregion
    }
}
