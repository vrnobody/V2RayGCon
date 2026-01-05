using System;
using System.Diagnostics;
using System.Text;
using NLua;
using VgcApis.Interfaces.PostOfficeComponents;

namespace Luna.Interfaces
{
    public interface ILuaSys
    {
        /// <summary>
        /// 获取V2RayGCon版本信息
        /// </summary>
        /// <returns>1.2.3.4</returns>
        string GetAppVersion();

        #region ILuaSys.LuaCoreCtrl
        void SetWarnOnExit(bool isOn);
        #endregion

        #region ILuaSys.SnapCache

        // 申请者退出时自动清理
        string SnapCacheApply();

        // 注意要手动清理
        bool SnapCacheCreate(string token);
        bool SnapCacheRemove(string token);

        object SnapCacheGet(string token, string key);

        bool SnapCacheSet(string token, string key, object value);
        #endregion

        #region Lua VM

        // 这堆Lua***()函数给WebUI的Luna功能提供支持用的。
        // 函数名随时可能改变，目前不建议在脚本中使用这些的函数。

        // vmh: lua virtual machine handle
        // LuaServ***() 对应winform中的脚本管理器
        // LuaVm***() 对应winform中的脚本编辑器

        string LuaGenModuleSnippets(string code);

        string LuaGetStaticSnippets();

        string LuaAnalyzeCode(string code);

        string LuaAnalyzeModule(string name);

        // this function is disabled for safety reason
        string LuaAnalyzeModuleEx(string name);

        string LuaServGetAllCoreInfos();

        string LuaServGetCoreInfo(string name);

        bool LuaServChangeSettings(string name, string settings);

        bool LuaServSetIndex(string name, double index);

        bool LuaServRemove(string name);

        void LuaServRestart(string name);

        void LuaServStart(string name);

        void LuaServStop(string name);

        void LuaServAbort(string name);

        bool LuaServAdd(string name, string script);

        string LuaServGetAllScripts();

        bool LuaVmRemove(string vmh);

        void LuaVmRemoveStopped();

        string LuaVmGetScript(string vmh);

        void LuaVmWait(string vmh);

        void LuaVmWait(string vmh, int ms);

        string LuaVmCreate();

        // tag for logging
        string LuaVmCreate(string tag);

        string LuaVmGetResult(string vmh);

        bool LuaVmRun(string vmh, string script);
        bool LuaVmRun(string vmh, string script, bool isLoadClr);

        // obsolete backward compatible
        bool LuaVmRun(string vmh, string name, string script, bool isLoadClr);

        string LuaVmGetAllVmsInfo();

        void LuaVmAbort(string vmh);

        void LuaVmStop(string vmh);

        bool LuaVmIsRunning(string vmh);

        void LuaVmClearLog(string vmh);

        string LuaVmGetLog(string vmh);
        #endregion

        #region Net
        /// <summary>
        /// 创建一个HTTP服务器，它将接收到的请求存入inbox，回复时使用outbox。
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="inbox">接收请求邮箱</param>
        /// <param name="outbox">回复请求邮箱</param>
        /// <returns></returns>
        IRunnable CreateHttpServer(string url, ILuaMailBox inbox, ILuaMailBox outbox);

        /// <summary>
        /// 创建一个HTTP服务器，它将接收到的请求存入inbox，回复时使用outbox。
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="inbox">接收请求邮箱</param>
        /// <param name="outbox">回复请求邮箱</param>
        /// <param name="source">网页来源，可以是HTML字符串、文件路径、文件夹路径</param>
        /// <param name="allowCORS">允许跨域</param>
        /// <returns></returns>
        IRunnable CreateHttpServer(
            string url,
            ILuaMailBox inbox,
            ILuaMailBox outbox,
            string source,
            bool allowCORS
        );
        #endregion

        #region VGC
        /// <summary>
        /// 立即存盘把当前的全部配置数据。
        /// V2RayGCon作者的硬盘非常辣鸡，默认情况下每隔5分钟（如果有数据变化）才写一次盘。
        /// 如果突然蓝屏、死机、断电什么的可能会丢失5分钟内的数据。
        /// </summary>
        void SaveV2RayGConSettingsNow();
        #endregion

        #region core event
        /// <summary>
        /// 用于RegisterGlobalEvent中的evType参数
        /// </summary>
        int CoreEvStart { get; }

        /// <summary>
        /// 用于RegisterGlobalEvent中的evType参数
        /// </summary>
        int CoreEvClosing { get; }

        /// <summary>
        /// 用于RegisterGlobalEvent中的evType参数
        /// </summary>
        int CoreEvStop { get; }

