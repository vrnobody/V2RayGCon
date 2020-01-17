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
                    data[url] = new Models.Datas.LockValuePair<string>();
                }
            }

            var c = data[url];
            lock (c.rwLock)
            {
                var retry = VgcApis.Models.Consts.Import.ParseImportRetry;

                for (var i = 0;
                    i < retry && string.IsNullOrEmpty(c.content);
                    i++)
                {
                    c.content = Misc.Utils.Fetch(
                        url,
                        VgcApis.Models.Consts.Import.ParseImportTimeout);
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
