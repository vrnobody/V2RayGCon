using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScintillaNET;
using VgcApis.Models.Consts;
using VgcApis.Resources.Langs;

namespace VgcApis.Misc
{
    public static class Utils
    {
        #region collections
        public static bool TryGetDictValue<T>(
            Dictionary<string[], T> dict,
            string[] key,
            out T value
        )
        {
            value = default;

            foreach (var kv in dict)
            {
                var dkey = kv.Key;
                if (dkey.Length != key.Length)
                {
                    continue;
                }

                bool isEqual = true;
                for (var i = 0; i < key.Length; i++)
                {
                    if (key[i] != dkey[i])
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual)
                {
                    value = kv.Value;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region editor
        public static void ClearControlKeys(Scintilla scintilla, List<Keys> extra)
        {
            // key binding
            var keys = new List<Keys>() { Keys.F, Keys.G, Keys.H, Keys.K, Keys.N, Keys.P, Keys.S, };

            if (extra != null && extra.Count() > 0)
            {
                keys.AddRange(extra);
            }

            foreach (var key in keys)
            {
                // clear default keyboard shortcut
                scintilla.ClearCmdKey(Keys.Control | key);
                // assign null action
                scintilla.AssignCmdKey(Keys.Control | key, Command.Null);
            }
        }

        public static void BindEditorDragDropEvent(Scintilla editor)
        {
            editor.AllowDrop = true;

            editor.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };

            editor.DragDrop += (s, a) =>
            {
                var data = a.Data;
                if (data.GetDataPresent(DataFormats.FileDrop))
                {
                    var filenames = a.Data.GetData(DataFormats.FileDrop) as string[];
                    HandleEditorFileDropEvent(editor, filenames);
                }
            };
        }

        public static void HandleEditorFileDropEvent(Scintilla editor, string[] filenames)
        {
            if (filenames == null)
            {
                return;
            }

            foreach (var filename in filenames)
            {
                if (!File.Exists(filename))
                {
                    continue;
                }

                string content;
                string scriptName;

                try
                {
                    content = File.ReadAllText(filename);
                    scriptName = Path.GetFileName(filename);
                }
                catch
                {
                    continue;
                }

                var name = AutoEllipsis(scriptName, 40);
                if (string.IsNullOrWhiteSpace(content))
                {
                    var err = string.Format(I18N.FileIsEmpty, name);
                    UI.MsgBox(err);
                    continue;
                }

                var msg = string.Format(I18N.ConfirmLoadFileContent, name);
                if (string.IsNullOrEmpty(editor.Text) || UI.Confirm(msg))
                {
                    editor.Text = content;
                }
            }
        }

        public static string GetWordFromCurPos(Scintilla editor)
        {
            var line = editor.Lines[editor.CurrentLine];
            var text = line.Text;

            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            var start = editor.CurrentPosition - line.Position - 1;
            start = Clamp(start, 0, text.Length);
            var end = start;
            for (; start >= 0; start--)
            {
                var c = text[start];
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    break;
                }
            }

            for (; end < text.Length; end++)
            {
                var c = text[end];
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    break;
                }
            }

            var len = end - start - 1;
            if (len < 1)
            {
                return "";
            }

            return text.Substring(start + 1, len);
        }
        #endregion

        #region system
        /// <summary>
        /// UseShellExecute = false,
        /// RedirectStandardOutput = true,
        /// CreateNoWindow = true,
        /// </summary>
        /// <param name="exe"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Process CreateHeadlessProcess(
            string exe,
            string args,
            string workingDir,
            Encoding encoding
        )
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            if (!string.IsNullOrEmpty(workingDir))
            {
                startInfo.WorkingDirectory = workingDir;
            }

            if (encoding != null)
            {
                startInfo.StandardOutputEncoding = encoding;
            }

