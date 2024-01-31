using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using V2RayGCon.Resources.Resx;
using VgcApis.Models.Datas;

namespace V2RayGCon.Misc
{
    public static class Utils
    {
        #region strings

        public static StatsSample ParseStatApiResult(bool isXray, string result)
        {
            return isXray ? ParseXrayStatApiResult(result) : ParseV2RayStatApiResult(result);
        }

        private static StatsSample ParseXrayStatApiResult(string result)
        {
            var up = 0;
            var down = 0;
            /*
            {
                "stat":  [
                    {
                        "name":  "inbound>>>agentin>>>traffic>>>uplink",
                        "value":  "0"
                    },
                    {
                        "name":  "inbound>>>agentin>>>traffic>>>downlink",
                        "value":  "0"
                    }
                ]
            }*/

            try
            {
                var json = JObject.Parse(result);
                foreach (JObject o in json["stat"].Cast<JObject>())
                {
                    var name = o["name"].ToString();
                    if (name.EndsWith("uplink"))
                    {
                        up += o["value"].Value<int>();
                    }
                    if (name.EndsWith("downlink"))
                    {
                        down += o["value"].Value<int>();
                    }
                }
            }
            catch { }
            return new StatsSample(up, down);
        }

        private static StatsSample ParseV2RayStatApiResult(string result)
        {
            var pat = StrConst.StatApiResultPattern;
            Regex rgx = new Regex(pat, RegexOptions.Singleline);
            var ms = rgx.Matches(result ?? string.Empty);
            var up = 0;
            var down = 0;
            foreach (Match match in ms)
            {
                if (!match.Success)
                {
                    continue;
                }
                var name = match.Groups[1].Value;
                if (!name.StartsWith(@"inbound>"))
                {
                    continue;
                }

                var value = VgcApis.Misc.Utils.Str2Int(match.Groups[3].Value);
                if (name.EndsWith(@">uplink"))
                {
                    up += value;
                }
                else
                {
                    down += value;
                }
            }
            return new StatsSample(up, down);
        }

        static string appNameAndVersion = null;

        public static string GetAppNameAndVer()
        {
            if (string.IsNullOrEmpty(appNameAndVersion))
            {
                var rawVer = GetAssemblyVersion();
                var ver = TrimVersionString(rawVer);
                var name = VgcApis.Misc.Utils.GetAppName();
                appNameAndVersion = VgcApis.Misc.Utils.PrependTag($"{name} v{ver}");
            }
            return appNameAndVersion;
        }

        #endregion

        #region convert

        /// <summary>
        /// http is equal to https
        /// </summary>
        /// <param name="text"></param>
        /// <param name="linkType"></param>
        /// <returns></returns>
        public static List<string> ExtractLinks(string text, Enums.LinkTypes linkType)
        {
            var links = new List<string>();
            try
            {
                string pattern = GenPattern(linkType);
                var matches = Regex.Matches("\n" + text, pattern, RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    var link = match.Value.Substring(1);
                    link = VgcApis.Misc.Utils.UnescapeUnicode(link);
                    link = VgcApis.Misc.Utils.DecodeAmpersand(link);
                    links.Add(link);
                }
            }
            catch { }
            return links;
        }

        public static Models.Datas.Vmess VmessLink2Vmess(string link)
        {
            try
            {
                string plainText = VgcApis.Misc.Utils.Base64DecodeToString(GetLinkBody(link));
                var vmess = JsonConvert.DeserializeObject<Models.Datas.Vmess>(plainText);
                if (
                    !string.IsNullOrEmpty(vmess.add)
                    && !string.IsNullOrEmpty(vmess.port)
                    && !string.IsNullOrEmpty(vmess.id)
                    && VgcApis.Misc.Utils.IsValidPort(vmess.port)
                    && new Guid(vmess.id) != new Guid()
                )
                {
                    return vmess;
                }
            }
            catch { }
            return null;
        }

        public static string TranslateSIP002Body(string body)
        {
            if (!body.Contains("@"))
            {
                return body;
            }

            var parts = body.Split(new char[] { '@', '/', '?' });
            if (parts.Length < 2)
            {
                return body;
            }

            var ss = VgcApis.Misc.Utils.Base64DecodeToString(parts[0]) + "@" + parts[1];
            return VgcApis.Misc.Utils.Base64EncodeString(ss);
        }

