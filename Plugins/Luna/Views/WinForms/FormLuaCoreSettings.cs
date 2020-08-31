using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    internal partial class FormLuaCoreSettings : Form
    {
        #region Sigleton
        static FormLuaCoreSettings _instant;
        static readonly object formInstanLocker = new object();
        public static void ShowForm(Controllers.LuaCoreCtrl luaCoreCtrl)
        {
            FormLuaCoreSettings f = null;

            if (_instant == null || _instant.IsDisposed)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    f = new FormLuaCoreSettings();
                });
            }

            lock (formInstanLocker)
            {
                if (_instant == null || _instant.IsDisposed)
                {
                    _instant = f;
                    f = null;
                }
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                f?.Close();
                var inst = _instant;
                if (inst != null)
                {
                    inst.InitControls(luaCoreCtrl);
                    inst.Show();
                    inst.Activate();
                }
            });
        }
        #endregion

        Controllers.LuaCoreCtrl luaCoreCtrl = null;

        FormLuaCoreSettings()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        #region public methods

        #endregion

        #region private methods
        void InitControls(Controllers.LuaCoreCtrl luaCoreCtrl)
        {
            this.luaCoreCtrl = luaCoreCtrl;
            UpdateUi();
        }

        void UpdateUi()
        {
            VgcApis.Misc.UI.Invoke(() =>
            {
                tboxName.Text = luaCoreCtrl.name;
                chkAutorun.Checked = luaCoreCtrl.isAutoRun;
                chkHidden.Checked = luaCoreCtrl.isHidden;
                chkClrSupports.Checked = luaCoreCtrl.isLoadClr;
            });
        }
        #endregion

        #region UI events
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            luaCoreCtrl.name = tboxName.Text;
            luaCoreCtrl.isAutoRun = chkAutorun.Checked;
            luaCoreCtrl.isHidden = chkHidden.Checked;
            luaCoreCtrl.isLoadClr = chkClrSupports.Checked;
            Close();
        }
        #endregion

    }
}
