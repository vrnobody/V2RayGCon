using System;

namespace VgcApis.Interfaces
{
    public interface ICoreServCtrl

    {
        //内部使用，脚本一般用不到
        event EventHandler OnPropertyChanged, OnCoreStop, OnCoreClosing, OnCoreStart;

        //内部使用，脚本一般用不到
        void InvokeEventOnPropertyChange();

        //服务器的名字、摘要、选中状态等各种信息
        CoreCtrlComponents.ICoreStates GetCoreStates();

        //控制服务器开启、关闭等各种操作
        CoreCtrlComponents.ICoreCtrl GetCoreCtrl();

        //服务器的日志窗口
        CoreCtrlComponents.ILogger GetLogger();

        //修改、替换服务器的config.json
        CoreCtrlComponents.IConfiger GetConfiger();

        //内部使用，脚本一般用不到
        void UpdateCoreSettings(Models.Datas.CoreServSettings coreServSettings);
    }
}
