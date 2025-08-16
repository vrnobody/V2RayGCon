using System.Collections.Generic;
using NLua;
using VgcApis.Interfaces;

namespace Luna.Interfaces
{
    public interface ILuaMisc : ILogable
    {
        #region winform things
        /// <summary>
        /// 在UI线程执行lua函数
        /// </summary>
        /// <param name="func">lua函数</param>
        void Invoke(LuaFunction func);

        /// <summary>
        /// 复制内容到剪切板
        /// </summary>
        /// <param name="content">字符串内容</param>
        /// <returns></returns>
        bool CopyToClipboard(string content);

        /// <summary>
        /// 从剪切板读取字符串
        /// </summary>
        /// <returns>字符串内容</returns>
        string ReadFromClipboard();

        /// <summary>
        /// 调出Json编辑器窗口
        /// </summary>
        /// <param name="config">预置内容</param>
        void ShowFormTextEditor(string config);

        /// <summary>
        /// 调出设定及二维码窗口
        /// </summary>
        /// <param name="coreServ">一个服务器</param>
        void ShowFormServerSettings(ICoreServCtrl coreServ);

        /// <summary>
        /// 调出简易编辑器窗口
        /// </summary>
        /// <param name="coreServ">一个服务器</param>
        void ShowFormSimpleEditor(ICoreServCtrl coreServ);

        /// <summary>
        /// 调出选项窗口
        /// </summary>
        void ShowFormOption();

        /// <summary>
        /// 调出Luna脚本管理器窗口
        /// </summary>
        void ShowFormLunaMgr();

        /// <summary>
        /// 调出Luna脚本编辑器窗口
        /// </summary>
        void ShowFormLunaEditor();

        /// <summary>
        /// 调出主窗口
        /// </summary>
        void ShowFormMain();

        /// <summary>
        /// 调出密钥生成器窗口
        /// </summary>
        void ShowFormKeyGen();

        /// <summary>
        /// 调出WebUI窗口
        /// </summary>
        void ShowFormWebUI();

        /// <summary>
        /// 调出日志窗口
        /// </summary>
        void ShowFormLog();

        #endregion

        #region vgc

        // timeout = long.MaxValue
        /// <summary>
        /// 获取测速超时的准确数值
        /// </summary>
        /// <returns>超时的数值</returns>
        long GetTimeoutValue();

        /// <summary>
        /// 扫描屏幕上的二维码
        /// </summary>
        /// <returns>二维码解码后的内容</returns>
        string ScanQrcode();

        /// <summary>
        /// 获取当前用户设置
        /// </summary>
        /// <returns>一个序列化的JObject</returns>
        string GetUserSettings();

        /// <summary>
        /// 获取当前用户设置
        /// </summary>
        /// <param name="props">属性列表['prop1', 'prop2', ...]</param>
        /// <returns>一个序列化的JObject</returns>
        string GetUserSettings(string props);

        /// <summary>
        /// 替换当前的用户配置（还没实现这个功能）
        /// </summary>
        /// <param name="props">执行是否成功</param>
        /// <returns></returns>
        bool SetUserSettings(string props);

        /// <summary>
        /// 获取全部订阅设置
        /// </summary>
        /// <returns>订阅设置Json字符串</returns>
        string GetSubscriptionConfig();

        /// <summary>
        /// 替换当前订阅设置
        /// </summary>
        /// <param name="cfgStr">订阅设置Json字符串</param>
        void SetSubscriptionConfig(string cfgStr);

        // share among all scripts
        /// <summary>
        /// 从本地存储读取一个键为key的字符串
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        string ReadLocalStorage(string key);

        // share among all scripts
        /// <summary>
        /// 向本地存储写入一字符串，以key为键名
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">字符串内容</param>
        void WriteLocalStorage(string key, string value);

        // remove a key from local storage
        /// <summary>
        /// 删除一个键名为key的本地存储字符串
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        bool RemoveLocalStorage(string key);

        // get all keys of local storage
        /// <summary>
        /// 查询本地存储中所有键名
        /// </summary>
        /// <returns>所有键名</returns>
        List<string> LocalStorageKeys();
        #endregion

        #region utils
        new void Log(string message);

