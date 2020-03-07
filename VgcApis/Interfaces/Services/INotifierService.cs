using System;

namespace VgcApis.Interfaces.Services
{
    public interface INotifierService
    {
        void RefreshNotifyIcon();
        void RunInUiThreadIgnoreErrorThen(Action updater, Action next);
    }
}
