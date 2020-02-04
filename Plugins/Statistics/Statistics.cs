using Statistics.Resources.Langs;
using System.Drawing;

namespace Statistics
{
    public class Statistics : VgcApis.BaseClasses.Plugin
    {
        VgcApis.Interfaces.Services.IApiService api;
        VgcApis.Interfaces.Services.IServersService vgcServers;
        VgcApis.Interfaces.Services.ISettingsService vgcSetting;
        Views.WinForms.FormMain formMain = null;
        Services.Settings settings;

        #region properties
        public override string Name => Properties.Resources.Name;
        public override string Version => Properties.Resources.Version;
        public override string Description => I18N.Description;

        public override Image Icon => Properties.Resources.ClientStatistcs_16x;
        #endregion

        #region protected override methods
        protected override void Start(VgcApis.Interfaces.Services.IApiService api)
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
            VgcApis.Misc.UI.CloseFormIgnoreError(formMain);
            settings?.Cleanup();
        }
        #endregion

    }
}