        /// <summary>
        /// 互斥锁池当前计数
        /// </summary>
        /// <returns>当前计数</returns>
        int GetMutexPoolCount();

        /// <summary>
        /// 互斥锁池历史最大计数
        /// </summary>
        /// <returns>历史最大计数</returns>
        int GetMutexPoolMaxCount();

        /// <summary>
        /// 获取日志窗口的内容
        /// </summary>
        /// <returns></returns>
        string GetLogAsString();

        /// <summary>
        /// 获取当前并行测速队列服务器总数。一个服务器只记一次。
        /// </summary>
        /// <returns>测速队列长度</returns>
        int GetSpeedtestQueueLength();

        /// <summary>
        /// 获取本软件所处目录
        /// </summary>
        /// <returns>当前目录</returns>
        string GetAppDir();

        /// <summary>
        /// 获取预定义函数源码
        /// </summary>
        /// <returns>预定义源码</returns>
        string PredefinedFunctions();

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="contents">若干内容</param>
        void Print(params object[] contents);

        /// <summary>
        /// 等待一段时间
        /// </summary>
        /// <param name="ms">毫秒</param>
        void Sleep(int ms);

        /// <summary>
        /// 查找替换字串中的内容
        /// </summary>
        /// <param name="text">整个字串</param>
        /// <param name="oldStr">要查找的字串</param>
        /// <param name="newStr">替换为字串</param>
        /// <returns>替换后字串</returns>
        string Replace(string text, string oldStr, string newStr);

        /// <summary>
        /// 生成指定长度的随机十六进制字符串
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        string RandomHex(int len);

        /// <summary>
        /// 生成一个随机UUID
        /// </summary>
        /// <returns></returns>
        string NewGuid();

        #endregion

        #region UI thing
        /// <summary>
        /// 调出浏览目录窗口
        /// </summary>
        /// <returns>选中的目录路径</returns>
        string BrowseFolder();

        /// <summary>
        /// 调出浏览文件窗口
        /// </summary>
        /// <returns>选中的文件路径</returns>
        string BrowseFile();

        /// <summary>
        /// 调出浏览指定后缀文件窗口
        /// </summary>
        /// <param name="extends">后缀</param>
        /// <returns>选中的文件路径</returns>
        string BrowseFile(string extends);

        // 2MiB char max
        /// <summary>
        /// 调出输入字符串窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns>用户输入的内容</returns>
        string Input(string title);

        // 25 lines max
        /// <summary>
        /// 调出输入字符串窗口，指定初始行数
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="lines">初始行数</param>
        /// <returns></returns>
        string Input(string title, int lines);

        /// <summary>
        /// 调出输入字符串窗口，指定初始行数及初始内容
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">初始内容</param>
        /// <param name="lines">初始行数</param>
        /// <returns></returns>
        string Input(string title, string content, int lines);

        /// <summary>
        /// 调出数据展示窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="columns">列标题</param>
        /// <param name="rows">内容行</param>
        /// <returns></returns>
        string ShowData(string title, LuaTable columns, LuaTable rows);

        /// <summary>
        /// 调出数据展示窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="columns">列标题</param>
        /// <param name="rows">内容行</param>
        /// <param name="defColumn">初始选定列号</param>
        /// <returns></returns>
        string ShowData(string title, LuaTable columns, LuaTable rows, int defColumn);

        // 18 choices max
        /// <summary>
        /// 调出多选窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="choices">各选项</param>
        /// <returns>选中行号集合</returns>
        List<int> Choices(string title, params string[] choices);

        /// <summary>
        /// 调出多选窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="choices">各选项</param>
        /// <returns>选中行号集合</returns>
        List<int> Choices(string title, LuaTable choices);

        /// <summary>
        /// 调出多选窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="choices">各选项</param>
        /// <param name="isShowKey">是否显示行号</param>
        /// <returns>选中行号集合</returns>
        List<int> Choices(string title, LuaTable choices, bool isShowKey);

        /// <summary>
        /// 调整单选窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="choices">各选项</param>
        /// <returns>选中行号</returns>
        int Choice(string title, params string[] choices);

