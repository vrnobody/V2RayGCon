using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;


namespace V2RayGCon.Test
{
    [TestClass]
    public class ImportTest
    {
        [DataTestMethod]
        [DataRow(@"{'a':null}", @"{'a':''}", false)]
        [DataRow(
            @"{'a':1,'b':[{'a':{'c':['1','2','3'],'b':1}},{'b':1}],'c':3}",
            @"{'a':1,'b':[{'a':{'c':{'1':'2'}}}]}",
            false)]
        [DataRow(
            @"{'a':1,'b':[{'a':{'c':['1','2','3'],'b':1}},{'b':1}],'c':3}",
            @"{'a':1,'b':[{'a':{'c':['1','2']}}]}",
            true)]
        [DataRow(@"{'a':'1'}", @"{'a':'1'}", true)]
        [DataRow(
            @"{'a':1,'b':[{'a':{'a':1,'b':1}},{'b':1}],'c':3}",
            @"{'a':1,'b':[{'a':{'c':1}}]}",
            false)]
        [DataRow(
            @"{'a':1,'b':[{'a':{'a':1,'b':1}},{'b':1}],'c':3}",
            @"{'a':1,'b':[{'b':1}]}",
            true)]
        [DataRow(@"{'a':1,'b':2,'c':3}", @"{'a':1,'b':2}", true)]
        [DataRow(@"{'a':1,'b':2,'d':3}", @"{'a':1,'b':2,'c':3}", false)]
        [DataRow(@"{}", @"{}", true)]
        public void ContainsTest(string main, string sub, bool expect)
        {
            var m = JObject.Parse(main);
            var s = JObject.Parse(sub);
            Assert.AreEqual<bool>(expect, Lib.Utils.Contains(m, s));
        }

