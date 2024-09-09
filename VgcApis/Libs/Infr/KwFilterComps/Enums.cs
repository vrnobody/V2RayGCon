namespace VgcApis.Libs.Infr.KwFilterComps
{
    #region expr
    internal enum ExprTokenTypes
    {
        OPEN_PAREN,
        CLOSE_PAREN,
        BINARY_OP,
        LITERAL,
        EXPR_END,
    }
    #endregion

    #region strings

    public enum StringTagNames
    {
        Title,
        Name,
        Summary,
        Mark,
        Remark,
        Tag1,
        Tag2,
        Tag3,
        Selected,
        Core,
    }

    public enum StringOperators
    {
        IS,
        HAS,
        LIKE,
        STARTS,
        ENDS,
        MATCH,
    }

    #endregion

    #region numbers

    public enum NumberTagNames
    {
        Index,
        Latency,
        Upload,
        Download,
        Port,
        Modify,
    }

    public enum NumberOperators
    {
        LargerThen,
        SmallerThen,
        Between,
        Is,
    }

    #endregion
}
