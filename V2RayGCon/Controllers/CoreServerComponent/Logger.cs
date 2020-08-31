namespace V2RayGCon.Controllers.CoreServerComponent
{
    sealed public class Logger :
        VgcApis.BaseClasses.ComponentOf<CoreServerCtrl>,
        VgcApis.Interfaces.CoreCtrlComponents.ILogger
    {
        VgcApis.Libs.Sys.QueueLogger qLogger = new VgcApis.Libs.Sys.QueueLogger();
        Services.Settings setting;

        public Logger(Services.Settings setting)
        {
            this.setting = setting;
        }

        #region public methods
        public void Log(string message)
        {
            qLogger.Log(message);
            try
            {
                setting.SendLog($"[{coreInfo.GetIndex()}.{coreInfo.GetShortName()}] {message}");
            }
            catch { }
        }

        CoreStates coreInfo;
        public override void Prepare()
        {
            coreInfo = GetParent().GetChild<CoreStates>();
        }

        Views.WinForms.FormSingleServerLog logForm = null;
        readonly object formLogLocker = new object();
        public void ShowFormLog()
        {
            Views.WinForms.FormSingleServerLog form = null;

            if (logForm == null)
            {
                var title = coreInfo.GetSummary();
                VgcApis.Misc.UI.Invoke(() =>
                {
                    form = Views.WinForms.FormSingleServerLog.CreateLogForm(title, qLogger);
                });
            }

            lock (formLogLocker)
            {
                if (logForm == null)
                {
                    logForm = form;
                }
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                if (logForm == form && form != null)
                {
                    form.FormClosed += (s, a) => logForm = null;
                }
                else
                {
                    form?.Close();
                }
                logForm?.Activate();
            });
        }
        #endregion

        #region private methods

        #endregion

        #region protected methods
        protected override void CleanupAfterChildrenDisposed()
        {
            VgcApis.Misc.UI.CloseFormIgnoreError(logForm);
            qLogger.Dispose();
        }
        #endregion
    }
}
