using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormSingleServerLog : Form
    {
        public static FormSingleServerLog CreateLogForm(
            string title,
            VgcApis.Libs.Sys.QueueLogger logger
        )
        {
            FormSingleServerLog logForm = null;
            VgcApis.Misc.UI.Invoke(() =>
            {
                logForm = new FormSingleServerLog(title, logger);
                logForm.Show();
            });
            return logForm;
        }

        long updateTimestamp = -1;
        readonly VgcApis.Libs.Tasks.Routine logUpdater;
        readonly VgcApis.Libs.Sys.QueueLogger qLogger;

        bool isPaused = false;

        FormSingleServerLog(string title, VgcApis.Libs.Sys.QueueLogger logger)
        {
            this.qLogger = logger;
            logUpdater = new VgcApis.Libs.Tasks.Routine(
                RefreshUi,
                VgcApis.Models.Consts.Intervals.SiFormLogRefreshInterval
            );

            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            this.Text = I18N.Log + " - " + title;
            VgcApis.Misc.UI.AddTagToFormTitle(this);
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