        public static Models.Datas.Shadowsocks SsLink2Ss(string body)
        {
            try
            {
                var ss = new Models.Datas.Shadowsocks();
                var plainText = VgcApis.Misc.Utils.Base64DecodeToString(body);
                var parts = plainText.Split('@');
                var mp = parts[0].Split(':');
                if (parts[1].Length > 0 && mp[0].Length > 0 && mp[1].Length > 0)
                {
                    ss.method = mp[0];
                    ss.pass = mp[1];
                    ss.addr = parts[1];
                }
                return ss;
            }
            catch { }
            return null;
        }

        public static JArray Str2JArray(string content)
        {
            var arr = new JArray();
            var items = content.Replace(" ", "").Split(',');
            foreach (var item in items)
            {
                if (item.Length > 0)
                {
                    arr.Add(item);
                }
            }
            return arr;
        }

        public static string JArray2Str(JArray array)
        {
            if (array == null)
            {
                return string.Empty;
            }
            List<string> s = new List<string>();

            foreach (var item in array.Children())
            {
                try
                {
                    var v = item.Value<string>();
                    if (!string.IsNullOrEmpty(v))
                    {
                        s.Add(v);
                    }
                }
                catch { }
            }

            if (s.Count <= 0)
            {
                return string.Empty;
            }
            return string.Join(",", s);
        }
        #endregion

        #region net

        static public List<string> GetOnlineV2RayCoreVersionList(
            bool isSocks5,
            int proxyPort,
            string sourceUrl
        )
        {
            List<string> versions = new List<string> { };

            // dirty hack
            // https://api.github.com/repos/XTLS/Xray-core/releases
            // https://github.com/XTLS/Xray-core/releases",
            var apiUrl = sourceUrl.Replace(@"github.com", @"api.github.com/repos");

            string text = FetchWorker(
                isSocks5,
                apiUrl,
                VgcApis.Models.Consts.Webs.LoopBackIP,
                proxyPort,
                -1,
                null,
                null
            );
            if (string.IsNullOrEmpty(text))
            {
                return versions;
            }

            string pattern = VgcApis.Models.Consts.Patterns.V2RayCoreReleaseAssets;

            var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                var v = match.Groups[1].Value;
                if (!versions.Contains(v))
                {
                    versions.Add(v);
                }
            }

            return versions;
        }

        /// <summary>
        /// List( success ? ( vmess://... , mark ) : ( "", [alias] url ) )
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <param name="proxyPort"></param>
        /// <returns></returns>
        public static List<string[]> FetchLinksFromSubcriptions(
            List<Models.Datas.SubscriptionItem> subscriptions,
            bool isSocks5,
            int proxyPort
        )
        {
            string[] worker(Models.Datas.SubscriptionItem subItem)
            {
                var url = subItem.url;
                var mark = subItem.isSetMark ? subItem.alias : null;

                var subsString = FetchWorker(
                    isSocks5,
                    url,
                    VgcApis.Models.Consts.Webs.LoopBackIP,
                    proxyPort,
                    VgcApis.Models.Consts.Import.ParseImportTimeout,
                    null,
                    null
                );

                if (string.IsNullOrEmpty(subsString))
                {
                    return new string[] { string.Empty, $"[{subItem.alias}] {url}", };
                }

                var links = new List<string> { subsString };
                var b64s = VgcApis.Misc.Utils.ExtractBase64Strings(subsString, 24);
                foreach (var b64 in b64s)
                {
                    try
                    {
                        var text = VgcApis.Misc.Utils.Base64DecodeToString(b64);
                        links.Add(text);
                    }
                    catch { }
                }

                return new string[] { string.Join("\n", links), mark };
            }

            return ExecuteInParallel(subscriptions, worker);
        }

