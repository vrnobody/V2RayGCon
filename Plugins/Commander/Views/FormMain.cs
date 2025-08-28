using System;
using System.Windows.Forms;

namespace Commander.Views
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
            return new Controllers.FormMainCtrl();
        }
        #endregion

        #region UI events


        #endregion

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
