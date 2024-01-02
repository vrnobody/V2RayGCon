using System.Collections.Generic;
using Neo.IronLua;
using VgcApis.Interfaces;

namespace NeoLuna.Interfaces
{
    public interface ILuaServer
    {
        /// <summary>
        /// 用于向PackSelectedServers传入打包策略。
        /// </summary>
        int BalancerStrategyRandom { get; }

        /// <summary>
        /// 用于向PackSelectedServers传入打包策略。
        /// </summary>
        int BalancerStrategyLeastPing { get; }

        /// <summary>
        /// 获取服务器总数
        /// </summary>
        /// <returns>服务器总数</returns>
        int Count();

        /// <summary>
        /// 获取选中的服务器总数
        /// </summary>
        /// <returns>选中的服务器总数</returns>
        int CountSelected();

        /// <summary>
        /// 添加一个服务器
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="config">服务器的config.json</param>
        /// <param name="mark">标记</param>
        /// <returns>uid</returns>
        string AddNew(string name, string config, string mark);

        /// <summary>
        /// 添加一个服务器。obsolete!新脚本请用AddNew()
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="config">服务器的config.json</param>
        /// <param name="mark">标记</param>
        /// <returns>ok</returns>
        bool Add(string name, string config, string mark);

        /// <summary>
        /// 危险操作！
        /// </summary>
        /// <param name="config"></param>
        bool DeleteServerByConfig(string config);

        /// <summary>
        /// 危险操作！
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool DeleteServerByUid(string uid);

        /// <summary>
        /// 危险操作！！
        /// </summary>
        /// <param name="uids"></param>
        int DeleteServerByUids(LuaTable uids);

        /// <summary>
        /// 获取全部服务器（操作服务器的脚本通常都从这个函数开始）
        /// </summary>
        /// <returns>全部服务器</returns>
        List<ICoreServCtrl> GetAllServers();

        /// <summary>
        /// 根据index获取单个服务器
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>服务器或者null</returns>

        ICoreServCtrl GetServerByIndex(int index);

        /// <summary>
        /// 根据uid获取单个服务器
        /// </summary>
        /// <param name="uid">uid</param>
        /// <returns>服务器或者null</returns>

        ICoreServCtrl GetServerByUid(string uid);

        /// <summary>
        /// 根据config获取单个服务器
        /// </summary>
        /// <param name="config">config</param>
        /// <returns>服务器或者null</returns>

        ICoreServCtrl GetServerByConfig(string config);

        /// <summary>
        /// 根据uids获取服务器，跳过不存在的服务器
        /// </summary>
        /// <param name="uids">uid列表</param>
        /// <returns>服务器</returns>

        List<ICoreServCtrl> GetServersByUids(LuaTable uids);

        /// <summary>
        /// 将选中的服务器打包成均衡策略为random的服务器包
        /// </summary>
        /// <param name="orgUid">原服务器uid（为空则添加新服务器）</param>
        /// <param name="pkgName">服务器新名字</param>
        /// <returns>打包后新服务器的uid</returns>
        string PackSelectedServers(string orgUid, string pkgName);

        /// <summary>
        /// 将选中的服务器打包成均衡策略为random或leastPing的服务器包
        /// </summary>
        /// <param name="orgUid">原服务器uid（为空则添加新服务器）</param>
        /// <param name="pkgName">服务器新名字</param>
        /// <param name="strategy">均衡策略</param>
        /// <returns>打包后新服务器的uid</returns>
        string PackSelectedServers(string orgUid, string pkgName, int strategy);

        /// <summary>
        /// 将选中的服务器打包成均衡策略为leastPing的服务器包
        /// </summary>
        /// <param name="orgUid">原服务器uid（为空则添加新服务器）</param>
        /// <param name="pkgName">服务器新名字</param>
        /// <param name="strategy">均衡策略</param>
        /// <param name="interval">probeInterval</param>
        /// <param name="url">probeURL</param>
        /// <returns>打包后新服务器的uid</returns>
        string PackSelectedServers(
            string orgUid,
            string pkgName,
            int strategy,
            string interval,
            string url
        );

        /// <summary>
        /// 将服务器打包成blancer.random的服务器包
        /// </summary>
        /// <param name="uids">要打包的服务器uid列表</param>
        /// <returns>打包后的config.json字符串</returns>
        string PackServersToString(LuaTable uids);

        /// <summary>
        /// 将选中的服务器打包成均衡策略为leastPing的服务器包
        /// </summary>
        /// <param name="uids">要打包的服务器uid列表</param>
        /// <param name="orgUid">原服务器uid（为空则添加新服务器）</param>
        /// <param name="pkgName">服务器新名字</param>
        /// <param name="strategy">均衡策略</param>
        /// <param name="interval">probeInterval</param>
        /// <param name="url">probeURL</param>
        /// <returns>打包后新服务器的uid</returns>
        string PackServersWithUids(
            LuaTable uids,
            string orgUid,
            string pkgName,
            int strategy,
            string interval,
            string url
        );

