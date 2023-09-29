using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using static VgcApis.Misc.Utils;
using static VgcApis.Models.Datas.Enums;

namespace VgcApisTests
{
    [TestClass]
    public class UtilsTests
    {
        [DataTestMethod]
        [DataRow(@"😁=😁, 😁=😁, 😁=😁, ", 1)]
        [DataRow(@"", 0)]
        [DataRow(null, 0)]
        [DataRow(@"PATH=c:\\abc\,n_1=23", 2)]
        [DataRow(@"a=1 b=2", 2)]
        public void ParseEnvStringTest(string env, int exp)
        {
            var d = ParseEnvString(env);
            Assert.AreEqual(exp, d.Keys.Count);
        }

        [DataTestMethod]
        [DataRow(@"😁", 0)]
        [DataRow(@"   中文", 3)]
        [DataRow(@"   😀", 3)]
        [DataRow(@"   ..", 3)]
        [DataRow(@"      abc", 6)]
        [DataRow(@"   ", 3)]
        [DataRow(@" ", 1)]
        [DataRow(@"", 0)]
        [DataRow(null, 0)]
        public void CountLeadingSpacesTest(string content, int exp)
        {
            var r = CountLeadingSpaces(content);
            Assert.AreEqual(exp, r);
        }

        [DataTestMethod]
        [DataRow(
            @"name: hello

key: |
  e
  f

# this line will be retained
value: |
  1
  2
  aaa

# this line should be deleted",
            @"value: |
  a
  b

name: t0e中s2t😀文1",
            @"value: |
  a
  b

name: t0e中s2t😀文1
key: |
  e
  f

# this line will be retained
"
        )]
        [DataRow(
            @"name: hello

key: test
# this line will be retained

value: world
# this line should be deleted

",
            @"name: t0e中s2t😀文1

value: |
  a
  b
",
            @"name: t0e中s2t😀文1

value: |
  a
  b

key: test
# this line will be retained

"
        )]
        [DataRow("", "", "")]
        [DataRow(null, null, null)]
        [DataRow(null, "", null)]
        public void MergeYamlTest(string config, string inbound, string exp)
        {
            var c = MergeYaml(config, inbound);
            var r = c?.Replace("\r\n", "\n");
            var e = exp?.Replace("\r\n", "\n");
            Assert.AreEqual(e, r);
        }

        [DataTestMethod]
        [DataRow(
            @"TAG: agentout,inbounds,Inbounds,log,OUTBOUNDS,outbounds,tag: n10s10,tag: n1s2,tag: n1s0,tag: agentin",
            @"inbounds,Inbounds,log,OUTBOUNDS,outbounds,tag: agentin,TAG: agentout,tag: n1s0,tag: n1s2,tag: n10s10"
        )]
        [DataRow(
            @"tag: a10s10,tag: a10s0,tag: a0s1,tag: a0s0",
            @"tag: a0s0,tag: a0s1,tag: a10s0,tag: a10s10"
        )]
        [DataRow(@"b2,a10s0,a0s1,a1s10,b1,a1s5", "a0s1,a1s5,a1s10,a10s0,b1,b2")]
        [DataRow(@"2,,1", ",1,2")]
        [DataRow(@"0,1,2", "0,1,2")]
        public void TagStringComparerTest(string src, string exp)
        {
            var ls = src.Split(',').ToList();
            ls.Sort(TagStringComparer);
            var r = string.Join(",", ls);
            Assert.AreEqual(exp, r);
        }

        [DataTestMethod]
        [DataRow(@"{}", ConfigType.json)]
        [DataRow("{\n\n}", ConfigType.json)]
        [DataRow(@"{\n\n\n", ConfigType.text)]
        [DataRow(@"", ConfigType.text)]
        [DataRow(null, ConfigType.text)]
        [DataRow(@"  123:", ConfigType.text)]
        [DataRow(@"ab12-_中文:", ConfigType.yaml)]
        [DataRow(@"  ab12😀-_中文:", ConfigType.text)]
        [DataRow(
            @"# this is a comment

# hello
ab12-_中文: |
  hello
  world

# world
",
            ConfigType.yaml
        )]
        public void DetectConfigTypeTest(string config, ConfigType ty)
        {
            var r = DetectConfigType(config);
            Assert.AreEqual(ty, r);
        }

