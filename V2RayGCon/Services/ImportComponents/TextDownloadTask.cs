using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services.ShareLinkComponents;
using VgcApis.Interfaces;
using VgcApis.Libs.Infr;
using VgcApis.Models.Consts;
using static ScintillaNET.Style;

namespace V2RayGCon.Services.ImportComponents
{
    internal class TextDownloadTask : IDownloadTask
    {
        public readonly string mark;
        public readonly string url;

        public TextDownloadTask(string mark, string url)
        {
            this.mark = mark;
            this.url = url;
        }

        #region private methods

        public void Fetch(
            BlockingCollection<string[]> queue,
            bool isSocks5,
            int proxyPort,
            int timeout
        )
        {
            var html = VgcApis.Misc.Utils.FetchWorker(
                isSocks5,
                this.url,
                Webs.LoopBackIP,
                proxyPort,
                timeout,
                null,
                null
            );

            if (string.IsNullOrEmpty(html) || !TryAdd(queue, html))
            {
                return;
            }

            var sb = new StringBuilder();
            bool enqueue()
            {
                var text = sb.ToString();
                sb.Clear();
                if (string.IsNullOrEmpty(text))
                {
                    return true;
                }
                return TryAdd(queue, text);
            }

            bool push(string text)
            {
                sb.AppendLine(text);
                if (sb.Length < Import.ParseImportZipPkgChunkSize)
                {
                    return true;
                }
                return enqueue();
            }

            var b64s = VgcApis.Misc.Utils.ExtractBase64Strings(html, 300 * 4 / 3);
            foreach (var b64 in b64s)
            {
                if (b64.StartsWith("//"))
                {
                    continue;
                }

                var text = VgcApis.Misc.Utils.Base64DecodeToString(b64);
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }
                if (!push(text))
                {
                    return;
                }
            }
            enqueue();
        }
        #endregion
        #region private methods
        bool TryAdd(BlockingCollection<string[]> queue, string html)
        {
            try
            {
                queue.Add(new string[] { this.mark, html });

                return true;
            }
            catch { }
            return false;
        }

        #endregion
    }
}