            return new Process { StartInfo = startInfo, };
        }

        public static string ExecuteAndGetStdOut(
            string exeFileName,
            string args,
            int timeout,
            Encoding encoding
        )
        {
            var r = string.Empty;
            var p = CreateHeadlessProcess(exeFileName, args, null, encoding);
            try
            {
                p.Start();
                if (!p.WaitForExit(timeout))
                {
                    p.Kill();
                }
                r = p.StandardOutput.ReadToEnd() ?? string.Empty;
            }
            catch { }
            p?.Dispose();
            return r;
        }

        public static bool IsAdmin()
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return isElevated;
        }

        public static string GetCurCallStack()
        {
            var s = new List<string>();

            StackTrace stack = new StackTrace();
            foreach (var frame in stack.GetFrames())
            {
                var method = frame.GetMethod();
                var mn = GetFriendlyMethodDeclareInfo(method as MethodInfo);
                s.Add($" -> {mn}");
            }

            return string.Join("\n", s);
        }

        public static bool TryParseKeyMesssage(
            string keyName,
            bool hasAlt,
            bool hasCtrl,
            bool hasShift,
            out uint modifier,
            out uint keyCode
        )
        {
            keyCode = 0;
            modifier = 0;

            if (!(hasCtrl || hasShift || hasAlt) || !Enum.TryParse(keyName, out Keys key))
            {
                return false;
            }

            keyCode = (uint)key;

            uint ctrl = hasCtrl ? (uint)Models.Datas.Enums.ModifierKeys.Control : 0;
            uint alt = hasAlt ? (uint)Models.Datas.Enums.ModifierKeys.Alt : 0;
            uint shift = hasShift ? (uint)Models.Datas.Enums.ModifierKeys.Shift : 0;

            modifier = ctrl | alt | shift;

            return true;
        }

        #endregion

        #region List

        public static List<T> Shuffle<T>(IEnumerable<T> source)
        {
            var list = source.ToList();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Libs.Infr.PseudoRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
        #endregion

        #region files
        public static Mutex TryLockFile(string fullPath)
        {
            // https://stackoverflow.com/questions/19147/what-is-the-correct-way-to-create-a-single-instance-application
            var md5 = Md5Hex(fullPath);
            var name = "{84d287ae-c0b0-4c1a-9ecc-d98c26577c02}" + md5;
            var mutex = new Mutex(true, name);
            if (mutex.WaitOne(0))
            {
                return mutex;
            }
            mutex.Dispose();
            return null;
        }

        public static string ReplaceFileExtention(string file, string dotExt)
        {
            try
            {
                var ext = Path.GetExtension(file);
                if (!string.IsNullOrEmpty(ext))
                {
                    return file.Substring(0, file.Length - ext.Length) + dotExt;
                }
            }
            catch { }
            return file + dotExt;
        }

        public static void ClearFile(string path)
        {
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
            }
        }

        public static string GetImageResolution(string filename)
        {
            try
            {
                var img = Image.FromFile(filename);
                return $"{img.Width}x{img.Height}";
            }
            catch { }
            return null;
        }

        public static string PickRandomLine(string filename)
        {
            string url = string.Empty;

            if (!File.Exists(filename))
            {
                return url;
            }

            int numberSeen = 0;
            var lines = File.ReadLines(filename);
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line) && Libs.Infr.PseudoRandom.Next(++numberSeen) == 0)
                {
                    url = line;
                }
            }

            return url.Replace("\r", "").Replace("\n", "");
        }
        #endregion

        #region string

        public static List<string> SplitAndKeep(string s, IEnumerable<string> delimiters)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new List<string>();
            }
            var unique = delimiters.Distinct().ToList();
            return SplitAndKeepCore(s, 0, s.Length, unique);
        }

        static List<string> SplitAndKeepCore(
            string s,
            int start,
            int end,
            IEnumerable<string> delimiters
        )
        {
            var r = new List<string>();
            var delim = delimiters.FirstOrDefault();
            if (start >= end)
            {
                return r;
            }
            if (string.IsNullOrEmpty(delim))
            {
                r.Add(s.Substring(start, end - start));
                return r;
            }
            var otherDelims = delimiters.Skip(1).ToList();
            int iFirst = start;
            do
            {
                int iLast = s.IndexOf(delim, iFirst, end - iFirst);
                if (iLast < 0)
                {
                    var remains = SplitAndKeepCore(s, iFirst, end, otherDelims);
                    r.AddRange(remains);
                    break;
                }
                var left = SplitAndKeepCore(s, iFirst, iLast, otherDelims);
                r.AddRange(left);
                r.Add(delim);
                iFirst = iLast + delim.Length;
            } while (iFirst < end);
            return r;
        }

        static ArrayPool<byte> bufferPool = ArrayPool<byte>.Shared;

        public static byte[] RentBuffer()
        {
            return bufferPool.Rent(Models.Consts.Libs.DefaultBufferSize);
        }

        public static byte[] RentBuffer(int size)
        {
            return bufferPool.Rent(size);
        }

        public static void ReturnBuffer(byte[] buff)
        {
            bufferPool.Return(buff);
        }

        public static string UriEncode(string content)
        {
            try
            {
                return Uri.EscapeDataString(content);
            }
            catch { }
            return null;
        }

        public static string UriDecode(string content)
        {
            try
            {
                return Uri.UnescapeDataString(content);
            }
            catch { }
            return null;
        }

        static string appTag = "";

        public static void SetAppTag(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                appTag = $"[{tag}]";
            }
        }

        public static string PrependTag(string content)
        {
            if (!string.IsNullOrEmpty(appTag))
            {
                var c = content ?? "";
                return $"{appTag} {c}";
            }
            return content;
        }

        public static string GetAppTagFirstChar()
        {
            if (appTag.Length > 1)
            {
                return appTag.Substring(1, 1);
            }
            return null;
        }

        public static int CountLeadingSpaces(string str)
        {
            return str?.TakeWhile(c => c == ' ').Count() ?? 0;
        }

        public static Dictionary<string, int> GetConfigTags(List<string> lines)
        {
            var patTag = @"^ *""?tag""?: *""?([\w\-_]+)""?";
            var patYaml = @"^([a-zA-Z][\w\-_]*):";

            var patJsonInit = @"^ +""([a-zA-Z][\w\-_]*)"":";
            var patJsonTpl = @"^{0}""([a-zA-Z][\w\-_]*)"":";
            string patJson = null;

            var r = new Dictionary<string, int>();
            for (int i = 0; i < lines.Count; i++)
            {
                try
                {
                    var line = lines[i];
                    var gs = Regex.Match(line, patTag).Groups;
                    if (gs.Count > 1)
                    {
                        r[$"tag: {gs[1].Value}"] = i;
                        continue;
                    }
                    gs = Regex.Match(line, patYaml).Groups;
                    if (gs.Count > 1)
                    {
                        r[gs[1].Value] = i;
                        continue;
                    }
                    gs = Regex.Match(line, patJson ?? patJsonInit).Groups;
                    if (gs.Count > 1)
                    {
                        r[gs[1].Value] = i;
                        if (patJson == null)
                        {
                            var c = CountLeadingSpaces(line);
                            patJson = string.Format(patJsonTpl, new string(' ', c));
                        }
                        continue;
                    }
                }
                catch { }
            }
            return r;
        }

        public static string MergeYaml(string body, string mixin)
        {
            if (!IsYaml(body) || !IsYaml(mixin))
            {
                return body;
            }

            var patKey = @"^([a-zA-Z][\w\-_]*):";

            var sb = new StringBuilder();

            var mLines = mixin.Replace("\r\n", "\n").Split(new char[] { '\n' });
            var keys = new HashSet<string>();
            foreach (var line in mLines)
            {
                sb.AppendLine(line);
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var c = line[0];
                if (c == '#' || c == ' ')
                {
                    continue;
                }
                var g = Regex.Match(line, patKey).Groups;
                if (g.Count > 1)
                {
                    keys.Add(g[1].Value);
                }
            }

            var bLines = body.Replace("\r\n", "\n").Split(new char[] { '\n' });
            for (int i = 0; i < bLines.Length; i++)
            {
                var line = bLines[i];
                if (!string.IsNullOrEmpty(line))
                {
                    do
                    {
                        var g = Regex.Match(line, patKey).Groups;
                        if (g.Count > 1 && keys.Contains(g[1].Value))
                        {
                            for (i++; i < bLines.Length; i++)
                            {
                                line = bLines[i];
                                if (
                                    !string.IsNullOrEmpty(line) && Regex.IsMatch(line, @"^[a-zA-Z]")
                                )
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    } while (i < bLines.Length);
                }

                if (i < bLines.Length)
                {
                    sb.AppendLine(line);
                }
            }
            return sb.ToString()?.Replace("\r\n", "\n");
        }

        public static Models.Datas.Enums.ConfigType DetectConfigType(string config)
        {
            var text = Models.Datas.Enums.ConfigType.text;
            if (string.IsNullOrEmpty(config) || config.Length < 2)
            {
                return text;
            }

            var mark = $"{config[0]}{config[config.Length - 1]}";
            if (mark == "{}" || mark == "[]")
            {
                return Models.Datas.Enums.ConfigType.json;
            }

            if (
                Regex.IsMatch(config, @"^[a-zA-Z][\w\-_]*:")
                || Regex.IsMatch(config, @"\n[a-zA-Z][\w\-_]*:")
            )
            {
                return Models.Datas.Enums.ConfigType.yaml;
            }

            return text;
        }

        public static List<int> FindAll(string haystack, string needle)
        {
            List<int> indexes = new List<int>();
            if (
                !string.IsNullOrEmpty(haystack)
                && haystack.Length > 0
                && !String.IsNullOrEmpty(needle)
            )
            {
                for (int index = 0; ; index += needle.Length)
                {
                    index = haystack.IndexOf(needle, index);
                    if (index == -1)
                    {
                        break;
                    }
                    indexes.Add(index);
                }
            }
            return indexes;
        }

        public static string UnescapeUnicode(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            // https://stackoverflow.com/questions/8558671/how-to-unescape-unicode-string-in-c-sharp
            return Regex.Replace(
                source,
                @"\\[Uu]([0-9A-Fa-f]{4})",
                m =>
                    char.ToString(
                        (char)
                            ushort.Parse(
                                m.Groups[1].Value,
                                System.Globalization.NumberStyles.AllowHexSpecifier
                            )
                    )
            );
        }

        public static string DecodeAmpersand(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }
            const string amp = "&amp;";
            while (source.IndexOf(amp) >= 0)
            {
                source = source.Replace(amp, "&");
            }
            return source;
        }

        public static string TrimTrailingNewLine(string content) =>
            content?.TrimEnd(Environment.NewLine.ToCharArray());

        public static string ToHexString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return "";
            }

            // js hash function return lower case hex string
            return BitConverter.ToString(bytes)?.Replace("-", "")?.ToLower();
        }

        public static byte[] Sha512Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            using (SHA512 shaM = new SHA512Managed())
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                return shaM.ComputeHash(bytes);
            }
        }

        public static byte[] Sha256Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            using (SHA256 shaM = new SHA256Managed())
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                return shaM.ComputeHash(bytes);
            }
        }

        public static string Sha512Hex(string text)
        {
            var b = Sha512Hash(text);
            return ToHexString(b);
        }

        public static string Sha256Hex(string text)
        {
            var b = Sha256Hash(text);
            return ToHexString(b);
        }

        public static string Md5Hex(string text)
        {
            var b = Md5Hash(text);
            return ToHexString(b);
        }

        public static byte[] Md5Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            using (MD5 md5Hasher = MD5.Create())
            {
                return md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(text));
            }
        }

        public static string GetAppName() => Properties.Resources.AppName;

        public static string AutoEllipsis(string text, int lenInAscii)
        {
            var ellipsis = Models.Consts.AutoEllipsis.ellipsis;
            var defFont = Models.Consts.AutoEllipsis.defFont;
            if (string.IsNullOrEmpty(text) || lenInAscii <= 0)
            {
                return string.Empty;
            }

            var t = text.Replace('\r', ' ').Replace('\n', ' ');
            var width = TextRenderer.MeasureText(t, defFont).Width;
            var baseline = TextRenderer.MeasureText(new string('a', lenInAscii), defFont).Width;
            if (width <= baseline)
            {
                return text;
            }
            int end = Math.Min(t.Length, lenInAscii);
            int pos = BinarySearchForEllipsisPos(t, 0, end, baseline);
            return text.Substring(0, pos) + ellipsis;
        }

        static int BinarySearchForEllipsisPos(string text, int start, int end, int baseline)
        {
            int mid = (start + end) / 2;
            while (mid != start && mid != end)
            {
                var s = text.Substring(0, mid) + Models.Consts.AutoEllipsis.ellipsis;
                var w = TextRenderer.MeasureText(s, Models.Consts.AutoEllipsis.defFont).Width;
                if (w == baseline)
                {
                    return mid;
                }

                if (w < baseline)
                {
                    start = mid;
                }
                else
                {
                    end = mid;
                }
                mid = (start + end) / 2;
            }
            return mid;
        }

        public static bool TryPatchGitHubUrl(string url, out string patched)
        {
            patched = string.Empty;

            try
            {
                var groups = Regex.Match(url, Patterns.GitHuhFileUrl).Groups;
                if (groups != null && groups.Count == 3)
                {
                    var repo = groups[1];
                    var tail = groups[2];
                    patched = $"https://raw.githubusercontent.com{repo}{tail}";
                    return true;
                }
            }
            catch (ArgumentException) { }
            catch (RegexMatchTimeoutException) { }

            try
            {
                var groups = Regex.Match(url, Patterns.GitHuhFileUrl).Groups;
                if (groups != null && groups.Count == 3)
                {
                    var repo = groups[1];
                    var tail = groups[2];
                    patched = $"https://raw.githubusercontent.com{repo}{tail}";
                    return true;
                }
            }
            catch (ArgumentException) { }
            catch (RegexMatchTimeoutException) { }

            return false;
        }

        public static bool TryExtractAliasFromSubscriptionUrl(string url, out string alias)
        {
            alias = string.Empty;
            try
            {
                var groups = Regex.Match(url, Patterns.ExtractAliasFromSubscriptUrl).Groups;
                if (groups != null && groups.Count == 2)
                {
                    alias = groups[1].Value;
                    return !string.IsNullOrEmpty(alias);
                }
            }
            catch (ArgumentException) { }
            catch (RegexMatchTimeoutException) { }

            return false;
        }

        public static string ReverseSummary(string summary)
        {
            if (string.IsNullOrEmpty(summary))
            {
                return "";
            }

            const char separator = '@';
            if (summary.IndexOf(separator) < 0)
            {
                return summary;
            }

            var rs = summary.Split(separator).Reverse();
            return string.Join(separator.ToString(), rs);
        }

        public static List<string> SortPacList(IEnumerable<string> pacList)
        {
            var tmpList = new List<string>();
            var result = new List<string>();
            foreach (var item in pacList)
            {
                if (
                    !string.IsNullOrWhiteSpace(item)
                    && !string.IsNullOrEmpty(item)
                    && !item.StartsWith(@"//")
                )
                {
                    tmpList.Add(item);
                    continue;
                }

                tmpList.Sort(StringComparer.Ordinal);
                result.AddRange(tmpList);
                result.Add(item);
                tmpList.Clear();
            }
            tmpList.Sort(StringComparer.Ordinal);
            result.AddRange(tmpList);
            return result;
        }

        public static int StrLenInBytes(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            return s.Length * 2;
        }

        #endregion

        #region net
        public static bool IsIpv6(string ip)
        {
            if (
                !string.IsNullOrEmpty(ip)
                && IPAddress.TryParse(ip, out var address)
                && address != null
            )
            {
                if (address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    return true;
                }
            }
            return false;
        }

        public static string FormatHost(string host)
        {
            if (
                !string.IsNullOrEmpty(host)
                && IPAddress.TryParse(host, out var address)
                && address != null
            )
            {
                if (address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    return $"[{address}]";
                }
            }
            return host;
        }

        public static WebClient CreateWebClient(
            bool isSocks5,
            string host,
            int proxyPort,
            string username,
            string password
        )
        {
            var wc = new WebClient { Encoding = Encoding.UTF8 };
            wc.Headers.Add(Webs.UserAgent);
            if (proxyPort < 0 || proxyPort > 65535)
            {
                return wc;
            }

            var needAuth = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
            if (isSocks5)
            {
                // throw exception if username or password is empty
                wc.Proxy = needAuth
                    ? new MihaZupan.HttpToSocks5Proxy(host, proxyPort, username, password)
                    : new MihaZupan.HttpToSocks5Proxy(host, proxyPort);
            }
            else
            {
                wc.Proxy = needAuth
                    ? new WebProxy(
                        new Uri(string.Format("http://{0}:{1}", host, proxyPort)),
                        true,
                        null,
                        new NetworkCredential(username, password)
                    )
                    : new WebProxy(host, proxyPort);
            }
            return wc;
        }

        /// <summary>
        /// return (ms, recvBytesLen)
        /// </summary>
        /// <returns>(ms, recvBytesLen)</returns>
        public static Tuple<long, long> TimedDownloadTestWorker(
            bool isSocks5,
            string url,
            int port,
            int expectedSizeInKiB,
            int timeout,
            string username,
            string password
        )
        {
            if (string.IsNullOrEmpty(url))
            {
                return new Tuple<long, long>(-1, 0);
            }

            long expectedSizeInBytes = expectedSizeInKiB * 1024;
            timeout = timeout > 0 ? timeout : Intervals.DefaultSpeedTestTimeout;
            var localhost = Webs.LoopBackIP;
            var wc = CreateWebClient(isSocks5, localhost, port, username, password);
            DoItLater(
                () =>
                {
                    CancelWebClientAsync(wc);
                },
                timeout
            );

            var buffer = RentBuffer();
            long size = 0;
            var sw = new Stopwatch();
            sw.Restart();
            try
            {
                using (var stream = wc.OpenRead(url))
                {
                    stream.ReadTimeout = timeout;
                    int n = 1;
                    while (
                        n > 0
                        && (expectedSizeInBytes < 0 || size <= expectedSizeInBytes)
                        && timeout >= sw.ElapsedMilliseconds
                    )
                    {
                        n = stream.Read(buffer, 0, buffer.Length);
                        size += n;
                    }
                }
            }
            catch { }
            sw.Stop();
            ReturnBuffer(buffer);
            wc.Dispose();

            var time = sw.ElapsedMilliseconds;
            if (!(time <= timeout && size > 0 && size > expectedSizeInBytes))
            {
                time = Core.SpeedtestTimeout;
            }
            return new Tuple<long, long>(time, size);
        }

        public static void CancelWebClientAsync(WebClient webClient)
        {
            try
            {
                webClient?.CancelAsync();
            }
            catch { }
        }

        public static bool IsValidPort(string port)
        {
            return IsValidPort(Str2Int(port));
        }

        public static bool IsValidPort(int port)
        {
            return port > 0 && port < 65536;
        }

        public static bool TryParseAddress(string address, out string ip, out int port)
        {
            ip = Webs.LoopBackIP;
            port = 1080;

            int index = address.LastIndexOf(':');
            if (index < 0)
            {
                return false;
            }

            var ipStr = address.Substring(0, index);
            var portInt = Str2Int(address.Substring(index + 1));
            if (string.IsNullOrEmpty(ipStr) || portInt < 1 || portInt > 65535)
            {
                return false;
            }

            ip = ipStr;
            port = portInt;
            return true;
        }

        static readonly IPEndPoint _defaultLoopbackEndpoint = new IPEndPoint(
            IPAddress.Loopback,
            port: 0
        );

        public static int GetFreeTcpPort()
        {
            // https://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net
            var port = -1;

            lock (_defaultLoopbackEndpoint)
            {
                try
                {
                    using (
                        var socket = new Socket(
                            AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp
                        )
                    )
                    {
                        // socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                        // socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        socket.Bind(_defaultLoopbackEndpoint);
                        var ep = (IPEndPoint)socket.LocalEndPoint;
                        port = ep.Port;
                    }
                }
                catch { }
            }
            return port;
        }
        #endregion

        #region Task
        public static void DoItLater(Action action, TimeSpan span)
        {
            _ = Task.Delay(span).ContinueWith(_ => action());
        }

        public static void DoItLater(Action action, long ms)
        {
            DoItLater(action, TimeSpan.FromMilliseconds(ms));
        }

        public static void Sleep(TimeSpan timespan)
        {
            try
            {
                Thread.Sleep(timespan);
            }
            catch { }
        }

        public static void Sleep(int ms)
        {
            try
            {
                Thread.Sleep(ms);
            }
            catch { }
        }

        public static Dictionary<string, string> ParseEnvString(string envs)
        {
            var r = new Dictionary<string, string>();
            var kvs =
                envs?.Replace(", ", ",")
                    ?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?? new string[0];
            foreach (var kvp in kvs)
            {
                var kv = kvp.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length > 1 && !string.IsNullOrEmpty(kv[0]) && !string.IsNullOrEmpty(kv[1]))
                {
                    r[kv[0]] = kv[1];
                }
            }
            return r;
        }

        public static void SetProcessEnvs(Process proc, string envs)
        {
            var ds = ParseEnvString(envs);
            SetProcessEnvs(proc, ds);
        }

        public static void SetProcessEnvs(Process proc, Dictionary<string, string> envs)
        {
            if (envs != null && envs.Count <= 0)
            {
                return;
            }

            var procEnv = proc.StartInfo.EnvironmentVariables;
            foreach (var d in envs)
            {
                procEnv[d.Key] = d.Value;
            }
        }

        static readonly AutoResetEvent sendCtrlCLocker = new AutoResetEvent(true);

        public static bool SendStopSignal(Process proc)
        {
            // https://stackoverflow.com/questions/283128/how-do-i-send-ctrlc-to-a-process-in-c

            const int CTRL_C_EVENT = 0;

            var success = false;
            if (!sendCtrlCLocker.WaitOne(Core.SendCtrlCTimeout))
            {
                return false;
            }
            try
            {
                if (Libs.Sys.ConsoleCtrls.AttachConsole((uint)proc.Id))
                {
                    Libs.Sys.ConsoleCtrls.SetConsoleCtrlHandler(null, true);
                    try
                    {
                        if (
                            Libs.Sys.ConsoleCtrls.GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0)
                            && proc.WaitForExit(Core.SendCtrlCTimeout)
                        )
                        {
                            success = true;
                        }
                    }
                    catch { }
                    Libs.Sys.ConsoleCtrls.FreeConsole();
                    Libs.Sys.ConsoleCtrls.SetConsoleCtrlHandler(null, false);
                }
            }
            catch { }
            sendCtrlCLocker.Set();

            return success;
        }

        public static Task RunInBackground(Action worker, bool configAwait = false)
        {
            void job()
            {
                try
                {
                    var missionId = RandomHex(8);
                    if (UI.IsInUiThread())
                    {
                        Libs.Sys.FileLogger.Warn($"Task [{missionId}] running in UI thread");
#if DEBUG
                        Libs.Sys.FileLogger.DumpCallStack("Caller stack:");
#endif
                    }
                    worker?.Invoke();
                    if (UI.IsInUiThread())
                    {
                        Libs.Sys.FileLogger.Warn($"task [{missionId}] finished");
                    }
                }
                catch (Exception e)
                {
                    Libs.Sys.FileLogger.Error($"Background task error:\n{e}");
                    throw;
                }
            }

            try
            {
                var t = new Task(job, TaskCreationOptions.LongRunning);
                if (!configAwait)
                {
                    t.ConfigureAwait(false);
                }
                t.Start();
                return t;
            }
            catch (Exception e)
            {
                Libs.Sys.FileLogger.Error($"Create background task error:\n{e}");
            }
            return Task.FromResult(false);
        }
        #endregion

        #region Json
        public static JToken GenHttpInbound(int port)
        {
            return GenHttpInbound(Webs.LoopBackIP, port);
        }

        public static JToken GenHttpInbound(string host, int port)
        {
            var tpl = Config
                .HttpInboundsTemplate.Replace("%host%", host)
                .Replace("%port%", port.ToString());
            return JToken.Parse(tpl);
        }

        public static string GetProtocolFromConfig(string config)
        {
            if (string.IsNullOrEmpty(config) || config.Length < 3 || config[0] != '{')
            {
                return null;
            }

            try
            {
                var json = JObject.Parse(config);
                return GetProtocolFromConfig(json);
            }
            catch { }
            return null;
        }

        public static string GetProtocolFromConfig(JObject config)
        {
            var keys = new string[] { "outbounds.0.protocol", "outbound.protocol" };

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

            var def =
                default(T) == null && typeof(T) == typeof(string)
                    ? (T)(object)string.Empty
                    : default;

            try
            {
                switch (key?.Type)
                {
                    case JTokenType.Object:
                    case JTokenType.Array:
                    case null:
                        return def;
                    default:
                        return key.Value<T>();
                }
            }
            catch { }
            return def;
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

        /// <summary>
        /// return parsed T object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T LoadAndParseJsonFile<T>(string filename)
            where T : class
        {
            if (File.Exists(filename))
            {
                try
                {
                    var content = File.ReadAllText(filename);
                    var result = JsonConvert.DeserializeObject<T>(content);
                    return result;
                }
                catch { }
            }
            return null;
        }

        public static int TagStringComparer(string a, string b)
        {
            if (a == b)
            {
                return 0;
            }

            var pa = ParseTagString(a);
            var pb = ParseTagString(b);
            var la = pa.Count;
            var lb = pb.Count;
            var len = Math.Min(la, lb);
            for (int i = 0; i < len; i++)
            {
                var ia = pa[i];
                var ib = pb[i];
                if (ia != ib)
                {
                    if (int.TryParse(ia, out var na) && int.TryParse(ib, out var nb))
                    {
                        return na.CompareTo(nb);
                    }
                    else
                    {
                        return ia?.CompareTo(ib) ?? -1;
                    }
                }
            }
            return la.CompareTo(lb);
        }

        static List<string> ParseTagString(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    var ma = Regex.Match(s.ToLower(), @"([^\d]*)(\d*)([^\d]*)(\d*)");
                    var g = ma.Groups;
                    var r = new List<string>();
                    for (int i = 1; i < g.Count; i++)
                    {
                        r.Add(g[i].Value);
                    }
                    return r;
                }
                catch { }
            }
            return new List<string>();
        }

        /// <summary>
        /// a<b: -, a=b: 0, a>b: +
        /// </summary>
        public static int JsonKeyComparer(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            {
                return a.CompareTo(b);
            }

            var listA = a.Split('.').ToList();
            var listB = b.Split('.').ToList();
            var lenA = listA.Count;
            var lenB = listB.Count;

            var maxLen = Math.Min(lenA, lenB);
            var result = 0;
            for (int i = 0; i < maxLen && result == 0; i++)
            {
                var itemA = listA[i];
                var itemB = listB[i];
                if (int.TryParse(itemA, out int numA) && int.TryParse(itemB, out int numB))
                {
                    result = numA.CompareTo(numB);
                }
                else
                {
                    result = itemA.CompareTo(itemB);
                }
            }

            return result == 0 ? lenA - lenB : result;
        }

        public static string FilterControlChars(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return Regex.Replace(str, @"\p{Cc}+", string.Empty);
        }

        public static string ToJson(object o)
        {
            try
            {
                return JsonConvert.SerializeObject(o, Formatting.Indented);
            }
            catch { }
            return null;
        }

        public static string FormatConfig(JToken config)
        {
            try
            {
                return config?.ToString() ?? string.Empty;
            }
            catch { }
            return string.Empty;
        }

        public static string FormatConfig(string config)
        {
            var json = ParseJToken(config);
            return FormatConfig(json);
        }

        public static bool IsYaml(string config)
        {
            return DetectConfigType(config) == Models.Datas.Enums.ConfigType.yaml;
        }

        public static bool IsJson(string config)
        {
            return DetectConfigType(config) == Models.Datas.Enums.ConfigType.json;
        }

        public static JToken ParseJToken(string config)
        {
            if (IsJson(config))
            {
                try
                {
                    return JToken.Parse(config);
                }
                catch { }
            }
            return null;
        }

        public static JObject ParseJObject(string config)
        {
            if (IsJson(config))
            {
                try
                {
                    return JObject.Parse(config);
                }
                catch { }
            }
            return null;
        }

        public static void SavePluginSetting<T>(
            string pluginName,
            T userSettings,
            Interfaces.Services.ISettingsService vgcSetting
        )
            where T : class
        {
            var content = SerializeObject(userSettings);
            vgcSetting.SavePluginsSetting(pluginName, content);
        }

        public static T LoadPluginSetting<T>(
            string pluginName,
            Interfaces.Services.ISettingsService vgcSetting
        )
            where T : class, new()
        {
            var empty = new T();
            var userSettingString = vgcSetting.GetPluginsSetting(pluginName);

            if (string.IsNullOrEmpty(userSettingString))
            {
                return empty;
            }

            try
            {
                var result = DeserializeObject<T>(userSettingString);
                return result ?? empty;
            }
            catch { }
            return empty;
        }

        /// <summary>
        /// return null if fail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string content)
            where T : class
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            try
            {
                var result = JsonConvert.DeserializeObject<T>(content);
                if (result != null)
                {
                    return result;
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// return null if fail
        /// </summary>
        /// <param name="serializeObject"></param>
        /// <returns></returns>
        public static string SerializeObject(object serializeObject)
        {
            if (serializeObject == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(serializeObject);
        }

        /// <summary>
        /// return null if fail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static T Clone<T>(T a)
            where T : class
        {
            if (a == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(a));
            }
            catch { }
            return null;
        }
        #endregion

        #region string processor
        public static string Base64EncodeBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1)
            {
                return "";
            }
            return Convert.ToBase64String(bytes);
        }

        public static string Base64EncodeString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            var utf8Bytes = Encoding.UTF8.GetBytes(plainText);
            return Base64EncodeBytes(utf8Bytes);
        }

        public static string Base64PadRight(string base64)
        {
            // 一位顾客点了一份炒饭，酒吧炸了
            var str = base64
                .Replace('_', '/')
                .Replace('-', '+')
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("=", "");

            var len = str.Length;
            return str.PadRight(len + (4 - len % 4) % 4, '=');
        }

        public static byte[] Base64DecodeToBytes(string b64Str)
        {
            if (!string.IsNullOrEmpty(b64Str))
            {
                try
                {
                    var padded = Base64PadRight(b64Str);
                    return Convert.FromBase64String(padded);
                }
                catch { }
            }
            return null;
        }

        public static string Base64DecodeToString(string b64Str)
        {
            if (!string.IsNullOrEmpty(b64Str))
            {
                try
                {
                    var padded = Base64PadRight(b64Str);
                    var bytes = Convert.FromBase64String(padded);
                    return Encoding.UTF8.GetString(bytes);
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// Return empty list if some thing goes wrong.
        /// </summary>
        public static List<string> ExtractBase64Strings(string text, int minLen)
        {
            var b64s = new List<string>();
            try
            {
                var matches = Regex.Matches(text, Patterns.Base64NonStandard);
                foreach (Match match in matches)
                {
                    var v = match.Value;
                    if (string.IsNullOrEmpty(v) || v.Length < minLen)
                    {
                        continue;
                    }
                    b64s.Add(v);
                }
            }
            catch { }
            return b64s;
        }

        public static string GetFragment(Scintilla editor, string searchPattern)
        {
            // https://github.com/Ahmad45123/AutoCompleteMenu-ScintillaNET

            var selectedText = editor.SelectedText;
            if (selectedText.Length > 0)
            {
                return selectedText;
            }

            string text = editor.Text;
            var regex = new Regex(searchPattern);

            var startPos = editor.CurrentPosition;

            //go forward
            int i = startPos;
            while (i >= 0 && i < text.Length)
            {
                if (!regex.IsMatch(text[i].ToString()))
                    break;
                i++;
            }

            var endPos = i;

            //go backward
            i = startPos;
            while (i > 0 && (i - 1) < text.Length)
            {
                if (!regex.IsMatch(text[i - 1].ToString()))
                    break;
                i--;
            }
            startPos = i;

            return GetSubString(startPos, endPos, text);
        }

        static string GetSubString(int start, int end, string text)
        {
            // https://github.com/Ahmad45123/AutoCompleteMenu-ScintillaNET

            if (string.IsNullOrEmpty(text))
                return "";
            if (start >= text.Length)
                return "";
            if (end > text.Length)
                return "";

            return text.Substring(start, end - start);
        }

        public static bool PartialMatchCi(string source, string partial) =>
            PartialMatch(source.ToLower(), partial.ToLower());

        public static bool PartialMatch(string source, string partial) =>
            MeasureSimilarity(source, partial) > 0;

        public static long MeasureSimilarityCi(string source, string partial) =>
            MeasureSimilarity(source.ToLower(), partial.ToLower());

        /// <summary>
        /// -1: not match
        ///  1: equal
        /// >1: the smaller the value, the more similar
        /// </summary>
        public static long MeasureSimilarity(string source, string partial)
        {
            if (string.IsNullOrEmpty(partial))
            {
                return 1;
            }

            if (string.IsNullOrEmpty(source))
            {
                return -1;
            }

            long marks = 1;

            var s = source;
            var p = partial;

            int idxS = 0,
                idxP = 0;
            int lenS = s.Length,
                lenP = p.Length;
            while (idxS < lenS && idxP < lenP)
            {
                if (s[idxS] == p[idxP])
                {
                    idxP++;
                }
                else
                {
                    marks += lenP - idxP;
                }
                idxS++;
            }

            if (idxP != lenP)
            {
                return -1;
            }

            return marks;
        }

        public static string GetLinkPrefix(string shareLink)
        {
            var index = shareLink.IndexOf(@"://");
            if (index == -1)
            {
                return null;
            }

            var prefix = shareLink.Substring(0, index);
            return prefix.ToLower();
        }

        public static Models.Datas.Enums.LinkTypes DetectLinkType(string shareLink)
        {
            var unknow = Models.Datas.Enums.LinkTypes.unknow;
            var prefix = GetLinkPrefix(shareLink);
            if (
                !string.IsNullOrEmpty(prefix)
                && Enum.TryParse(prefix, out Models.Datas.Enums.LinkTypes linkType)
            )
            {
                return linkType;
            }
            return unknow;
        }

        /// <summary>
        /// regex = @"(?&lt;groupName>pattern)"
        /// <para>return string.Empty if sth. goes wrong</para>
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="pattern"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ExtractStringWithPattern(
            string groupName,
            string pattern,
            string content
        )
        {
            var ptnStr = string.Format(@"(?<{0}>{1})", groupName, pattern);
            Regex rgx = new Regex(ptnStr);
            Match match = rgx.Match(content ?? string.Empty);
            if (match.Success)
            {
                return match.Groups[groupName].Value;
            }
            return string.Empty;
        }

        #endregion

        #region numbers

        public static List<string> EnumToList<TEnum>()
            where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(e => e.ToString()).ToList();
        }

        public static bool TryParseEnum<TEnum>(string value, out TEnum outEnum)
            where TEnum : struct
        {
            return Enum.TryParse(value, out outEnum);
        }

        public static bool TryParseEnum<TEnum>(int value, out TEnum outEnum)
            where TEnum : struct
        {
            outEnum = (TEnum)(object)value;
            if (Enum.IsDefined(typeof(TEnum), value))
            {
                return true;
            }
            return false;
        }

        public static int Str2Int(string value)
        {
            if (float.TryParse(value, out float f))
            {
                return (int)Math.Round(f);
            }
            ;
            return 0;
        }

        public static bool AreEqual(double left, double right)
        {
            return Math.Abs(left - right) < Config.FloatPointNumberTolerance;
        }

        public static long SpeedtestMean(long left, long right, double weight) =>
            (long)SpeedtestMean((double)left, (double)right, weight);

        public static double SpeedtestMean(double left, double right, double weight)
        {
            if (weight <= 0 || weight >= 1)
            {
                throw new ArgumentOutOfRangeException("weight should between 0 to 1");
            }

            if (left <= 0 || right <= 0)
            {
                return Math.Max(left, right);
            }

            /*
             * 预期：
             * 由于最后一次测速服务器速度已经稳定，很有价值。
             * 而首次测速通常是没有缓存的，对分析服务器的速度也很重要，
             * 中间测速结果的重要程度则随测速次数递增。
             *
             * 假设：
             * 连续做三次速度测试，权重为0.6，
             * 将三次速度测试迭代进这个求平均函数中将得到：
             * first * 0.6 * 0.6 + second * 0.6 * 0.4 + third * 0.4;
             * 即 first * 0.36 + second * 0.24 + third * 0.4;
             * 可见首次和末次测速结果占比较重，中间那次占比较低，符合预期。
             *
             * 测试3至6次结果如下，均符合预期
             * 3    36% 24% 40%
             * 4    22% 14% 24% 40%
             * 5    13%  9% 14% 24% 40%
             * 6     8%  5%  9% 14% 24% 40%
             *
             * p.s.我不会告诉你，其实是因为我懒得写个列表存中间结果才这么搞的。
             */

            return left * weight + right * (1 - weight);
        }

        public static int GetLenInBitsOfInt(int value)
        {
            var k = 0;
            while (value > 0)
            {
                value >>= 1;
                k++;
            }
            return value < 0 ? -1 : k;
        }

        #endregion

        #region Misc
        public static string GetLuaModuleName(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return null;
            }

            var appDir = GetAppDir();
            if (fullPath.StartsWith(appDir))
            {
                fullPath = fullPath.Substring(appDir.Length);
            }

            var mn = fullPath.Replace("\\", ".").Replace("/", ".");
            while (mn != null && mn.StartsWith("."))
            {
                mn = mn.Substring(1);
            }

            string ext = ".lua";
            if (mn != null && mn.ToLower().EndsWith(ext))
            {
                mn = mn.Substring(0, mn.Length - ext.Length);
            }

            return mn;
        }

        public static bool IsImportResultSuccess(string[] result)
        {
            return result[3] == Import.MarkImportSuccess;
        }

        public static bool IsHttpLink(string link)
        {
            if (!string.IsNullOrEmpty(link) && link.ToLower().StartsWith("http"))
            {
                return true;
            }
            return false;
        }

        public static string RelativePath2FullPath(string path)
        {
            if (string.IsNullOrEmpty(path) || Path.IsPathRooted(path))
            {
                return path;
            }

            var appDir = GetAppDir();
            return Path.Combine(appDir, path);
        }

        public static string CopyFromClipboard()
        {
            try
            {
                return Clipboard.GetText();
            }
            catch { }
            return string.Empty;
        }

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

        // Assembly location may change while app running.
        // So we should cache it when app starts.
        static readonly string appDirCache = GenAppDir();

        static string GenAppDir()
        {
            // z:\vgc\libs\vgcapi.dll
            var vgcApiDllFile = Assembly.GetExecutingAssembly().Location;
            var parent = new DirectoryInfo(vgcApiDllFile).Parent;
            if (parent.Name == Files.LibsDir)
            {
                parent = parent.Parent;
            }
            return parent.FullName;
        }

        public static string GetCoreFolderFullPath() =>
            Path.Combine(GetAppDir(), Files.CoreFolderNameInside3rd);

        public static string GetAppDir() => appDirCache;

        /// <summary>
        /// min to max - 1
        /// </summary>
        public static int Clamp(int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max - 1), min);
        }

        public static string RandomHex(int length)
        {
            //  https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
            if (length <= 0)
            {
                return string.Empty;
            }

            const string chars = "0123456789abcdef";
            int charLen = chars.Length;

            int rndIndex;
            StringBuilder sb = new StringBuilder("");

            for (int i = 0; i < length; i++)
            {
                rndIndex = Libs.Infr.PseudoRandom.Next(charLen);
                sb.Append(chars[rndIndex]);
            }

            return sb.ToString();
        }
        #endregion

        #region reflection

        static public string GetPublicFieldsInfoOfType(Type type)
        {
            var fields = type.GetFields(
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance
                )
                .Select(field =>
                {
                    var pf = field.IsStatic ? "Static " : "";
                    var tn = GetFriendlyTypeName(field.FieldType);
                    return $"{pf}{tn} {field.Name}";
                })
                .OrderBy(fn => fn);

            return string.Join("\n", fields);
        }

        public static string GetPublicMethodsInfoOfType(Type type)
        {
            List<string> staticMems = new List<string>();
            List<string> dynamicMems = new List<string>();
            List<string> allMems = new List<string>();

            var methods = type.GetMethods().Where(m => m.IsPublic).ToList();

            foreach (var method in methods)
            {
                var fn = GetFriendlyMethodDeclareInfo(method);
                if (method.IsStatic)
                {
                    staticMems.Add(fn);
                }
                else
                {
                    dynamicMems.Add(fn);
                }
            }

            staticMems.Sort();
            dynamicMems.Sort();
            allMems.AddRange(staticMems);
            allMems.AddRange(dynamicMems);

            return string.Join("\n", allMems);
        }

        public static List<Type> GetAllAssembliesType()
        {
            return AppDomain
                .CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass)
                .ToList();
        }

        /// <summary>
        /// e.g. static void Sum&lt;int>(int a, int b)
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        static public string GetFriendlyMethodDeclareInfo(MethodInfo method)
        {
            var pms = method
                .GetParameters()
                .Select(arg =>
                {
                    var tn = GetFriendlyTypeName(arg.ParameterType);
                    var name = arg.Name;
                    return $"{tn} {name}";
                });

            var head = method.IsStatic ? @"Static " : @"";
            var rtt = GetFriendlyTypeName(method.ReturnType);
            var fn = GetFriendlyMethodName(method);
            var args = string.Join(@", ", pms);
            return $"{head}{rtt} {fn}({args})";
        }

        public static string GetFriendlyMethodName(MethodInfo method)
        {
            var name = method.Name;
            if (!method.IsGenericMethod)
            {
                return name;
            }

            var args = method.GetGenericArguments().Select(arg => GetFriendlyTypeName(arg));

            return $"{name}<{string.Join(@", ", args)}>";
        }

        public static string GetFriendlyTypeName(Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyTypeName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

        public static List<Tuple<string, string>> GetPublicPropsInfoOfType(Type type) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Select(field =>
                {
                    var tn = GetFriendlyTypeName(field.PropertyType);
                    return new Tuple<string, string>(tn, field.Name);
                })
                .ToList();

        public static List<Tuple<string, string>> GetPublicEventsInfoOfType(Type type) =>
            type.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Select(field =>
                {
                    var tn = GetFriendlyTypeName(field.EventHandlerType);
                    return new Tuple<string, string>(tn, field.Name);
                })
                .ToList();

        /// <summary>
        /// [0: ReturnType 1: MethodName 2: ParamsStr 3: ParamsWithType]
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<List<string>> GetPublicMethodNameAndParam(Type type)
        {
            var fullNames = new List<List<string>>();
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (!method.IsPublic)
                {
                    continue;
                }
                var name = method.Name;
                if (name.StartsWith("add_On") || name.StartsWith("remove_On"))
                {
                    continue;
                }
                var paramStrs = GenParamStr(method);
                var returnType = GetFriendlyTypeName(method.ReturnType);
                fullNames.Add(
                    new List<string>() { returnType, name, paramStrs.Item1, paramStrs.Item2, }
                );
            }
            return fullNames;
        }

        static Tuple<string, string> GenParamStr(MethodInfo methodInfo)
        {
            var fullParamList = new List<string>();
            var paramList = new List<string>();

            foreach (var paramInfo in methodInfo.GetParameters())
            {
                fullParamList.Add(paramInfo.ParameterType.Name + " " + paramInfo.Name);

                paramList.Add(paramInfo.Name);
            }

            return new Tuple<string, string>(
                string.Join(@", ", paramList),
                string.Join(@", ", fullParamList)
            );
        }

        public static List<string> GetPublicMethodNames(Type type)
        {
            var exceptList = new List<string>
            {
                "add_OnPropertyChanged",
                "remove_OnPropertyChanged",
            };

            var methodsName = new List<string>();
            _ = type.GetMethods();
            foreach (var method in type.GetMethods())
            {
                var name = method.Name;
                if (method.IsPublic && !exceptList.Contains(name))
                {
                    methodsName.Add(name);
                }
            }
            return methodsName;
        }
        #endregion
    }
}
