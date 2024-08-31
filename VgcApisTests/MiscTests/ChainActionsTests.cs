using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VgcApis.Misc;

namespace VgcApisTests.MiscTests
{
    [TestClass]
    public class SearchItemTests
    {
        [TestMethod]
        public void ChainActionsSyncTest()
        {
            var data = new List<int>();
            for (int i = 1; i <= 10; i++)
            {
                data.Add(i * 10);
            }

            var tail = false;
            var result = new List<int>();
            var delta = 3;

            void done()
            {
                tail = true;
            }

            void job(int idx, Action next)
            {
                result.Add(data[idx] + delta);
                next();
            }

            Utils.InvokeChainActions(data.Count, job, done);
            Assert.IsTrue(tail);
            for (int i = 0; i < data.Count; i++)
            {
                var exp = data[i] + delta;
                Assert.AreEqual(exp, result[i]);
            }
        }
    }
}
