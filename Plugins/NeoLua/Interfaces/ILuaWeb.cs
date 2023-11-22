using HtmlAgilityPack;
using System.Collections.Generic;

namespace NeoLuna.Interfaces
{
    public interface ILuaWeb
    {
        // ---- 预计2024-06删除 ----
        List<string> ExtractV2cfgLinks(string text);
        List<string> ExtractVmessLinks(string text);
        List<string> ExtractSsLinks(string text);

        // ------------------------


        /// <summary>
        /// html-agility-pack
        /// </summary>
        /// <param name="html">string content</param>
        /// <returns>HtmlNode</returns>
        HtmlNode ParseHtml(string html);

        /// <summary>
        /// html-agility-pack
        /// </summary>
        /// <returns>html-agility-pack HtmlDocument</returns>
        HtmlDocument GetHtmlParser();

        /// <summary>
        /// 获取一个随机的空闲TCP端口
        /// </summary>
        /// <returns>-1: 没可用端口， number: 空闲端口号</returns>
        int GetFreeTcpPort();

        /// <summary>
        /// 从字符串中提取出Base64编码的内容
        /// </summary>
        /// <param name="text">一段字符串</param>
        /// <param name="minLen">Base64内容最小长度</param>
        /// <returns>Base64编码的内容</returns>
        List<string> ExtractBase64String(string text, int minLen);

        /// <summary>
        /// 从字符串中提取出Base64编码的内容
        /// </summary>
        /// <param name="text">一段字符串</param>
        /// <returns>Base64编码的内容</returns>
        List<string> ExtractBase64String(string text);

        /// <summary>
        /// 从字符串中提取全部支持的链接
        /// </summary>
        /// <param name="text">一段字符串</param>
        /// <returns></returns>
        List<string> ExtractAllShareLinks(string text);

        /// <summary>
        /// 从字符串中提取出全部(prefix)://...链接
        /// 如果prefix不是支持的类型则返回nil
        /// 如果是支持的类型但找不到链接则返回空List
        /// </summary>
        /// <param name="text">一段字符串</param>
        /// <param name="prefix">链接名</param>
        /// <returns>nil / List&lt;string></returns>
        List<string> ExtractShareLinks(string text, string prefix);

        /// <summary>
        /// 下载url网页的内容
        /// </summary>
        /// <param name="url">一个网址</param>
        /// <returns>指定网址的内容</returns>
        string Fetch(string url);

        /// <summary>
        /// 下载url网页的内容
        /// </summary>
        /// <param name="url">一个网址</param>
        /// <param name="proxyPort">代理端口</param>
        /// <param name="milliSeconds">超时</param>
        /// <returns>指定网址的内容</returns>
        string Fetch(string url, int milliSeconds);

        /// <summary>
        /// 下载url网页的内容
        /// </summary>
        /// <param name="url">一个网址</param>
        /// <param name="proxyPort">代理端口</param>
        /// <param name="milliSeconds">超时</param>
        /// <returns>指定网址的内容</returns>
        string Fetch(string url, int proxyPort, int milliSeconds);

        /// <summary>
        /// 下载url网页的内容
        /// </summary>
        /// <param name="url">一个网址</param>
        /// /// <param name="host">代理主机地址</param>
        /// <param name="port">代理端口</param>
        /// <param name="milliSeconds">超时</param>
        /// /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>指定网址的内容</returns>
        string FetchHttp(
            string url,
            string host,
            int proxyPort,
            int milliSeconds,
            string username,
            string password
        );

        /// <summary>
        /// 使用SOCKS5代理协议通过代理端口下载网页内容
        /// </summary>
        /// <param name="url">一个网址</param>
        /// <param name="host">代理主机地址</param>
        /// <param name="prot">代理端口</param>
        /// <param name="ms">超时</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>指定网址的内容</returns>
        string FetchSocks5(
            string url,
            string host,
            int port,
            int ms,
            string username,
            string password
        );

        /// <summary>
        /// 用自定义配置下载网页内容
        /// </summary>
        /// <param name="rawConfig">自定义配置</param>
        /// <param name="url">网址</param>
        /// <returns></returns>
        string FetchWithCustomConfig(string rawConfig, string url);

        /// <summary>
        /// 用自定义配置下载网页内容
        /// </summary>
        /// <param name="rawConfig">自定义配置</param>
        /// <param name="coreName">自定义core</param>
        /// <param name="url">网址</param>
        /// <param name="timeout">超时（毫秒）</param>
        /// <returns></returns>
        string FetchWithCustomConfig(string rawConfig, string coreName, string url, int timeout);

