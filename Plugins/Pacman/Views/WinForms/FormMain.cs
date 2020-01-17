using System;
using System.Windows.Forms;

namespace Pacman.Views.WinForms
{
    public partial class FormMain : Form
    {
        Services.Settings settings;
        Controllers.FormMainCtrl formMainCtrl;

        public FormMain(Services.Settings settings)
        {
            this.settings = settings;
            InitializeComponent();

            VgcApis.Libs.UI.AutoSetFormIcon(this);
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
                btnPull,
                btnImport,

                btnSelectAll,
                btnSelectInvert,
                btnSelectNone,
                btnDeleteSelected,
                btnRefreshSelected);
        }
        #endregion
    }
}
