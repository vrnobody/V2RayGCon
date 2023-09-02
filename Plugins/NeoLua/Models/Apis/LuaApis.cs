using NeoLuna.Services;
using System;

namespace NeoLuna.Models.Apis
{
    internal class LuaApis : VgcApis.BaseClasses.ComponentOf<LuaApis>
    {
        public readonly Settings settings;
        public readonly VgcApis.Interfaces.Services.IApiService vgcApi;
        public readonly FormMgrSvc formMgr;
        Action<string> redirectLogWorker;

        public LuaApis(FormMgrSvc formMgr)
        {
            settings = formMgr.settings;
            redirectLogWorker = settings.SendLog;
            vgcApi = formMgr.vgcApi;
            this.formMgr = formMgr;
        }

        #region public methods
        public VgcApis.Interfaces.Services.IServersService GetVgcServerService() =>
            vgcApi.GetServersService();

        public VgcApis.Interfaces.Services.IUtilsService GetVgcUtilsService() =>
            vgcApi.GetUtilsService();

        public VgcApis.Interfaces.Services.IPostOffice GetPostOffice() =>
            vgcApi.GetPostOfficeService();

        public string RegisterHotKey(
            Action hotKeyHandler,
            string keyName,
            bool hasAlt,
            bool hasCtrl,
            bool hasShift
        )
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
        public void SendLog(string message) => redirectLogWorker?.Invoke(message);

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
