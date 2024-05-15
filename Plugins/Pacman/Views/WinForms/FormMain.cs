using System;
using System.Windows.Forms;

namespace Pacman.Views.WinForms
{
    public partial class FormMain : Form
    {
        readonly Services.Settings settings;
        Controllers.FormMainCtrl formMainCtrl;

        public static FormMain CreateForm(Services.Settings setting)
        {
            FormMain r = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                r = new FormMain(setting);
            });
            return r;
        }

        FormMain(Services.Settings settings)
        {
            this.settings = settings;
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            this.Text = Properties.Resources.Name + " v" + Properties.Resources.Version;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            formMainCtrl = InitFormMainCtrl();
            formMainCtrl.Run();
        }
        #region public methods
        #endregion

        #region private methods
        Controllers.FormMainCtrl InitFormMainCtrl()
        {
            //TextBox tboxName,
            //FlowLayoutPanel flyContent,
            //ListBox lstBoxPackages,
            //Button btnSave,
            //Button btnDelete,
            //Button btnPull,
            //Button btnGenerate)
            return new Controllers.FormMainCtrl(
                settings,
                tboxName,
                flyContents,
                lstBoxPackages,
                btnSave,
                btnDelete,
                btnChain,
                btnImport,
                cboxBalancerStrategy,
                cboxObsInterval,
                cboxObsUrl,
                btnPull,
                btnSelectAll,
                btnSelectInvert,
                btnSelectNone,
                btnDeleteSelected,
                btnRefreshSelected
            );
        }
        #endregion
        #region UI events

        private void flyContents_Scroll(object sender, ScrollEventArgs e)
        {
            flyContents.Refresh();
        }

        private void cboxBalancerStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            var visible = false;
            var strategy = (VgcApis.Models.Datas.Enums.BalancerStrategies)
                cboxBalancerStrategy.SelectedIndex;
            switch (strategy)
            {
                case VgcApis.Models.Datas.Enums.BalancerStrategies.LeastLoad:
                case VgcApis.Models.Datas.Enums.BalancerStrategies.LeastPing:
                    visible = true;
                    break;
                default:
                    break;
            }
            cboxObsInterval.Enabled = visible;
            cboxObsUrl.Enabled = visible;
        }
        #endregion
    }
}