        [DataTestMethod]
        [DataRow("  abc: 123\na_b-c: |\n  1\n2\n3\n4\n      d: 7\na中文: 8", "a_b-c,1;a中文,7")]
        [DataRow("{\nlog:{},\n      tag:\"123\"\n}", "log,1;tag: 123,2")]
        [DataRow("log:123,\n      tag:1-2-3\n}", "log,0;tag: 1-2-3,1")]
        [DataRow("", "")]
        [DataRow(null, "")]
        [DataRow(
            "{\n  \"abc1\": 123,\n  \"a_b\": \"abc\",\n  \"c\":1,\n        \"d\":1 }",
            "abc1,1;a_b,2;c,3"
        )]
        [DataRow(
            "{\n        \"abc1\": 123,\n  \"a_b\": \"abc\",\n        \"c\":1,\n\"d\":1 }",
            "abc1,1;c,3"
        )]
        public void GetConfigKeysTest(string config, string rawExp)
        {
            var exp = rawExp
                .Split(';')
                .Select(kvs =>
                {
                    var kvp = kvs.Split(',');
                    if (kvp.Length > 1)
                    {
                        var num = int.Parse(kvp[1]);
                        return new KeyValuePair<string, int>(kvp[0], num);
                    }
                    return new KeyValuePair<string, int>(string.Empty, -1);
                })
                .Where(kv => kv.Value >= 0)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            var lines = config?.Split('\n')?.ToList() ?? new List<string>();
            var r = GetConfigTags(lines);

            foreach (var key in exp.Keys)
            {
                var e = exp[key];
                var t = r[key];
                Assert.AreEqual(e, t);
            }
            foreach (var key in r.Keys)
            {
                var e = exp[key];
                var t = r[key];
                Assert.AreEqual(e, t);
            }
        }

        [DataTestMethod]
        [DataRow(@"{outbounds:[{protocol:'vmess'}]}", "vmess")]
        [DataRow(@"{outbounds:[{protocol:'Trojan'}]}", "trojan")]
        [DataRow(@"{outbound:{protocol:'vless'}}", "vless")]
        [DataRow(@"{inbound:{protocol:'vless'}}", null)]
        public void GetProtocolFromConfigTest(string config, string exp)
        {
            var r = GetProtocolFromConfig(config);
            Assert.AreEqual(exp, r);
        }

        [DataTestMethod]
        [DataRow(",,,1中2,❤文？,,,", ",,,1中2,❤文？,,")]
        [DataRow(",,,", ",,")]
        [DataRow("1中2,❤文？,,,", "1中2,❤文？,,,")]
        [DataRow(",,,1中2,❤文？,,,", ",,,1中2,❤文？,,,")]
        [DataRow(",,,", ",,,")]
        [DataRow("", "")]
        public void TryGetDictValueTest(string src, string exp)
        {
            var key = src?.Split(',')?.ToArray() ?? new string[] { };
            var dkey = exp?.Split(',')?.ToArray() ?? new string[] { };

            var dict = new Dictionary<string[], bool>();
            if (TryGetDictValue(dict, key, out _))
            {
                Assert.Fail();
            }

            dict[dkey] = true;
            var eq = src == exp;
            if (TryGetDictValue(dict, key, out var v) && eq)
            {
                Assert.AreEqual(true, v);
            }
            else
            {
                Assert.AreEqual(false, v);
            }

            if (!eq && TryGetDictValue(dict, key, out _))
            {
                Assert.Fail();
            }

            if (eq && !TryGetDictValue(dict, key, out _))
            {
                Assert.Fail();
            }
        }

        [DataTestMethod]
        [DataRow(
            @"vless://1234@[1::2:3:4]:1234?security=reality\u0026encryption=none\u0026headerType=none\u0026type=tcp\u0026flow=xtls-rprx-vision#1aA中😀",
            @"vless://1234@[1::2:3:4]:1234?security=reality&encryption=none&headerType=none&type=tcp&flow=xtls-rprx-vision#1aA中😀"
        )]
        [DataRow(@"\u8ba1\u7b97\u673a\u2022\u7f51\u7edc\u2022\u6280\u672f\u7c7b", "计算机•网络•技术类")]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow("1aA中😀", "1aA中😀")]
        public void UnescapeUnicodeTest(string unicodeString, string expect)
        {
            var result = UnescapeUnicode(unicodeString);
            Assert.AreEqual(expect, result);
        }

