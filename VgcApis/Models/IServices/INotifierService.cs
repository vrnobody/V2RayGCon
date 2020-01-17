using System;

namespace VgcApis.Models.IServices
{
    public interface INotifierService
    {
        void RefreshNotifyIcon();
        void RunInUiThread(Action updater);
    }
}
