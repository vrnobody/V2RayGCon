using System;
using VgcApis.Interfaces;
using VgcApis.Interfaces.CoreCtrlComponents;
using VgcApis.Models.Datas;

namespace V2RayGCon.Controllers
{
#pragma warning disable CA1036 // Override methods on comparable types
    public class CoreServerCtrl
#pragma warning restore CA1036 // Override methods on comparable types
        : VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
            ICoreServCtrl,
            IComparable
    {
        public event EventHandler OnPropertyChanged,
            OnCoreClosing,
            OnCoreStop,
            OnCoreStart,
            OnIndexChanged;

        readonly CoreInfo coreInfo;
        CoreServerComponent.CoreStates states;
        CoreServerComponent.Logger logger;
        CoreServerComponent.Configer configer;
        CoreServerComponent.CoreCtrl coreCtrl;

        bool isDisposed = false;

        public CoreServerCtrl(CoreInfo coreInfo)
        {
            this.coreInfo = coreInfo;
        }

        Services.Servers servSvc = null;

        public void Run(
            Services.Cache cache,
            Services.Settings setting,
            Services.ConfigMgr configMgr,
            Services.Servers servers
        )
        {
            servSvc = servers;

            //external dependency injection
            coreCtrl = new CoreServerComponent.CoreCtrl(setting, coreInfo, configMgr);
            states = new CoreServerComponent.CoreStates(servers, coreInfo);
            logger = new CoreServerComponent.Logger(setting);
            configer = new CoreServerComponent.Configer(setting, cache, coreInfo);

            AddChild(coreCtrl);
            AddChild(states);
            AddChild(logger);
            AddChild(configer);

            //inter-container dependency injection
            coreCtrl.Prepare();
            states.Prepare();
            logger.Prepare();
            configer.Prepare();

            //other initializiations
            coreCtrl.BindEvents();
        }

        #region event relay
        public void InvokeEventOnCoreClosing() => OnCoreClosing?.Invoke(this, EventArgs.Empty);

        public void InvokeEventOnIndexChange() => InvokeEmptyEventIgnoreError(OnIndexChanged);

        public void InvokeEventOnPropertyChange() => InvokeEmptyEventIgnoreError(OnPropertyChanged);

        public void InvokeEventOnCoreStop() => OnCoreStop?.Invoke(this, EventArgs.Empty);

        public void InvokeEventOnCoreStart() => OnCoreStart?.Invoke(this, EventArgs.Empty);

        #endregion

        #region private methods

        bool SetCustomInboundInfo(CoreServSettings cs)
        {
            var ci = coreInfo;
            var restartCore = false;
            if (cs.inboundName != ci.inbName)
            {
                ci.inbName = cs.inboundName;
                restartCore = true;
            }

            if (VgcApis.Misc.Utils.TryParseAddress(cs.inboundAddress, out var ip, out var port))
            {
                if (ci.inbIp != ip)
                {
                    ci.inbIp = ip;
                    restartCore = true;
                }
                if (ci.inbPort != port)
                {
                    ci.inbPort = port;
                    restartCore = true;
                }
            }

            return restartCore;
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is ICoreServCtrl target)
            {
                return this.GetCoreStates().GetIndex().CompareTo(target.GetCoreStates().GetIndex());
            }

            throw new ArgumentException("Object is not a VgcApis.Interfaces.ICoreServCtrl");
        }
        #endregion

        #region public methods
        public IWrappedCoreServCtrl Wrap()
        {
            return CoreServerComponent.Wrapper.Wrap(this);
        }

        public void UpdateCoreSettings(CoreServSettings coreServSettings)
        {
            if (isDisposed)
            {
                return;
            }

            var cs = coreServSettings;
            var ci = coreInfo;
            var cst = GetCoreStates();

            cst.SetName(cs.serverName);
            ci.customMark = cs.mark;
            ci.customRemark = cs.remark;
            ci.isAutoRun = cs.isAutorun;
            ci.isUntrack = cs.isUntrack;

            bool indexChanged = false;
            if (!VgcApis.Misc.Utils.AreEqual(ci.index, cs.index))
            {
                indexChanged = true;
                var dt = ci.index > cs.index ? -0.01 : +0.01;
                ci.index = cs.index + dt;
            }

            bool restartCore = SetCustomInboundInfo(cs);
            if (GetCoreCtrl().SetCustomCoreName(cs.customCoreName))
            {
                restartCore = true;
            }

            if (ci.templates != cs.templates)
            {
                ci.templates = cs.templates;
                restartCore = true;
            }

            GetConfiger().UpdateSummary();

            if (indexChanged)
            {
                servSvc.ResetIndexQuiet();
                servSvc.RequireFormMainReload();
            }

            if (restartCore && GetCoreCtrl().IsCoreRunning())
            {
                GetCoreCtrl().RestartCore();
            }
        }

        public ICoreStates GetCoreStates() => states;

        public ICoreCtrl GetCoreCtrl() => coreCtrl;

        public ILogger GetLogger() => logger;

        public IConfiger GetConfiger() => configer;
        #endregion

        #region private method
        void InvokeEmptyEvent(EventHandler evHandler)
        {
            evHandler?.Invoke(null, EventArgs.Empty);
        }

        void InvokeEmptyEventIgnoreError(EventHandler evHandler)
        {
            try
            {
                InvokeEmptyEvent(evHandler);
            }
            catch { }
        }
        #endregion

        #region protected methods
        protected override void CleanupBeforeChildrenDispose()
        {
            isDisposed = true;
            InvokeEventOnCoreClosing();
            coreCtrl?.StopCore();
            coreCtrl?.ReleaseEvents();
        }
        #endregion
    }
}
