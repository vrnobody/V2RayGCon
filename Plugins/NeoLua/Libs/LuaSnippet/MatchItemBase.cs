using AutocompleteMenuNS;

namespace NeoLuna.Libs.LuaSnippet
{
    internal class MatchItemBase : AutocompleteItem
    {
        public MatchItemBase(string keyword)
            : base(keyword)
        {
            if (keyword == null)
            {
                throw new System.ArgumentException(@"Keyword is null!");
            }

            Text = keyword;
        }

        public string GetLowerText() => Text.ToLower();

        public long MeasureSimilarityCi(string fragment) =>
            VgcApis.Misc.Utils.MeasureSimilarityCi(Text, fragment);
    }
}
