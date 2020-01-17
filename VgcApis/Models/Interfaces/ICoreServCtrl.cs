using System;

namespace VgcApis.Models.Interfaces
{
    public interface ICoreServCtrl
    {
        event EventHandler OnPropertyChanged;
        event EventHandler OnCoreStop;
        event EventHandler OnCoreStart;

        void InvokeEventOnPropertyChange();
        CoreCtrlComponents.ICoreStates GetCoreStates();
        CoreCtrlComponents.ICoreCtrl GetCoreCtrl();
        CoreCtrlComponents.ILogger GetLogger();
        CoreCtrlComponents.IConfiger GetConfiger();
    }
}
