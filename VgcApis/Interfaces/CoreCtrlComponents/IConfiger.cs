using Newtonsoft.Json.Linq;
using System;

namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface IConfiger
    {
        /// <summary>
        /// 获取传给v2ray-core的最终配置
        /// </summary>
        /// <returns>最终配置</returns>
        JObject GetFinalConfig();

        /// <summary>
        /// 获取整个config.json
        /// </summary>
        /// <returns>完整配置</returns>
        string GetConfig();

        /// <summary>
        /// 修改config.json
        /// </summary>
        /// <param name="newConfig">新的config.json</param>
        void SetConfig(string newConfig);

        /// <summary>
        /// 判断当前服务器是否能设置为系统代理
        /// </summary>
        /// <param name="isGlobal">系统代理是否为全局模式</param>
        /// <param name="isSocks">当前服务器代理协议是否为SOCKS</param>
        /// <param name="port">当前服务器代理端口号</param>
        /// <returns></returns>
        bool IsSuitableToBeUsedAsSysProxy(bool isGlobal, out bool isSocks, out int port);

        /// <summary>
        /// 更新服务器摘要（摘要需要从config.json得出，所以更新功能放入IConfiger）
        /// </summary>
        void UpdateSummary();

        /// <summary>
        /// 更新服务器摘要但不刷新界面
        /// </summary>
        void UpdateSummaryQuiet();

        /// <summary>
        /// 内部使用，脚本不使用此函数。
        /// </summary>
        /// <param name="next"></param>
        void GetterInfoForNotifyIconf(Action<string> next);
    }
}
