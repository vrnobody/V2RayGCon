using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NeoLunaTests
{
    [TestClass]
    public class SnippetsCacheTest
    {
        [TestMethod]
        public void CreateBestMatchSnippetsTest()
        {
            var editor = new ScintillaNET.Scintilla();
            var sc = new NeoLuna.Libs.LuaSnippet.SnippetsCache();
            try
            {
                var best = sc.CreateBestMatchSnippets(editor);
                // parse snippets successfully
                Assert.IsNotNull(best);
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
