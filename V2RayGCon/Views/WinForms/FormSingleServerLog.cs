﻿using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormSingleServerLog : Form
    {
        public static FormSingleServerLog CreateLogForm(
            string title,
            Controllers.CoreServerComponent.Logger logger)
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
        VgcApis.Libs.Tasks.Routine logUpdater;
        VgcApis.Libs.Sys.QueueLogger qLogger = new VgcApis.Libs.Sys.QueueLogger();
        Controllers.CoreServerComponent.Logger coreLogger;

        bool isPaused = false;

        FormSingleServerLog(
            string title,
            Controllers.CoreServerComponent.Logger logger)
        {
            coreLogger = logger;

            coreLogger.OnLog += OnLogHandler;

            logUpdater = new VgcApis.Libs.Tasks.Routine(
                RefreshUi,
                VgcApis.Models.Consts.Intervals.SiFormLogRefreshInterval);

            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            this.Text = I18N.Log + " - " + title;
        }

        void OnLogHandler(object sender, string msg)
        {
            qLogger.Log(msg);
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
            coreLogger.OnLog -= OnLogHandler;
            qLogger?.Dispose();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmClearLog))
            {
                qLogger.Reset();
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
    }
}
