using System;
using System.Windows.Forms;
using ProxySetter.Resources.Langs;

namespace ProxySetter.Controllers.VGCPluginComponents
{
    class TabStatus : ComponentCtrl
    {
        Label lbPacUrl,
            lbPacServerStatus;
        readonly Services.PacServer pacServer;

        public TabStatus(
            Services.PacServer pacServer,
            Label lbPacServerStatus,
            Label lbPacUrl,
            Button btnRestart,
            Button btnStop,
            Button btnViewInNotepad,
            Button btnDebug,
            Button btnCopy
        )
        {
            this.pacServer = pacServer;

            BindControls(lbPacServerStatus, lbPacUrl);
            BindEvents(pacServer, btnRestart, btnStop, btnViewInNotepad, btnDebug, btnCopy);

            OnPacServerStateChangedHandler(null, EventArgs.Empty);

            pacServer.OnPACServerStateChanged += OnPacServerStateChangedHandler;
        }

        private void BindEvents(
            Services.PacServer pacServer,
            Button btnRestart,
            Button btnStop,
            Button btnViewInNotepad,
            Button btnDebug,
            Button btnCopy
        )
        {
            btnViewInNotepad.Click += (s, a) =>
            {
                try
                {
                    var content = pacServer.GetCurPacFileContent();
                    VgcApis.Libs.Sys.NotepadHelper.ShowMessage(content, @"PAC.js");
                }
                catch
                {
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.LaunchNotepadFail);
                }
            };

            btnRestart.Click += (s, a) => pacServer.StartPacServer();

            btnStop.Click += (s, a) => pacServer.StopPacServer();

            btnDebug.Click += (s, a) =>
                VgcApis.Misc.UI.VisitUrl(I18N.VisitPacDebugger, GetDebugUrl());

            btnCopy.Click += (s, a) =>
            {
                VgcApis.Misc.UI.MsgBox(
                    VgcApis.Misc.Utils.CopyToClipboard(this.lbPacUrl.Text)
                        ? I18N.CopySuccess
                        : I18N.CopyFail
                );
            };
        }

        #region private methods
        string GetDebugUrl()
        {
            return string.Format("{0}?&debug=true", pacServer.GetPacUrl());
        }

        void OnPacServerStateChangedHandler(object sender, EventArgs args)
        {
            var text = pacServer.isRunning ? I18N.PacServerIsOn : I18N.PacServerIsOff;
            VgcApis.Misc.UI.Invoke(() => lbPacServerStatus.Text = text);
        }

        private void BindControls(Label lbPacServerStatus, Label lbPacUrl)
        {
            this.lbPacServerStatus = lbPacServerStatus;
            this.lbPacUrl = lbPacUrl;
        }
        #endregion

        #region public method
        public override bool IsOptionsChanged()
        {
            return false;
        }

        public override bool SaveOptions()
        {
            return false;
        }

        public override void Cleanup()
        {
            pacServer.OnPACServerStateChanged -= OnPacServerStateChangedHandler;
        }
        #endregion
    }
}
