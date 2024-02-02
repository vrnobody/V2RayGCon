using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Misc;

namespace VgcApisTests.MiscTests
{
    [TestClass]
    public class FetchTests
    {
        [DataTestMethod]
#if DEBUG
        [DataRow("https://www.baidu.com/")]
#else
        [DataRow("https://www.github.com/")]
#endif
        public void FetchTest(string url)
        {
            var html = Utils.Fetch(url, -1, -1);
            Assert.AreEqual(false, string.IsNullOrEmpty(html));
        }

        [DataTestMethod]
#if DEBUG
        [DataRow("https://www.baidu.com/", 3)]
#else
        [DataRow("https://www.github.com/", 3)]
#endif
        public void FetchBatchTest(string url, int times)
        {
            var urls = new List<string>();
            for (int i = 0; i < times; i++)
            {
                urls.Add(url);
            }

            List<string> htmls = new List<string>();
            try
            {
                htmls = Utils.ExecuteInParallel(
                    urls,
                    (u) =>
                    {
                        return Utils.Fetch(u, -1, -1);
                    }
                );
            }
            catch
            {
                Assert.Fail();
            }

            foreach (var html in htmls)
            {
                Assert.AreEqual(false, string.IsNullOrEmpty(html));
            }
        }

        [DataTestMethod]
        [DataRow("http://a.com/b/c/", "/d/e/abc.html", "http://a.com/d/e/abc.html")]
        [DataRow("vv/b/c/", "/e/abc.html", "/e/abc.html")]
        [DataRow("http://a.com/c", "/d/abc.html", "http://a.com/d/abc.html")]
        public void PatchUrlTest(string url, string relativeUrl, string expect)
        {
            var patchedUrl = Utils.PatchHref(url, relativeUrl);
            Assert.AreEqual(expect, patchedUrl);
        }
    }
}
