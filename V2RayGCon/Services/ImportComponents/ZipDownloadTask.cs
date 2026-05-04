using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using V2RayGCon.Services.ShareLinkComponents;
using VgcApis.Libs.Infr;
using VgcApis.Models.Consts;

namespace V2RayGCon.Services.ImportComponents
{
    internal class ZipDownloadTask : IDownloadTask
    {
        private readonly ImportResultRecorder recoder;
        readonly string mark;
        readonly string url;

        readonly StringBuilder sb = new StringBuilder();

        public ZipDownloadTask(ImportResultRecorder recoder, string mark, string url)
        {
            this.recoder = recoder;
            this.mark = mark;
            this.url = url;
        }

        #region public methods


        public void Fetch(
            BlockingCollection<string[]> queue,
            bool isSocks5,
            int proxyPort,
            int timeout
        )
        {
            var cts = new CancellationTokenSource(timeout);
            var token = cts.Token;
            void enqueue()
            {
                var text = sb.ToString();
                try
                {
                    queue.Add(new string[] { this.mark, text });
                }
                catch
                {
                    SetTimeoutError();
                    throw;
                }
            }

            var chunkSize = Import.ParseImportZipPkgChunkSize;
            var highWater = chunkSize * 0.8;
            var overlapSize = 10 * 1024;
            void readChars(char[] buff, int len)
            {
                sb.Append(buff, 0, len);
                if (sb.Length < highWater)
                {
                    return;
                }
                enqueue();
                sb.Remove(0, sb.Length - overlapSize);
            }

            var readBuffer = new char[chunkSize];
            void readChunks(StreamReader reader)
            {
                while (!token.IsCancellationRequested)
                {
                    var n = reader.Read(readBuffer, 0, chunkSize);
                    if (n < 1)
                    {
                        // file ends
                        return;
                    }
                    readChars(readBuffer, n);
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
                                readChunks(reader);
                            }
                        }
                        catch { }
                    }
                }
                enqueue();
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }

            if (token.IsCancellationRequested)
            {
                SetTimeoutError();
            }
        }

        #endregion

        #region private methods
        void SetTimeoutError() => recoder.SetErrorMessage("Timeout.");

        void SetError(string msg) => recoder.SetErrorMessage(msg);
        #endregion
    }
}
