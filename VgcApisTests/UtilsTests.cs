using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static VgcApis.Misc.Utils;
using static VgcApis.Models.Datas.Enums;

namespace VgcApisTests
{
    [TestClass]
    public class UtilsTests
    {
        #region short date int

        [DataTestMethod]
        [DataRow("中aGVsbG8=文aGVsbG8\naGVsbG8=💛aGVsbG8=", 10, 0)]
        [DataRow("中aGVsbG8=文aGVsbG8\naGVsbG8=💛aGVsbG8=", 0, 4)]
        [DataRow("aGVsbG8=", 0, 1)]
        [DataRow("", 0, 0)]
        [DataRow("", 1, 0)]
        [DataRow(null, 1, 0)]
        public void ExtractBase64StringsTest(string text, int minLen, int expCount)
        {
            // b64(hello) = "aGVsbG8="
            var matches = ExtractBase64Strings(text, minLen);
            Assert.AreEqual(expCount, matches.Count);
            foreach (var match in matches)
            {
                Assert.AreEqual("aGVsbG8", match);
            }
        }

        [TestMethod]
        public void ToShortDateIntTest()
        {
            var now = DateTime.UtcNow.ToLocalTime();
            long r = ToShortDateInt(now, 0);
            long exp = Str2Int(now.ToString("yyyyMMdd"));
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, -1111);
            exp = Str2Int(now.ToString("yyyyMMdd"));
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 1);
            exp = Str2Int(now.ToString("yyyyMM")) * 100 + 1;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 1);
            exp = Str2Int(now.ToString("yyyyMM")) * 100 + 1;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 12);
            exp = Str2Int(now.ToString("yyyyMM")) * 100 + 12;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 100);
            exp = Str2Int(now.ToString("yyyy")) * 10000 + 100;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 123);
            exp = Str2Int(now.ToString("yyyy")) * 10000 + 123;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 4400);
            exp = Str2Int(now.ToString("yyyy")) * 10000 + 4400;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 4444);
            exp = Str2Int(now.ToString("yyyy")) * 10000 + 4444;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 12344);
            exp = (Str2Int(now.ToString("yyyy")) / 100) * 1000000 + 12344;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 10044);
            exp = (Str2Int(now.ToString("yyyy")) / 100) * 1000000 + 10044;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 12300);
            exp = (Str2Int(now.ToString("yyyy")) / 100) * 1000000 + 12300;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 102344);
            exp = (Str2Int(now.ToString("yyyy")) / 100) * 1000000 + 102344;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 240230);
            exp = (Str2Int(now.ToString("yyyy")) / 100) * 1000000 + 240230;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 990044);
            exp = (Str2Int(now.ToString("yyyy")) / 100) * 1000000 + 990044;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 1102344);
            exp = 1102344;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 20240230);
            exp = 20240230;
            Assert.AreEqual(exp, r);

            r = ToShortDateInt(now, 2221102344);
            exp = 2221102344;
            Assert.AreEqual(exp, r);
        }

        #endregion

        [DataTestMethod]
        [DataRow("", false, 0, "")]
        [DataRow("1", false, 0, "1")]
        [DataRow("#-1", true, -1, "")]
        [DataRow("#-0", true, 0, "")]
        [DataRow("#0", true, 0, "")]
        [DataRow("#", true, 0, "")]
        [DataRow("#123", true, 123, "")]
        [DataRow("##123", false, 0, "#123")]
        [DataRow("a B cDEf 1  2,,,3中文😀", false, 0, "abcdef12,,,3中文😀")]
        [DataRow("#a B cDEf 1  2,,,3中文😀", true, 0, "")]
        [DataRow("##a #B cD#Ef 1  2,,,3中文😀", false, 0, "#a#bcd#ef12,,,3中文😀")]
        public void TryParseSearchKeywordAsIndexTest(
            string s,
            bool expIsNumber,
            int expIndex,
            string expKeyword
        )
        {
            var isNumber = TryParseSearchKeywordAsIndex(s, out var index, out var keyword);
            Assert.AreEqual(expIsNumber, isNumber);
            if (isNumber)
            {
                Assert.AreEqual(expIndex, index);
            }
            else
            {
                Assert.AreEqual(expKeyword, keyword);
            }
        }

        [DataTestMethod]
        [DataRow(@"", false, "")]
        [DataRow(@"w1.2.3", false, "")]
        [DataRow(@"v1.2.3.", false, "")]
        [DataRow(@"v1.2.3.4.5", false, "")]
        [DataRow(@"v1.2.3.4", true, "1.2.3.4")]
        [DataRow(@"v1.2-exp1", true, "1.2")]
        [DataRow(@"v1.2tttttt-exp1", true, "1.2")]
        [DataRow(@"v0.0.0.0-exp13", true, "0.0.0.0")]
        public void TryParseVersionStringTest(string str, bool success, string exp)
        {
            var ok = TryParseVersionString(str, out var ver);
            Assert.AreEqual(success, ok);
            if (ok)
            {
                var expVer = new Version(exp);
                Assert.IsTrue(expVer.Equals(ver));
            }
        }

        [DataTestMethod]
        [DataRow(@"http://abc.com", @"abc.com")]
        [DataRow(@"v://abc.com", @"abc.com")]
        [DataRow(@"vee://", @"")]
        [DataRow(@"vmess://abc", @"abc")]
        [DataRow(@"v2cfg://abc", @"abc")]
        [DataRow(@"http://abc", @"abc")]
        [DataRow(@"https://abc", @"abc")]
        [DataRow(@"any://://", @"://")]
        public void GetLinkBodyNormalTest(string link, string expect)
        {
            var body = GetLinkBody(link);
            Assert.AreEqual(expect, body);
        }

        [DataTestMethod]
        [DataRow(@"v/")]
        [DataRow(@":v/v/")]
        public void GetLinkBodyFailTest(string link)
        {
            try
            {
                var body = GetLinkBody(link);
            }
            catch
            {
                return;
            }
            Assert.Fail();
        }

        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("1", "1")]
        [DataRow("1 , 2", "1,2")]
        [DataRow(",  ,  ,", "")]
        [DataRow(",,,  ,1  ,  ,2,  ,3,,,", "1,2,3")]
        public void Str2JArray2Str(string value, string expect)
        {
            var array = Str2JArray(value);
            var str = JArray2Str(array);
            Assert.AreEqual(expect, str);
        }

        [DataTestMethod]
        [DataRow(0.1, 0.2, false)]
        [DataRow(0.000000001, 0.00000002, true)]
        [DataRow(0.001, 0.002, false)]
        [DataRow(-0.1, 0.1, false)]
        [DataRow(2, 2, true)]
        public void AreEqualTest(double a, double b, bool expect)
        {
            Assert.AreEqual(expect, AreEqual(a, b));
        }

        [DataTestMethod]
        [DataRow("0.0.0.0.", "0.0.0.0.")]
        [DataRow("0.0.1.11", "0.0.1.11")]
        [DataRow("0.0.1.0", "0.0.1")]
        [DataRow("0.0.0.1", "0.0.0.1")]
        [DataRow("0.0.0.0", "0.0")]
        public void TrimVersionStringTest(string version, string expect)
        {
            var result = TrimVersionString(version);
            Assert.AreEqual(expect, result);
        }

        [TestMethod]
        public void HashFunctionsBaselineTests()
        {
            var s = "hello123中文😀";
            Assert.AreEqual("32cf9edfde0bc4468f6dab923dc20d3c", Md5Hex(s));
            Assert.AreEqual(
                "3376a5a910db4818bae776cea3295e1797e4e2d9d1a8d1d6e773fcec8071cc9c",
                Sha256Hex(s)
            );
            Assert.AreEqual(
                "1498275515c38e6f4269e8602a1f5a9ad84690c523dd16d06cad8b0a7f55a71927d8587d947e5e9cd98da0e9736341fff976fd70cdd65389749a07e1ddc6530d",
                Sha512Hex(s)
            );

            Assert.AreEqual(
                "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4",
                Sha256Hex("1234")
            );
        }

        [DataTestMethod]
        [DataRow(
            "VAL中文UE中1=5V文ALU😀E2=6.He😀😀😀ll🤗🤩oWorld",
            "😀😀,中1,中文,🤗,2,He",
            "VAL,中文,UE,中1,=5V文ALU😀E,2,=6.,He,😀😀,😀ll,🤗,🤩oWorld"
        )]
        [DataRow("[a link|http://www.google.com]", "[,|,]", "[,a link,|,http://www.google.com,]")]
        [DataRow("VALUE1=5VALUE2=6.HelloWorld", ".,=,=,.", "VALUE1,=,5VALUE2,=,6,.,HelloWorld")]
        [DataRow(
            "VALUE1=5VALUE2=6.HelloWorld",
            "World,Hello,.,=,VALUE,,.,=,VALUE,,,,",
            "VALUE,1,=,5,VALUE,2,=,6,.,Hello,World"
        )]
        [DataRow("", ",,,,,", "")]
        public void SplitAndKeepTests(string s, string delimiters, string exp)
        {
            var delims = delimiters.Split(',');
            var list = SplitAndKeep(s, delims);
            var r = string.Join(",", list);
            Assert.AreEqual(exp, r);
        }

        [TestMethod]
        public void DoItLaterTest()
        {
            var d = 0;
            DoItLater(
                () =>
                {
                    d++;
                },
                500
            );
            Assert.AreEqual(0, d);
            Sleep(1000);
            Assert.AreEqual(1, d);
            DoItLater(
                () =>
                {
                    d += 2;
                },
                TimeSpan.FromMilliseconds(500)
            );
            Assert.AreEqual(1, d);
            Sleep(1000);
            Assert.AreEqual(3, d);
        }

        [DataTestMethod]
        [DataRow("::ffff:1.2.3.4", true)]
        [DataRow("[::ffff:1234:1234]", true)]
        [DataRow("2001:4860:4860::8888", true)]
        [DataRow("[2001:4860:4860::8888]", true)]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow("1.2.3.4", false)]
        [DataRow("1.2.3.4.5", false)]
        public void IsIpv6Test(string host, bool expected)
        {
            var r = IsIpv6(host);
            Assert.AreEqual(expected, r);
        }

        enum EnumForTest
        {
            replace = 0,
            concat = 1,
            union = 2,
            merge = 3,
            tag = 4,
        }

        [TestMethod]
        public void EnumTest()
        {
            var values = new string[] { "replace", "concat", "union", "merge", "tag" };

            for (int i = 0; i < values.Length; i++)
            {
                var s = values[i];
                Assert.IsTrue(TryParseEnum<EnumForTest>(s, out var em1));
                Assert.IsTrue(TryParseEnum<EnumForTest>(i, out var em2));
                Assert.AreEqual(em1, em2);
            }

            var list = EnumToList<EnumForTest>();

            Assert.AreEqual(values.Length, list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(values[i], list[i]);
            }
        }

        [DataTestMethod]
        [DataRow(@"😁=😁, 😁=😁, 😁=😁, ", 1)]
        [DataRow(@"", 0)]
        [DataRow(null, 0)]
        [DataRow(@"PATH=c:\\abc\,n_1=23", 2)]
        [DataRow(@"a=1 b=2", 1)]
        [DataRow(@"a=1, b=2", 2)]
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
            @"vless://1234@[1::2:3:4]:1234?security=reality\u0026amp;encryption=none\u0026headerType=none\u0026type=tcp\u0026flow=xtls-rprx-vision#1aA中😀",
            @"vless://1234@[1::2:3:4]:1234?security=reality&amp;encryption=none&headerType=none&type=tcp&flow=xtls-rprx-vision#1aA中😀"
        )]
        [DataRow(
            @"vless://1234@[1::2:3:4]:1234?security=reality\u0026encryption=none\u0026headerType=none\u0026type=tcp\u0026flow=xtls-rprx-vision#1aA中😀",
            @"vless://1234@[1::2:3:4]:1234?security=reality&encryption=none&headerType=none&type=tcp&flow=xtls-rprx-vision#1aA中😀"
        )]
        [DataRow(
            @"\u8ba1\u7b97\u673a\u2022\u7f51\u7edc\u2022\u6280\u672f\u7c7b",
            @"计算机•网络•技术类"
        )]
        [DataRow(@"\a\b2022\\c2022\\u2022\u\2022", @"\a\b2022\\c2022\•\u\2022")]
        [DataRow(
            @"\\\\u8ba1\u\u7B97\u673A\u2022\u7f51\U7edc\u2022\U6280\u672f\u1\u12\u123\u7c7b\ut\uu\u",
            @"\\\计\u算机•网络•技术\u1\u12\u123类\ut\uu\u"
        )]
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
        [DataRow("中iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii试", 34, "中iiiiiiiiiiiiiiiiiiiiiiiiiiiiii")]
        [DataRow("中aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa文", 34, "中aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [DataRow("中iiiii试", 6, "中ii")]
        [DataRow("中文aa测试", 8, "中文aa")]
        [DataRow("中文aa测试", 7, "中文a")]
        [DataRow("中文aa测试", 6, "中文")]
        [DataRow("中aaa文测试", 6, "中aa")]
        [DataRow("中aaa文测试", 5, "中a")]
        [DataRow("中文测试", 4, "中")]
        [DataRow("中文测试", 3, "")]
        [DataRow("a中文测试", 9, "a中文测试")]
        [DataRow("a中文测试", 7, "a中文")]
        [DataRow("a中文测试", 6, "a中")]
        [DataRow("a中文测试", 5, "a中")]
        [DataRow("a中文测试", 4, "a")]
        [DataRow("a中文测试", 3, "a")]
        [DataRow("a中文测试", 2, "")]
        [DataRow("a中文测试", 1, "")]
        [DataRow("a中文测试", 0, "")]
        [DataRow("a中文测试", -1, "")]
        [DataRow("aaaaaaaaa", 5, "aaa")]
        [DataRow("", 100, "")]
        public void AutoEllipsisTest(string org, int len, string expect)
        {
            var ellipsis = VgcApis.Models.Consts.AutoEllipsis.ellipsis;
            var cut = AutoEllipsis(org, len);
            var exp = expect;
            if (len < ellipsis.Length)
            {
                exp = "";
            }
            else if (org != expect)
            {
                exp = $"{expect}{ellipsis}";
            }
            Assert.AreEqual(exp, cut);
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
        [DataRow(
            @"中文 abc😀123",
            @"https://www.github.com/abc/a/b/c/d.html?remarks=%E4%B8%AD%E6%96%87%20abc%F0%9F%98%80123"
        )]
        [DataRow(
            @"中文 abc😀123",
            @"https://www.github.com/abc/a/b/c/d.html;remarks=%E4%B8%AD%E6%96%87%20abc%F0%9F%98%80123"
        )]
        [DataRow(@"中文", @"https://www.github.com/abc/a/b/c/d.html?&remarks=中文😀123abc")]
        [DataRow(
            @"1234",
            @"https://www.github.com/abc/a/b/c/d.html?&remarks=1234&remarks=中文😀123abc"
        )]
        [DataRow(@"abc", @"https://www.github.com/abc/a/b/c/d.htmlremarks=test")]
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
        [DataRow("20240902", 20240902)]
        [DataRow("-1.678", -2)]
        public void Str2IntTest(string value, int expect)
        {
            Assert.AreEqual(expect, VgcApis.Misc.Utils.Str2Int(value));
        }
    }
}
