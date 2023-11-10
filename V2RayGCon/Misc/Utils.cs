using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                foreach (JObject o in json["stat"])
                {
                    var name = o["name"].ToString();
                    if (name.EndsWith("uplink"))
                    {
                        up = o["value"].Value<int>();
                    }
                    if (name.EndsWith("downlink"))
                    {
                        down = o["value"].Value<int>();
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
                appNameAndVersion = $"{name} v{ver}";
            }
            return appNameAndVersion;
        }

        #endregion

        #region Json
        public static JArray ExtractOutboundsFromConfig(JObject json)
        {
            var result = new JArray();
            if (json == null)
            {
                return result;
            }

            try
            {
                var outbound = GetKey(json, "outbound");
                if (outbound != null && outbound is JObject)
                {
                    result.Add(outbound);
                }
            }
            catch { }

            foreach (var key in new string[] { "outboundDetour", "outbounds" })
            {
                try
                {
                    var outboundDtr = GetKey(json, key);
                    if (outboundDtr != null && outboundDtr is JArray)
                    {
                        foreach (JObject item in outboundDtr)
                        {
                            result.Add(item);
                        }
                    }
                }
                catch { }
            }
            return result;
        }

        public static string GetAliasFromConfig(JObject config)
        {
            var name = GetValue<string>(config, "v2raygcon.alias");
            return string.IsNullOrEmpty(name) ? I18N.Empty : name;
        }

        public static string ExtractSummaryFromYaml(string config)
        {
            if (VgcApis.Misc.Utils.IsYaml(config))
            {
                var pat = @"server: *([^\n]*)";
                var g = Regex.Match(config, pat).Groups;
                if (g.Count > 1)
                {
                    return $"unknow@{g[1].Value}";
                }
            }
            return "";
        }

        public static string ExtractSummaryFromJson(string config)
        {
            var json = VgcApis.Misc.Utils.ParseJObject(config);
            if (json == null)
            {
                return string.Empty;
            }

            var count = json["outbounds"]?.Count() ?? 0;
            var strategy = GetValue<string>(json, "routing.balancers.0.strategy.type");
            if (!string.IsNullOrEmpty(strategy))
            {
                return $"balancer: {count} {strategy}";
            }

            var tag = GetValue<string>(json, "routing.balancers.0.tag");
            if (!string.IsNullOrEmpty(tag))
            {
                return $"balancer: {count} random";
            }

            var proxy = GetValue<string>(json, "outbounds.0.proxySettings.tag");
            if (!string.IsNullOrEmpty(proxy))
            {
                return $"proxychain: {count}";
            }

            var result = GetSummaryFromConfig(json, "outbounds.0");
            if (string.IsNullOrEmpty(result))
            {
                result = GetSummaryFromConfig(json, "outbound");
            }
            return result;
        }

        static string GetStreamSettingInfo(JObject json, string root)
        {
            var streamType = GetValue<string>(json, root + ".streamSettings.network")?.ToLower();
            // "tcp" | "kcp" | "ws" | "http" | "domainsocket" | "quic"
            string result;
            switch (streamType)
            {
                case null:
                    result = "";
                    break;
                case "domainsocket":
                    result = "ds";
                    break;
                default:
                    result = streamType;
                    break;
            }

            var sec = GetValue<string>(json, root + ".streamSettings.security")?.ToLower();
            if (!string.IsNullOrWhiteSpace(sec) && sec != "none")
            {
                result += $".{sec}";
            }
            return result;
        }

        static string GetSummaryFromConfig(JObject json, string root)
        {
            var protocol = GetValue<string>(json, root + ".protocol")?.ToLower();
            if (protocol == null)
            {
                return string.Empty;
            }

            string addrKey = root;
            switch (protocol)
            {
                case "vless":
                case "vmess":
                    addrKey += ".settings.vnext.0.address";
                    break;
                case "shadowsocks":
                    protocol = "ss";
                    addrKey += ".settings.servers.0.address";
                    break;
                case "trojan":
                case "socks":
                case "http":
                    addrKey += ".settings.servers.0.address";
                    break;
            }

            string addr = GetValue<string>(json, addrKey);
            string streamType = GetStreamSettingInfo(json, root);

            return protocol
                + (string.IsNullOrEmpty(streamType) ? "" : $".{streamType}")
                + (string.IsNullOrEmpty(addr) ? "" : "@" + VgcApis.Misc.Utils.FormatHost(addr));
        }

        static bool Contains(JProperty main, JProperty sub)
        {
            return Contains(main.Value, sub.Value);
        }

        static bool Contains(JArray main, JArray sub)
        {
            foreach (var sItem in sub)
            {
                foreach (var mItem in main)
                {
                    if (Contains(mItem, sItem))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static bool Contains(JObject main, JObject sub)
        {
            foreach (var item in sub)
            {
                var key = item.Key;
                if (!main.ContainsKey(key))
                {
                    return false;
                }

                if (!Contains(main[key], sub[key]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Contains(JValue main, JValue sub)
        {
            return main.Equals(sub);
        }

        public static bool Contains(JToken main, JToken sub)
        {
            if (main.Type != sub.Type)
            {
                return false;
            }

            switch (sub.Type)
            {
                case JTokenType.Property:
                    return Contains(main as JProperty, sub as JProperty);
                case JTokenType.Object:
                    return Contains(main as JObject, sub as JObject);
                case JTokenType.Array:
                    return Contains(main as JArray, sub as JArray);
                default:
                    return Contains(main as JValue, sub as JValue);
            }
        }

        public static Tuple<string, string> ParsePathIntoParentAndKey(string path)
        {
            var index = path.LastIndexOf('.');
            string key;
            string parent = string.Empty;
            if (index < 0)
            {
                key = path;
            }
            else if (index == 0)
            {
                key = path.Substring(1);
            }
            else
            {
                key = path.Substring(index + 1);
                parent = path.Substring(0, index);
            }

            return new Tuple<string, string>(parent, key);
        }

        public static JObject CreateJObject(string path)
        {
            return CreateJObject(path, null);
        }

        public static JObject CreateJObject(string path, JToken child)
        {
            JToken result;
            if (child == null)
            {
                result = JToken.Parse(@"{}");
            }
            else
            {
                result = child;
            }

            if (string.IsNullOrEmpty(path))
            {
                return JObject.Parse(@"{}");
            }

            JToken tempNode;
            foreach (var p in path.Split('.').Reverse())
            {
                if (string.IsNullOrEmpty(p))
                {
                    throw new KeyNotFoundException("Parent contain empty key");
                }

                if (int.TryParse(p, out int num))
                {
                    if (num != 0)
                    {
                        throw new KeyNotFoundException("All parents must be JObject");
                    }
                    tempNode = JArray.Parse(@"[{}]");
                    tempNode[0] = result;
                }
                else
                {
                    tempNode = JObject.Parse(@"{}");
                    tempNode[p] = result;
                }
                result = tempNode;
            }

            return result as JObject;
        }

        public static bool TrySetValue<T>(JToken json, string path, T value)
        {
            var parts = ParsePathIntoParentAndKey(path);
            var key = parts.Item2;
            var parent = parts.Item1;

            var node = string.IsNullOrEmpty(parent) ? json : GetKey(json, parent);
            if (node == null)
            {
                return false;
            }

            try
            {
                switch (node)
                {
                    case JObject o:
                        o[key] = new JValue(value);
                        return true;
                    case JArray a:
                        a[VgcApis.Misc.Utils.Str2Int(key)] = new JValue(value);
                        return true;
                    default:
                        break;
                }
            }
            catch { }
            return false;
        }

        public static bool TryExtractJObjectPart(JObject source, string path, out JObject result)
        {
            var parts = ParsePathIntoParentAndKey(path);
            var key = parts.Item2;
            var parentPath = parts.Item1;
            result = null;

            if (string.IsNullOrEmpty(key))
            {
                // throw new KeyNotFoundException("Key is empty");
                return false;
            }

            var node = GetKey(source, path);
            if (node == null)
            {
                // throw new KeyNotFoundException("This JObject has no key: " + path);
                return false;
            }

            result = CreateJObject(parentPath);

            var parent = string.IsNullOrEmpty(parentPath) ? result : GetKey(result, parentPath);

            if (parent == null || !(parent is JObject))
            {
                // throw new KeyNotFoundException("Create parent JObject fail!");
                return false;
            }

            parent[key] = node.DeepClone();
            return true;
        }

        public static void RemoveKeyFromJObject(JObject json, string path)
        {
            var parts = ParsePathIntoParentAndKey(path);

            var parent = parts.Item1;
            var key = parts.Item2;

            if (string.IsNullOrEmpty(key))
            {
                throw new KeyNotFoundException();
            }

            var node = string.IsNullOrEmpty(parent) ? json : GetKey(json, parent);

            if (node == null || !(node is JObject))
            {
                throw new KeyNotFoundException();
            }

            (node as JObject).Property(key)?.Remove();
        }

        static void ConcatJson(JObject body, JObject mixin)
        {
            body.Merge(
                mixin,
                new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Concat,
                    MergeNullValueHandling = MergeNullValueHandling.Ignore,
                }
            );
        }

        public static void UnionJson(JObject body, JObject mixin)
        {
            body.Merge(
                mixin,
                new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union,
                    MergeNullValueHandling = MergeNullValueHandling.Ignore,
                }
            );
        }

        public static void CombineConfigWithRoutingInFront(ref JObject body, JObject mixin)
        {
            List<string> keys = new List<string>
            {
                "inbounds",
                "outbounds",
                "inboundDetour",
                "outboundDetour",
                "routing.rules",
                "routing.balancers",
                "routing.settings.rules",
            };
            CombineConfigWorker(ref body, mixin, keys);
        }

        static JObject CombineJArray(JObject body, JObject mixin, string key)
        {
            if (mixin == null)
            {
                return body;
            }

            if (body == null)
            {
                body = JObject.Parse(@"{}");
                ConcatJson(body, mixin);
                return body;
            }

            if (!(body[key] is JArray))
            {
                body[key] = JArray.Parse(@"[]");
            }

            foreach (JObject n in mixin[key])
            {
                void innerLoop()
                {
                    foreach (JObject m in body[key])
                    {
                        var mt = m["tag"];
                        var nt = n["tag"];
                        if (mt != null && nt != null && mt.ToString() == nt.ToString())
                        {
                            UnionJson(m, n);
                            return;
                        }
                    }
                    (body[key] as JArray).Insert(0, n);
                }
                innerLoop();
            }
            return body;
        }

        static void CombineConfigWorker(ref JObject body, JObject mixin, IEnumerable<string> keys)
        {
            JObject backup = JObject.Parse(@"{}");

            // add to front
            foreach (var key in keys)
            {
                if (TryExtractJObjectPart(body, key, out JObject nodeBody))
                {
                    RemoveKeyFromJObject(body, key);
                }

                if (TryExtractJObjectPart(mixin, key, out JObject nodeMixin))
                {
                    ConcatJson(backup, nodeMixin);
                    RemoveKeyFromJObject(mixin, key);

                    switch (key)
                    {
                        case "inbounds":
                        case "outbounds":
                        case "inboundDetour":
                        case "outboundDetour":
                            nodeBody = CombineJArray(nodeBody, nodeMixin, key);
                            break;
                        default:
                            ConcatJson(body, nodeMixin);
                            break;
                    }
                }

                if (nodeBody != null)
                {
                    UnionJson(body, nodeBody);
                }
            }

            MergeJson(body, mixin);

            // restore mixin
            ConcatJson(mixin, backup);
        }

        public static void MergeJson(JObject body, JObject mixin)
        {
            body.Merge(
                mixin,
                new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Merge,
                    MergeNullValueHandling = MergeNullValueHandling.Merge
                }
            );
        }

        public static JToken GetKey(JToken json, string path) =>
            VgcApis.Misc.Utils.GetKey(json, path);

        public static T GetValue<T>(JToken json, string prefix, string key) =>
            VgcApis.Misc.Utils.GetValue<T>(json, prefix, key);

        /// <summary>
        /// return null if not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetValue<T>(JToken json, string path) =>
            VgcApis.Misc.Utils.GetValue<T>(json, path);

        public static string GetAddr(JObject json, string prefix, string keyIP, string keyPort)
        {
            var ip = GetValue<String>(json, prefix, keyIP) ?? VgcApis.Models.Consts.Webs.LoopBackIP;
            var port = GetValue<string>(json, prefix, keyPort);
            return string.Join(":", ip, port);
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

        static public List<string> GetOnlineV2RayCoreVersionList(int proxyPort, string sourceUrl)
        {
            List<string> versions = new List<string> { };

            // dirty hack
            // https://api.github.com/repos/XTLS/Xray-core/releases
            // https://github.com/XTLS/Xray-core/releases",
            var apiUrl = sourceUrl.Replace(@"github.com", @"api.github.com/repos");

            string text = Fetch(apiUrl, proxyPort, -1);
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
            int proxyPort
        )
        {
            string[] worker(Models.Datas.SubscriptionItem subItem)
            {
                var url = subItem.url;
                var mark = subItem.isSetMark ? subItem.alias : null;

                var subsString = Fetch(
                    url,
                    proxyPort,
                    VgcApis.Models.Consts.Import.ParseImportTimeout
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
                string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(href) || !href.StartsWith("/")
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

        static bool DownloadFileWorker(string url, string filename, int proxyPort, int timeout)
        {
            var success = false;

            timeout = timeout > 0 ? timeout : VgcApis.Models.Consts.Intervals.DefaultFetchTimeout;
            if (!VgcApis.Misc.Utils.IsHttpLink(url))
            {
                url = VgcApis.Misc.Utils.RelativePath2FullPath(url);
                proxyPort = -1;
            }

            var dlCompleted = new AutoResetEvent(false);

            var wc = VgcApis.Misc.Utils.CreateWebClient(false, proxyPort);

            wc.DownloadFileCompleted += (s, a) =>
            {
                if (!a.Cancelled)
                {
                    success = true;
                }
                dlCompleted.Set();
                (s as WebClient)?.Dispose();
            };

            try
            {
                wc.DownloadFileAsync(new Uri(url), filename);
                dlCompleted.WaitOne(timeout);
            }
            catch { }

            VgcApis.Misc.Utils.CancelWebClientAsync(wc);

            return success;
        }

        public static bool DownloadFile(string url, string filename, int proxyPort, int timeout) =>
            DownloadFileWorker(url, filename, proxyPort, timeout);

        /// <summary>
        /// Download through http://127.0.0.1:proxyPort. Return string.Empty if sth. goes wrong.
        /// </summary>
        /// <param name="url">string</param>
        /// <param name="proxyPort">1-65535, other value means download directly</param>
        /// <param name="timeout">millisecond, if &lt;1 then use default value 30000</param>
        /// <returns>If sth. goes wrong return string.Empty</returns>
        static string FetchWorker(bool isSocks5, string url, int proxyPort, int timeout)
        {
            var html = string.Empty;

            timeout = timeout > 0 ? timeout : VgcApis.Models.Consts.Intervals.DefaultFetchTimeout;

            if (!VgcApis.Misc.Utils.IsHttpLink(url))
            {
                url = VgcApis.Misc.Utils.RelativePath2FullPath(url);
                proxyPort = -1;
            }
            var dlCompleted = new AutoResetEvent(false);

            var wc = VgcApis.Misc.Utils.CreateWebClient(isSocks5, proxyPort);

            wc.DownloadStringCompleted += (s, a) =>
            {
                if (!a.Cancelled)
                {
                    try
                    {
                        // 如果下载过程中遇到错误，a.Result会调用RaiseExceptionIfNecessary()抛出异常
                        html = a.Result.ToString();
                    }
                    catch { }
                }
                dlCompleted.Set();
                (s as WebClient)?.Dispose();
            };

            try
            {
                wc.DownloadStringAsync(new Uri(url));
                dlCompleted.WaitOne(timeout);
            }
            catch { }

            VgcApis.Misc.Utils.CancelWebClientAsync(wc);
            return html;
        }

        public static string Fetch(string url, int proxyPort, int timeout) =>
            Fetch(url, proxyPort, timeout, false);

        public static string Fetch(string url, int proxyPort, int timeout, bool isSocks5) =>
            FetchWorker(isSocks5, url, proxyPort, timeout);

        public static string Fetch(string url) => Fetch(url, -1, -1);

        public static string Fetch(string url, int timeout) => Fetch(url, -1, timeout);

        #endregion

        #region files
        internal static bool SerializeToFile(
            string path,
            Models.Datas.UserSettings userSettings,
            List<CoreInfo> coreInfos,
            Dictionary<string, string> pluginsSetting
        )
        {
            try
            {
                // for debugging
                if (coreInfos == null && pluginsSetting == null)
                {
                    WriteToFileAtOnce(path, userSettings);
                    return true;
                }

                List<string> parts = SplitSerializedUserSettings(userSettings);
                if (parts.Count != 5)
                {
                    return false;
                }
                VgcApis.Misc.Utils.ClearFile(path);
                WriteToFileInParts(path, parts, coreInfos, pluginsSetting);
                return true;
            }
            catch (Exception e)
            {
                VgcApis.Libs.Sys.FileLogger.Error($"WriteAllTextNow() exception: {e}");
            }
            return false;
        }

        // must lock userSettings.CompressedUnicodePluginsSetting first!
        static List<string> SplitSerializedUserSettings(Models.Datas.UserSettings userSettings)
        {
            var coreInfoPlaceHolder = VgcApis.Models.Consts.Libs.coreInfoPlaceHolder;
            var pluginPlaceHolder = VgcApis.Models.Consts.Libs.pluginPlaceHolder;

            userSettings.CompressedUnicodePluginsSetting = pluginPlaceHolder;
            userSettings.CompressedUnicodeCoreInfoList = coreInfoPlaceHolder;

            var parts = new List<string>();
            try
            {
                var json = JsonConvert.SerializeObject(userSettings, Formatting.Indented);
                if (VgcApis.Misc.Utils.FindAll(json, pluginPlaceHolder).Count != 1)
                {
                    return parts;
                }

                var option = StringSplitOptions.RemoveEmptyEntries;
                var p = json.Split(new string[] { pluginPlaceHolder }, option);
                if (p.Length != 2)
                {
                    return parts;
                }

                if (VgcApis.Misc.Utils.FindAll(p[0], coreInfoPlaceHolder).Count == 1)
                {
                    var pp = p[0].Split(new string[] { coreInfoPlaceHolder }, option);
                    parts.Add(pp[0]);
                    parts.Add(coreInfoPlaceHolder);
                    parts.Add(pp[1]);
                    parts.Add(pluginPlaceHolder);
                    parts.Add(p[1]);
                }
                else if (VgcApis.Misc.Utils.FindAll(p[1], coreInfoPlaceHolder).Count == 1)
                {
                    var pp = p[1].Split(new string[] { coreInfoPlaceHolder }, option);
                    parts.Add(p[0]);
                    parts.Add(pluginPlaceHolder);
                    parts.Add(pp[0]);
                    parts.Add(coreInfoPlaceHolder);
                    parts.Add(pp[1]);
                }
            }
            catch { }
            return parts;
        }

        private static void WriteToFileAtOnce(string path, Models.Datas.UserSettings userSettings)
        {
            // https://stackoverflow.com/questions/25366534/file-writealltext-not-flushing-data-to-disk
            using (var fs = File.Create(path, 64 * 1024, FileOptions.WriteThrough))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    using (JsonTextWriter jw = new JsonTextWriter(sw))
                    {
                        JsonSerializer js = new JsonSerializer();
                        js.Serialize(jw, userSettings);
                    }
                }
            }
        }

        private static void WriteToFileInParts(
            string path,
            List<string> parts,
            List<CoreInfo> coreInfos,
            Dictionary<string, string> pluginsSetting
        )
        {
            var pluginPlaceHolder = VgcApis.Models.Consts.Libs.pluginPlaceHolder;
            var coreInfoPlaceHolder = VgcApis.Models.Consts.Libs.coreInfoPlaceHolder;

            foreach (var part in parts)
            {
                if (part == pluginPlaceHolder)
                {
                    VgcApis.Libs.Infr.ZipExtensions.SerializeObjectAsCompressedUnicodeBase64StringToFile(
                        path,
                        pluginsSetting
                    );
                }
                else if (part == coreInfoPlaceHolder)
                {
                    VgcApis.Libs.Infr.ZipExtensions.SerializeObjectAsCompressedUnicodeBase64StringToFile(
                        path,
                        coreInfos
                    );
                }
                else
                {
                    using (var writer = File.AppendText(path))
                    {
                        writer.Write(part);
                    }
                }
            }
        }

        internal static bool ClumsyWriter(
            Models.Datas.UserSettings userSettings,
            List<CoreInfo> coreInfos,
            Dictionary<string, string> pluginsSetting,
            string mainFilename,
            string bakFilename
        )
        {
            try
            {
                if (SerializeToFile(mainFilename, userSettings, coreInfos, pluginsSetting))
                {
                    if (SerializeToFile(bakFilename, userSettings, coreInfos, pluginsSetting))
                    {
                        return true;
                    }
                    else
                    {
                        VgcApis.Libs.Sys.FileLogger.Error(
                            $"ClumsyWriter(): Write bak file failed!"
                        );
                    }
                }
                else
                {
                    VgcApis.Libs.Sys.FileLogger.Error($"ClumsyWriter(): Write main file failed!");
                }
            }
            catch { }
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
            VgcApis.Misc.Utils.RunInBgSlim(() =>
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