        /// <summary>
        /// 调整单选窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="choices">各选项</param>
        /// <returns>选中行号</returns>
        int Choice(string title, LuaTable choices);

        /// <summary>
        /// 调整单选窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="choices">各选项</param>
        /// <param name="isShowKey">是否显示行号</param>
        /// <returns>选中行号</returns>
        int Choice(string title, LuaTable choices, bool isShowKey);

        /// <summary>
        /// 调整单选窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="choices">各选项</param>
        /// <param name="isShowKey">是否显示行号</param>
        /// <param name="selected">默认选中行号</param>
        /// <returns>选中行号</returns>
        int Choice(string title, LuaTable choices, bool isShowKey, int selected);

        /// <summary>
        /// 调出确认窗口
        /// </summary>
        /// <param name="content">显示内容</param>
        /// <returns></returns>
        bool Confirm(string content);

        /// <summary>
        /// 调出信息框
        /// </summary>
        /// <param name="content">显示内容</param>
        void Alert(string content);

        // sort server panel by index
        /// <summary>
        /// 刷新主窗口（修改服务器序号后重新排序）
        /// </summary>
        void RefreshFormMain();

        #endregion

        #region encode decode
        string ToJson(object o);
        string Md5(string str);
        string Sha256(string str);
        string Sha512(string str);

        // GetLinkBody("vmess://abcdefg") == "abcdefg"
        /// <summary>
        /// vmess://abcdefg => abcdefg
        /// </summary>
        /// <param name="link">任意链接</param>
        /// <returns>链接内容</returns>
        string GetLinkBody(string link);

        // v2cfg://(b64Str)
        /// <summary>
        /// abcdefg => v2cfg://abcdefg
        /// </summary>
        /// <param name="b64Str">任意内容</param>
        /// <returns>v2cfg://（任意内容）</returns>
        string AddV2cfgPrefix(string b64Str);

        // vmess://(b64Str)
        /// <summary>
        /// abcdefg => vmess://abcdefg
        /// </summary>
        /// <param name="b64Str">任意内容</param>
        /// <returns>vmess://（任意内容）</returns>
        string AddVmessPrefix(string b64Str);

        /// <summary>
        /// 对字符串进行Base64编码
        /// </summary>
        /// <param name="text">任意字符串</param>
        /// <returns>Base64字符串</returns>
        string Base64Encode(string text);

        /// <summary>
        /// 将Base64字符串解码
        /// </summary>
        /// <param name="b64Str">Base64字符串</param>
        /// <returns>解码后的字符串（解码失败返回null）</returns>
        string Base64Decode(string b64Str);

        string Basse64EncodeBytes(byte[] bytes);
        byte[] Base64DecodeToBytes(string b64Str);

        /// <summary>
        /// 从json类型的config中提取分享链接元数据
        /// </summary>
        /// <param name="config">json类型的config</param>
        /// <returns>分享链接元数据</returns>
        VgcApis.Models.Datas.SharelinkMetaData EncodeToShareLinkMetaData(string config);

        /// <summary>
        /// 将config编码为分享链接
        /// </summary>
        /// <param name="name">链接名</param>
        /// <param name="config">服务器完整config</param>
        /// <returns>vmess://..., vless://...</returns>
        string EncodeToShareLink(string name, string config);

        /// <summary>
        /// 将config编码为v2cfg链接
        /// </summary>
        /// <param name="name">链接名</param>
        /// <param name="config">服务器完整config</param>
        /// <returns>v2cfg://...</returns>
        string EncodeToV2cfgShareLink(string name, string config);

        /// <summary>
        /// 将各种分享链接解码为{name, config}
        /// </summary>
        /// <param name="shareLink">各种分享链接</param>
        /// <returns>{name, config}</returns>
        VgcApis.Models.Datas.DecodeResult DecodeShareLink(string shareLink);

        // links = "vmess://... ss://...  (...)"
        /// <summary>
        /// 从links字符串导入分享链接，并添加mark标记
        /// </summary>
        /// <param name="links">各种分享链接</param>
        /// <param name="mark">标记</param>
        /// <returns>成功导入的链接数</returns>
        int ImportLinks(string links, string mark);

        #endregion
    }
}