        [DataTestMethod]
        [DataRow("lua", "lua")]
        [DataRow("lua\\hello", "lua.hello")]
        [DataRow("lua/hello.lua", "lua.hello")]
        [DataRow("", "")]
        public void GetLuaModuleNameTest(string relativePath, string expect)
        {
            var root = GetAppDir();
            var fullPath = Path.Combine(root, relativePath);
            var result = GetLuaModuleName(fullPath);
            Assert.AreEqual(expect, result);
        }

        [DataTestMethod]
        [DataRow(
            "<td>vless://abcd@1.2.3.4:443?security=reality&amp;amp;encryption=none&amp;amp;pbk=a1b2&amp;amp;headerType=none&amp;amp;fp=chrome&amp;amp;spx=%2F&amp;amp;type=tcp&amp;amp;flow=xtls-rprx-vision&amp;amp;sni=baidu.com#test</td>",
            "<td>vless://abcd@1.2.3.4:443?security=reality&encryption=none&pbk=a1b2&headerType=none&fp=chrome&spx=%2F&type=tcp&flow=xtls-rprx-vision&sni=baidu.com#test</td>"
        )]
        [DataRow("&amp;amp;amp;amp;", "&")]
        /*
        [DataRow(null, "")]
        [DataRow("", "")]
        [DataRow("&amp;", "&")]
        */
        public void DecodeAmpersandTest(string source, string expect)
        {
            var result = DecodeAmpersand(source);
            Assert.AreEqual(expect, result);
        }

        [DataTestMethod]
        [DataRow(@"{outbounds:[{protocol:'vmess'}]}", "outbounds.0.protocol", "")]
        public void WTF(string str, string path, string exp)
        {
            // 啊，这？
            var r = GetValue<string>(str, path);

            Assert.AreEqual(r, exp);
        }

        [DataTestMethod]
        [DataRow("D1", true, true, false, true, 3u, 49u)]
        public void TryParseKeyMessageTests(
            string keyName,
            bool hasAlt,
            bool hasCtrl,
            bool hasShift,
            bool ok,
            uint modifier,
            uint keyCode
        )
        {
            var result = TryParseKeyMesssage(
                keyName,
                hasAlt,
                hasCtrl,
                hasShift,
                out var m,
                out var c
            );
            Assert.AreEqual(ok, result);
            Assert.AreEqual(modifier, m);
            Assert.AreEqual(keyCode, c);
        }

