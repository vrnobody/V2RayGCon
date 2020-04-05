using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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

namespace V2RayGCon.Misc
{
    public static class Utils
    {
        static readonly long SpeedtestTimeout = VgcApis.Models.Consts.Core.SpeedtestTimeout;

        #region copy from vgc

        #region strings
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
        public static JArray ExtractOutboundsFromConfig(string config)
        {
            var result = new JArray();
            JObject json = null;
            try
            {
                json = JObject.Parse(config);
                if (json == null)
                {
                    return result;
                }
            }
            catch { }
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

        public static string GetConfigRoot(bool isInbound, bool isV4)
        {
            return (isInbound ? "inbound" : "outbound")
                + (isV4 ? "s.0" : "");
        }

        public static JObject ParseImportRecursively(
          Func<List<string>, List<string>> fetcher,
          JObject config,
          int depth)
        {
            var empty = JObject.Parse(@"{}");

            if (depth <= 0)
            {
                return empty;
            }

            // var config = JObject.Parse(configString);

            var urls = Misc.Utils.ExtractImportUrlsFrom(config);
            var contents = fetcher(urls);

            if (contents.Count <= 0)
            {
                return config;
            }

            var configList =
                Misc.Utils.ExecuteInParallel<string, JObject>(
                    contents,
                    (content) =>
                    {
                        return ParseImportRecursively(
                            fetcher,
                            JObject.Parse(content),
                            depth - 1);
                    });

            var result = empty;
            foreach (var c in configList)
            {
                Misc.Utils.CombineConfigWithRoutingInFront(ref result, c);
            }
            Misc.Utils.CombineConfigWithRoutingInFront(ref result, config);

            return result;
        }

        public static List<string> ExtractImportUrlsFrom(JObject config)
        {
            List<string> urls = null;
            var empty = new List<string>();
            var import = Misc.Utils.GetKey(config, "v2raygcon.import");
            if (import != null && import is JObject)
            {
                urls = (import as JObject).Properties().Select(p => p.Name).ToList();
            }
            return urls ?? new List<string>();
        }

        public static string GenCmdArgFromConfig(string config)
        {
            // "-config=stdin: -format=json",
            var stdIn = VgcApis.Models.Consts.Core.StdIn;
            var confArg = VgcApis.Models.Consts.Core.ConfigArg;

            var jsonFormat = @"-format=json";

            var r = $"{jsonFormat} -{confArg}={stdIn}";
            try
            {
                var jobj = JObject.Parse(config);
                var confs = GetKey(jobj, "v2raygcon.configs")?.ToObject<Dictionary<string, string>>()?.Keys;
                if (confs == null)
                {
                    return r;
                }

                var hasStdIn = false;
                var args = string.Empty;
                foreach (var conf in confs)
                {
                    if (stdIn == conf)
                    {
                        hasStdIn = true;
                    }
                    args = $"{args} -{confArg}={conf}";
                }

                return hasStdIn ? $"{jsonFormat} {args}" : $"{jsonFormat} -{confArg}={stdIn} {args}";
            }
            catch { }
            return r;
        }

        public static Dictionary<string, string> GetEnvVarsFromConfig(JObject config)
        {
            var empty = new Dictionary<string, string>();

            var env = GetKey(config, "v2raygcon.env");
            if (env == null)
            {
                return empty;
            }

            try
            {
                return env.ToObject<Dictionary<string, string>>();
            }
            catch (JsonSerializationException)
            {
                return empty;
            }
        }

        public static string GetAliasFromConfig(JObject config)
        {
            var name = GetValue<string>(config, "v2raygcon.alias");
            return string.IsNullOrEmpty(name) ? I18N.Empty : name;
        }

        public static string GetSummaryFromConfig(JObject config)
        {
            var result = GetSummaryFromConfig(config, "outbound");

            if (string.IsNullOrEmpty(result))
            {
                return GetSummaryFromConfig(config, "outbounds.0");
            }

            return result;
        }

        public static string GetStreamSettingInfo(JObject config, string root)
        {
            var streamType = GetValue<string>(config, root + ".streamSettings.network")?.ToLower();
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

            var sec = GetValue<string>(config, root + ".streamSettings.security")?.ToLower();
            if (sec != null && sec.Equals(@"tls"))
            {
                result += $".{sec}";
            }
            return result;
        }

        public static string GetSummaryFromConfig(JObject config, string root)
        {
            var protocol = GetValue<string>(config, root + ".protocol")?.ToLower();
            if (protocol == null)
            {
                return string.Empty;
            }

            string ipKey = root;
            switch (protocol)
            {
                case "vmess":
                    ipKey += ".settings.vnext.0.address";
                    break;
                case "shadowsocks":
                    protocol = "ss";
                    ipKey += ".settings.servers.0.address";
                    break;
                case "socks":
                case "http":
                    ipKey += ".settings.servers.0.address";
                    break;
            }

            string ip = GetValue<string>(config, ipKey);
            string streamType = GetStreamSettingInfo(config, root);

            return protocol
                + (string.IsNullOrEmpty(streamType) ? "" : $".{streamType}")
                + (string.IsNullOrEmpty(ip) ? "" : @"@" + ip);
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

        public static bool SetValue<T>(JToken json, string path, T value)
        {
            var parts = ParsePathIntoParentAndKey(path);
            var r = json;

            var key = parts.Item2;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var parent = parts.Item1;
            if (!string.IsNullOrEmpty(parent))
            {
                var p = GetKey(json, parent);
                if (p == null || !(p is JObject))
                {
                    return false;
                }
                r = p as JObject;
            }

            r[key] = new JValue(value);
            return true;
        }

        public static bool TryExtractJObjectPart(
            JObject source,
            string path,
            out JObject result)
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

            var parent = string.IsNullOrEmpty(parentPath) ?
                result : GetKey(result, parentPath);

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

            var node = string.IsNullOrEmpty(parent) ?
                json : GetKey(json, parent);

            if (node == null || !(node is JObject))
            {
                throw new KeyNotFoundException();
            }

            (node as JObject).Property(key)?.Remove();
        }

        static void ConcatJson(ref JObject body, JObject mixin)
        {
            body.Merge(mixin, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Concat,
                MergeNullValueHandling = MergeNullValueHandling.Ignore,
            });
        }

