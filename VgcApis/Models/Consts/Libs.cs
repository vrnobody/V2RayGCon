namespace VgcApis.Models.Consts
{
    static public class Libs
    {
        #region system
        public static string UiThreadName = @"VgcUiThread";
        #endregion

        #region VgcApis.Libs.Sys.CacheLogger

        public const int TrimdownLogCacheDelay = 5000;
        public const int MaxCacheLoggerLineNumber = 1000;
        public const int MinCacheLoggerLineNumber = 300;
        #endregion
    }
}
