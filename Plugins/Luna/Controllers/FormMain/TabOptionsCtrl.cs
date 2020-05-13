using Luna.Resources.Langs;
using System.Windows.Forms;

namespace Luna.Controllers
{
    internal class TabOptionsCtrl
    {
        private readonly Form formMgr;
        private readonly CheckBox chkLoadClrLib;
        private readonly CheckBox chkEnableCodeAnalyze;
        private readonly Button btnSaveOptions;
        private readonly Button btnExit;

        public TabOptionsCtrl(
            Form formMgr,
            CheckBox chkLoadClrLib,
            CheckBox chkEnableCodeAnalyze,
            Button btnSaveOptions,
            Button btnExit)
        {
            this.formMgr = formMgr;
            this.chkLoadClrLib = chkLoadClrLib;
            this.chkEnableCodeAnalyze = chkEnableCodeAnalyze;
            this.btnSaveOptions = btnSaveOptions;
            this.btnExit = btnExit;
        }

        Services.Settings settings;
        public void Run(Services.Settings settings)
        {
            this.settings = settings;
            InitControls();
            BindEvents();
        }

        public bool IsChanged()
        {
            if (chkLoadClrLib.Checked != settings.isLoadClrLib
                || chkEnableCodeAnalyze.Checked != settings.isEnableCodeAnalyze)
            {
                return true;
            }
            return false;
        }

        #region private methods
        void BindEvents()
        {
            btnSaveOptions.Click += (s, a) =>
            {
                if (IsChanged())
                {
                    settings.isLoadClrLib = chkLoadClrLib.Checked;
                    settings.isEnableCodeAnalyze = chkEnableCodeAnalyze.Checked;
                }
                VgcApis.Misc.UI.MsgBox(I18N.Done);
            };

            btnExit.Click += (s, a) => formMgr.Close();
        }

        void InitControls()
        {
            chkLoadClrLib.Checked = settings.isLoadClrLib;
            chkEnableCodeAnalyze.Checked = settings.isEnableCodeAnalyze;
        }
        #endregion

    }
}
