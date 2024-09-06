using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VgcApis.Libs.Infr.KwFilterComps
{
    public interface ISimpleFilter
    {
        List<Interfaces.ICoreServCtrl> Filter(List<Interfaces.ICoreServCtrl> coreServs);

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