        [DataTestMethod]
        [DataRow(
            "中iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii试",
            34,
            true,
            "中iiiiiiiiiiiiiiiiiiiiiiiiiiiiii"
        )]
        [DataRow(
            "中aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa文",
            34,
            true,
            "中aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        )]
        [DataRow("中iiiii试", 6, true, "中ii")]
        [DataRow("中文aa测试", 8, true, "中文aa")]
        [DataRow("中文aa测试", 7, true, "中文a")]
        [DataRow("中文aa测试", 6, true, "中文")]
        [DataRow("中aaa文测试", 6, true, "中aa")]
        [DataRow("中aaa文测试", 5, true, "中a")]
        [DataRow("中文测试", 4, true, "中")]
        [DataRow("中文测试", 3, true, "")]
        [DataRow("a中文测试", 9, false, "a中文测试")]
        [DataRow("a中文测试", 7, true, "a中文")]
        [DataRow("a中文测试", 6, true, "a中")]
        [DataRow("a中文测试", 5, true, "a中")]
        [DataRow("a中文测试", 4, true, "a")]
        [DataRow("a中文测试", 3, true, "a")]
        [DataRow("a中文测试", 2, true, "")]
        [DataRow("a中文测试", 1, true, "")]
        [DataRow("a中文测试", 0, false, "")]
        [DataRow("a中文测试", -1, false, "")]
        [DataRow("aaaaaaaaa", 5, true, "aaa")]
        [DataRow("", 100, false, "")]
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0060 // 删除未使用的参数
        public void AutoEllipsisTest(string org, int len, bool isEllipsised, string expect)
#pragma warning restore IDE0079 // 请删除不必要的忽略
#pragma warning restore IDE0060 // 删除未使用的参数
        {
            var defFont = VgcApis.Models.Consts.AutoEllipsis.defFont;
            var orgLen = org.Length;
            var result = AutoEllipsis(org, len);

            if (orgLen <= 0 || len <= 0)
            {
                Assert.AreEqual(string.Empty, result);
                return;
            }

            var orgWidth = TextRenderer.MeasureText(org, defFont).Width;
            var resultWidth = TextRenderer.MeasureText(result, defFont).Width;
            var expectedWidth = TextRenderer.MeasureText(new string('a', len), defFont).Width;

            if (orgWidth <= expectedWidth)
            {
                Assert.AreEqual(org, result);
                return;
            }

            var ellipsis = VgcApis.Models.Consts.AutoEllipsis.ellipsis;
            Assert.AreEqual(ellipsis.Last(), result.Last());

            var d = TextRenderer.MeasureText(ellipsis, defFont).Width;

            if (resultWidth <= d)
            {
                return;
            }

            Assert.IsTrue(resultWidth <= expectedWidth);
            Assert.IsTrue(resultWidth >= expectedWidth - d);

            /* 以下代码只对特定字体有效
            string ellipsis = VgcApis.Models.Consts.AutoEllipsis.ellipsis;
            var cut = AutoEllipsis(org, len);
            var exp = expect + (isEllipsised ? ellipsis : "");
            Assert.AreEqual(exp, cut);
            */
        }

        [DataTestMethod]
        [DataRow(
            @"https://github.com/user/reponame/blob/master/README.md",
            true,
            @"https://raw.githubusercontent.com/user/reponame/master/README.md"
        )]
        public void TryPatchGitHubUrlTest(string url, bool expectedResult, string expectedPatched)
        {
            var result = TryPatchGitHubUrl(url, out var patched);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(expectedPatched, patched);
        }

        [DataTestMethod]
        [DataRow(@"abc", @"https://www.github.com/abc/a/b/c/d.html")]
        [DataRow(@"a1中b2文", @"https://www.github.com/a1中b2文/a/b/c/d.html")]
        [DataRow(@"index.html", @"https://www.github.com/index.html")]
        [DataRow(@"a1中b2文?", @"fttp::ssh:://%20#2@&/a1中b2文?/a/b/c/d.html?#a=1&value=10")]
        public void TryExtractAliasFromSubsUrlNormalTest(string expected, string url)
        {
            if (TryExtractAliasFromSubscriptionUrl(url, out var alias))
            {
                Assert.AreEqual(expected, alias);
            }
            else
            {
                Assert.Fail();
            }
        }

        [DataTestMethod]
        [DataRow(@"https://www.github.com")]
        [DataRow(@"https://www.github.com//")]
        [DataRow(@"https://www.github.com/")]
        [DataRow(@"abcd1234")]
        public void TryExtractAliasFromSubsUrlFailTest(string url)
        {
            if (TryExtractAliasFromSubscriptionUrl(url, out var _))
            {
                Assert.Fail();
            }
        }

        [DataTestMethod]
        [DataRow("vmess.mkcp.tls@whatsurproblem.com", "whatsurproblem.com@vmess.mkcp.tls")]
        [DataRow("vmess.ws.tls@1.2.3.4", "1.2.3.4@vmess.ws.tls")]
        [DataRow("a@b@c", "c@b@a")]
        [DataRow("a@b", "b@a")]
        [DataRow("", "")]
        [DataRow(null, "")]
        [DataRow("a", "a")]
        public void ReverseSummaryTest(string summary, string expect)
        {
            var result = ReverseSummary(summary);
            Assert.AreEqual(expect, result);
        }

        [DataTestMethod]
        [DataRow(
            @"c,,a,//---,中文,3,,2,1,//abc中文,中文,a,1,,中文,a,1",
            @"c,,a,//---,3,中文,,1,2,//abc中文,1,a,中文,,1,a,中文"
        )]
        [DataRow(@"a,b,中文,3,2,1", @"1,2,3,a,b,中文")]
        [DataRow(@"3,2,1", @"1,2,3")]
        public void SortPacListTest(string source, string expStr)
        {
            var testList = source.Split(new char[] { ',' }, StringSplitOptions.None);
            var expectList = expStr.Split(',');
            var result = SortPacList(testList);

            Assert.IsTrue(expectList.SequenceEqual(result));
        }

        [DataTestMethod]
        [DataRow("a::b:1233333", false, "127.0.0.1", 1080)]
        [DataRow("a::b:123", true, "a::b", 123)]
        [DataRow("[a::b]:123", true, "[a::b]", 123)]
        [DataRow("ab123", false, "127.0.0.1", 1080)]
        [DataRow("ab123:", false, "127.0.0.1", 1080)]
        [DataRow(":123", false, "127.0.0.1", 1080)]
        [DataRow(":", false, "127.0.0.1", 1080)]
        public void TryParseIPAddrTest(string address, bool expResult, string expIp, int expPort)
        {
            var result = TryParseAddress(address, out string ip, out int port);
            Assert.AreEqual(expResult, result);
            Assert.AreEqual(expIp, ip);
            Assert.AreEqual(expPort, port);
        }

        [TestMethod]
        public void AreEqualTest()
        {
            var minVal = VgcApis.Models.Consts.Config.FloatPointNumberTolerance;
            var a = 0.1;
            var b1 = a + minVal * 2;
            var b2 = a - minVal * 2;
            var c1 = a + minVal / 2;
            var c2 = a - minVal / 2;

            Assert.IsFalse(AreEqual(a, b1));
            Assert.IsFalse(AreEqual(a, b2));
            Assert.IsTrue(AreEqual(a, c1));
            Assert.IsTrue(AreEqual(a, c2));
        }

        [DataTestMethod]
        [DataRow(1, 2, 0.6, (long)(0.6 * 1 + 0.4 * 2))]
        [DataRow(-1, 2, 0.6, 2)]
        [DataRow(1, -2, 0.6, 1)]
        [DataRow(-1, -2, 0.6, -1)]
        public void IntegerSpeedtestMeanTest(long first, long second, double weight, long expect)
        {
            var result = SpeedtestMean(first, second, weight);
            Assert.AreEqual(expect, result);
        }

        [DataTestMethod]
        [DataRow(0.1, 0.2, 0.3, 0.1 * 0.3 + 0.2 * 0.7)]
        [DataRow(-0.1, 0.2, 0.3, 0.2)]
        [DataRow(0.1, -0.2, 0.3, 0.1)]
        [DataRow(-0.1, -0.2, 0.3, -0.1)]
        public void DoubleSpeedtestMeanTest(
            double first,
            double second,
            double weight,
            double expect
        )
        {
            var result = SpeedtestMean(first, second, weight);
            Assert.IsTrue(AreEqual(expect, result));
        }

        [DataTestMethod]
        [DataRow(@"o,o.14,o.11,o.1,o.3,o.4", @"o,o.1,o.3,o.4,o.11,o.14")]
        [DataRow(@"b3.2,b3.1.3,a1", @"a1,b3.1.3,b3.2")]
        [DataRow(@"b3,b10,a1", @"a1,b10,b3")]
        [DataRow(@"b,a,1,,", @",,1,a,b")]
        [DataRow(@"c.10.a,a,c.3.b,c.3.a", @"a,c.3.a,c.3.b,c.10.a")]
        public void JsonKeyComparerTest(string rawKeys, string rawExpects)
        {
            var keyList = rawKeys.Split(',').ToList();
            var expect = rawExpects.Split(',');

            keyList.Sort((a, b) => JsonKeyComparer(a, b));

            for (int i = 0; i < keyList.Count; i++)
            {
                Assert.AreEqual(expect[i], keyList[i]);
            }
        }

        [DataTestMethod]
        [DataRow(@"abeec", @"abc", 3)]
        [DataRow(@"abeecee", @"abc", 3)]
        [DataRow(@"eabec", @"abc", 5)]
        [DataRow(@"aeebc", @"abc", 5)]
        [DataRow(@"eeabc", @"abc", 7)]
        [DataRow(@"", @"", 1)]
        [DataRow(@"abc", @"", 1)]
        [DataRow(@"abc", @"abc", 1)]
        public void MeasureSimilarityTest(string source, string partial, long expect)
        {
            var result = MeasureSimilarity(source, partial);
            Assert.AreEqual(expect, result);
        }

        [DataTestMethod]
        [DataRow("EvABk文,tv字vvc", "字文", false)]
        [DataRow("EvABk文,tv字vvc", "ab字", true)]
        [DataRow("ab vvvc", "bc", true)]
        [DataRow("abc", "ac", true)]
        [DataRow("", "a", false)]
        [DataRow("", "", true)]
        public void PartialMatchTest(string source, string partial, bool expect)
        {
            var result = PartialMatchCi(source, partial);
            Assert.AreEqual(expect, result);
        }

        [DataTestMethod]
        [DataRow(@"http://abc.com", @"http")]
        [DataRow(@"https://abc.com", @"https")]
        [DataRow(@"VMess://abc.com", @"vmess")]
        public void GetLinkPrefixTest(string link, string expect)
        {
            var prefix = GetLinkPrefix(link);
            Assert.AreEqual(expect, prefix);
        }

        [DataTestMethod]
        [DataRow(@"http://abc.com", LinkTypes.http)]
        [DataRow(@"vmess://abc.com", LinkTypes.vmess)]
        [DataRow(@"v2cfg://abc.com", LinkTypes.v2cfg)]
        [DataRow(@"linkTypeNotExist://abc.com", LinkTypes.unknow)]
        [DataRow(@"abc.com", LinkTypes.unknow)]
        [DataRow(@"ss://abc.com", LinkTypes.ss)]
        public void DetectLinkTypeTest(string link, LinkTypes expect)
        {
            var linkType = DetectLinkType(link);
            Assert.AreEqual(expect, linkType);
        }

        [DataTestMethod]
        [DataRow(-4, -1)]
        [DataRow(-65535, -1)]
        [DataRow(-65535, -1)]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(4, 3)]
        [DataRow(8, 4)]
        [DataRow(16, 5)]
        [DataRow(65535, 16)]
        [DataRow(65536, 17)]
        public void GetLenInBitsOfIntTest(int value, int expect)
        {
            var len = GetLenInBitsOfInt(value);
            Assert.AreEqual(expect, len);
        }

#if DEBUG
        [TestMethod]
        public void GetFreePortMultipleThreadsTest()
        {
            ConcurrentDictionary<int, bool> ports = new ConcurrentDictionary<int, bool>();

            void worker()
            {
                for (int i = 0; i < 200; i++)
                {
                    var freePort = GetFreeTcpPort();
                    if (!ports.TryAdd(freePort, true))
                    {
                        Assert.Fail();
                    }
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
                            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, port: freePort);
                            socket.Bind(ep);
                            Task.Delay(10).Wait();
                        }
                    }
                    catch
                    {
                        Assert.Fail();
                    }
                }
            }

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 30; i++)
            {
                tasks.Add(RunInBackground(worker));
            }

            Task.WaitAll(tasks.ToArray());
        }
