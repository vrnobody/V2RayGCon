using System;
using VgcApis.Libs.Streams.RawBitStream;

namespace VgcApis.Models.Consts
{
    public static class Import
    {
        public static readonly int ImportWorkerMaxNum = Misc.Utils.Clamp(
            Environment.ProcessorCount - 1,
            1,
            8
        );

        public static readonly int DecodeCacheSize = 10;

        public static readonly int HtmlCacheSize = 30;

        public static readonly int ParseImportZipPkgChunkSize = 1 * 1024 * 1024; // 5 MiB

        public static readonly int DefaultImportTimeout = 60 * 1000;

        public static readonly int ParseImportDepth = 5;

        public static readonly int ParseImportRetry = 3;

        public static readonly string MarkImportSuccess = @"√";

        public static readonly string MarkImportFail = @"×";
    }
}
