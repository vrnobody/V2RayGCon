using System;

namespace VgcApis.Interfaces.Services
{
    public interface INotifierService
    {
        #region winforms
        void ShowFormTextEditor(string config);
        void ShowFormServerSettings(ICoreServCtrl coreServ);
        void ShowFormSimpleEditor(ICoreServCtrl coreServ);
        void ShowFormOption();
        void ShowFormMain();

        void ShowFormWebUI();

        void ShowFormLog();
        #endregion

        void Notify(string title, string content, int timeout);

        string RegisterHotKey(
            Action hotKeyHandler,
            string keyName,
            bool hasAlt,
            bool hasCtrl,
            bool hasShift
        );

        void SetNotifyIconTag(string tag);

        bool UnregisterHotKey(string hotKeyHandle);

        void RefreshNotifyIconLater();

        void Invoke(Action updater);

        void InvokeThen(Action updater, Action next);
    }
}
