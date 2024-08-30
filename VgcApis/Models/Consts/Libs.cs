namespace VgcApis.Models.Consts
{
    public static class Libs
    {
        public static readonly int MinCompressStringLength = 256 * 1024;
        public static readonly int DefaultBufferSize = 4 * 1024;
        public static readonly int FilestreamBufferSize = 128 * 1024;

        #region system
        public static readonly string UiThreadName = @"VgcUiThread";
        #endregion

        #region VgcApis.Libs.Sys.CacheLogger
        public static readonly int DefaultLoggerCacheLines = 500;
        public static readonly int MainLoggerCacheLines = 800;
        #endregion
    }
}