#endif

        [TestMethod]
        public void GetFreePortSingleThreadTest()
        {
            List<int> ports = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                int port = GetFreeTcpPort();
                Assert.AreEqual(true, port > 0);
                Assert.AreEqual(false, ports.Contains(port));
                ports.Add(port);
            }
        }

#if DEBUG
        [TestMethod]
        public void LazyGuyChainedTaskTest()
        {
            var str = "";

            void secTask(Action done)
            {
                Task.Delay(200).Wait();
                str += "2";
                done();
            }

            void firstTask(Action done)
            {
                Task.Delay(200).Wait();
                str += "1";
                secTask(done);
            }

            var alex = new VgcApis.Libs.Tasks.LazyGuy(firstTask, 1000, 300);

            str = "";
            alex.Deadline();
            alex.Throttle();
            alex.Deadline();
            alex.Throttle();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual("12", str);

            str = "";
            alex.Postpone();
            alex.Deadline();
            alex.Postpone();
            alex.Throttle();
            Task.Delay(500).Wait();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual("12", str);

            str = "";
            alex.Postpone();
            Task.Delay(500).Wait();
            alex.Postpone();
            Task.Delay(500).Wait();
            alex.Postpone();
            Task.Delay(500).Wait();
            alex.Postpone();
            Task.Delay(500).Wait();
            alex.Postpone();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual("12", str);

            str = "";
            alex.Deadline();
            alex.Deadline();
            alex.Deadline();
            Assert.AreEqual("", str);
            Task.Delay(5000).Wait();
            Assert.AreEqual("12", str);

            str = "";
            alex.Deadline();
            alex.Deadline();
            alex.Deadline();
            alex.ForgetIt();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual("", str);
            alex.PickItUp();

            str = "";
            alex.Throttle();
            alex.Throttle();
            alex.Throttle();
            alex.ForgetIt();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            var ok = str == "" || str == "12";
            Assert.IsTrue(ok);
            alex.PickItUp();

            str = "";
            alex.Throttle();
            Task.Delay(100).Wait();
            alex.Throttle();
            alex.Throttle();
            Assert.AreEqual("", str);
            Task.Delay(5000).Wait();
            Assert.AreEqual("1212", str);
        }
