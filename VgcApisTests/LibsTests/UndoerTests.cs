using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VgcApisTests.LibsTests
{
    [TestClass]
    public class UndoerTests
    {
        [TestMethod]
        public void IntTest()
        {
            var c = 0;
            var u = new VgcApis.Libs.Infr.Undoer<int>(2);
            Assert.AreEqual(0, u.Count());
            Assert.AreEqual(0, u.Position());
            Assert.IsFalse(u.TryRedo(out var _));
            Assert.IsFalse(u.TryUndo(out var _));

            u.Push(1);
            Assert.IsFalse(u.TryRedo(out c));
            Assert.IsFalse(u.TryUndo(out c));
            Assert.AreEqual(1, u.Count());
            Assert.AreEqual(1, u.Position());

            u.Push(2);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(2, u.Position());
            Assert.IsTrue(u.TryUndo(out c));
            Assert.AreEqual(1, c);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(1, u.Position());
            Assert.IsTrue(u.TryRedo(out c));
            Assert.AreEqual(2, c);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(2, u.Position());

            u.Push(3);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(2, u.Position());
            Assert.IsTrue(u.TryUndo(out c));
            Assert.AreEqual(2, c);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(1, u.Position());
            Assert.IsTrue(u.TryRedo(out c));
            Assert.AreEqual(3, c);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(2, u.Position());
        }

        [TestMethod]
        public void StringTest()
        {
            var c = "";
            var u = new VgcApis.Libs.Infr.Undoer<string>(2);
            Assert.AreEqual(0, u.Count());
            Assert.AreEqual(0, u.Position());
            Assert.IsFalse(u.TryRedo(out var _));
            Assert.IsFalse(u.TryUndo(out var _));

            Assert.IsTrue(u.Push("a"));
            Assert.IsFalse(u.TryRedo(out c));
            Assert.IsFalse(u.TryUndo(out c));
            Assert.AreEqual(1, u.Count());
            Assert.AreEqual(1, u.Position());

            Assert.IsFalse(u.Push("a"));
            Assert.IsTrue(u.Push("b"));
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(2, u.Position());

            Assert.IsTrue(u.TryUndo(out c));
            Assert.AreEqual("a", c);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(1, u.Position());
            Assert.IsFalse(u.Push("a"));

            Assert.IsTrue(u.TryRedo(out c));
            Assert.AreEqual("b", c);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(2, u.Position());
            Assert.IsFalse(u.Push("b"));

            Assert.IsTrue(u.Push("c"));
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(2, u.Position());
            Assert.IsTrue(u.TryUndo(out c));
            Assert.AreEqual("b", c);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(1, u.Position());
            Assert.IsTrue(u.TryRedo(out c));
            Assert.AreEqual("c", c);
            Assert.AreEqual(2, u.Count());
            Assert.AreEqual(2, u.Position());
        }
    }
}
