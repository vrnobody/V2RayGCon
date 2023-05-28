using NLua;
using System.Collections.Generic;

namespace VgcApis.Interfaces.Lua
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

        bool Add(string config);

        bool Add(string config, string mark);

        /// <summary>
        /// 危险操作！
        /// </summary>
        /// <param name="config"></param>
        void DeleteServerByConfig(string config);

        /// <summary>
        /// 危险操作！！
        /// </summary>
        /// <param name="uids"></param>
        void DeleteServerByUids(LuaTable uids);


        /// <summary>
        /// 获取全部服务器（操作服务器的脚本通常都从这个函数开始）
        /// </summary>
        /// <returns>全部服务器</returns>
        List<ICoreServCtrl> GetAllServers();

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
        string PackSelectedServers(
            string orgUid, string pkgName, int strategy);

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
            string orgUid, string pkgName, int strategy,
            string interval, string url);

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
            string orgUid, string pkgName, int strategy,
            string interval, string url);

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
        /// <param name="testUrl">测速网址</param>
        /// <param name="testTimeout">超时值</param>
        /// <returns></returns>
        long RunCustomSpeedTest(string rawConfig, string testUrl, int testTimeout);

        // download google.com
        /// <summary>
        /// 用选项窗口设定的url对任意config.json进行延迟测试
        /// </summary>
        /// <param name="rawConfig">任意config.json</param>
        /// <returns></returns>
        long RunSpeedTest(string rawConfig);

        /// <summary>
        /// 对选定的服务器进行延迟测试
        /// </summary>
        /// <returns></returns>
        bool RunSpeedTestOnSelectedServers();

        bool RunSpeedTestOnSelectedServersBgQuiet();

        void StopSpeedTest();

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
        /// 停止所有服务器
        /// </summary>
        void StopAllServers();

        // refresh servers' title in form main
        /// <summary>
        /// 重新计算所有服务器的摘要
        /// </summary>
        void UpdateAllSummary();

    }
}