#endif

#if DEBUG
        [TestMethod]
        public void LazyGuySingleTaskTest()
        {
            var str = "";

            void task()
            {
                Task.Delay(200).Wait();
                str += ".";
            }

            var adam = new VgcApis.Libs.Tasks.LazyGuy(task, 1000, 300);

            str = "";
            adam.Postpone();
            Task.Delay(500).Wait();
            Console.WriteLine("500 x 1");
            adam.Postpone();
            Task.Delay(500).Wait();
            Console.WriteLine("500 x 2");
            adam.Postpone();
            Task.Delay(500).Wait();
            Console.WriteLine("500 x 3");
            adam.Postpone();
            Task.Delay(500).Wait();
            Console.WriteLine("500 x 4");
            adam.Postpone();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual(".", str);

            str = "";
            adam.Deadline();
            Task.Delay(10).Wait();
            adam.Deadline();
            Task.Delay(10).Wait();
            adam.Deadline();
            Task.Delay(10).Wait();
            adam.Deadline();
            Task.Delay(10).Wait();
            adam.Deadline();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual(".", str);

            str = "";
            adam.Deadline();
            Task.Delay(3000).Wait();
            Assert.AreEqual(".", str);

            str = "";
            adam.Deadline();
            adam.ForgetIt();
            Task.Delay(3000).Wait();
            Assert.AreEqual("", str);
            adam.PickItUp();

            str = "";
            adam.Throttle();
            adam.ForgetIt();
            Task.Delay(1000).Wait();
            Assert.AreEqual("", str);
            adam.PickItUp();

            str = "";
            adam.Throttle();
            Task.Delay(50).Wait(); // wait for task spin up
            adam.Throttle();
            adam.Throttle();
            adam.Throttle();
            adam.Throttle();
            Assert.AreEqual("", str);
            Task.Delay(3000).Wait();
            Assert.AreEqual("..", str);

            str = "";
            adam.Throttle();
            Task.Delay(1000).Wait();
            Assert.AreEqual(".", str);
        }
#endif

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("11,22,abc")]
        public void CloneTest(string orgStr)
        {
            var org = orgStr?.Split(',').ToList();
            var clone = Clone(org);
            var sClone = SerializeObject(clone);
            var sOrg = SerializeObject(org);
            Assert.AreEqual(sOrg, sClone);
        }

        [DataTestMethod]
        [DataRow("0", 0)]
        [DataRow("-1", -1)]
        [DataRow("str-1.234", 0)]
        [DataRow("-1.234str", 0)]
        [DataRow("-1.234", -1)]
        [DataRow("1.432", 1)]
        [DataRow("1.678", 2)]
        [DataRow("-1.678", -2)]
        public void Str2Int(string value, int expect)
        {
            Assert.AreEqual(expect, VgcApis.Misc.Utils.Str2Int(value));
        }
    }
}
