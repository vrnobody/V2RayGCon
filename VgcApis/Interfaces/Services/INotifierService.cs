using System;

namespace VgcApis.Interfaces.Services
{
    public interface INotifierService
    {
        void RefreshNotifyIcon();

        void RunInUiThreadIgnoreError(Action updater);

        void RunInUiThreadIgnoreErrorThen(Action updater, Action next);
    }
}
