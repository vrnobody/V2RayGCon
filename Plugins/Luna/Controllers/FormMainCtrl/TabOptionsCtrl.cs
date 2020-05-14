using Luna.Resources.Langs;
using System.Windows.Forms;

namespace Luna.Controllers.FormMainCtrl
{
    internal class TabOptionsCtrl
    {
        private readonly CheckBox chkLoadClrLib;
        private readonly CheckBox chkEnableCodeAnalyze;
        private readonly Button btnSaveOptions;

        public TabOptionsCtrl(
            CheckBox chkLoadClrLib,
            CheckBox chkEnableCodeAnalyze,
            Button btnSaveOptions)
        {
            this.chkLoadClrLib = chkLoadClrLib;
            this.chkEnableCodeAnalyze = chkEnableCodeAnalyze;
            this.btnSaveOptions = btnSaveOptions;
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
                || chkEnableCodeAnalyze.Checked != settings.isEnableAdvanceAutoComplete)
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
                    settings.isEnableAdvanceAutoComplete = chkEnableCodeAnalyze.Checked;
                }
                VgcApis.Misc.UI.MsgBox(I18N.Done);
            };
        }

        void InitControls()
        {
            chkLoadClrLib.Checked = settings.isLoadClrLib;
            chkEnableCodeAnalyze.Checked = settings.isEnableAdvanceAutoComplete;
        }
        #endregion

    }
}
