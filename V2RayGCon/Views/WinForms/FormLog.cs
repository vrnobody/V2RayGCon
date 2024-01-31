using System;
using System.Windows.Forms;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormLog : Form
    {
        #region Sigleton
        static readonly VgcApis.BaseClasses.AuxSiWinForm<FormLog> auxSiForm =
            new VgcApis.BaseClasses.AuxSiWinForm<FormLog>();

        public static FormLog GetForm() => auxSiForm.GetForm();

        public static void ShowForm() => auxSiForm.ShowForm();
        #endregion

        long updateTimeStamp = DateTime.Now.Ticks;

        VgcApis.Libs.Tasks.Routine logDisplayer;

        public FormLog()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);

            this.FormClosed += (s, e) => logDisplayer?.Dispose();
            Misc.UI.SetFormLocation(this, Models.Datas.Enums.FormLocations.BottomLeft);
        }

        private void FormLog_Load(object sender, EventArgs e)
        {
            logDisplayer = new VgcApis.Libs.Tasks.Routine(UpdateLog, 500);
            logDisplayer.Restart();
            // throw new NullReferenceException("for debugging");
        }

        #region private methods
        void UpdateLog()
        {
            var timestamp = VgcApis.Misc.Logger.GetTimestamp();
            if (updateTimeStamp == timestamp)
            {
                return;
            }
            updateTimeStamp = timestamp;
            var text = VgcApis.Misc.Logger.GetContent();

            VgcApis.Misc.UI.UpdateRichTextBox(rtBoxLogger, text);
        }

        #endregion

        #region UI events
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pause = !pauseToolStripMenuItem.Checked;
            pauseToolStripMenuItem.Checked = pause;
            if (pause)
            {
                logDisplayer.Stop();
            }
            else
            {
                logDisplayer.Restart();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logs = rtBoxLogger.Text;
            var msg = VgcApis.Misc.Utils.CopyToClipboard(logs)
                ? Resources.Resx.I18N.CopySuccess
                : Resources.Resx.I18N.CopyFail;
            VgcApis.Misc.UI.MsgBoxAsync(msg);
        }

        #endregion
    }
}
