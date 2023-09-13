using Newtonsoft.Json.Linq;
using System;

namespace VgcApis.Interfaces
{
    public interface IWrappedCoreServCtrl
        : CoreCtrlComponents.IConfiger,
            CoreCtrlComponents.ICoreCtrl,
            CoreCtrlComponents.ICoreStates,
            CoreCtrlComponents.ILogger
    {
        // 注意 ICore*** 系列接口不可以出现同名而且参数个数相同的方法
        // 不然反射时会匹配成错误的函数

        ICoreServCtrl Unwrap();

        #region IConfiger
        new JObject GetFinalConfig();
        new string GetRawConfig();
        new string GetConfig();
        new string GetShareLink();
        new void SetConfig(string newConfig);
        new bool IsSuitableToBeUsedAsSysProxy(bool isGlobal, out bool isSocks, out int port);
        new void UpdateSummary();
        new void GetterInfoForNotifyIconf(Action<string> next);
        #endregion

        #region ICoreCtrl
        new bool IsCoreRunning();
        new void RunSpeedTest();
        new void RunSpeedTestThen();
        new string Fetch(string url);
        new string Fetch(string url, int timeout);
        new void StopCore();
        new void StopCoreThen();
        new void StopCoreThen(Action next);
        new void RestartCore();
        new void RestartCoreIgnoreError();
        new void RestartCoreThen();
        new void RestartCoreThen(Action next);
        #endregion

        #region ICoreStates
        new Models.Datas.CoreInfo GetAllRawCoreInfo();
        new long GetUplinkTotalInBytes();
        new long GetDownlinkTotalInBytes();
        new void SetUplinkTotal(long sizeInBytes);
        new void SetDownlinkTotal(long sizeInBytes);
        new string GetInboundAddr();
        new string GetInboundIp();
        new int GetInboundPort();
        new int GetInboundType();
        new double GetIndex();
        new long GetLastModifiedUtcTicks();
        new long GetLastSpeedTestUtcTicks();
        new string GetMark();
        new string GetRemark();

        new string GetCustomCoreName();

        new void SetCustomCoreName(string name);

        new int GetStatPort();
        new string GetShortName();
        new string GetLongName();
        new string GetName();
        new void SetName(string name);
        new void SetDescription(string description);
        new string GetRawUid();
        new long GetSpeedTestResult();
        new string GetStatus();
        new string GetSummary();
        new string GetTag1();
        new string GetTag2();
        new string GetTag3();
        new string GetTitle();
        new string GetUid();
        new bool IsAutoRun();
        new bool IsUntrack();
        new bool IsSelected();
        new bool IsInjectSkipCnSite();
        new bool IsInjectGlobalImport();
        new void SetIndex(double index);
        new void SetIndexQuiet(double index);
        new void SetIsSelected(bool selected);
        new void SetInboundAddr(string ip, int port);
        new void SetInboundType(int type);
        new void SetLastModifiedUtcTicks(long utcTicks);
        new void SetMark(string mark);
        new void SetRemark(string remark);
        new void SetSpeedTestResult(long latency);
        new void SetTag1(string tag);
        new void SetTag2(string tag);
        new void SetTag3(string tag);
        new void SetIsAutoRun(bool isAutoRun);
        new void SetIsUntrack(bool isUntrack);
        new void SetIsInjectImport(bool isInjectImport);
        new void SetIsInjectSkipCnSite(bool isInjectSkipCnSite);
        #endregion

        #region ILogger
        new void ShowFormLog();
        new Action<string> GetLoggerInstance();
        new string GetLogAsString();
        #endregion
    }
}
