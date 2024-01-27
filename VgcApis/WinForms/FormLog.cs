using System;
using System.Windows.Forms;
using VgcApis.Resources.Langs;

namespace VgcApis.WinForms
{
    public partial class FormLog : Form
    {
        public static FormLog CreateLogForm(string title, Libs.Sys.QueueLogger logger)
        {
            FormLog logForm = null;
            Misc.UI.Invoke(() =>
            {
                logForm = new FormLog(title, logger);
                logForm.Show();
            });
            return logForm;
        }

        long updateTimestamp = -1;
        readonly Libs.Tasks.Routine logUpdater;
        readonly Libs.Sys.QueueLogger qLogger;

        bool isPaused = false;

        FormLog(string title, Libs.Sys.QueueLogger logger)
        {
            this.qLogger = logger;
            logUpdater = new Libs.Tasks.Routine(
                RefreshUi,
                Models.Consts.Intervals.SiFormLogRefreshInterval
            );

            InitializeComponent();
            Misc.UI.AutoSetFormIcon(this);
            this.Text = I18N.Log + " - " + title;
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
            Misc.UI.UpdateRichTextBox(rtBoxLogger, logs);
        }

        private void FormSingleServerLog_Load(object sender, EventArgs e)
        {
            logUpdater.Restart();
        }

        private void FormSingleServerLog_FormClosed(object sender, FormClosedEventArgs e)
        {
            logUpdater.Dispose();

            // Potential memory leaks
            // qLogger.Dispose();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Misc.UI.Confirm(I18N.ConfirmClearLog))
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
            isPaused = !isPaused;
            pauseToolStripMenuItem.Checked = isPaused;
            if (isPaused)
            {
                logUpdater.Stop();
            }
            else
            {
                logUpdater.Restart();
            }
        }
    }
}
