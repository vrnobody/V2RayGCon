using NeoLuna.Resources.Langs;
using System;
using System.Windows.Forms;

namespace NeoLuna.Views.WinForms
{
    public partial class FormLog : Form
    {
        long updateTimestamp = -1;
        readonly VgcApis.Libs.Tasks.Routine logUpdater;
        readonly VgcApis.Libs.Sys.QueueLogger qLogger;

        bool isPaused = false;

        public FormLog(VgcApis.Libs.Sys.QueueLogger logger)
        {
            this.qLogger = logger;
            logUpdater = new VgcApis.Libs.Tasks.Routine(
                RefreshUi,
                VgcApis.Models.Consts.Intervals.SiFormLogRefreshInterval
            );

            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void RefreshUi()
        {
            var timestamp = qLogger.GetTimestamp();
            if (updateTimestamp == timestamp)
            {
                return;
            }

            updateTimestamp = timestamp;
            var logs = qLogger.GetLogAsString(true);
            VgcApis.Misc.UI.UpdateRichTextBox(rtBoxLogger, logs);
        }

        private void FormSingleServerLog_Load(object sender, EventArgs e)
        {
            logUpdater.Run();
        }

        private void FormSingleServerLog_FormClosed(object sender, FormClosedEventArgs e)
        {
            logUpdater.Dispose();

            // Potential memory leaks
            // qLogger.Dispose();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmClearLog))
            {
                qLogger.Clear();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isPaused)
            {
                return;
            }
            isPaused = true;
            pauseToolStripMenuItem.Checked = isPaused;
            logUpdater.Pause();
        }

        private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                return;
            }

            isPaused = false;
            pauseToolStripMenuItem.Checked = isPaused;
            logUpdater.Run();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logs = rtBoxLogger.Text;
            var msg = VgcApis.Misc.Utils.CopyToClipboard(logs) ? I18N.CopySuccess : I18N.CopyFail;
            VgcApis.Misc.UI.MsgBoxAsync(msg);
        }
    }
}
