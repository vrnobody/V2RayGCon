using System;

namespace VgcApis.Interfaces
{
    public interface ICoreServCtrl
    {
        event EventHandler OnPropertyChanged, OnCoreStop, OnCoreClosing, OnCoreStart;

        void InvokeEventOnPropertyChange();
        CoreCtrlComponents.ICoreStates GetCoreStates();
        CoreCtrlComponents.ICoreCtrl GetCoreCtrl();
        CoreCtrlComponents.ILogger GetLogger();
        CoreCtrlComponents.IConfiger GetConfiger();

        void UpdateCoreSettings(Models.Datas.CoreServSettings coreServSettings);
    }
}
