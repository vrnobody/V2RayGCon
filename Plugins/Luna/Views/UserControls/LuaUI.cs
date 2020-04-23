using Luna.Resources.Langs;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Luna.Views.UserControls
{
    public partial class LuaUI : UserControl
    {
        Controllers.LuaCoreCtrl luaCoreCtrl;
        Services.LuaServer luaServer;

        VgcApis.Libs.Tasks.LazyGuy lazyUpdater;

        public LuaUI(
            Services.LuaServer luaServer,
            Controllers.LuaCoreCtrl luaCoreCtrl)
        {
            this.luaCoreCtrl = luaCoreCtrl;
            this.luaServer = luaServer;
            InitializeComponent();
        }

        private void LuaUI_Load(object sender, EventArgs e)
        {
            luaCoreCtrl.OnStateChange += OnLuaCoreStateChangeHandler;
            lazyUpdater = new VgcApis.Libs.Tasks.LazyGuy(UpdateUiWorker, 300)
            {
                Name = "Luna.UiPanelUpdater",
            };

            UpdateUiLater();
        }

        #region public methods
        public void SetIndex(double index)
        {
            luaCoreCtrl.index = index;
        }

        public double GetIndex() => luaCoreCtrl.index;

        public void Cleanup()
        {
            luaCoreCtrl.OnStateChange -= OnLuaCoreStateChangeHandler;
            lazyUpdater?.Dispose();
        }

        #endregion

        #region private methods
        void OnLuaCoreStateChangeHandler(object sender, EventArgs args)
        {
            lazyUpdater.Deadline();
        }

        void UpdateUiWorker()
        {
            VgcApis.Misc.UI.RunInUiThreadIgnoreError(lbName, () =>
            {
                UpdateNameLabel();
                UpdateOptionsLabel();
                UpdateRunningState();
            });
        }

        void UpdateUiLater() => lazyUpdater?.Deadline();

        void UpdateNameLabel()
        {
            var n = luaCoreCtrl.name;
            if (lbName.Text != n)
            {
                lbName.Text = n;
                toolTip1.SetToolTip(lbName, n);
            }
        }

        void UpdateOptionsLabel()
        {
            var a = luaCoreCtrl.isAutoRun ? "A" : @"";
            var h = luaCoreCtrl.isHidden ? "H" : "";
            var c = luaCoreCtrl.isLoadClr ? "C" : "";
            var text = $"{a}{c}{h}";

            if (string.IsNullOrEmpty(text))
            {
                text = I18N.LuaCtrlOptionNone;
            }

            if (rlbOptions.Text != text)
            {
                rlbOptions.Text = text;
            }
        }

        void UpdateRunningState()
        {
            var isRunning = luaCoreCtrl.isRunning;
            var text = isRunning ? "ON" : "OFF";
            var color = isRunning ? Color.DarkOrange : Color.Green;

            if (lbRunningState.Text != text)
            {
                lbRunningState.Text = text;
            }
            if (lbRunningState.ForeColor != color)
            {
                lbRunningState.ForeColor = color;
            }
        }
        #endregion

        #region UI event handlers
        private void btnKill_Click(object sender, EventArgs e)
        {
            luaCoreCtrl.Kill();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            luaCoreCtrl.Stop();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            luaCoreCtrl.Start();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var scriptName = luaCoreCtrl.name;
            if (string.IsNullOrEmpty(scriptName)
                || !VgcApis.Misc.UI.Confirm(I18N.ConfirmRemoveScript))
            {
                return;
            }

            Task.Run(() =>
            {
                if (!luaServer.RemoveScriptByName(scriptName))
                {
                    VgcApis.Misc.UI.MsgBoxAsync("", I18N.ScriptNotFound);
                }
            }).ConfigureAwait(false);
        }

        private void LuaUI_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void lbName_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void lbRunningState_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void rlbOptions_Click(object sender, EventArgs e)
        {
            Views.WinForms.FormLuaCoreSettings.ShowForm(luaCoreCtrl);
        }

        #endregion


    }
}