        // Download("http://baidu.com", "d:\index.html")
        /// <summary>
        /// 下载url网页并存成文件
        /// </summary>
        /// <param name="url">一个网址</param>
        /// <param name="filename">保存的文件名</param>
        /// <returns>是否下载成功</returns>
        bool Download(string url, string filename);

        /// <summary>
        /// 下载url网页并存成文件
        /// </summary>
        /// <param name="url">一个网址</param>
        /// <param name="filename">保存的文件名</param>
        /// <param name="millSeconds">超时</param>
        /// <returns>是否下载成功</returns>
        bool Download(string url, string filename, int millSeconds);

        /// <summary>
        /// 下载url网页并存成文件
        /// </summary>
        /// <param name="url">一个网址</param>
        /// <param name="filename">保存的文件名</param>
        /// <param name="proxyPort">代理端口</param>
        /// <param name="millSeconds">超时</param>
        /// <returns>是否下载成功</returns>
        bool Download(string url, string filename, int proxyPort, int millSeconds);

        // the first running http server's port number
        /// <summary>
        /// 获取当前运行的服务器的代理端口号
        /// </summary>
        /// <returns>代理端口号</returns>
        int GetProxyPort();

        // patch short href to full href
        // e.g. url = "http://baidu.com/" href = "/index.html" result = "http://baidu.com/index.html"
        string PatchHref(string url, string href);

        /// <summary>
        /// Ping("1.2.4.8", 3000) Ping("baidu.com", 1234)
        /// </summary>
        /// <param name="dest">IP 或者 host</param>
        /// <param name="ms">超时(毫秒)</param>
        /// <returns>成功: xx(ms) 失败: -1</returns>
        long Ping(string dest, int ms);

        /// <summary>
        /// Ping("1.2.4.8") Ping("baidu.com")
        /// </summary>
        /// <param name="dest">IP 或者 host</param>
        /// <returns>成功: xx(ms) 失败: -1</returns>
        long Ping(string dest);

        /// <summary>
        /// 向url发送post请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="text">请求内容</param>
        /// <returns>服务器回复内容</returns>
        string Post(string url, string text);

        /// <summary>
        /// 向url发送post请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="text">请求内容</param>
        /// <param name="timeout">超时</param>
        /// <returns>服务器回复内容</returns>
        string Post(string url, string text, int timeout);

        // using bing.com to search sth.
        string Search(string keywords, int first, int proxyPort);

        /// <summary>
        /// 同Tcping(url, ms, proxyPort)
        /// </summary>
        bool Tcping(string url, int ms);

        /// <summary>
        /// 其实是HTTP ping，访问url指定的网站，在指定时间内下载到数据即返回true。
        /// </summary>
        /// <param name="url">指定的网址</param>
        /// <param name="ms">超时（毫秒）</param>
        /// <param name="proxyPort">代理端口</param>
        /// <returns></returns>
        bool Tcping(string url, int ms, int proxyPort);

        /// <summary>
        /// 带超时的下载测试（直连）
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="timeout">超时</param>
        /// <param name="kib">下载数据大小</param>
        /// <returns>下载时长</returns>
        long TimedDownloadTesting(string url, int timeout, int kib);

        /// <summary>
        /// 带超时的下载测试（HTTP代理协议)
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="timeout">超时</param>
        /// <param name="kib">下载数据大小</param>
        /// <param name="proxyPort">代理端口</param>
        /// <returns>下载时长</returns>
        long TimedDownloadTesting(string url, int timeout, int kib, int proxyPort);

        /// <summary>
        /// 带超时的下载测试（HTTP代理协议+账号密码)
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="timeout">超时</param>
        /// <param name="kib">下载数据大小</param>
        /// <param name="proxyPort">代理端口</param>
        /// <param name="username">账号</param>
        /// <param name="password">密码</param>
        /// <returns>下载时长</returns>
        long TimedDownloadTestingHttp(
            string url,
            int timeout,
            int kib,
            int proxyPort,
            string username,
            string password
        );

        /// <summary>
        /// 带超时的下载测试（SOCKS5代理协议+账号密码）
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="timeout">超时</param>
        /// <param name="kib">下载数据大小</param>
        /// <param name="proxyPort">代理端口</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>下载时长</returns>
        long TimedDownloadTestingSocks5(
            string url,
            int timeout,
            int kib,
            int proxyPort,
            string username,
            string password
        );

        // direct connect
        /// <summary>
        /// 更新订阅
        /// </summary>
        /// <returns>获得新服务器数量</returns>
        int UpdateSubscriptions();

        // through proxy
        /// <summary>
        /// 通过代理更新订阅
        /// </summary>
        /// <param name="proxyPort">代理端口</param>
        /// <returns>获得新服务器数量</returns>
        int UpdateSubscriptions(int proxyPort);
    }
}