        /// <summary>
        /// 用于RegisterGlobalEvent中的evType参数
        /// </summary>
        int CoreEvPropertyChanged { get; }

        /// <summary>
        /// 注销全部服务器事钩子
        /// </summary>
        /// <param name="mailbox">接收事件的邮箱</param>
        /// <param name="handle">注册事件时返回的句柄</param>
        /// <returns>成功、失败</returns>
        bool UnregisterGlobalEvent(ILuaMailBox mailbox, string handle);

        /// <summary>
        /// 注册全部服务器事钩子
        /// </summary>
        /// <param name="mailbox">接收事件的邮箱</param>
        /// <param name="evType">事件类型</param>
        /// <param name="evCode">事件发生时向接收邮箱发送的代码</param>
        /// <returns>随机生成的句柄</returns>
        string RegisterGlobalEvent(ILuaMailBox mailbox, int evType, int evCode);

        /// <summary>
        /// 注销单个服务器事件钩子
        /// </summary>
        /// <param name="mailbox">接收事件的邮箱</param>
        /// <param name="handle">注册事件时生成的句柄</param>
        /// <returns></returns>
        bool UnregisterCoreEvent(ILuaMailBox mailbox, string handle);

        /// <summary>
        /// 注册单个服务器事钩子
        /// </summary>
        /// <param name="coreServ">服务器</param>
        /// <param name="mailbox">接收事件的邮箱</param>
        /// <param name="evType">事件类型</param>
        /// <param name="evCode">事件发生时向接收邮箱发送的代码</param>
        /// <returns>随机生成的句柄</returns>
        string RegisterCoreEvent(
            VgcApis.Interfaces.ICoreServCtrl coreServ,
            ILuaMailBox mailbox,
            int evType,
            int evCode
        );
        #endregion

        #region keyboard hotkey
        /// <summary>
        /// 获取全部可用热键名字（忘记键名时打印出来看看）
        /// </summary>
        /// <returns>全部可用热键名</returns>
        string GetAllKeyNames();

        /// <summary>
        /// 注销键盘钩子
        /// </summary>
        /// <param name="mailbox">接收按键信息邮箱</param>
        /// <param name="handle">注册热键时生成的句柄</param>
        /// <returns></returns>
        bool UnregisterHotKey(ILuaMailBox mailbox, string handle);

        /// <summary>
        /// 注册键盘热键钩子
        /// </summary>
        /// <param name="mailbox">接收按键信息的邮箱</param>
        /// <param name="evCode">按键时向接收邮箱发送的代码</param>
        /// <param name="keyName">热键名</param>
        /// <param name="hasAlt">Alt键是否按下</param>
        /// <param name="hasCtrl">Ctrl键是否按下</param>
        /// <param name="hasShift">Shift键是否按下</param>
        /// <returns>随机生成的句柄</returns>
        string RegisterHotKey(
            ILuaMailBox mailbox,
            int evCode,
            string keyName,
            bool hasAlt,
            bool hasCtrl,
            bool hasShift
        );
        #endregion

        #region reflection
        // 这几个函数用来查询CLR中的对象信息
        // 比如 list.Count，list.Count()，list:Count()

        string GetPublicInfosOfType(Type type);
        string GetPublicInfosOfObject(object @object);
        string GetChildrenInfosOfNamespace(string @namespace);
        string GetPublicInfosOfAssembly(string @namespace, string asm);
        #endregion

        #region post office
        /// <summary>
        /// 申请一个随机邮箱
        /// </summary>
        /// <returns>邮箱</returns>
        ILuaMailBox ApplyRandomMailBox();

        /// <summary>
        /// 检测邮箱是否有权使用（只有拥有邮箱的脚本才有权收邮件及注销邮箱）
        /// </summary>
        /// <param name="mailbox"></param>
        /// <returns></returns>
        bool ValidateMailBox(ILuaMailBox mailbox);

        /// <summary>
        /// 申请一个名为name的邮箱
        /// </summary>
        /// <param name="name">邮箱名</param>
        /// <returns>
        /// 申请成功（name不重复)时返回一个邮箱<br/>
        /// 申请失败时返回nil
        /// </returns>
        ILuaMailBox CreateMailBox(string name);

        ILuaMailBox ApplyRandomMailBox(int capacity);
        ILuaMailBox CreateMailBox(string name, int capacity);

        /// <summary>
        /// 注销一个邮箱
        /// </summary>
        /// <param name="mailbox">邮箱</param>
        /// <returns>
        /// true 注销成功<br/>
        /// false 注销失败
        /// </returns>
        bool RemoveMailBox(ILuaMailBox mailbox);
        #endregion

