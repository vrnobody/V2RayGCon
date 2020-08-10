using System;

namespace VgcApis.Interfaces.Services
{
    public interface INotifierService
    {
        void ShowFormOption();
        void ShowFormMain();

        void ShowFormLog();

        void ShowFormQrcode();

        string RegisterHotKey(Action hotKeyHandler,
            string keyName, bool hasAlt, bool hasCtrl, bool hasShift);

        bool UnregisterHotKey(string hotKeyHandle);

        void RefreshNotifyIconLater();

        void Invoke(Action updater);

        void InvokeThen(Action updater, Action next);

    }
}
