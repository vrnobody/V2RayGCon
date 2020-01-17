using System;

namespace Luna.Models.Apis
{
    public class LuaApis :
        VgcApis.Models.BaseClasses.ComponentOf<LuaApis>
    {
        Services.Settings settings;
        VgcApis.Models.IServices.IApiService vgcApi;
        Action<string> redirectLogWorker;

        public LuaApis(
            Services.Settings settings,
            VgcApis.Models.IServices.IApiService api)
        {
            this.settings = settings;
            this.redirectLogWorker = settings.SendLog;
            vgcApi = api;
        }

        #region public methods
        public override void Prepare()
        {
            var misc = new Components.Misc(settings, vgcApi);
            var json = new Components.Json(vgcApi);
            var web = new Components.Web(vgcApi);
            var server = new Components.Server(vgcApi);

            Plug(this, misc);
            Plug(this, json);
            Plug(this, web);
            Plug(this, server);
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
