using System.Net;

namespace V2RayGCon.Services.Caches
{
    public class HTML : BaseClasses.CacheComponent<string, string>
    {
        #region public method
        public new string this[string url]
        {
            get => GetCache(url);
        }
        #endregion

        #region private method
        string GetCache(string url)
        {
            lock (writeLock)
            {
                if (!ContainsKey(url))
                {
                    TrimHalf(VgcApis.Models.Consts.Import.HtmlCacheSize);
                    data[url] = new Models.Datas.LockValuePair<string>();
                }
            }

            var c = data[url];
            lock (c.rwLock)
            {
                var retry = VgcApis.Models.Consts.Import.ParseImportRetry;

                for (var i = 0; i < retry && string.IsNullOrEmpty(c.content); i++)
                {
                    var port = -1;
                    try
                    {
                        if (Settings.Instance.isUpdateUseProxy)
                        {
                            port = Servers.Instance.GetAvailableHttpProxyPort();
                        }
                    }
                    catch
                    {
                        // under unit tests Service.Servers is not initialized
                        // the code above will throw a NullReferenceException
                    }
                    c.content = Misc.Utils.Fetch(
                        url,
                        port,
                        VgcApis.Models.Consts.Import.ParseImportTimeout
                    );
                }
            }

            if (string.IsNullOrEmpty(c.content))
            {
                throw new WebException("Download fail!");
            }
            return c.content;
        }

        #endregion
    }
}
