using System.Drawing;
using Pacman.Resources.Langs;

namespace Pacman
{
    public class Pacman : VgcApis.BaseClasses.Plugin
    {
        Services.Settings settings;
        Views.WinForms.FormMain formMain = null;

        // form=null;

        #region properties
        public override string Name => Properties.Resources.Name;
        public override string Version => Properties.Resources.Version;
        public override string Description => I18N.Description;

        // png source https://www.flaticon.com/free-icon/pacman_1191124#term=pacman&page=1&position=31
        public override Image Icon => Properties.Resources.pacman_x32;

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
                if (formMain != null)
                {
                    formMain.Activate();
                    return;
                }

                formMain = Views.WinForms.FormMain.CreateForm(settings);
                formMain.FormClosed += (s, a) => formMain = null;
                formMain.Show();
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
