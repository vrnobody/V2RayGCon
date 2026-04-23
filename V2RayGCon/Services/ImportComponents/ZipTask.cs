using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using V2RayGCon.Services.ShareLinkComponents;
using VgcApis.Models.Consts;

namespace V2RayGCon.Services.ImportComponents
{
    internal class ZipTask
    {
        readonly string mark;
        readonly string url;
        private readonly int timeout;
        readonly StringBuilder sb = new StringBuilder();
        private readonly int maxCount;
        string errorMsg = null;

        public ZipTask(string mark, string url)
            : this(mark, url, 0, Import.ParseImportTimeout) { }

        public ZipTask(string mark, string url, int maxCount, int timeout)
        {
            this.mark = mark;
            this.url = url;
            this.maxCount = maxCount;
            this.timeout = timeout;
        }

        #region public methods
        public string GetErrorMessage() => errorMsg;

        public void Exec(
            Action<string, string, Action, CancellationToken> decode,
            bool isSocks5,
            int proxyPort
        )
        {
            var cts =
                this.timeout > 0
                    ? new CancellationTokenSource(timeout)
                    : new CancellationTokenSource();
            var token = cts.Token;

            var success = 0;
            void onAddNew()
            {
                if (maxCount < 1)
                {
                    return;
                }
                success++;
                if (success >= maxCount)
                {
                    SetError("Reach max server count limit.");
                    cts.Cancel();
                }
            }

            void import()
            {
                var text = sb.ToString();
                decode(mark, text, onAddNew, token);
            }

            var chunkSize = Import.ParseImportZipPkgChunkSize;
            var highWater = chunkSize * 0.8;
            var overlapSize = 10 * 1024;
            void push(char[] buff, int len)
            {
                sb.Append(buff, 0, len);
                if (sb.Length < highWater)
                {
                    return;
                }
                import();
                sb.Remove(0, sb.Length - overlapSize);
            }

            var readBuffer = new char[chunkSize];
            void parseFileStream(StreamReader reader)
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    var n = reader.Read(readBuffer, 0, chunkSize);
                    if (n < 1)
                    {
                        // file ends
                        return;
                    }
                    push(readBuffer, n);
                }
            }

            try
            {
                using (
                    var wc = VgcApis.Misc.Utils.CreateStreamWebClient(
                        url,
                        isSocks5,
                        proxyPort,
                        timeout,
                        null,
                        null
                    )
                )
                using (var zipStream = wc.OpenRead(url))
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        try
                        {
                            using (var reader = new StreamReader(entry.Open()))
                            {
                                parseFileStream(reader);
                            }
                        }
                        catch { }
                    }
                }
                import();
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }

            if (token.IsCancellationRequested)
            {
                SetError("Timeout.");
            }
        }

        #endregion

        #region private methods
        void SetError(string msg)
        {
            if (!string.IsNullOrEmpty(errorMsg))
            {
                return;
            }
            this.errorMsg = msg;
        }
        #endregion
    }
}
