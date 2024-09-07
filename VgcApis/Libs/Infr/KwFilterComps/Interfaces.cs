using System.Collections.Generic;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    public interface IFilter
    {
        IReadOnlyCollection<Interfaces.ICoreServCtrl> Filter(
            IReadOnlyCollection<Interfaces.ICoreServCtrl> coreServs
        );
    }

    public interface ISimpleFilter : IFilter
    {
        Highlighter GetHighlighter();
    }

    public interface IAdvanceFilter<TContent, TOpertor, TValue> : ISimpleFilter
    {
        #region for unit testing
        TOpertor GetOperator();

        HashSet<TContent> GetTagNames();

        bool MatchCore(TValue value);
        #endregion
    }
}
