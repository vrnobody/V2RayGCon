using System;
using VgcApis.Interfaces.CoreCtrlComponents;

namespace V2RayGCon.Controllers
{
    public class CoreServerCtrl :
        VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
        VgcApis.Interfaces.ICoreServCtrl
    {
        public event EventHandler
            OnPropertyChanged,
            OnCoreClosing,
            OnCoreStop,
            OnCoreStart;

        VgcApis.Models.Datas.CoreInfo coreInfo;

        CoreServerComponent.CoreStates states;
        CoreServerComponent.Logger logger;
        CoreServerComponent.Configer configer;
        CoreServerComponent.CoreCtrl coreCtrl;

        public CoreServerCtrl(
            VgcApis.Models.Datas.CoreInfo coreInfo)
        {
            this.coreInfo = coreInfo;
        }

        public void Run(
             Services.Cache cache,
             Services.Settings setting,
             Services.ConfigMgr configMgr,
             Services.Servers servers)
        {
            //external dependency injection
            coreCtrl = new CoreServerComponent.CoreCtrl(setting, configMgr);
            states = new CoreServerComponent.CoreStates(servers, coreInfo);
            logger = new CoreServerComponent.Logger(setting);
            configer = new CoreServerComponent.Configer(
                setting, cache, configMgr, servers, coreInfo);

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
        public void InvokeEventOnCoreClosing() =>
            OnCoreClosing?.Invoke(this, EventArgs.Empty);

        public void InvokeEventOnPropertyChange() =>
            InvokeEmptyEventIgnoreError(OnPropertyChanged);

        public void InvokeEventOnCoreStop() =>
            OnCoreStop?.Invoke(this, EventArgs.Empty);

        public void InvokeEventOnCoreStart() =>
            OnCoreStart?.Invoke(this, EventArgs.Empty);

        #endregion

        #region public method
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
            InvokeEventOnCoreClosing();
            coreCtrl?.StopCore();
            coreCtrl?.ReleaseEvents();
        }
        #endregion
    }
}
