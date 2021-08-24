using System;

namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface ICoreCtrl
    {
        /// <summary>
        /// 服务器是否在运行
        /// </summary>
        /// <returns>
        /// true 运行中<br/>
        /// false 未运行
        /// </returns>
        bool IsCoreRunning();

        /// <summary>
        /// 进行延迟测试并等待测试完成
        /// </summary>
        void RunSpeedTest();

        /// <summary>
        /// 进行延迟测试但不等待测试完成
        /// </summary>
        void RunSpeedTestThen();

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
        /// 重启服务器但不等待操作完成
        /// </summary>
        void RestartCoreThen();

        /// <summary>
        /// 内部使用，脚本一般用不到
        /// </summary>
        /// <param name="next"></param>
        void RestartCoreThen(Action next);

        // Models.Datas.StatsSample TakeStatisticsSample();
    }
}