        [DataTestMethod]
        [DataRow(".")]
        [DataRow("a.1")]
        [DataRow("b.")]
        public void CreateJObjectFailTest(string path)
        {
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                Lib.Utils.CreateJObject(path);
            });
        }

        [DataTestMethod]
        [DataRow("a.0.b", @"{}", @"{a:[{b:{}}]}")]
        [DataRow("a.0.b", @"{c:1}", @"{a:[{b:{c:1}}]}")]
        [DataRow("a", @"[{c:1}]", @"{'a':[{c:1}]}")]
        [DataRow("", @"[{c:1}]", @"{}")]
        public void CreateJObjectWithChildTest(string path, string child, string expect)
        {
            var c = JToken.Parse(child);
            var result = Lib.Utils.CreateJObject(path, c);
            var e = JObject.Parse(expect);
            Assert.AreEqual<bool>(true, JObject.DeepEquals(result, e));
        }

        [DataTestMethod]
        [DataRow("a.0.b", @"{a:[{b:{}}]}")]
        [DataRow("a", @"{'a':{}}")]
        [DataRow("a.b", @"{'a':{'b':{}}}")]
        [DataRow("a.b.c.d.e", @"{'a':{'b':{'c':{'d':{'e':{}}}}}}")]
        [DataRow("", @"{}")]
        public void CreateJObjectNormalTest(string path, string expect)
        {
            var result = Lib.Utils.CreateJObject(path);
            var e = JObject.Parse(expect);
            Assert.AreEqual<bool>(true, JObject.DeepEquals(result, e));
        }


        [DataTestMethod]
        [DataRow(@"{'a':1}", "b")]
        [DataRow(@"{}", "")]
        public void TryExtractJObjectPartFailTest(string json, string path)
        {
            var stat = Lib.Utils.TryExtractJObjectPart(JObject.Parse(json), path, out JObject part);
            Assert.AreEqual(false, stat);
            Assert.AreEqual(null, part);
        }

        [DataTestMethod]
        [DataRow(@"{'a':{'b':{'c':[]}},'b':1}", "a", @"{'a':{'b':{'c':[]}}}")]
        [DataRow(@"{'a':1,'b':1}", "a", @"{'a':1}")]
        [DataRow(@"{'a':1}", "a", @"{'a':1}")]
        public void TryExtractJObjectPartNormalTest(string json, string path, string expect)
        {
            var source = JObject.Parse(json);
            var stat = Lib.Utils.TryExtractJObjectPart(source, path, out JObject part);
            var e = JObject.Parse(expect);

            Assert.AreEqual(true, stat);
            Assert.AreEqual<bool>(true, JObject.DeepEquals(e, part));
        }

        [DataTestMethod]
        [DataRow("a.b.", "a.b", "")]
        [DataRow("a.b.c", "a.b", "c")]
        [DataRow(".", "", "")]
        [DataRow(".b", "", "b")]
        [DataRow("", "", "")]
        public void PathParseTest(string path, string parent, string key)
        {
            var v = Lib.Utils.ParsePathIntoParentAndKey(path);
            Assert.AreEqual<string>(parent, v.Item1);
            Assert.AreEqual<string>(key, v.Item2);

        }

        [DataTestMethod]
        [DataRow(
            @"{routing:{settings:{rules:[{a:[1,3]}]}}}",
            @"{routing:{settings:{rules:[{a:[2]}]}}}",
            @"{routing:{settings:{rules:[{a:[2]},{a:[1,3]}]}}}")]
        [DataRow(
            @"{routing:{settings:{rules:[{b:2},{a:[1,2,3,{c:1}]}]}}}",
            @"{routing:{settings:{rules:[{a:[1,2,3,{c:1}]},{c:2}]}}}",
            @"{routing:{settings:{rules:[{a:[1,2,3,{c:1}]},{c:2},{b:2}]}}}")]
        [DataRow(
            @"{routing:{settings:{rules:[{b:2},{a:1}]}}}",
            @"{routing:{settings:{rules:[{a:1}]}}}",
            @"{routing:{settings:{rules:[{a:1},{b:2}]}}}")]
        [DataRow(
            @"{'a':1,'arr':[1,2,3],'routing':{'a':1,'settings':{'c':1,'rules':[{'b':2}]}}}",
            @"{'a':2,'b':1,'arr':null,'routing':{'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}",
            @"{'a':2,'b':1,'arr':null,'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1},{'b':2}]}}}")]
        [DataRow(
            @"{'arr':[1,2],'routing':{'a':1,'settings':{'c':1,'rules':[{'b':2}]}}}",
            @"{'arr':[4,5,6],'routing':{'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}",
            @"{'arr':[4,5,6],'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1},{'b':2}]}}}")]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'c':1,'rules':[{'b':2}]}}}",
            @"{'routing':{'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1},{'b':2}]}}}")]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'c':1,'rules':[{'a':1}]}}}",
            @"{'routing':{'b':2,'settings':{'c':2,'rules':[]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}")]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'c':1,'rules':null}}}",
            @"{'routing':{'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}")]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'rules':[]}}}",
            @"{'routing':{'b':2,'settings':{'rules':[]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'rules':[]}}}")]
        [DataRow(
            @"{'routing':{'a':1}}",
            @"{'routing':{'b':2,'settings':{'rules':[{'b':1}]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'rules':[{'b':1}]}}}")]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'rules':[{'b':2}]}}}",
            @"{'routing':{'a':2,'settings':{'rules':[{'b':1}]}}}",
            @"{'routing':{'a':2,'settings':{'rules':[{'b':1},{'b':2}]}}}")]
        [DataRow(
            @"{'inboundDetour':[{'a':1}],'outboundDetour':null}",
            @"{'inboundDetour':null,'outboundDetour':[{'b':1}]}",
            @"{'inboundDetour':[{'a':1}],'outboundDetour':[{'b':1}]}")]
        [DataRow(
            @"{'inboundDetour':[{'a':1}]}",
            @"{'inboundDetour':[{'b':1}]}",
            @"{'inboundDetour':[{'b':1},{'a':1}]}")]
        [DataRow(@"{'a':1,'b':1}", @"{'b':2}", @"{'a':1,'b':2}")]
        [DataRow(@"{'a':1}", @"{'b':1}", @"{'a':1,'b':1}")]
        [DataRow(@"{}", @"{}", @"{}")]
        public void CombineConfigTest(string left, string right, string expect)
        {
            // outboundDetour inboundDetour
            var body = JObject.Parse(left);
            var mixin = JObject.Parse(right);

            Lib.Utils.CombineConfigWithRoutingInFront(ref body, mixin);

            var e = JObject.Parse(expect);
            var dbg = body.ToString();
            var equal = JObject.DeepEquals(e, body);

            Assert.AreEqual(true, equal);

            // test whether mixin changed
            var orgMixin = JObject.Parse(right);
            var same = JObject.DeepEquals(orgMixin, mixin);
            Assert.AreEqual(true, same);
        }

        [TestMethod]
        public void DetectJArrayTest()
        {
            var domainOverride = Service.Cache.Instance.
                tpl.LoadExample("inTpl.sniffing.destOverride");
            var isArray = domainOverride is JArray;
            var isObject = domainOverride is JObject;

            string[] list = null;
            if (isArray)
            {
                list = domainOverride.ToObject<string[]>();
            }

            Assert.AreEqual(true, isArray);
            Assert.AreEqual(false, isObject);
            Assert.AreNotEqual(null, list);
        }

        [DataTestMethod]

        // deepequals regardless dictionary's keys order 
        [DataRow(@"{a:'123',b:null,c:{b:1}}",
            @"{a:null,c:{a:1,c:1}}",
            @"{a:null,b:null,c:{a:1,b:1,c:1}}")]

        [DataRow(@"{a:'123',b:null,c:{a:2,b:1}}",
            @"{a:null,b:'123',c:{a:1,c:1}}",
            @"{a:null,b:'123',c:{a:1,b:1,c:1}}")]

        [DataRow(@"{}", @"{}", @"{}")]
        [DataRow(@"{a:'123',b:null}", @"{a:null,b:'123'}", @"{a:null,b:'123'}")]
        [DataRow(@"{a:[1,2],b:{}}", @"{a:[3],b:{a:[1,2,3]}}", @"{a:[3,2],b:{a:[1,2,3]}}")]
        public void MergeJson(string bodyStr, string mixinStr, string expect)
        {
            var body = JObject.Parse(bodyStr);
            Lib.Utils.MergeJson(ref body, JObject.Parse(mixinStr));

            var e = JObject.Parse(expect);
            Assert.AreEqual(true, JObject.DeepEquals(body, e));
        }

        [TestMethod]
        public void ImportItemList2JObject()
        {
            Model.Data.ImportItem GenItem(bool includeSpeedTest, bool includeActivate, string url, string alias)
            {
                return new Model.Data.ImportItem
                {
                    isUseOnActivate = includeActivate,
                    isUseOnSpeedTest = includeSpeedTest,
                    isUseOnPackage = false,
                    url = url,
                    alias = alias,
                };
            }
            var items = new List<List<Model.Data.ImportItem>>();
            var expects = new List<string>();

            items.Add(new List<Model.Data.ImportItem> {
                GenItem(true,true,"a.com","a"),
                GenItem(false,true,"b.com","b"),
                GenItem(true,false,"c.com",""),
            });

            expects.Add(@"{'v2raygcon':{'import':{'a.com':'a','c.com':''}}}");

            items.Add(new List<Model.Data.ImportItem> { });
            expects.Add(@"{'v2raygcon':{'import':{}}}");

            for (var i = 0; i < items.Count; i++)
            {
                var expect = JObject.Parse(expects[i]);
                var json = Lib.Utils.ImportItemList2JObject(items[i], true, false, false);
                var result = JObject.DeepEquals(expect, json);
                Assert.AreEqual(true, result);
            }
        }

        [TestMethod]
        public void ParseImportTest()
        {
            var data = new Dictionary<string, string>();

            void kv(string name, string key, string val)
            {
                var json = JObject.Parse(@"{}");
                if (data.ContainsKey(name))
                {
                    json = JObject.Parse(data[name]);
                }
                json[key] = val;
                data[name] = json.ToString(Newtonsoft.Json.Formatting.None);
            }

            void import(string name, string url)
            {
                var json = JObject.Parse(@"{}");
                if (data.ContainsKey(name))
                {
                    json = JObject.Parse(data[name]);
                }
                var imp = Lib.Utils.GetKey(json, "v2raygcon.import");
                if (imp == null || !(imp is JObject))
                {
                    json["v2raygcon"] = JObject.Parse(@"{'import':{}}");

                }
                json["v2raygcon"]["import"][url] = "";
                data[name] = json.ToString(Newtonsoft.Json.Formatting.None);
            }

            List<string> fetcher(List<string> keys)
            {
                var result = new List<string>();

                foreach (var key in keys)
                {
                    try
                    {
                        // Debug.WriteLine(key);
                        result.Add(data[key]);
                    }
                    catch
                    {
                        throw new System.Net.WebException();
                    }
                }

                return result;
            }

            bool eq(JObject left, JObject right)
            {
                var jleft = left.DeepClone() as JObject;
                var jright = right.DeepClone() as JObject;
                jleft["v2raygcon"] = null;
                jright["v2raygcon"] = null;
                return JObject.DeepEquals(jleft, jright);
            }

            JObject parse(string key, int depth = 3)
            {
                var config = JObject.Parse(data[key]);
                return Lib.Utils.ParseImportRecursively(fetcher, config, depth);
            }

            void check(string expect, string value)
            {
                Assert.AreEqual(true, eq(JObject.Parse(expect), parse(value)));
            }

            data["base"] = "{'v2raygcon':{}}";
            kv("a", "a", "1");
            kv("b", "b", "1");
            kv("baser", "r", "1");
            import("baser", "baser");
            import("mixAB", "a");
            import("mixAB", "b");
            import("mixC", "mixAB");
            kv("mixC", "a", "2");
            kv("mixC", "c", "1");
            import("mixCAb", "mixC");
            import("mixCAb", "mixAB");
            kv("mixCAb", "c", "2");
            import("mixABC", "a");
            import("mixABC", "b");
            import("mixABC", "mixC");
            import("final", "mixAB");
            import("final", "mixC");
            import("final", "mixCAb");
            import("final", "baser");
            kv("final", "msg", "omg");

            check(@"{'a':'2','b':'1','c':'2','r':'1','msg':'omg'}", "final");
            check(@"{'a':'2','b':'1','c':'1'}", "mixABC");
            check(@"{'a':'1','b':'1','c':'2'}", "mixCAb");
            check(@"{'a':'2','c':'1','b':'1'}", "mixC");
            check(@"{'a':'1','b':'1'}", "mixAB");
            check(data["base"], "base");
            check(data["baser"], "baser");
        }
    }
}
