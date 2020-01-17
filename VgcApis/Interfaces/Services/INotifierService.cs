using System;

namespace VgcApis.Interfaces.Services
{
    public interface INotifierService
    {
        void RefreshNotifyIcon();
        void RunInUiThread(Action updater);
    }
}
