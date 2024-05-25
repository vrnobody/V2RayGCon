using System;
using System.Collections.Generic;
using System.Threading;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.CoreServerComponent
{
    public class CoreStates
        : VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
            VgcApis.Interfaces.CoreCtrlComponents.ICoreStates
    {
        readonly VgcApis.Models.Datas.CoreInfo coreInfo;
        readonly Services.Servers servers;

        public CoreStates(Services.Servers servers, VgcApis.Models.Datas.CoreInfo coreInfo)
        {
            this.servers = servers;
            this.coreInfo = coreInfo;
        }

        CoreCtrl coreCtrl;
        Configer configer;

        public override void Prepare()
        {
            coreCtrl = GetSibling<CoreCtrl>();
            configer = GetSibling<Configer>();

            UpdateStatusWithSpeedTestResult();
        }

        #region public methods
        public string GetCustomTemplateNames()
        {
            return coreInfo.templates;
        }

        public void SetCustomTemplateNames(string tpls)
        {
            if (coreInfo.templates == tpls)
            {
                return;
            }

            coreInfo.templates = tpls;
            configer.UpdateSummary();
            if (coreCtrl.IsCoreRunning())
            {
                coreCtrl.RestartCore();
            }
            GetParent().InvokeEventOnPropertyChange();
        }

        public void SetName(string name)
        {
            name = VgcApis.Misc.Utils.FilterControlChars(name);
            var isEmpty = string.IsNullOrEmpty(name);
            if (name == coreInfo.name && !isEmpty)
            {
                return;
            }

            coreInfo.name = name;

            if (isEmpty)
            {
                name = I18N.Empty;
            }

            coreInfo.longName = VgcApis.Misc.Utils.AutoEllipsis(
                name,
                VgcApis.Models.Consts.AutoEllipsis.ServerLongNameMaxLength
            );

            coreInfo.shortName = VgcApis.Misc.Utils.AutoEllipsis(
                name,
                VgcApis.Models.Consts.AutoEllipsis.ServerShortNameMaxLength
            );

            coreInfo.title = string.Empty;
            GetParent().InvokeEventOnPropertyChange();
        }

        public void AddStatSample(VgcApis.Models.Datas.StatsSample sample)
        {
            if (sample == null)
            {
                return;
            }

            var up = sample.statsUplink;
            var down = sample.statsDownlink;

            var changed = false;
            if (up > 0)
            {
                Interlocked.Add(ref coreInfo.totalUplinkInBytes, up);
                changed = true;
            }

            if (down > 0)
            {
                Interlocked.Add(ref coreInfo.totalDownlinkInBytes, down);
                changed = true;
            }

            if (changed)
            {
                GetParent().InvokeEventOnPropertyChange();
            }
        }

        public long GetUplinkTotalInBytes() => coreInfo.totalUplinkInBytes;

        public long GetDownlinkTotalInBytes() => coreInfo.totalDownlinkInBytes;

        public void SetUplinkTotal(long sizeInBytes)
        {
            if (coreInfo.totalUplinkInBytes != sizeInBytes)
            {
                Interlocked.Exchange(ref coreInfo.totalUplinkInBytes, sizeInBytes);
                GetParent().InvokeEventOnPropertyChange();
            }
        }

        public void SetDownlinkTotal(long sizeInBytes)
        {
            if (coreInfo.totalDownlinkInBytes != sizeInBytes)
            {
                Interlocked.Exchange(ref coreInfo.totalDownlinkInBytes, sizeInBytes);
                GetParent().InvokeEventOnPropertyChange();
            }
        }

        public void SetIndexQuiet(double index) => SetIndexWorker(index, true);

        public void SetIndex(double index) => SetIndexWorker(index, false);

        void SetIndexWorker(double index, bool quiet)
        {
            if (VgcApis.Misc.Utils.AreEqual(coreInfo.index, index))
            {
                return;
            }

            coreInfo.index = index;
            coreInfo.title = string.Empty;
            GetParent().InvokeEventOnIndexChange();
            if (!quiet)
            {
                GetParent().InvokeEventOnPropertyChange();
            }
        }

        public bool IsIgnoreSendThrough() => coreInfo.ignoreSendThrough;

        public void SetIgnoreSendThrough(bool ignored) =>
            SetSettingsPropertyOnDemand(ref coreInfo.ignoreSendThrough, ignored);

        public void SetIsAcceptInjection(bool isEnabled) =>
            SetSettingsPropertyOnDemand(ref coreInfo.isAcceptInjection, isEnabled);

        public void SetIsAutoRun(bool isAutoRun) =>
            SetSettingsPropertyOnDemand(ref coreInfo.isAutoRun, isAutoRun);

        public void SetIsUntrack(bool isUntrack) =>
            SetSettingsPropertyOnDemand(ref coreInfo.isUntrack, isUntrack);

        public VgcApis.Models.Datas.CoreInfo GetAllRawCoreInfo() => coreInfo;

        public string GetUid()
        {
            var changed = false;
            lock (coreInfo)
            {
                if (string.IsNullOrEmpty(coreInfo.uid))
                {
                    coreInfo.uid = Guid.NewGuid().ToString();
                    changed = true;
                }
            }

            if (changed)
            {
                // 保存配置
                GetParent().InvokeEventOnPropertyChange();
            }
            return coreInfo.uid;
        }

        public double GetIndex() => coreInfo.index;

        public string GetMark() => coreInfo.customMark;

        public string GetSummary() => coreInfo.summary ?? @"";

        public long GetLastModifiedUtcTicks() => coreInfo.lastModifiedUtcTicks;

        public void SetLastModifiedUtcTicks(long utcTicks) =>
            SetPropertyOnDemand(ref coreInfo.lastModifiedUtcTicks, utcTicks);

        public void SetIsSelected(bool selected)
        {
            if (selected == coreInfo.isSelected)
            {
                return;
            }
            coreInfo.isSelected = selected;
            GetParent().InvokeEventOnPropertyChange();
        }

        public void SetInboundIp(string ip)
        {
            if (ip != coreInfo.inbIp)
            {
                coreInfo.inbIp = ip;
                configer.UpdateSummary();
                GetParent().InvokeEventOnPropertyChange();
            }
        }

        public void SetInboundPort(int port)
        {
            if (port != coreInfo.inbPort)
            {
                coreInfo.inbPort = port;
                configer.UpdateSummary();
                GetParent().InvokeEventOnPropertyChange();
            }
        }

        public void SetInboundAddr(string ip, int port)
        {
            var changed = false;

            if (ip != coreInfo.inbIp)
            {
                coreInfo.inbIp = ip;
                changed = true;
            }

            if (port != coreInfo.inbPort)
            {
                coreInfo.inbPort = port;
                changed = true;
            }

            if (changed)
            {
                configer.UpdateSummary();
                GetParent().InvokeEventOnPropertyChange();
            }
        }

        public void SetInboundName(string name)
        {
            if (coreInfo.inbName == name)
            {
                return;
            }
            coreInfo.inbName = name;

            configer.UpdateSummary();
            GetParent().InvokeEventOnPropertyChange();
            if (coreCtrl.IsCoreRunning())
            {
                coreCtrl.RestartCoreThen();
            }
        }

        public string GetInboundName()
        {
            return coreInfo.inbName;
        }

        public string GetInboundAddr() => $"{coreInfo.inbIp}:{coreInfo.inbPort}";

        public void SetMark(string mark)
        {
            if (coreInfo.customMark == mark)
            {
                return;
            }

            coreInfo.customMark = mark;
            servers.AddNewMark(mark);
            GetParent().InvokeEventOnPropertyChange();
        }

        public void SetRemark(string remark)
        {
            if (coreInfo.customRemark == remark)
            {
                return;
            }

            coreInfo.customRemark = remark;
            GetParent().InvokeEventOnPropertyChange();
        }

        public string GetTag1() => coreInfo.tag1;

        public string GetTag2() => coreInfo.tag2;

        public string GetTag3() => coreInfo.tag3;

        public void SetTag1(string tag)
        {
            if (tag == coreInfo.tag1)
            {
                return;
            }
            coreInfo.tag1 = tag;
            GetParent().InvokeEventOnPropertyChange();
        }

        public void SetTag2(string tag)
        {
            if (tag == coreInfo.tag2)
            {
                return;
            }
            coreInfo.tag2 = tag;
            GetParent().InvokeEventOnPropertyChange();
        }

        public void SetTag3(string tag)
        {
            if (tag == coreInfo.tag3)
            {
                return;
            }
            coreInfo.tag3 = tag;
            GetParent().InvokeEventOnPropertyChange();
        }

        public string GetRemark() => coreInfo.customRemark;

        public string GetInboundIp() => coreInfo.inbIp;

        public int GetInboundPort() => coreInfo.inbPort;

        public bool IsAcceptInjection() => coreInfo.isAcceptInjection;

        public bool IsAutoRun() => coreInfo.isAutoRun;

        public bool IsSelected() => coreInfo.isSelected;

        public bool IsUntrack() => coreInfo.isUntrack;

        public string GetTitle()
        {
            var ci = coreInfo;
            if (string.IsNullOrEmpty(ci.title))
            {
                var result = $"{GetIndex()}.[{GetShortName()}] {GetSummary()}";
                ci.title = VgcApis.Misc.Utils.AutoEllipsis(
                    result,
                    VgcApis.Models.Consts.AutoEllipsis.ServerTitleMaxLength
                );
            }
            return ci.title;
        }

        public string GetLongName()
        {
            return coreInfo.longName;
        }

        public string GetShortName()
        {
            return coreInfo.shortName;
        }

        public string GetName() => coreInfo.name;

        int statPort = -1;

        public int GetStatPort() => statPort;

        /// <summary>
        /// less or eq 0 means unavailable
        /// </summary>
        public void SetStatPort(int port) => statPort = port;

        string status = @"";

        public string GetStatus() => status;

        public void SetStatus(string text)
        {
            if (status == text)
            {
                return;
            }

            status = text;
            GetParent().InvokeEventOnPropertyChange();
        }

        public long GetLastSpeedTestUtcTicks() => coreInfo.lastSpeedTestUtcTicks;

        public long GetSpeedTestResult() => coreInfo.speedTestResult;

        public void SetSpeedTestResult(long latency)
        {
            coreInfo.lastSpeedTestUtcTicks = DateTime.UtcNow.Ticks;

            // 0: testing <0: none long.max: timeout >0: ???ms
            if (coreInfo.speedTestResult == latency)
            {
                return;
            }

            coreInfo.speedTestResult = latency;
            UpdateStatusWithSpeedTestResult();
            GetParent().InvokeEventOnPropertyChange();
        }

        public string GetRawUid() => coreInfo.uid;
        #endregion

        #region private methods

        void UpdateStatusWithSpeedTestResult()
        {
            var latency = GetSpeedTestResult();

            var status = @"";
            if (latency > 0)
            {
                status = latency == long.MaxValue ? I18N.Timeout : $"{latency}ms";
            }
            SetStatus(status);
        }

        void SetSettingsPropertyOnDemand(ref bool property, bool value, bool requireRestart = false)
        {
            if (property == value)
            {
                return;
            }

            property = value;

            // refresh UI immediately
            GetParent().InvokeEventOnPropertyChange();

            // time consuming things
            if (requireRestart && coreCtrl.IsCoreRunning())
            {
                coreCtrl.RestartCoreThen();
            }
        }

        bool SetPropertyOnDemand<T>(ref T property, T value)
            where T : struct => SetPropertyOnDemandWorker(ref property, value);

        bool SetPropertyOnDemandWorker<T>(ref T property, T value)
        {
            bool changed = false;
            if (!EqualityComparer<T>.Default.Equals(property, value))
            {
                property = value;
                GetParent().InvokeEventOnPropertyChange();
                changed = true;
            }
            return changed;
        }
        #endregion
    }
}
