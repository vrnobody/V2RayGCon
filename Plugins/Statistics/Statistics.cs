using Statistics.Resources.Langs;
using System.Drawing;

namespace Statistics
{
    public class Statistics : VgcApis.Models.BaseClasses.Plugin
    {
        VgcApis.Models.IServices.IApiService api;
        VgcApis.Models.IServices.IServersService vgcServers;
        VgcApis.Models.IServices.ISettingsService vgcSetting;
        Views.WinForms.FormMain formMain = null;
        Services.Settings settings;

        #region properties
        public override string Name => Properties.Resources.Name;
        public override string Version => Properties.Resources.Version;
        public override string Description => I18N.Description;

        public override Image Icon => Properties.Resources.ClientStatistcs_16x;
        #endregion

        #region protected override methods
        protected override void Start(VgcApis.Models.IServices.IApiService api)
        {
            this.api = api;
            vgcSetting = api.GetSettingService();
            vgcServers = api.GetServersService();

            settings = new Services.Settings();
            settings.Run(vgcSetting, vgcServers);
        }

        protected override void Popup()
        {
            if (formMain != null)
            {
                formMain.Activate();
                return;
            }

            formMain = new Views.WinForms.FormMain(
                settings,
                vgcServers);
            formMain.FormClosed += (s, a) => formMain = null;
            formMain.Show();
        }

        protected override void Stop()
        {
            if (formMain != null)
            {
                formMain.Close();
            }

            settings?.Cleanup();
        }
        #endregion

    }
}
