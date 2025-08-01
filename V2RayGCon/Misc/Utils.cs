﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using V2RayGCon.Resources.Resx;
using VgcApis.Models.Consts;
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
                foreach (var item in json["stat"])
                {
                    if (!(item is JObject o))
                    {
                        continue;
                    }

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
                var ver = VgcApis.Misc.Utils.TrimVersionString(rawVer);
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
            var protocol = $"{linkType}://";
            switch (linkType)
            {
                case Enums.LinkTypes.vless:
                case Enums.LinkTypes.trojan:
                case Enums.LinkTypes.ss:
                case Enums.LinkTypes.socks:
                    return LinksTextExtractor(text, protocol, Patterns.NonStandardUriBodyChars);
                case Enums.LinkTypes.vmess:
                case Enums.LinkTypes.v2cfg:
                    return LinksTextExtractor(text, protocol, Patterns.Base64Chars);
                case Enums.LinkTypes.http:
                case Enums.LinkTypes.https:
                    return LinksRegexExtractor(text, Patterns.HttpUrl);
                default:
                    break;
            }
            return new List<string>();
        }

        public static List<string> LinksRegexExtractor(string text, string pattern)
        {
            var r = new List<string>();
            try
            {
                var matches = Regex.Matches("\n" + text, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var link = match.Value.Substring(1);
                    link = VgcApis.Misc.Utils.UnescapeUnicode(link);
                    link = VgcApis.Misc.Utils.DecodeAmpersand(link);
                    r.Add(link);
                }
            }
            catch { }
            return r;
        }

        public static List<string> LinksTextExtractor(
            string text,
            string protocol,
            string bodyChars
        )
        {
            var links = new List<string>();
            if (string.IsNullOrEmpty(protocol) || string.IsNullOrEmpty(bodyChars))
            {
                return links;
            }

            void addToLinks(string link)
            {
                link = VgcApis.Misc.Utils.UnescapeUnicode(link);
                link = VgcApis.Misc.Utils.DecodeAmpersand(link);
                while (link.Length > protocol.Length)
                {
                    links.Add(link);
                    var pos = link.IndexOf(protocol, protocol.Length);
                    if (pos < 0)
                    {
                        break;
                    }
                    link = link.Substring(pos);
                }
            }

            var idx = 0;
            var sb = new StringBuilder();
            foreach (var c in text)
            {
                if (idx < protocol.Length)
                {
                    idx = c == protocol[idx] ? idx + 1 : (c == protocol[0] ? 1 : 0);
                    continue;
                }

                if (bodyChars.IndexOf(c) >= 0)
                {
                    sb.Append(c);
                    continue;
                }

                if (sb.Length > 0)
                {
                    addToLinks($"{protocol}{sb}");
                    sb.Append(c);
                    idx = VgcApis.Misc.Utils.MatchTailIndex(protocol, sb);
                    sb.Clear();
                    continue;
                }

                idx = c == protocol[0] ? 1 : 0;
            }

            if (sb.Length > 0)
            {
                addToLinks($"{protocol}{sb}");
            }
            return links;
        }

        public static Vmess VmessLink2Vmess(string link)
        {
            try
            {
                var body = VgcApis.Misc.Utils.GetLinkBody(link);
                string plainText = VgcApis.Misc.Utils.Base64DecodeToString(body);
                if (string.IsNullOrEmpty(plainText))
                {
                    return null;
                }
                var vmess = JsonConvert.DeserializeObject<Vmess>(plainText);
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

            string text = VgcApis.Misc.Utils.FetchWorker(
                isSocks5,
                apiUrl,
                VgcApis.Models.Consts.Webs.LoopBackIP,
                proxyPort,
                VgcApis.Models.Consts.Intervals.DefaultCheckNewCoreVersionsTimeout,
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

                var subsString = VgcApis.Misc.Utils.FetchWorker(
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
                    return new string[] { string.Empty, $"[{subItem.alias}] {url}" };
                }

                var links = new List<string> { subsString };

                // average length of vless://... is 200
                var b64s = VgcApis.Misc.Utils.ExtractBase64Strings(subsString, 200 * 4 / 3);
                foreach (var b64 in b64s)
                {
                    if (b64.StartsWith("//"))
                    {
                        continue;
                    }

                    try
                    {
                        var text = VgcApis.Misc.Utils.Base64DecodeToString(b64);
                        links.Add(text);
                    }
                    catch { }
                }

                return new string[] { string.Join("\n", links), mark };
            }

            return VgcApis.Misc.Utils.ExecuteInParallel(subscriptions, worker);
        }

        #endregion

        #region files

        private static void WriteToFileAtOnce(string path, string userSettings)
        {
            // https://stackoverflow.com/questions/25366534/file-writealltext-not-flushing-data-to-disk
            var bufferSize = VgcApis.Models.Consts.Libs.FilestreamBufferSize;
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

        public static string GetSysAppDataFolder()
        {
            var appData = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData
            );
            var appName = VgcApis.Misc.Utils.GetAppName();
            return Path.Combine(appData, appName);
        }

        public static void CreateSysAppDataFolder()
        {
            var path = GetSysAppDataFolder();

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch { }
            }
        }

        public static void DeleteAppDataFolder()
        {
            Directory.Delete(GetSysAppDataFolder(), recursive: true);
        }
        #endregion

        #region Miscellaneous



        public static string GetAssemblyVersion()
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            return version.ToString();
        }

        public static bool VerifyZipFile(string zipFile)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            {
                var c = archive.Entries.Count;
                return c > 0;
            }
        }

        public static void DecompressZipFile(string zipFile, string outFolder)
        {
            // let downloader handle exception
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            {
                ExtractToDirectory(archive, outFolder, true);
            }
        }

        // credit: https://stackoverflow.com/questions/14795197/forcefully-replacing-existing-files-during-extracting-file-using-system-io-compr
        static void ExtractToDirectory(
            ZipArchive archive,
            string destinationDirectoryName,
            bool overwrite
        )
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }

            DirectoryInfo di = Directory.CreateDirectory(destinationDirectoryName);
            string destinationDirectoryFullPath = di.FullName;

            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.GetFullPath(
                    Path.Combine(destinationDirectoryFullPath, file.FullName)
                );

                if (
                    !completeFileName.StartsWith(
                        destinationDirectoryFullPath,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    throw new IOException(
                        "Trying to extract file outside of destination directory. See this link for more info: https://snyk.io/research/zip-slip-vulnerability"
                    );
                }

                if (file.Name == "")
                { // Assuming Empty for Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                file.ExtractToFile(completeFileName, true);
            }
        }
        #endregion

        #region UI related


        public static void CopyToClipboardAndPrompt(string content)
        {
            var ok = VgcApis.Misc.Utils.CopyToClipboard(content);
            VgcApis.Misc.UI.MsgBox(ok ? I18N.CopySuccess : I18N.CopyFail);
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

        #endregion
        #region for Testing
        public static string[] TestingGetResourceConfigJson()
        {
            return new string[] { StrConst.config_min, StrConst.config_tpl, StrConst.config_pkg };
        }
        #endregion
    }
}