        #region encoding
        /*
         * Encoding 用于向Sys:Run()传递编码类型，解决乱码问题。
         */
        Encoding GetEncoding(int codepage);
        Encoding EncodingDefault { get; }

        Encoding EncodingCmd936 { get; }
        Encoding EncodingUtf8 { get; }

        Encoding EncodingAscII { get; }
        Encoding EncodingUnicode { get; }
        #endregion

        #region process

        /// <summary>
        /// 在非UI线程使用WinForm时，处理WinForm消息
        /// </summary>
        void DoEvents();

        /// <summary>
        /// 清理Sys:Run()创建的进程
        /// </summary>
        /// <param name="proc">Sys:Run()创建的进程</param>
        void Cleanup(Process proc);

        /// <summary>
        /// 关闭Sys:Run()创建的窗口
        /// </summary>
        /// <param name="proc">Sys:Run()创建的窗口</param>
        /// <returns>是否成功关闭窗口</returns>
        bool CloseMainWindow(Process proc);

        /// <summary>
        /// 检测Sys:Run()创建的进程是否已经结束
        /// </summary>
        /// <param name="proc">Sys:Run()创建的进程</param>
        /// <returns>进程是否已结束</returns>
        bool HasExited(Process proc);

        /// <summary>
        /// 杀死Sys:Run()创建的进程
        /// </summary>
        /// <param name="proc">Sys:Run()创建的进程</param>
        void Kill(Process proc);

        /// <summary>
        /// 相当到windows的win+R，不过没设置PATH，所以功能差点。
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string Start(string param);

        /// <summary>
        /// 参考Process RunAndForgot(string exePath, string args, string stdin,
        /// LuaTable envs, bool hasWindow, bool redirectOutput,
        /// Encoding inputEncoding, Encoding outputEncoding);
        /// </summary>
        Process RunAndForgot(string exePath);

        /// <summary>
        /// 参考Process RunAndForgot(string exePath, string args, string stdin,
        /// LuaTable envs, bool hasWindow, bool redirectOutput,
        /// Encoding inputEncoding, Encoding outputEncoding);
        /// </summary>
        Process RunAndForgot(string exePath, string args);

        /// <summary>
        /// 参考Process RunAndForgot(string exePath, string args, string stdin,
        /// LuaTable envs, bool hasWindow, bool redirectOutput,
        /// Encoding inputEncoding, Encoding outputEncoding);
        /// </summary>
        Process RunAndForgot(string exePath, string args, string stdin);

        /// <summary>
        /// 参考Process RunAndForgot(string exePath, string args, string stdin,
        /// LuaTable envs, bool hasWindow, bool redirectOutput,
        /// Encoding inputEncoding, Encoding outputEncoding);
        /// </summary>
        Process RunAndForgot(
            string exePath,
            string args,
            string stdin,
            LuaTable envs,
            bool hasWindow,
            bool redirectOutput
        );

        /// <summary>
        /// 创建进程后撒手不管（脚本结束进程也继续运行）
        /// </summary>
        /// <param name="exePath">可执行文件地址</param>
        /// <param name="args">传入参数</param>
        /// <param name="stdin">向stdin注入数据</param>
        /// <param name="envs">传入环境变量</param>
        /// <param name="hasWindow">是否显示窗口</param>
        /// <param name="redirectOutput">是否接收stdout和stderror信息</param>
        /// <param name="inputEncoding">stdin的编码</param>
        /// <param name="outputEncoding">stdout的编码</param>
        /// <returns>进程句柄</returns>
        Process RunAndForgot(
            string exePath,
            string args,
            string stdin,
            LuaTable envs,
            bool hasWindow,
            bool redirectOutput,
            Encoding inputEncoding,
            Encoding outputEncoding
        );

        /// <summary>
        /// 参考 Process Run(string exePath, string args, string stdin,
        /// LuaTable envs, bool hasWindow, bool redirectOutput,
        /// Encoding inputEncoding, Encoding outputEncoding);
        /// </summary>
        Process Run(string exePath);

        /// <summary>
        /// 参考 Process Run(string exePath, string args, string stdin,
        /// LuaTable envs, bool hasWindow, bool redirectOutput,
        /// Encoding inputEncoding, Encoding outputEncoding);
        /// </summary>
        Process Run(string exePath, string args);

        /// <summary>
        /// 参考 Process Run(string exePath, string args, string stdin,
        /// LuaTable envs, bool hasWindow, bool redirectOutput,
        /// Encoding inputEncoding, Encoding outputEncoding);
        /// </summary>
        Process Run(string exePath, string args, string stdin);