        public static string GetBaseUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var baseUri = uri.GetLeftPart(UriPartial.Authority);
                return baseUri;
            }
            catch (ArgumentNullException) { }
            catch (UriFormatException) { }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }
            return "";
        }

        public static string PatchHref(string url, string href)
        {
            var baseUrl = GetBaseUrl(url);

            if (
                string.IsNullOrEmpty(baseUrl)
                || string.IsNullOrEmpty(href)
                || !href.StartsWith("/")
            )
            {
                return href;
            }

            return baseUrl + href;
        }

        public static string GenSearchUrl(string query, int start)
        {
            var url = VgcApis.Models.Consts.Webs.SearchUrlPrefix + UrlEncode(query);
            if (start > 0)
            {
                url += VgcApis.Models.Consts.Webs.SearchPagePrefix + start.ToString();
            }
            return url;
        }

        public static string UrlEncode(string value) => HttpUtility.UrlEncode(value);

        public static bool DownloadFileWorker(
            string url,
            string filename,
            string host,
            int proxyPort,
            int timeout
        )
        {
            var success = false;

            timeout = timeout > 0 ? timeout : VgcApis.Models.Consts.Intervals.DefaultFetchTimeout;
            if (!VgcApis.Misc.Utils.IsHttpLink(url))
            {
                url = VgcApis.Misc.Utils.RelativePath2FullPath(url);
                proxyPort = -1;
            }

            var dlCompleted = new AutoResetEvent(false);

            var wc = VgcApis.Misc.Utils.CreateWebClient(false, host, proxyPort, null, null);

            wc.DownloadFileCompleted += (s, a) =>
            {
                if (!a.Cancelled && a.Error == null)
                {
                    success = true;
                }
                try
                {
                    dlCompleted.Set();
                }
                catch { }
            };

            try
            {
                wc.DownloadFileAsync(new Uri(url), filename);
                dlCompleted.WaitOne(timeout);
                dlCompleted.Dispose();
            }
            catch { }

            VgcApis.Misc.Utils.CancelWebClientAsync(wc);
            wc.Dispose();
            return success;
        }

        /// <summary>
        /// Download through HTTP or SOCKS5 proxy. Return string.Empty if sth. goes wrong.
        /// </summary>
        /// <param name="url">string</param>
        /// <param name="proxyPort">1-65535, other value means download directly</param>
        /// <param name="timeout">millisecond, if &lt;1 then use default value 30000</param>
        /// <returns>If sth. goes wrong return string.Empty</returns>
        public static string FetchWorker(
            bool isSocks5,
            string url,
            string host,
            int proxyPort,
            int timeout,
            string username,
            string password
        )
        {
            var html = string.Empty;

            timeout = timeout > 0 ? timeout : VgcApis.Models.Consts.Intervals.DefaultFetchTimeout;

            if (!VgcApis.Misc.Utils.IsHttpLink(url))
            {
                url = VgcApis.Misc.Utils.RelativePath2FullPath(url);
                proxyPort = -1;
            }
            var dlCompleted = new AutoResetEvent(false);

            var wc = VgcApis.Misc.Utils.CreateWebClient(
                isSocks5,
                host,
                proxyPort,
                username,
                password
            );

            wc.DownloadStringCompleted += (s, a) =>
            {
                if (!a.Cancelled && a.Error == null)
                {
                    try
                    {
                        // 如果下载过程中遇到错误，a.Result会调用RaiseExceptionIfNecessary()抛出异常
                        html = a.Result.ToString();
                    }
                    catch { }
                }
                try
                {
                    dlCompleted.Set();
                }
                catch { }
            };

            try
            {
                wc.DownloadStringAsync(new Uri(url));
                dlCompleted.WaitOne(timeout);
                dlCompleted.Dispose();
            }
            catch { }

            VgcApis.Misc.Utils.CancelWebClientAsync(wc);
            wc.Dispose();
            return html;
        }

        public static string Fetch(string url, int proxyPort, int timeout)
        {
            var host = VgcApis.Models.Consts.Webs.LoopBackIP;
            return FetchWorker(false, url, host, proxyPort, timeout, null, null);
        }
        #endregion

        #region files

        private static void WriteToFileAtOnce(string path, string userSettings)
        {
            // https://stackoverflow.com/questions/25366534/file-writealltext-not-flushing-data-to-disk
            var bufferSize = VgcApis.Models.Consts.Libs.DefaultBufferSize;
            using (var fs = File.Create(path, bufferSize, FileOptions.WriteThrough))
            {
                using (var w = new StreamWriter(fs))
                {
                    w.Write(userSettings);
                }
            }
        }

        internal static bool ClumsyWriter(
            string userSettings,
            string mainFilename,
            string bakFilename
        )
        {
            try
            {
                WriteToFileAtOnce(mainFilename, userSettings);
                WriteToFileAtOnce(bakFilename, userSettings);
                return true;
            }
            catch
            {
                VgcApis.Libs.Sys.FileLogger.Error($"ClumsyWriter(): Write file failed!");
            }
            return false;
        }

        public static string GetSha256SumFromFile(string file)
        {
            // http://peterkellner.net/2010/11/24/efficiently-generating-sha256-checksum-for-files-using-csharp/
            try
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    var sha = new SHA256Managed();
                    byte[] checksum = sha.ComputeHash(stream);
                    return BitConverter.ToString(checksum).Replace("-", String.Empty).ToLower();
                }
            }
            catch { }
            return string.Empty;
        }

        public static string GetSysAppDataFolder()
        {
            var appData = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData
            );
            var appName = VgcApis.Misc.Utils.GetAppName();
            return Path.Combine(appData, appName);
        }

        public static void CreateAppDataFolder()
        {
            var path = GetSysAppDataFolder();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void DeleteAppDataFolder()
        {
            Directory.Delete(GetSysAppDataFolder(), recursive: true);
        }
        #endregion

        #region Miscellaneous


        public static string TrimVersionString(string version)
        {
            for (int i = 0; i < 2; i++)
            {
                if (!version.EndsWith(".0"))
                {
                    return version;
                }
                var len = version.Length;
                version = version.Substring(0, len - 2);
            }

            return version;
        }

        public static string GetAssemblyVersion()
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            return version.ToString();
        }

        public static bool AreEqual(double a, double b)
        {
            return Math.Abs(a - b) < 0.000001;
        }

        public static string SHA256(string randomString)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString ?? string.Empty));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        // 懒得改调用处的代码了。
        public static int Clamp(int value, int min, int max) =>
            VgcApis.Misc.Utils.Clamp(value, min, max);

        public static int GetIndexIgnoreCase(Dictionary<int, string> dict, string value)
        {
            foreach (var data in dict)
            {
                if (
                    !string.IsNullOrEmpty(data.Value)
                    && data.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase)
                )
                {
                    return data.Key;
                }
            }
            return -1;
        }

        static string GenLinkPrefix(Enums.LinkTypes linkType) => $"{linkType}";

        public static string GenPattern(Enums.LinkTypes linkType)
        {
            string pattern;
            switch (linkType)
            {
                case Enums.LinkTypes.ss:
                case Enums.LinkTypes.socks:
                    pattern =
                        GenLinkPrefix(linkType)
                        + "://"
                        + VgcApis.Models.Consts.Patterns.SsShareLinkContent;
                    break;
                case Enums.LinkTypes.vmess:
                case Enums.LinkTypes.v2cfg:
                    pattern =
                        GenLinkPrefix(linkType)
                        + "://"
                        + VgcApis.Models.Consts.Patterns.Base64NonStandard;
                    break;
                case Enums.LinkTypes.http:
                case Enums.LinkTypes.https:
                    pattern = VgcApis.Models.Consts.Patterns.HttpUrl;
                    break;
                case Enums.LinkTypes.trojan:
                    pattern =
                        GenLinkPrefix(linkType)
                        + "://"
                        + VgcApis.Models.Consts.Patterns.UriContentNonStandard;
                    break;
                case Enums.LinkTypes.vless:
                    // pattern = GenLinkPrefix(linkType) + "://" + VgcApis.Models.Consts.Patterns.UriContent;
                    pattern =
                        GenLinkPrefix(linkType)
                        + "://"
                        + VgcApis.Models.Consts.Patterns.UriContentNonStandard;
                    break;
                default:
                    throw new NotSupportedException($"Not supported link type {linkType}:// ...");
            }

            return VgcApis.Models.Consts.Patterns.NonAlphabets + pattern;
        }

        public static string AddLinkPrefix(string b64Content, Enums.LinkTypes linkType)
        {
            return GenLinkPrefix(linkType) + "://" + b64Content;
        }

        public static string GetLinkBody(string link)
        {
            var needle = @"://";
            var index = link.IndexOf(needle);

            if (index < 0)
            {
                throw new ArgumentException($"Not a valid link ${link}");
            }

            return link.Substring(index + needle.Length);
        }

        public static void ZipFileDecompress(string zipFile, string outFolder)
        {
            // let downloader handle exception
            using (ZipFile zip = ZipFile.Read(zipFile))
            {
                zip.ExtractAll(outFolder, ExtractExistingFileAction.OverwriteSilently);
            }
        }
        #endregion

        #region UI related
        public static void CopyToClipboardAndPrompt(string content) =>
            VgcApis.Misc.UI.MsgBox(CopyToClipboard(content) ? I18N.CopySuccess : I18N.CopyFail);

        public static bool CopyToClipboard(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            try
            {
                Clipboard.SetText(content);
                return true;
            }
            catch { }
            return false;
        }

        public static void EnableTls13Support()
        {
            // https://www.medo64.com/2020/05/using-tls-1-3-from-net-4-0-application/
            try
            { //try TLS 1.3
                ServicePointManager.SecurityProtocol =
                    (SecurityProtocolType)12288
                    | (SecurityProtocolType)3072
                    | (SecurityProtocolType)768
                    | SecurityProtocolType.Tls;
            }
            catch (NotSupportedException)
            {
                try
                { //try TLS 1.2
                    ServicePointManager.SecurityProtocol =
                        (SecurityProtocolType)3072
                        | (SecurityProtocolType)768
                        | SecurityProtocolType.Tls;
                }
                catch (NotSupportedException)
                {
                    try
                    { //try TLS 1.1
                        ServicePointManager.SecurityProtocol =
                            (SecurityProtocolType)768 | SecurityProtocolType.Tls;
                    }
                    catch (NotSupportedException)
                    { //TLS 1.0
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    }
                    VgcApis.Misc.UI.MsgBox(I18N.SysNotSupportTLS12);
                }
            }
        }

        public static string GetClipboardText()
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                return Clipboard.GetText(TextDataFormat.Text);
            }
            return string.Empty;
        }
        #endregion

        #region task and process

        /*
         * ChainActionHelper loops from [count - 1] to [0]
         *
         * These integers, which is index in this example,
         * will be transfered into worker function one by one.
         *
         * The second parameter "next" is generated automatically
         * for chaining up all workers.
         *
         * e.g.
         *
         * Action<int,Action> worker = (index, next)=>{
         *
         *   // do something accroding to index
         *   Debug.WriteLine(index);
         *
         *   // call next when done
         *   next();
         * }
         *
         * Action done = ()=>{
         *   // do something when all done
         *   // or simply set to null
         * }
         *
         * Finally call this function like this.
         * ChainActionHelper(10, worker, done);
         */

        public static void ChainActionHelperAsync(
            int countdown,
            Action<int, Action> worker,
            Action done = null
        )
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                ChainActionHelperWorker(countdown, worker, done)();
            });
        }

        // wrapper
        public static void ChainActionHelper(
            int countdown,
            Action<int, Action> worker,
            Action done = null
        )
        {
            ChainActionHelperWorker(countdown, worker, done)();
        }

        static Action ChainActionHelperWorker(
            int countdown,
            Action<int, Action> worker,
            Action done = null
        )
        {
            int _index = countdown - 1;

            return () =>
            {
                if (_index < 0)
                {
                    done?.Invoke();
                    return;
                }

                worker(_index, ChainActionHelperWorker(_index, worker, done));
            };
        }

        public static void ExecuteInParallel<TParam>(
            IEnumerable<TParam> param,
            Action<TParam> worker
        ) =>
            ExecuteInParallel(
                param,
                (p) =>
                {
                    worker(p);
                    // ExecuteInParallel require a return value
                    return "nothing";
                }
            );

        public static List<TResult> ExecuteInParallel<TParam, TResult>(
            IEnumerable<TParam> param,
            Func<TParam, TResult> worker
        )
        {
            var result = new List<TResult>();

            if (param.Count() <= 0)
            {
                return result;
            }

            var taskList = new List<Task<TResult>>();
            foreach (var value in param)
            {
                var task = new Task<TResult>(() => worker(value), TaskCreationOptions.LongRunning);
                taskList.Add(task);
                task.Start();
            }

            Task.WaitAll(taskList.ToArray());

            foreach (var task in taskList)
            {
                result.Add(task.Result);
                task.Dispose();
            }

            return result;
        }
        #endregion

        #region for Testing
        public static string[] TestingGetResourceConfigJson()
        {
            return new string[] { StrConst.config_min, StrConst.config_tpl, StrConst.config_pkg, };
        }
        #endregion
    }
}
