using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static VgcApis.Misc.UI;

namespace VgcApisTests
{
    [TestClass]
    public class UiTests
    {
        [DataTestMethod]
        [DataRow(Keys.D0, 9)]
        [DataRow(Keys.D9, 8)]
        [DataRow(Keys.D5, 4)]
        [DataRow(Keys.NumPad0, 9)]
        [DataRow(Keys.NumPad9, 8)]
        [DataRow(Keys.NumPad1, 0)]
        public void TryParseNumKeyToIndexTest(Keys keyCode, int exp)
        {
            var ok = TryParseNumKeyToIndex(keyCode, out int index);
            Assert.IsTrue(ok);
            Assert.AreEqual(exp, index);
        }
    }
}
