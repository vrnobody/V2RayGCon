namespace VgcApis.Models.Consts
{
    public static class Libs
    {
        public static int DefaultBufferSize = 4 * 1024;

        #region system
        public static string UiThreadName = @"VgcUiThread";
        #endregion

        #region VgcApis.Libs.Sys.CacheLogger
        public static int DefaultLoggerCacheLines = 500;
        public static int MainLoggerCacheLines = 800;
        #endregion
    }
}