        public static void UnionJson(ref JObject body, JObject mixin)
        {
            body.Merge(mixin, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union,
                MergeNullValueHandling = MergeNullValueHandling.Ignore,
            });

        }

        public static void CombineConfigWithRoutingInTheEnd(ref JObject body, JObject mixin)
        {
            List<string> keys = new List<string>
            {
                "inbounds",
                "outbounds",
                "inboundDetour",
                "outboundDetour",
            };
            CombineConfigWorker(ref body, mixin, keys);
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

        static void CombineConfigWorker(
            ref JObject body,
            JObject mixin,
            IEnumerable<string> keys)
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
                    ConcatJson(ref backup, nodeMixin);
                    RemoveKeyFromJObject(mixin, key);
                    ConcatJson(ref body, nodeMixin);
                }

                if (nodeBody != null)
                {
                    UnionJson(ref body, nodeBody);
                }
            }

            MergeJson(ref body, mixin);

            // restore mixin
            ConcatJson(ref mixin, backup);
        }

        public static JObject ImportItemList2JObject(
            List<Models.Datas.ImportItem> items,
            bool isIncludeSpeedTest,
            bool isIncludeActivate,
            bool isIncludePackage)
        {
            var result = CreateJObject(@"v2raygcon.import");
            foreach (var item in items)
            {
                var url = item.url;
                if (string.IsNullOrEmpty(url))
                {
                    continue;
                }
                if ((isIncludeSpeedTest && item.isUseOnSpeedTest)
                    || (isIncludeActivate && item.isUseOnActivate)
                    || (isIncludePackage && item.isUseOnPackage))
                {
                    result["v2raygcon"]["import"][url] = item.alias ?? string.Empty;
                }
            }
            return result;
        }

        public static void MergeJson(ref JObject body, JObject mixin)
        {
            body.Merge(mixin, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge,
                MergeNullValueHandling = MergeNullValueHandling.Merge
            });
        }

        /// <summary>
        /// Return null if not found!
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetProtocolFromConfig(JObject config)
        {
            var keys = new string[]
            {
                "outbound.protocol",
                "outbounds.0.protocol",
            };

            foreach (var key in keys)
            {
                var value = GetValue<string>(config, key);
                if (!string.IsNullOrEmpty(value))
                {
                    return value.ToLower();
                }
            }
            return null;
        }

        /// <summary>
        /// return null if path is null or path not exists.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JToken GetKey(JToken json, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }


            var curPos = json;
            var keys = path.Split('.');

            int depth;
            for (depth = 0; depth < keys.Length; depth++)
            {
                if (curPos == null || !curPos.HasValues)
                {
                    break;
                }

                try
                {
                    if (int.TryParse(keys[depth], out int n))
                    {
                        curPos = curPos[n];
                    }
                    else
                    {
                        curPos = curPos[keys[depth]];
                    }
                }
                catch
                {
                    return null;
                }
            }

            return depth < keys.Length ? null : curPos;

        }

        public static T GetValue<T>(JToken json, string prefix, string key)
        {
            return GetValue<T>(json, $"{prefix}.{key}");
        }

        /// <summary>
        /// return null if not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetValue<T>(JToken json, string path)
        {
            var key = GetKey(json, path);

            var def = default(T) == null && typeof(T) == typeof(string) ?
                (T)(object)string.Empty :
                default;

            if (key == null)
            {
                return def;
            }
            try
            {
                return key.Value<T>();
            }
            catch { }
            return def;
        }

        public static Func<string, string, string> GetStringByPrefixAndKeyHelper(JObject json)
        {
            var o = json;
            return (prefix, key) =>
            {
                return GetValue<string>(o, $"{prefix}.{key}");
            };
        }

        public static Func<string, string> GetStringByKeyHelper(JObject json)
        {
            var o = json;
            return (key) =>
            {
                return GetValue<string>(o, $"{key}");
            };
        }

        public static string GetAddr(JObject json, string prefix, string keyIP, string keyPort)
        {
            var ip = GetValue<String>(json, prefix, keyIP) ?? VgcApis.Models.Consts.Webs.LoopBackIP;
            var port = GetValue<string>(json, prefix, keyPort);
            return string.Join(":", ip, port);
        }

        #endregion

        #region convert
        public static string Config2String(JObject config)
        {
            return config.ToString(Formatting.None);
        }

        public static string Config2Base64String(JObject config)
        {
            return Base64Encode(config.ToString(Formatting.None));
        }

        public static List<string> Str2ListStr(string serial)
        {
            var list = new List<string> { };
            var items = serial.Split(',');
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    list.Add(item);
                }

            }
            return list;
        }

        /// <summary>
        /// http is equal to https
        /// </summary>
        /// <param name="text"></param>
        /// <param name="linkType"></param>
        /// <returns></returns>
        public static List<string> ExtractLinks(
            string text,
            VgcApis.Models.Datas.Enums.LinkTypes linkType)
        {
            var links = new List<string>();
            try
            {
                string pattern = GenPattern(linkType);
                var matches = Regex.Matches("\n" + text, pattern, RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    links.Add(match.Value.Substring(1));
                }
            }
            catch { }
            return links;
        }

        public static Models.Datas.Vmess VmessLink2Vmess(string link)
        {
            try
            {
                string plainText = Base64Decode(GetLinkBody(link));
                var vmess = JsonConvert.DeserializeObject<Models.Datas.Vmess>(plainText);
                if (!string.IsNullOrEmpty(vmess.add)
                    && !string.IsNullOrEmpty(vmess.port)
                    && !string.IsNullOrEmpty(vmess.id)
                    && VgcApis.Misc.Utils.IsValidPort(vmess.port)
                    && new Guid(vmess.id) != new Guid())
                {
                    return vmess;
                }
            }
            catch { }
            return null;
        }

        public static Models.Datas.Shadowsocks SsLink2Ss(string ssLink)
        {
            string b64 = GetLinkBody(ssLink);

            try
            {
                var ss = new Models.Datas.Shadowsocks();
                var plainText = Base64Decode(b64);
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

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64PadRight(string base64)
        {
            var str = base64.Replace("\r", "").Replace("\n", "");
            var len = str.Length;
            return str.PadRight(len + (4 - len % 4) % 4, '=');
        }

        public static string Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return string.Empty;
            }
            var padded = Base64PadRight(base64EncodedData);
            var base64EncodedBytes = Convert.FromBase64String(padded);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        #endregion

        #region net
        /// <summary>
        /// List( success ? ( vmess://... , mark ) : ( "", [alias] url ) )
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <param name="proxyPort"></param>
        /// <returns></returns>
        public static List<string[]> FetchLinksFromSubcriptions(
            List<Models.Datas.SubscriptionItem> subscriptions,
            int proxyPort)
        {
            string[] worker(Models.Datas.SubscriptionItem subItem)
            {
                var url = subItem.url;
                var mark = subItem.isSetMark ? subItem.alias : null;

                var subsString = Fetch(
                    url, proxyPort, VgcApis.Models.Consts.Import.ParseImportTimeout);

                if (string.IsNullOrEmpty(subsString))
                {
                    return new string[] {
                        string.Empty,
                        $"[{subItem.alias}] {url}",
                    };
                }

                var links = new List<string>();
                foreach (var substr in VgcApis.Misc.Utils.ExtractBase64Strings(subsString))
                {
                    try
                    {
                        var text = Base64Decode(substr);
                        links.Add(text);
                    }
                    catch { }
                }

                return new string[] { string.Join("\n", links), mark };
            }

            return Misc.Utils.ExecuteInParallel(subscriptions, worker);
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

            if (string.IsNullOrEmpty(baseUrl)
                || string.IsNullOrEmpty(href)
                || !href.StartsWith("/"))
            {
                return href;
            }

            return baseUrl + href;
        }

        public static List<string> FindAllHrefs(string text)
        {
            var empty = new List<string>();

            if (string.IsNullOrEmpty(text))
            {
                return empty;
            }

            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(text);

                var result = doc.DocumentNode.SelectNodes("//a")
                    ?.Select(p => p.GetAttributeValue("href", ""))
                    ?.Where(s => !string.IsNullOrEmpty(s))
                    ?.ToList();

                return result ?? empty;
            }
            catch { }
            return empty;
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

        public static long VisitWebPageSpeedTest(
            string url,
            int port,
            int expectedSizeInKiB,
            int timeout)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("URL must not null!");
            }

            var maxTimeout = timeout > 0 ? timeout : VgcApis.Models.Consts.Intervals.SpeedTestTimeout;

            WebClient wc = new WebClient
            {
                Encoding = Encoding.UTF8,
            };
            wc.Headers.Add(VgcApis.Models.Consts.Webs.UserAgent);

            if (port > 0 && port < 65536)
            {
                wc.Proxy = new WebProxy(VgcApis.Models.Consts.Webs.LoopBackIP, port);
            }

            Stopwatch sw = new Stopwatch();
            AutoResetEvent dlCompleted = new AutoResetEvent(false);
            long totalReceived = 0;
            var expectedBytes = expectedSizeInKiB * 1024;

            if (expectedSizeInKiB >= 0)
            {
                wc.DownloadProgressChanged += (s, a) =>
                {
                    Interlocked.Add(ref totalReceived, a.BytesReceived);
                    if (totalReceived > expectedBytes)
                    {
                        sw.Stop();
                        wc.CancelAsync();
                    }
                };
            }

            wc.DownloadStringCompleted += (s, a) =>
            {
                sw.Stop();
                dlCompleted.Set();
                wc.Dispose();
            };

            try
            {
                var patchedUrl = VgcApis.Misc.Utils.IsHttpLink(url) ? url : VgcApis.Misc.Utils.RelativePath2FullPath(url);
                sw.Start();
                wc.DownloadStringAsync(new Uri(patchedUrl));
                // 收到信号为True
                if (!dlCompleted.WaitOne(maxTimeout))
                {
                    wc.CancelAsync();
                    return SpeedtestTimeout;
                }
            }
            catch
            {
                // network operation always buggy.
                wc.CancelAsync();
                return SpeedtestTimeout;
            }

            return totalReceived <= expectedBytes ? SpeedtestTimeout : sw.ElapsedMilliseconds;
        }

        static bool DownloadFileWorker(string url, string filename, int proxyPort, int timeout)
        {
            var success = true;
            if (timeout <= 0)
            {
                timeout = VgcApis.Models.Consts.Intervals.FetchDefaultTimeout;
            }

            WebClient wc = new WebClient();
            wc.Headers.Add(VgcApis.Models.Consts.Webs.UserAgent);

            if (proxyPort > 0 && proxyPort < 65536)
            {
                wc.Proxy = new WebProxy(VgcApis.Models.Consts.Webs.LoopBackIP, proxyPort);
            }

            AutoResetEvent dlCompleted = new AutoResetEvent(false);
            wc.DownloadFileCompleted += (s, a) =>
            {
                if (a.Cancelled)
                {
                    success = false;
                }
                dlCompleted.Set();
            };

            try
            {
                if (!VgcApis.Misc.Utils.IsHttpLink(url))
                {
                    url = VgcApis.Misc.Utils.RelativePath2FullPath(url);
                }

                wc.DownloadFileAsync(new Uri(url), filename);

                // 收到信号为True
                if (!dlCompleted.WaitOne(timeout))
                {
                    success = false;
                    wc.CancelAsync();
                }
            }
            catch
            {
                success = false;
            }
            finally
            {
                wc.Dispose();
            }

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
        static string FetchWorker(string url, int proxyPort, int timeout)
        {
            var html = string.Empty;

            if (timeout <= 0)
            {
                timeout = VgcApis.Models.Consts.Intervals.FetchDefaultTimeout;
            }

            WebClient wc = new WebClient
            {
                Encoding = Encoding.UTF8,
            };

            wc.Headers.Add(VgcApis.Models.Consts.Webs.UserAgent);

            if (proxyPort > 0 && proxyPort < 65536)
            {
                wc.Proxy = new WebProxy(VgcApis.Models.Consts.Webs.LoopBackIP, proxyPort);
            }

            AutoResetEvent dlCompleted = new AutoResetEvent(false);
            wc.DownloadStringCompleted += (s, a) =>
            {
                try
                {
                    if (!a.Cancelled)
                    {
                        html = a.Result;
                    }
                }
                catch { }

                dlCompleted.Set();
                wc.Dispose();
            };

            try
            {
                if (!VgcApis.Misc.Utils.IsHttpLink(url))
                {
                    url = VgcApis.Misc.Utils.RelativePath2FullPath(url);
                }

                wc.DownloadStringAsync(new Uri(url));
                // 收到信号为True
                if (!dlCompleted.WaitOne(timeout))
                {
                    wc.CancelAsync();
                }
            }
            catch
            {
                // network operation always buggy.
                wc.CancelAsync();
            }

            return html;
        }

        public static string Fetch(string url, int proxyPort, int timeout) =>
            FetchWorker(url, proxyPort, timeout);

        public static string Fetch(string url) =>
            Fetch(url, -1, -1);

        public static string Fetch(string url, int timeout) =>
            Fetch(url, -1, timeout);

        public static string GetLatestVgcVersion()
        {
            string html = Fetch(StrConst.UrlLatestVGC);

            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            string p = StrConst.PatternLatestVGC;
            var match = Regex.Match(html, p, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        public static List<string> GetOnlineV2RayCoreVersionList(int proxyPort)
        {
            List<string> versions = new List<string> { };
            var url = StrConst.V2rayCoreReleasePageUrl;

            string html = Fetch(url, proxyPort, -1);
            if (string.IsNullOrEmpty(html))
            {
                return versions;
            }

            string pattern = StrConst.PatternDownloadLink;
            var matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase);
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
        #endregion

        #region files

        public static string GetSha256SumFromFile(string file)
        {
            // http://peterkellner.net/2010/11/24/efficiently-generating-sha256-checksum-for-files-using-csharp/
            try
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    var sha = new SHA256Managed();
                    byte[] checksum = sha.ComputeHash(stream);
                    return BitConverter
                        .ToString(checksum)
                        .Replace("-", String.Empty)
                        .ToLower();
                }
            }
            catch { }
            return string.Empty;
        }


        public static string GetSysAppDataFolder()
        {
            var appData = System.Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData);
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

        #region speed test helper

        public static string InboundTypeNumberToName(int typeNumber)
        {
            var table = Models.Datas.Table.customInbTypeNames;
            return table[Clamp(typeNumber, 0, table.Length)];
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
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString ?? string.Empty));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        static readonly object genRandomNumberLocker = new object();
        public static string RandomHex(int length)
        {
            //  https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
            if (length <= 0)
            {
                return string.Empty;
            }

            Random random = new Random();
            const string chars = "0123456789abcdef";
            int charLen = chars.Length;

            int rndIndex;
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < length; i++)
            {
                lock (genRandomNumberLocker)
                {
                    rndIndex = random.Next(charLen);
                }
                sb.Append(chars[rndIndex]);
            }
            return sb.ToString();
        }

        public static int Clamp(int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max - 1), min);
        }

        public static int GetIndexIgnoreCase(Dictionary<int, string> dict, string value)
        {
            foreach (var data in dict)
            {
                if (!string.IsNullOrEmpty(data.Value)
                    && data.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    return data.Key;
                }
            }
            return -1;
        }

        static string GenLinkPrefix(
            VgcApis.Models.Datas.Enums.LinkTypes linkType) =>
            $"{linkType.ToString()}";

        public static string GenPattern(
            VgcApis.Models.Datas.Enums.LinkTypes linkType)
        {
            string pattern;
            switch (linkType)
            {
                case VgcApis.Models.Datas.Enums.LinkTypes.ss:
                    pattern = GenLinkPrefix(linkType) + "://" +
                        VgcApis.Models.Consts.Patterns.SsShareLinkContent;
                    break;
                case VgcApis.Models.Datas.Enums.LinkTypes.vmess:
                case VgcApis.Models.Datas.Enums.LinkTypes.v2cfg:
                case VgcApis.Models.Datas.Enums.LinkTypes.v:
                    pattern = GenLinkPrefix(linkType) + "://" +
                        VgcApis.Models.Consts.Patterns.Base64NonStandard;
                    break;
                case VgcApis.Models.Datas.Enums.LinkTypes.http:
                case VgcApis.Models.Datas.Enums.LinkTypes.https:
                    pattern = VgcApis.Models.Consts.Patterns.HttpUrl;
                    break;
                default:
                    throw new NotSupportedException(
                        $"Not supported link type {linkType.ToString()}:// ...");
            }

            return VgcApis.Models.Consts.Patterns.NonAlphabets + pattern;
        }

        public static string AddLinkPrefix(
            string b64Content,
            VgcApis.Models.Datas.Enums.LinkTypes linkType)
        {
            return GenLinkPrefix(linkType) + "://" + b64Content;
        }

        public static string GetLinkBody(string link)
        {
            var needle = @"://";
            var index = link.IndexOf(needle);

            if (index < 0)
            {
                throw new ArgumentException(
                    $"Not a valid link ${link}");
            }

            return link.Substring(index + needle.Length);
        }

        public static void ZipFileDecompress(string zipFile, string outFolder)
        {
            // let downloader handle exception
            using (ZipFile zip = ZipFile.Read(zipFile))
            {
                zip.ExtractAll(
                    outFolder,
                    ExtractExistingFileAction.OverwriteSilently);
            }
        }
        #endregion

        #region UI related
        public static void CopyToClipboardAndPrompt(string content) =>
            MessageBox.Show(
                CopyToClipboard(content) ?
                I18N.CopySuccess :
                I18N.CopyFail);

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

        public static void SupportProtocolTLS12()
        {
            try
            {
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12
                    | SecurityProtocolType.Tls11
                    | SecurityProtocolType.Tls;
            }
            catch (NotSupportedException)
            {
                MessageBox.Show(I18N.SysNotSupportTLS12);
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

        public static void ChainActionHelperAsync(int countdown, Action<int, Action> worker, Action done = null)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                ChainActionHelperWorker(countdown, worker, done)();
            });
        }

        // wrapper
        public static void ChainActionHelper(int countdown, Action<int, Action> worker, Action done = null)
        {
            ChainActionHelperWorker(countdown, worker, done)();
        }

        static Action ChainActionHelperWorker(int countdown, Action<int, Action> worker, Action done = null)
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
            Action<TParam> worker) =>
            ExecuteInParallel(param,
                (p) =>
                {
                    worker(p);
                    // ExecuteInParallel require a return value
                    return "nothing";
                });

        public static List<TResult> ExecuteInParallel<TParam, TResult>(
            IEnumerable<TParam> param,
            Func<TParam, TResult> worker)
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

        /// <summary>
        /// UseShellExecute = false,
        /// RedirectStandardOutput = true,
        /// CreateNoWindow = true,
        /// </summary>
        /// <param name="exeFileName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Process CreateHeadlessProcess(string exeFileName, string args)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = exeFileName,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            return new Process
            {
                StartInfo = startInfo,
            };
        }

        public static string GetOutputFromExecutable(
            string exeFileName,
            string args,
            int timeout)
        {
            var p = CreateHeadlessProcess(exeFileName, args);
            try
            {
                p.Start();
                if (!p.WaitForExit(timeout))
                {
                    p.Kill();
                }
                return p.StandardOutput.ReadToEnd() ?? string.Empty;
            }
            catch { }
            return string.Empty;
        }

        public static void KillProcessAndChildrens(int pid)
        {
            ManagementObjectSearcher processSearcher = new ManagementObjectSearcher
              ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection processCollection = processSearcher.Get();

            // We must kill child processes first!
            if (processCollection != null)
            {
                foreach (ManagementObject mo in processCollection)
                {
                    KillProcessAndChildrens(Convert.ToInt32(mo["ProcessID"])); //kill child processes(also kills childrens of childrens etc.)
                }
            }

            // Then kill parents.
            try
            {
                Process proc = Process.GetProcessById(pid);
                if (!proc.HasExited)
                {
                    proc.Kill();
                    proc.WaitForExit(1000);
                }
            }
            catch
            {
                // Process already exited.
            }
        }
        #endregion

        #region for Testing
        public static string[] TestingGetResourceConfigJson()
        {
            return new string[]
            {
                StrConst.config_example,
                StrConst.config_min,
                StrConst.config_tpl,
                StrConst.config_pkg,
            };
        }
        #endregion

        #endregion
    }
}
