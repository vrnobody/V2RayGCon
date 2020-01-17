using System;
using System.Drawing;
using System.Windows.Forms;

namespace VgcApis.Models.BaseClasses
{
    public class Plugin : Models.Interfaces.IPlugin
    {
        public virtual string Name => throw new NotImplementedException();
        public virtual string Version => throw new NotImplementedException();
        public virtual string Description => throw new NotImplementedException();
        public virtual Image Icon => throw new NotImplementedException();

        public virtual ToolStripMenuItem GetMenu()
        {
            var menu = new ToolStripMenuItem(Name, Icon, (s, a) => Popup());
            menu.ToolTipText = Description;
            return menu;
        }

        protected virtual void Start(Models.IServices.IApiService api) { }
        protected virtual void Stop() { }
        protected virtual void Popup() { }

        bool isPluginRunning;
        object isRunningLocker = new object();
        public void Cleanup()
        {
            lock (isRunningLocker)
            {
                if (!isPluginRunning)
                {
                    return;
                }
                isPluginRunning = false;
            }

            Stop();
        }

        public void Run(Models.IServices.IApiService api)
        {
            lock (isRunningLocker)
            {
                if (isPluginRunning)
                {
                    return;
                }
                isPluginRunning = true;
            }

            Start(api);
        }

        public void Show()
        {
            lock (isRunningLocker)
            {
                if (!isPluginRunning)
                {
                    return;
                }
            }

            Popup();
        }
    }
}
