using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.IComponentOfTests
{
    [TestClass]
    public class IPlugableTests
    {
        public IPlugableTests() { }

        [TestMethod]
        public void ConstructTest()
        {
            var c1 = new CompLv3();
            var c2 = new CompLv3("param");
            var c3 = new CompLv3("param")
            {
                Name = "something else",
            };

            Assert.AreEqual("def property", c1.Name);
            Assert.AreEqual("param", c2.Name);
            Assert.AreEqual("something else", c3.Name);
        }

        [TestMethod]
        public void NormalTest()
        {
            var container = new Container();
            var compLv1 = new CompLv1();
            var compLv2 = new CompLv2();

            container.AddChild(compLv1);
            compLv1.AddChild(compLv2);

            var nc = container.Name();
            var n1 = container.GetChild<CompLv1>().Name();
            var n2 = container.GetChild<CompLv1>().GetChild<CompLv2>().Name();

            var c2 = compLv2.Name();
            var c1 = compLv2.GetParent().Name();
            var cc = compLv2.GetParent().GetParent().Name();

            Assert.AreEqual(nc, cc);
            Assert.AreEqual(n2, c2);
            Assert.AreEqual(n1, c1);

            Assert.AreEqual(null, container.GetParent());

            container.Dispose();

        }
    }
}
