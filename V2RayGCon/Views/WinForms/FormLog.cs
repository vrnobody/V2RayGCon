using System;
using System.Windows.Forms;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormLog : Form
    {
        #region Sigleton
        static readonly VgcApis.BaseClasses.AuxSiWinForm<FormLog> auxSiForm =
            new VgcApis.BaseClasses.AuxSiWinForm<FormLog>();
        static public FormLog GetForm() => auxSiForm.GetForm();
        static public void ShowForm() => auxSiForm.ShowForm();
        #endregion

        Services.Settings setting;

        VgcApis.UserControls.RepaintController repaintCtrl;
        bool isPaused = false;
        long updateTimeStamp = DateTime.Now.Ticks;
        Timer updateLogTimer = new Timer { Interval = 500 };
        VgcApis.Libs.Tasks.Bar bar = new VgcApis.Libs.Tasks.Bar();

        public FormLog()
        {
            setting = Services.Settings.Instance;

            InitializeComponent();

            this.FormClosed += (s, e) =>
            {
                if (updateLogTimer != null)
                {
                    updateLogTimer.Stop();
                    updateLogTimer.Tick -= UpdateLog;
                    updateLogTimer.Dispose();
                }
            };

            Misc.UI.SetFormLocation<FormLog>(this, Models.Datas.Enums.FormLocations.BottomLeft);
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormLog_Load(object sender, EventArgs e)
        {
            repaintCtrl = new VgcApis.UserControls.RepaintController(rtBoxLogger);
            updateLogTimer.Tick += UpdateLog;
            updateLogTimer.Start();
            SetIsPause(false);

            // throw new NullReferenceException("for debugging");
        }

        #region private methods

        private void ScrollToBottom(VgcApis.UserControls.ExRichTextBox box)
        {
            box.SelectionStart = box.Text.Length;
            box.ScrollToCaret();
        }

        void UpdateLog(object sender, EventArgs args)
        {
            var logBox = rtBoxLogger;
            if (isPaused || logBox == null || logBox.IsDisposed)
            {
                return;
            }

            if (!bar.Install())
            {
                return;
            }

            var timestamp = setting.GetLogTimestamp();

            if (updateTimeStamp == timestamp)
            {
                bar.Remove();
                return;
            }

            try
            {
                repaintCtrl.DisableRepaintEvent();
                logBox.Text = setting.GetLogContent();
                ScrollToBottom(logBox);
                updateTimeStamp = timestamp;
                repaintCtrl.EnableRepaintEvent();
            }
            catch { }
            finally
            {
                bar.Remove();
            }
        }

        void SetIsPause(bool isPaused)
        {
            this.isPaused = isPaused;
            pauseToolStripMenuItem.Checked = isPaused;
        }
        #endregion

        #region UI events
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetIsPause(!isPaused);
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logs = setting.GetLogContent();
            var msg = VgcApis.Misc.Utils.CopyToClipboard(logs) ?
                Resources.Resx.I18N.CopySuccess :
                Resources.Resx.I18N.CopyFail;
            VgcApis.Misc.UI.MsgBoxAsync(msg);
        }

        #endregion


    }
}
