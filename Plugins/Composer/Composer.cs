using System.Drawing;
using Composer.Resources.Langs;

namespace Composer
{
    public class Composer : VgcApis.BaseClasses.Plugin
    {
        Services.Settings settings;
        Views.WinForms.FormMain formMain = null;

        // form=null;

        #region properties
        public override string Name => Properties.Resources.Name;
        public override string Version => Properties.Resources.Version;
        public override string Description => I18N.Description;

        public override Image Icon => Properties.Resources.UserCode_16x;

        #endregion

        #region protected overrides
        public override void ShowMainForm()
        {
            if (!GetRunningState())
            {
                return;
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                if (formMain == null)
                {
                    formMain = new Views.WinForms.FormMain(this.settings);
                    formMain.FormClosed += (s, a) => formMain = null;
                    formMain.Show();
                    return;
                }
                formMain.Activate();
            });
        }

        public override void Run(VgcApis.Interfaces.Services.IApiService api)
        {
            if (!SetRunningState(true))
            {
                return;
            }

            settings = new Services.Settings();
            settings.Run(api);
        }

        public override void Stop()
        {
            if (!SetRunningState(false))
            {
                return;
            }

            VgcApis.Misc.UI.CloseFormIgnoreError(formMain);
            settings?.Cleanup();
        }
        #endregion
    }
}
