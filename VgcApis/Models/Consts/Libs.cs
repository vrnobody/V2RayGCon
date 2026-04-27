namespace VgcApis.Models.Consts
{
    public static class Libs
    {
        public static readonly int MinCompressPluginSettingsLength = 128 * 1024;
        public static readonly int DefCompressConfigLevel = 6;
        public static readonly int MaxCompressConfigLength = 1 * 1024 * 1024;
        public static readonly int DefaultBufferSize = 4 * 1024;
        public static readonly int FilestreamBufferSize = 512 * 1024;

        #region system
        public static readonly string UiThreadName = @"VgcUiThread";
        #endregion

        #region VgcApis.Libs.Sys.CacheLogger
        public static readonly int DefaultLoggerCacheLines = 500;
        public static readonly int MainLoggerCacheLines = 800;
        #endregion
    }
}
