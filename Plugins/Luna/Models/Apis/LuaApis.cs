using Luna.Services;
using System;

namespace Luna.Models.Apis
{
    internal class LuaApis :
        VgcApis.BaseClasses.ComponentOf<LuaApis>
    {
        Services.Settings settings;
        VgcApis.Interfaces.Services.IApiService vgcApi;
        private readonly FormMgr formMgr;
        Action<string> redirectLogWorker;
        readonly SysCmpos.PostOffice postOffice;

        public LuaApis(
            VgcApis.Interfaces.Services.IApiService api,
            Services.Settings settings,
            Services.FormMgr formMgr)
        {
            this.settings = settings;
            redirectLogWorker = settings.SendLog;

            vgcApi = api;
            this.formMgr = formMgr;
            postOffice = new SysCmpos.PostOffice();
        }

        #region public methods
        public SysCmpos.PostOffice GetPostOffice() => postOffice;

        public string RegisterHotKey(Action hotKeyHandler,
              string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {
            var vgcNotifier = vgcApi.GetNotifierService();
            return vgcNotifier.RegisterHotKey(hotKeyHandler, keyName, hasAlt, hasCtrl, hasShift);
        }

        public bool UnregisterHotKey(string hotKeyHandle)
        {
            var vgcNotifier = vgcApi.GetNotifierService();
            return vgcNotifier.UnregisterHotKey(hotKeyHandle);
        }

        public override void Prepare()
        {
            var misc = new Components.Misc(vgcApi, settings, formMgr);
            var web = new Components.Web(vgcApi);
            var server = new Components.Server(vgcApi);

            AddChild(misc);
            AddChild(web);
            AddChild(server);
        }

        #endregion

        #region public methods
        public void SendLog(string message) =>
            redirectLogWorker?.Invoke(message);

        public void SetRedirectLogWorker(Action<string> worker)
        {
            if (worker != null)
            {
                redirectLogWorker = worker;
            }
        }
        #endregion

        #region private methods
        #endregion
    }
}
