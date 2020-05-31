using System;

namespace VgcApis.Interfaces.CoreCtrlComponents
{
    public interface ICoreStates
    {
        Models.Datas.CoreInfo GetAllRawCoreInfo();

        bool GetterInfoForSearch(Func<string[], bool> filter);

        string GetInboundAddr();
        string GetInboundIp();
        int GetInboundPort();
        int GetInboundType();
        double GetIndex();
        long GetLastModifiedUtcTicks();

        long GetLastSpeedTestUtcTicks();

        string GetMark();
        string GetRemark();

        string GetShortName();
        string GetLongName();
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

        void SetIndex(double index);
        void SetIndexQuiet(double index);
        void SetIsSelected(bool selected);
        void SetInboundAddr(string ip, int port);
        void SetInboundType(int type);
        void SetLastModifiedUtcTicks(long utcTicks);
        void SetMark(string mark);

        void SetRemark(string remark);
        void SetSpeedTestResult(long latency);

        void SetIsAutoRun(bool isAutoRun);
        void SetIsUntrack(bool isUntrack);
        void SetIsInjectImport(bool isInjectImport);
        void SetIsInjectSkipCnSite(bool isInjectSkipCnSite);

    }
}