        /// <summary>
        /// 参考 Process Run(string exePath, string args, string stdin,
        /// LuaTable envs, bool hasWindow, bool redirectOutput,
        /// Encoding inputEncoding, Encoding outputEncoding);
        /// </summary>
        Process Run(
            string exePath,
            string args,
            string stdin,
            LuaTable envs,
            bool hasWindow,
            bool redirectOutput
        );

        /// <summary>
        /// 创建进程。在脚本结束时进程也会被杀死。
        /// </summary>
        /// <param name="exePath">可执行文件地址</param>
        /// <param name="args">传入参数</param>
        /// <param name="stdin">向stdin注入数据</param>
        /// <param name="envs">传入环境变量</param>
        /// <param name="hasWindow">是否显示窗口</param>
        /// <param name="redirectOutput">是否接收stdout和stderror信息</param>
        /// <param name="inputEncoding">stdin的编码</param>
        /// <param name="outputEncoding">stdout的编码</param>
        /// <param name="customLogger">自定义的Logger</param>
        /// <returns>进程句柄</returns>
        Process Run(
            string exePath,
            string args,
            string stdin,
            LuaTable envs,
            bool hasWindow,
            bool redirectOutput,
            Encoding inputEncoding,
            Encoding outputEncoding,
            VgcApis.Interfaces.ILogable logable
        );

        /// <summary>
        /// 向进程发送Ctrl+C信号
        /// </summary>
        /// <param name="proc">进程</param>
        /// <returns>是否发送成功</returns>
        bool SendStopSignal(Process proc);

        /// <summary>
        /// 等待进程结束
        /// </summary>
        /// <param name="proc">进程</param>
        void WaitForExit(Process proc);
        #endregion

        #region system
        /// <summary>
        /// 指定时间后向指定邮箱发送一封邮件
        /// </summary>
        /// <param name="mailbox">目标邮箱地址</param>
        /// <param name="timeout">延迟</param>
        /// <param name="id">事件代号</param>
        void SetTimeout(ILuaMailBox mailbox, int timeout, double id);

        /// <summary>
        /// 执行一次GC()，其实没什么实际效果。
        /// </summary>
        void GarbageCollect();

        /// <summary>
        /// 调高系统音量（我的键盘默认没这个功能T.T）
        /// </summary>
        void VolumeUp();

        /// <summary>
        /// 高低系统音量
        /// </summary>
        void VolumeDown();

        /// <summary>
        /// 静音
        /// </summary>
        void VolumeMute();

        /// <summary>
        /// 获取系统版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        string GetOsVersion();

        /// <summary>
        /// 获取系统发行信息
        /// </summary>
        /// <returns>发行信息</returns>
        string GetOsReleaseInfo();

        /// <summary>
        /// 设置桌面壁纸（对部分win7无效）
        /// </summary>
        /// <param name="filename">壁纸路径</param>
        /// <returns></returns>
        int SetWallpaper(string filename);

        /// <summary>
        /// 清空回收站
        /// </summary>
        /// <returns>（我也不知道这个返回值代表什么）</returns>
        uint EmptyRecycle();

        #endregion

        #region file system
        /// <summary>
        /// 得到一张图片的分辨率（配合设置壁纸函数使用）
        /// </summary>
        /// <param name="filename">图片文件路径</param>
        /// <returns>分辨率(123x456)</returns>
        string GetImageResolution(string filename);

        /// <summary>
        /// 从一个文本文件中随机抽取一行内容
        /// </summary>
        /// <param name="filename">文本文件路径</param>
        /// <returns>随机一行内容</returns>
        string PickRandomLine(string filename);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>是否创建成功</returns>
        bool CreateFolder(string path);

        /// <summary>
        /// 检测文件是否存在
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件是否存在</returns>
        bool IsFileExists(string path);

        /// <summary>
        /// 检测文件夹是否存在
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>文件夹是否存在</returns>
        bool IsDirExists(string path);

        /// <summary>
        /// Path.Combine(root, path)
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        string CombinePath(string root, string path);

        /// <summary>
        /// 见 string Ls(string path, string exts);
        /// </summary>
        string Ls(string path);

        /// <summary>
        /// 列出path中的文件及文件夹
        /// </summary>
        /// <param name="path">C:\windows</param>
        /// <param name="exts">txt|lua|json</param>
        /// <returns>{folders:[...], files:[...]}</returns>
        string Ls(string path, string exts);
        #endregion
    }
}
