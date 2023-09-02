using System;

namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface ILogger : ILogable
    {
        /// <summary>
        /// 调出此服务器日志窗口
        /// </summary>
        void ShowFormLog();

        Action<string> GetLoggerInstance();

        string GetLogAsString();
    }
}
