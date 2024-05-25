namespace VgcApis.Models.Consts
{
    public static class Patterns
    {
        public static string V2RayCoreReleaseAssets =
            @"/[v2X]+ray-core/releases/download/(v[\.0-9]+)/[v2X]+ray-windows-64.zip";

        public const string GitHubReadmeUrl = @"https://github.com(/[^/]+/[^/]+)/blob(/.*)";
        public const string GitHuhFileUrl = @"https://github.com(/[^/]+/[^/]+)/blob(/.*)";

        public const string ExtractAliasFromSubscriptUrl = @"//[^/]+/([^/]+)";

        public const string JsonSnippetSearchPattern = @"[:/,_\.\-\\\*\$\w]";

        public const string LuaSnippetSearchPattern = @"[\w\.:\(]";

        public const string NonAlphabets = @"[^0-9a-zA-Z]";

        public const string Base64NonStandard = @"[A-Za-z0-9+/_\-]+={0,3}";

        public const string SsShareLinkContent = @"[a-zA-Z0-9@#\-\.\?\,\'\/\+&;%\$_\[\]\+:=]+";

        public const string UriContent = @"[\w\-\.\:\?\,\'\[\]+&;@%\$#_=]+";

        // [/]for alpn=http/1.1
        // \u0026 => &
        public const string UriContentNonStandard = @"[\w\-\.\:\?\,\'\[\]+&;@%\$#_=/\\]+";

        public const string HttpUrl =
            @"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&;%\$#_=]*)?";

        public const string Base64Standard =
            @"(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{4})";
    }
}
