namespace VgcApis.Models.Consts
{
    public static class Patterns
    {
        public static readonly string VgcWebUiUrlPrefix = @"webui";
        public static readonly string VgcWebUiUrl = $"^{VgcWebUiUrlPrefix}s?://.*";

        public static string V2RayCoreReleaseAssets =
            @"/releases/download/(v[\.0-9a-zA-Z\-]+)/[v2X]+ray-windows-64.zip";

        public static readonly string GitHubReadmeUrl =
            @"https://github.com(/[^/]+/[^/]+)/blob(/.*)";
        public static readonly string GitHuhFileUrl = @"https://github.com(/[^/]+/[^/]+)/blob(/.*)";

        public static readonly string ExtractRemarksFromSubscriptUrl = @"[^\w]remarks=([%\w]+)";
        public static readonly string ExtractAliasFromSubscriptUrl = @"//[^/]+/([^/]+)";

        public static readonly string JsonSnippetSearchPattern = @"[:/,_\.\-\\\*\$\w]";

        public static readonly string LuaSnippetSearchPattern = @"[\w\.:\(]";

        public static readonly string NonAlphabets = @"[^0-9a-zA-Z]";

        public static readonly string Base64Characters =
            @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/-_";

        public static readonly string Base64NonStandard = @"[A-Za-z0-9+/_\-]+={0,3}";

        public static readonly string SsShareLinkContent =
            @"[a-zA-Z0-9@#\-\.\?\,\'\/\+&;%\$_\[\]\+:=]+";

        public static readonly string UriContent = @"[\w\-\.\:\?\,\'\[\]+&;@%\$#_=]+";

        // [/]for alpn=http/1.1
        // \u0026 => &
        public static readonly string UriContentNonStandard = @"[\w\-\.\:\?\,\'\[\]+&;@%\$#_=/\\]+";

        public static readonly string HttpUrl =
            @"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&;%\$#_=]*)?";

        public static readonly string Base64Standard =
            @"(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{4})";
    }
}
