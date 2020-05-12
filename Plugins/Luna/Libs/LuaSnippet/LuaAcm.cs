using AutocompleteMenuNS;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Luna.Libs.LuaSnippet
{
    internal sealed class LuaAcm :
        VgcApis.BaseClasses.Disposable
    {
        const string SearchPattern =
            VgcApis.Models.Consts.Patterns.LuaSnippetSearchPattern;

        List<LuaKeywordSnippets> keywordCache;
        List<LuaFuncSnippets> functionCache;
        List<LuaSubFuncSnippets> subFunctionCache;
        List<LuaImportClrSnippets> importClrCache;
        List<ApiFunctionSnippets> apiFunctionCache;

        public LuaAcm()
        {
            GenSnippetCaches();
        }

        #region public methods
        public AutocompleteMenu BindToEditor(Scintilla editor)
        {
            var imageList = new System.Windows.Forms.ImageList();
            imageList.Images.Add(Properties.Resources.KeyDown_16x);
            imageList.Images.Add(Properties.Resources.Method_16x);
            imageList.Images.Add(Properties.Resources.Class_16x);

            var acm = new AutocompleteMenu()
            {
                SearchPattern = SearchPattern,
                MaximumSize = new Size(300, 200),
                ToolTipDuration = 20000,
                ImageList = imageList,
            };

            acm.TargetControlWrapper = new ScintillaWrapper(editor);

            acm.SetAutocompleteItems(
                new BestMatchSnippets(
                    editor,
                    SearchPattern,
                    apiFunctionCache,
                    functionCache,
                    keywordCache,
                    subFunctionCache,
                    importClrCache));

            return acm;
        }

        #endregion

        #region private methods
        string GetFilteredLuaKeywords() =>
            VgcApis.Models.Consts.Lua.LuaKeywords
            .Replace("do", "")
            .Replace("then", "")
            .Replace("end", "");

        List<string> GetAllNameapaces() => VgcApis.Misc.Utils.GetAllAssembliesType()
            .Select(t => t.Namespace)
            .Distinct()
            .Where(n => !(
                string.IsNullOrEmpty(n)
                || n.StartsWith("<")
                || n.StartsWith("AutocompleteMenuNS")
                || n.StartsWith("AutoUpdaterDotNET")
                || n.StartsWith("Internal.Cryptography")
                || n.StartsWith("Luna")
                || n.StartsWith("Pacman")
                || n.StartsWith("ProxySetter")
                || n.StartsWith("ResourceEmbedderCompilerGenerated")
                || n.StartsWith("Statistics")
                || n.StartsWith("V2RayGCon")
                || n.StartsWith("VgcApis")
            ))
            .ToList();

        List<string> GetAllAssembliesName()
        {
            var nsps = GetAllNameapaces();
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(asm => asm.FullName)
                .Where(fn => !string.IsNullOrEmpty(fn) && nsps.Where(nsp => fn.StartsWith(nsp)).FirstOrDefault() != null)
                .Union(nsps)
                .OrderBy(n => n)
                .Select(n => $"import('{n}')")
                .ToList();
        }

        /*

        List<string> GetAllAssembliesName() => VgcApis.Misc.Utils.GetAllAssembliesType()
            .Select(t => t.Namespace)
            .Distinct()
            .Where(n => !(
                string.IsNullOrEmpty(n)
                || n.StartsWith("<")
                || n.StartsWith("AutocompleteMenuNS")
                || n.StartsWith("AutoUpdaterDotNET")
                || n.StartsWith("Internal.Cryptography")
                || n.StartsWith("Luna")
                || n.StartsWith("Pacman")
                || n.StartsWith("ProxySetter")
                || n.StartsWith("ResourceEmbedderCompilerGenerated")
                || n.StartsWith("Statistics")
                || n.StartsWith("V2RayGCon")
                || n.StartsWith("VgcApis")
            ))
            .Select(n => $"import('{n}')")
            .OrderBy(n => n)
            .ToList();
            */

        List<LuaImportClrSnippets> GenLuaImportClrSnippet() =>
            GetAllAssembliesName()
                .Select(e =>
                {
                    try
                    {
                        return new LuaImportClrSnippets(e);
                    }
                    catch { }
                    return null;
                })
                .Where(e => e != null)
                .ToList();


        List<string> GenKeywords(
            IEnumerable<string> initValues) =>
            new StringBuilder(VgcApis.Models.Consts.Lua.LuaModules)
            .Append(@" ")
            .Append(GetFilteredLuaKeywords())
            .ToString()
            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Union(initValues)
            .OrderBy(e => e)
            .ToList();

        List<LuaFuncSnippets> GenLuaFunctionSnippet() =>
            VgcApis.Models.Consts.Lua.LuaFunctions
            .Replace("dofile", "")
            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .OrderBy(s => s)
            .Select(e =>
            {
                try
                {
                    return new LuaFuncSnippets(e);
                }
                catch { }
                return null;
            })
            .Where(e => e != null)
            .ToList();

        List<LuaSubFuncSnippets> GenLuaSubFunctionSnippet() =>
            VgcApis.Models.Consts.Lua.LuaSubFunctions
            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .OrderBy(s => s)
            .Select(e =>
            {
                try
                {
                    return new LuaSubFuncSnippets($"{e}()", ".");
                }
                catch { }
                return null;
            })
            .Where(e => e != null)
            .ToList();

        void GenSnippetCaches()
        {
            var apis = new List<Tuple<string, Type>>
            {
                new Tuple<string,Type>("mailbox", typeof(VgcApis.Interfaces.Lua.ILuaMailBox)),
                new Tuple<string,Type>("mail", typeof(VgcApis.Interfaces.Lua.ILuaMail)),
                new Tuple<string,Type>("Sys", typeof(VgcApis.Interfaces.Lua.ILuaSys)),
                new Tuple<string,Type>("Misc", typeof(VgcApis.Interfaces.Lua.ILuaMisc)),
                new Tuple<string,Type>("Server", typeof(VgcApis.Interfaces.Lua.ILuaServer)),
                new Tuple<string,Type>("Web", typeof(VgcApis.Interfaces.Lua.ILuaWeb)),
                new Tuple<string,Type>("Signal", typeof(VgcApis.Interfaces.Lua.ILuaSignal)),
                new Tuple<string, Type>("coreServ",typeof(VgcApis.Interfaces.ICoreServCtrl)),
                new Tuple<string, Type>("coreConfiger",typeof(VgcApis.Interfaces.CoreCtrlComponents.IConfiger)),
                new Tuple<string, Type>("coreCtrl",typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreCtrl)),
                new Tuple<string, Type>("coreState",typeof(VgcApis.Interfaces.CoreCtrlComponents.ICoreStates)),
                new Tuple<string, Type>("coreLogger",typeof(VgcApis.Interfaces.CoreCtrlComponents.ILogger)),
            };

            keywordCache = GenKeywordSnippetItems(GenKeywords(apis.Select(e => e.Item1)));
            functionCache = GenLuaFunctionSnippet();
            subFunctionCache = GenLuaSubFunctionSnippet();
            importClrCache = GenLuaImportClrSnippet();
            apiFunctionCache = apis
               .SelectMany(api => GenApiFunctionSnippetItems(api.Item1, api.Item2))
               .ToList();
        }

        List<LuaKeywordSnippets> GenKeywordSnippetItems(IEnumerable<string> keywords) =>
            keywords
            .OrderBy(k => k)
            .Select(e => new LuaKeywordSnippets(e))
            .ToList();

        IEnumerable<ApiFunctionSnippets> GenApiFunctionSnippetItems(
            string apiName, Type type) =>
            VgcApis.Misc.Utils.GetPublicMethodNameAndParam(type)
            .OrderBy(info => info.Item2)  // item2 = method name
            .Select(info => new ApiFunctionSnippets(
                info.Item1, // return type
                apiName,
                info.Item2, // methodName,
                info.Item3, // paramStr,
                info.Item4, // paramWithType,
                @"")
            );

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            // acm will dispose it self.
            // acm?.Dispose();
        }
        #endregion
    }
}
