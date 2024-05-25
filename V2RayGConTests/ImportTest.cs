﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

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
            false
        )]
        [DataRow(
            @"{'a':1,'b':[{'a':{'c':['1','2','3'],'b':1}},{'b':1}],'c':3}",
            @"{'a':1,'b':[{'a':{'c':['1','2']}}]}",
            true
        )]
        [DataRow(@"{'a':'1'}", @"{'a':'1'}", true)]
        [DataRow(
            @"{'a':1,'b':[{'a':{'a':1,'b':1}},{'b':1}],'c':3}",
            @"{'a':1,'b':[{'a':{'c':1}}]}",
            false
        )]
        [DataRow(
            @"{'a':1,'b':[{'a':{'a':1,'b':1}},{'b':1}],'c':3}",
            @"{'a':1,'b':[{'b':1}]}",
            true
        )]
        [DataRow(@"{'a':1,'b':2,'c':3}", @"{'a':1,'b':2}", true)]
        [DataRow(@"{'a':1,'b':2,'d':3}", @"{'a':1,'b':2,'c':3}", false)]
        [DataRow(@"{}", @"{}", true)]
        public void ContainsTest(string main, string sub, bool expect)
        {
            var m = JObject.Parse(main);
            var s = JObject.Parse(sub);
            Assert.AreEqual(expect, VgcApis.Misc.Utils.Contains(m, s));
        }

        [DataTestMethod]
        [DataRow(".")]
        [DataRow("a.1")]
        [DataRow("b.")]
        public void CreateJObjectFailTest(string path)
        {
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                VgcApis.Misc.Utils.CreateJObject(path);
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
            var result = VgcApis.Misc.Utils.CreateJObject(path, c);
            var e = JObject.Parse(expect);
            Assert.AreEqual(true, JToken.DeepEquals(result, e));
        }

        [DataTestMethod]
        [DataRow("a.0.b", @"{a:[{b:{}}]}")]
        [DataRow("a", @"{'a':{}}")]
        [DataRow("a.b", @"{'a':{'b':{}}}")]
        [DataRow("a.b.c.d.e", @"{'a':{'b':{'c':{'d':{'e':{}}}}}}")]
        [DataRow("", @"{}")]
        public void CreateJObjectNormalTest(string path, string expect)
        {
            var result = VgcApis.Misc.Utils.CreateJObject(path);
            var e = JObject.Parse(expect);
            Assert.AreEqual(true, JToken.DeepEquals(result, e));
        }

        [DataTestMethod]
        [DataRow(@"{'a':1}", "b")]
        [DataRow(@"{}", "")]
        public void TryExtractJObjectPartFailTest(string json, string path)
        {
            var stat = VgcApis.Misc.Utils.TryExtractJObjectPart(
                JObject.Parse(json),
                path,
                out JObject part
            );
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
            var stat = VgcApis.Misc.Utils.TryExtractJObjectPart(source, path, out JObject part);
            var e = JObject.Parse(expect);

            Assert.AreEqual(true, stat);
            Assert.AreEqual(true, JToken.DeepEquals(e, part));
        }

        [DataTestMethod]
        [DataRow("a.b.", "a.b", "")]
        [DataRow("a.b.c", "a.b", "c")]
        [DataRow(".", "", "")]
        [DataRow(".b", "", "b")]
        [DataRow("", "", "")]
        public void PathParseTest(string path, string parent, string key)
        {
            var v = VgcApis.Misc.Utils.ParsePathIntoParentAndKey(path);
            Assert.AreEqual(parent, v.Item1);
            Assert.AreEqual(key, v.Item2);
        }

        [DataTestMethod]
        [DataRow(
            @"{routing:{settings:{rules:[{a:[1,3]}]}}}",
            @"{routing:{settings:{rules:[{a:[2]}]}}}",
            @"{routing:{settings:{rules:[{a:[2]},{a:[1,3]}]}}}"
        )]
        [DataRow(
            @"{routing:{settings:{rules:[{b:2},{a:[1,2,3,{c:1}]}]}}}",
            @"{routing:{settings:{rules:[{a:[1,2,3,{c:1}]},{c:2}]}}}",
            @"{routing:{settings:{rules:[{a:[1,2,3,{c:1}]},{c:2},{b:2}]}}}"
        )]
        [DataRow(
            @"{routing:{settings:{rules:[{b:2},{a:1}]}}}",
            @"{routing:{settings:{rules:[{a:1}]}}}",
            @"{routing:{settings:{rules:[{a:1},{b:2}]}}}"
        )]
        [DataRow(
            @"{'a':1,'arr':[1,2,3],'routing':{'a':1,'settings':{'c':1,'rules':[{'b':2}]}}}",
            @"{'a':2,'b':1,'arr':null,'routing':{'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}",
            @"{'a':2,'b':1,'arr':null,'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1},{'b':2}]}}}"
        )]
        [DataRow(
            @"{'arr':[1,2],'routing':{'a':1,'settings':{'c':1,'rules':[{'b':2}]}}}",
            @"{'arr':[4,5,6],'routing':{'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}",
            @"{'arr':[4,5,6],'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1},{'b':2}]}}}"
        )]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'c':1,'rules':[{'b':2}]}}}",
            @"{'routing':{'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1},{'b':2}]}}}"
        )]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'c':1,'rules':[{'a':1}]}}}",
            @"{'routing':{'b':2,'settings':{'c':2,'rules':[]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}"
        )]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'c':1,'rules':null}}}",
            @"{'routing':{'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'c':2,'rules':[{'a':1}]}}}"
        )]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'rules':[]}}}",
            @"{'routing':{'b':2,'settings':{'rules':[]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'rules':[]}}}"
        )]
        [DataRow(
            @"{'routing':{'a':1}}",
            @"{'routing':{'b':2,'settings':{'rules':[{'b':1}]}}}",
            @"{'routing':{'a':1,'b':2,'settings':{'rules':[{'b':1}]}}}"
        )]
        [DataRow(
            @"{'routing':{'a':1,'settings':{'rules':[{'b':2}]}}}",
            @"{'routing':{'a':2,'settings':{'rules':[{'b':1}]}}}",
            @"{'routing':{'a':2,'settings':{'rules':[{'b':1},{'b':2}]}}}"
        )]
        [DataRow(
            @"{'inboundDetour':[{'a':1}]}",
            @"{'inboundDetour':[{'b':1}]}",
            @"{'inboundDetour':[{'b':1},{'a':1}]}"
        )]
        [DataRow(@"{'a':1,'b':1}", @"{'b':2}", @"{'a':1,'b':2}")]
        [DataRow(@"{'a':1}", @"{'b':1}", @"{'a':1,'b':1}")]
        [DataRow(@"{}", @"{}", @"{}")]
        [DataRow(
            @"{'inboundDetour':[{'a':1}],'outboundDetour':null}",
            @"{'inboundDetour':null,'outboundDetour':[{'b':1}]}",
            @"{'inboundDetour':[{'a':1}],'outboundDetour':[{'b':1}]}"
        )]
        public void CombineConfigTest(string left, string right, string expect)
        {
            // outboundDetour inboundDetour
            var body = JObject.Parse(left);
            var mixin = JObject.Parse(right);

            VgcApis.Misc.Utils.CombineConfigWithRoutingInFront(ref body, mixin);

            var e = JObject.Parse(expect);
            // var dbg = body.ToString();
            var equal = JToken.DeepEquals(e, body);

            Assert.AreEqual(true, equal);

            // test whether mixin changed
            var orgMixin = JObject.Parse(right);
            var same = JToken.DeepEquals(orgMixin, mixin);
            Assert.AreEqual(true, same);
        }

        [DataTestMethod]
        // deepequals regardless dictionary's keys order
        [DataRow(
            @"{a:'123',b:null,c:{b:1}}",
            @"{a:null,c:{a:1,c:1}}",
            @"{a:null,b:null,c:{a:1,b:1,c:1}}"
        )]
        [DataRow(
            @"{a:'123',b:null,c:{a:2,b:1}}",
            @"{a:null,b:'123',c:{a:1,c:1}}",
            @"{a:null,b:'123',c:{a:1,b:1,c:1}}"
        )]
        [DataRow(@"{}", @"{}", @"{}")]
        [DataRow(@"{a:'123',b:null}", @"{a:null,b:'123'}", @"{a:null,b:'123'}")]
        [DataRow(@"{a:[1,2],b:{}}", @"{a:[3],b:{a:[1,2,3]}}", @"{a:[3,2],b:{a:[1,2,3]}}")]
        public void MergeJson(string bodyStr, string mixinStr, string expect)
        {
            var body = JObject.Parse(bodyStr);
            VgcApis.Misc.Utils.MergeJson(body, JObject.Parse(mixinStr));

            var e = JObject.Parse(expect);
            Assert.AreEqual(true, JToken.DeepEquals(body, e));
        }
    }
}
