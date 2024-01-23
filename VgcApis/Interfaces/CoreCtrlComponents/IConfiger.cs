using System;
using System.Collections.Generic;

namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface IConfiger
    {
        /// <summary>
        /// 获取当前inbound主要信息
        /// </summary>
        /// <returns>inbound信息</returns>
        Models.Datas.InboundInfo GetInboundInfo();

        List<Models.Datas.InboundInfo> GetAllInboundsInfo();

        /// <summary>
        /// 获取传给v2ray-core的最终配置
        /// </summary>
        /// <returns>最终配置</returns>
        string GetFinalConfig();

        /// <summary>
        /// 获取整个config.json
        /// </summary>
        /// <returns>完整配置</returns>
        string GetConfig();

        /// <summary>
        /// 获取config的原始数据
        /// </summary>
        /// <returns>（可能）压缩后的conig</returns>
        string GetRawConfig();

        /// <summary>
        /// 获取ss/vless/vmess分享链接
        /// </summary>
        /// <returns>分享链接</returns>
        string GetShareLink();

        /// <summary>
        /// 修改config.json，但不重启服务器。
        /// </summary>
        /// <param name="newConfig">新的config.json</param>
        /// <returns>是否修改成功</returns>
        bool SetConfigQuiet(string newConfig);

        /// <summary>
        /// 修改config.json，并重启正在运行的服务器
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
        /// 内部使用，脚本不要使用此函数。
        /// </summary>
        /// <param name="next">为null时清空cache</param>
        void GatherInfoForNotifyIcon(Action<string> next);
    }
}
