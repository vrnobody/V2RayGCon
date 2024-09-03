namespace VgcApis.Libs.Infr.KwFilterComps
{
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
    }

    public enum StringOperators
    {
        IS,
        NOT,
        HAS,
        HASNOT,
        LIKE,
        UNLIKE,
    }

    #endregion
    #region numbers

    public enum NumberTagNames
    {
        Latency,
        Upload,
        Download,
    }

    public enum NumberOperators
    {
        LargerThen,
        SmallerThen,
        Between,
        Is,
        Not,
    }

    #endregion
}
