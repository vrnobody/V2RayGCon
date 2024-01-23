using System;

namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface ICoreCtrl
    {
        /// <summary>
        /// 获取自定义core名字
        /// </summary>
        /// <returns>名字</returns>
        string GetCustomCoreName();

        /// <summary>
        /// 设置自定义core名字
        /// </summary>
        /// <param name="name">名字</param>
        bool SetCustomCoreName(string name);

        /// <summary>
        /// 服务器是否在运行
        /// </summary>
        /// <returns>
        /// true 运行中<br/>
        /// false 未运行
        /// </returns>
        bool IsCoreRunning();

        /// <summary>
        /// 正在测速
        /// </summary>
        /// <returns></returns>
        bool IsSpeedTesting();

        /// <summary>
        /// 内部使用
        /// </summary>
        void ReleaseSpeedTestLock();

        /// <summary>
        /// 进行延迟测试并等待测试完成
        /// </summary>
        void RunSpeedTest();

        /// <summary>
        /// 进行延迟测试但不等待测试完成
        /// </summary>
        void RunSpeedTestThen();

        /// <summary>
        /// 用这个core下载网页
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns></returns>
        string Fetch(string url);

        /// <summary>
        /// 用这个core下载网页
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="timeout">超时（毫秒）</param>
        /// <returns></returns>
        string Fetch(string url, int timeout);

        /// <summary>
        /// 停止服务器并等待操作完成
        /// </summary>
        void StopCore();

        /// <summary>
        /// 停止服务器但不等待操作完成
        /// </summary>
        void StopCoreThen();

        /// <summary>
        /// 内部使用，脚本一般用不到
        /// </summary>
        /// <param name="next"></param>
        void StopCoreThen(Action next);

        /// <summary>
        /// 重启服务器并等待操作完成
        /// </summary>
        void RestartCore();

        /// <summary>
        /// 重启服务器（不弹窗）并等待操作完成
        /// </summary>
        void RestartCoreIgnoreError();

        /// <summary>
        /// 重启服务器但不等待操作完成
        /// </summary>
        void RestartCoreThen();

        /// <summary>
        /// 内部使用，脚本一般用不到
        /// </summary>
        /// <param name="next"></param>
        void RestartCoreThen(Action next);

        /// <summary>
        /// 内部使用，脚本不使用此函数。
        /// </summary>
        /// <param name="next"></param>
        void GetterInfoForNotifyIconf(Action<string> next);
    }
}
