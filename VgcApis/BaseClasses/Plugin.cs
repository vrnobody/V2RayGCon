using System;
using System.Drawing;
using System.Windows.Forms;

namespace VgcApis.BaseClasses
{
    public class Plugin : Interfaces.IPlugin
    {
        #region properties
        public virtual string Name => throw new NotImplementedException();
        public virtual string Version => throw new NotImplementedException();
        public virtual string Description => throw new NotImplementedException();
        public virtual Image Icon => throw new NotImplementedException();
        #endregion

        #region IPlugin
        public virtual void ShowMainForm() => throw new NotImplementedException();

        public virtual void Stop() => throw new NotImplementedException();

        public virtual void Run(Interfaces.Services.IApiService api) =>
            throw new NotImplementedException();

        // 默认只有一个菜单项，即弹出主窗口
        public virtual ToolStripMenuItem GetToolStripMenu()
        {
            var menu = new ToolStripMenuItem(Name, Icon, (s, a) => ShowMainForm());
            menu.ToolTipText = Description;
            return menu;
        }
        #endregion

        #region protected

        // 预防多次执行IPlugin.Run()及IPlugin.Stop()的辅助函数。

        object locker = new object();
        bool isPluginRunning = false;

        protected bool GetRunningState() => isPluginRunning && !disposedValue;

        protected bool SetRunningState(bool isRunning)
        {
            if (disposedValue && isRunning)
            {
                return false;
            }

            lock (locker)
            {
                if (isPluginRunning == isRunning)
                {
                    return false;
                }
                isPluginRunning = isRunning;
                return true;
            }
        }
        #endregion

        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    if (GetRunningState())
                    {
                        Stop();
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Plugin()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
