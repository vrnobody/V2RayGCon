using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.MiscTests
{
    [TestClass]
    public class ResetIndexTest
    {
#if DEBUG
        [TestMethod]
        public void ResetIndexKeepOriginalOrderTest()
        {
            var datas = new List<Data>();
            for (int i = 1; i < 10; i++)
            {
                datas.Add(new Data() { tag = $"{i}" });
            }

            VgcApis.Misc.Utils.ResetIndex(datas);
            foreach (var data in datas)
            {
                Assert.AreEqual(data.tag, $"{data.index}");
            }
        }
#endif

        class Data : VgcApis.Interfaces.IHasIndex
        {
            public string tag = "";
            public double index = 1;

            public Data() { }

            public double GetIndex()
            {
                return index;
            }

            public void SetIndex(double value)
            {
                this.index = value;
            }
        }
    }
}
