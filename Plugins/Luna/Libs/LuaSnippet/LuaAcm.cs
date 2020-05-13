using AutocompleteMenuNS;

namespace Luna.Libs.LuaSnippet
{
    internal sealed class LuaAcm : AutocompleteMenu
    {
        private readonly BestMatchSnippets bestMatchSnippets;

        public LuaAcm(BestMatchSnippets bestMatchSnippets) : base()
        {
            this.bestMatchSnippets = bestMatchSnippets;
        }

        #region public methods
        public void SetIsEnableCodeAnalyze(bool isEnable)
        {
            bestMatchSnippets.SetIsEnableCodeAnalyze(isEnable);
        }

        #endregion
    }
}
