using System;
using System.Collections.Generic;
using System.Threading;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services.ShareLinkComponents;
using VgcApis.Interfaces;
using VgcApis.Libs.Infr;
using VgcApis.Models.Consts;
using static ScintillaNET.Style;

namespace V2RayGCon.Services.ImportComponents
{
    internal class TextTask
    {
        public readonly string mark;
        public readonly string url;

        public TextTask(string mark, string url)
        {
            this.mark = mark;
            this.url = url;
        }

        #region private methods
        public void Exec(
            Action<string, string, Action, CancellationToken> decode,
            bool isSocks5,
            int proxyPort
        )
        {
            var htmls = Download(isSocks5, proxyPort);
            foreach (var html in htmls)
            {
                decode(mark, html, null, CancellationToken.None);
            }
        }
        #endregion
        #region private methods

        List<string> Download(bool isSocks5, int proxyPort)
        {
            var htmls = new List<string>();

            var html = VgcApis.Misc.Utils.FetchWorker(
                isSocks5,
                this.url,
                Webs.LoopBackIP,
                proxyPort,
                Import.ParseImportTimeout,
                null,
                null
            );
            if (string.IsNullOrEmpty(html))
            {
                return htmls;
            }

            htmls.Add(html);
            var b64s = VgcApis.Misc.Utils.ExtractBase64Strings(html, 200 * 4 / 3);
            foreach (var b64 in b64s)
            {
                if (b64.StartsWith("//"))
                {
                    continue;
                }

                var text = VgcApis.Misc.Utils.Base64DecodeToString(b64);
                if (!string.IsNullOrEmpty(text))
                {
                    htmls.Add(text);
                }
            }
            return htmls;
        }
        #endregion
    }
}
