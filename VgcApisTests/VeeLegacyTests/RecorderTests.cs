using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.VeeLegacyTests
{
    [TestClass]
    public class RecorderTests
    {
        [TestMethod]
        public void RecorderNormalTest()
        {
            var recorder = new VgcApis.Libs.Infr.Recorder(5);

            for (int i = 0; i < 20; i++)
            {
                recorder.Add(i);
            }

            var cur = recorder.Current();
            Assert.AreEqual(19, cur);

            for (int i = 18; i > 14; i--)
            {
                var ok = recorder.Backward();
                Assert.IsTrue(ok);
                var c = recorder.Current();
                Assert.AreEqual(i, c);
            }

            Assert.IsFalse(recorder.Backward());
            for (int i = 16; i < 18; i++)
            {
                var ok = recorder.Forward();
                Assert.IsTrue(ok);
                var c = recorder.Current();
                Assert.AreEqual(i, c);
            }

            recorder.Add(1);
            Assert.IsFalse(recorder.Forward());
            Assert.IsTrue(recorder.Backward());
            for (int i = 16; i > 14; i--)
            {
                var ok = recorder.Backward();
                Assert.IsTrue(ok);
                var c = recorder.Current();
                Assert.AreEqual(i, c);
            }
        }
    }
}
