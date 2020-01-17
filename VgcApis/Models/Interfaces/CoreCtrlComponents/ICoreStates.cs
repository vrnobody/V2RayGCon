using System;

namespace VgcApis.Models.Interfaces.CoreCtrlComponents
{
    public interface ICoreStates
    {
        Datas.CoreInfo GetAllRawCoreInfo();

        bool GetterInfoForSearch(Func<string[], bool> filter);

        int GetFoldingState();
        string GetInboundAddr();
        string GetInboundIp();
        int GetInboundPort();
        int GetInboundType();
        double GetIndex();
        long GetLastModifiedUtcTicks();
        string GetMark();
        string GetName();
        string GetRawUid();
        long GetSpeedTestResult();
        string GetStatus();
        string GetSummary();
        string GetTitle();
        string GetUid();

        bool IsAutoRun();
        bool IsUntrack();
        bool IsSelected();
        bool IsInjectSkipCnSite();
        bool IsInjectGlobalImport();

        void SetFoldingState(int state);
        void SetIndex(double index);
        void SetIndexQuiet(double index);
        void SetIsSelected(bool selected);
        void SetInboundAddr(string ip, int port);
        void SetInboundType(int type);
        void SetLastModifiedUtcTicks(long utcTicks);
        void SetMark(string mark);

        void ToggleIsAutoRun();
        void ToggleIsUntrack();
        void ToggleIsInjectImport();
        void ToggleIsInjectSkipCnSite();

    }
}