        /// <summary>
        /// 将服务器打包成代理链
        /// </summary>
        /// <param name="uids">要打包的服务器uid列表</param>
        /// <param name="orgUid">原服务器uid（为空则添加新服务器）</param>
        /// <param name="pkgName">服务器新名字</param>
        /// <returns>打包后新服务器的uid</returns>
        string ChainSelectedServers(string orgUid, string pkgName);

        /// <summary>
        /// 将服务器打包成代理链
        /// </summary>
        /// <param name="orgUid">原服务器uid（为空则添加新服务器）</param>
        /// <param name="pkgName">新服务器名字</param>
        /// <returns>打包后新服务器的uid</returns>
        string ChainServersWithUids(LuaTable uids, string orgUid, string pkgName);

        // wont refresh form main
        /// <summary>
        /// 重新计算服务器的序号
        /// </summary>
        void ResetIndexes();

        // download (testUrl)
        /// <summary>
        /// 测试任意config.json的延迟
        /// </summary>
        /// <param name="rawConfig">任意config.json</param>
        /// <param name="coreName">自定义core名</param>
        /// <param name="testUrl">测速网址</param>
        /// <param name="testTimeout">超时值</param>
        /// <returns></returns>
        long RunCustomSpeedTest(string rawConfig, string coreName, string testUrl, int testTimeout);

        // download google.com
        /// <summary>
        /// 用选项窗口设定的url对任意config.json进行延迟测试
        /// </summary>
        /// <param name="rawConfig">任意config.json</param>
        /// <returns></returns>
        long RunSpeedTest(string rawConfig);

        /// <summary>
        /// 对选定的服务器进行延迟测试，完成时弹出提示窗口。
        /// </summary>
        /// <returns></returns>
        bool RunSpeedTestOnSelectedServers();

        /// <summary>
        /// 对选定的服务器进行延迟测试，没有任务提示。
        /// </summary>
        /// <returns></returns>
        bool RunSpeedTestOnSelectedServersBgQuiet();

        /// <summary>
        /// 对指定uid的服务器进行延迟测试
        /// </summary>
        /// <param name="uids"></param>
        /// <returns></returns>
        bool RunSpeedTestByUids(LuaTable uids);

        /// <summary>
        /// 停止延迟测试
        /// </summary>
        void StopSpeedTest();

        /// <summary>
        /// 是否正在进行延迟测试
        /// </summary>
        /// <returns></returns>
        bool IsRunningSpeedTest();

        /// <summary>
        /// 将选中的服务器按序号逆序排列
        /// </summary>
        void ReverseSelectedByIndex();

        /// <summary>
        /// 按最后修改时间排序选中的服务器
        /// </summary>
        void SortSelectedServersByLastModifiedDate();

        /// <summary>
        /// 按延迟排序选中的服务器
        /// </summary>
        void SortSelectedServersBySpeedTest();

        /// <summary>
        /// 按摘要排序选中的服务器
        /// </summary>
        void SortSelectedServersBySummary();

        /// <summary>
        /// 按流量统计下载量排序选中的服务器
        /// </summary>
        void SortSelectedByDownloadTotal();

        /// <summary>
        /// 按流量统计上传量排序选中的服务器
        /// </summary>
        void SortSelectedByUploadTotal();

        /// <summary>
        /// 按序号反序排列服务器
        /// </summary>
        void ReverseServersByIndex(LuaTable uids);

        /// <summary>
        /// 按最后修改时间排序服务器
        /// </summary>
        void SortServersByLastModifiedDate(LuaTable uids);

        /// <summary>
        /// 按延迟排序服务器
        /// </summary>
        void SortServersBySpeedTest(LuaTable uids);

        /// <summary>
        /// 按摘要排序服务器
        /// </summary>
        void SortServersBySummary(LuaTable uids);

        /// <summary>
        /// 按流量统计下载量排序服务器
        /// </summary>
        void SortServersByDownloadTotal(LuaTable uids);

        /// <summary>
        /// 按流量统计上传量排序服务器
        /// </summary>
        void SortServersByUploadTotal(LuaTable uids);

        /// <summary>
        /// 停止所有服务器
        /// </summary>
        void StopAllServers();

        // refresh servers' title in form main
        /// <summary>
        /// 重新计算所有服务器的摘要
        /// </summary>
        void UpdateAllSummary();

        #region wrap interface
        IWrappedCoreServCtrl GetWrappedServerByIndex(int index);
        IWrappedCoreServCtrl GetWrappedServerByUid(string uid);
        IWrappedCoreServCtrl GetWrappedServerByConfig(string config);
        List<IWrappedCoreServCtrl> GetWrappedServersByUids(LuaTable uids);
        #endregion
    }
}
