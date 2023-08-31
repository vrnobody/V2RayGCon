using Luna.Resources.Langs;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Luna.Views.UserControls
{
    internal partial class LuaUI : UserControl
    {
        Controllers.LuaCoreCtrl luaCoreCtrl;
        VgcApis.Libs.Tasks.LazyGuy lazyUpdater;

        public LuaUI()
        {

            InitializeComponent();
        }

        private void LuaUI_Load(object sender, EventArgs e)
        {
            lazyUpdater = new VgcApis.Libs.Tasks.LazyGuy(UpdateUiWorker, 300, 2000)
            {
                Name = "Luna.UiPanelUpdater",
            };
        }

        #region public methods
        Services.LuaServer luaServerSvc = null;
        Services.FormMgrSvc formMgrSvc = null;
        public void Reload(
            Services.LuaServer luaServerSvc,
            Services.FormMgrSvc formMgrSvc,

            Controllers.LuaCoreCtrl luaCoreCtrl)
        {
            this.luaServerSvc = luaServerSvc;
            this.formMgrSvc = formMgrSvc;

            var org = this.luaCoreCtrl;
            this.luaCoreCtrl = luaCoreCtrl;
            ReleaseEvent(org);
            BindEvent(luaCoreCtrl);
            UpdateUiLater();
        }

        public void SetIndex(double index)
        {
            var ctrl = luaCoreCtrl;
            if (ctrl == null)
            {
                return;
            }
            ctrl.index = index;
        }

        public double GetIndex() => luaCoreCtrl?.index ?? -1;

        public void Cleanup()
        {
            ReleaseEvent(this.luaCoreCtrl);
            lazyUpdater?.Dispose();
            this.formMgrSvc = null;
            this.luaServerSvc = null;
        }
        #endregion

        #region private methods
        void BindEvent(Controllers.LuaCoreCtrl ctrl)
        {
            if (ctrl == null)
            {
                return;
            }
            ctrl.OnStateChange += OnLuaCoreStateChangeHandler;
        }

        void ReleaseEvent(Controllers.LuaCoreCtrl ctrl)
        {
            if (ctrl == null)
            {
                return;
            }
            ctrl.OnStateChange -= OnLuaCoreStateChangeHandler;
        }

        void OnLuaCoreStateChangeHandler(object sender, EventArgs args)
        {
            UpdateUiLater();
        }

        void UpdateUiWorker()
        {
            VgcApis.Misc.UI.Invoke(() =>
           {
               var ctrl = this.luaCoreCtrl;

               if (this.Parent == null || ctrl == null)
               {
                   return;
               }

               UpdateNameLabel(ctrl);
               UpdateOptionsLabel(ctrl);
               UpdateRunningState(ctrl);
           });
        }

        void UpdateUiLater() => lazyUpdater?.Deadline();

        void UpdateNameLabel(Controllers.LuaCoreCtrl ctrl)
        {
            var n = ctrl.name;
            if (lbName.Text != n)
            {
                lbName.Text = n;
                toolTip1.SetToolTip(lbName, n);
            }
        }

        void UpdateOptionsLabel(Controllers.LuaCoreCtrl ctrl)
        {
            var a = ctrl.isAutoRun ? "A" : @"";
            var h = ctrl.isHidden ? "H" : "";
            var c = ctrl.isLoadClr ? "C" : "";
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

        void UpdateRunningState(Controllers.LuaCoreCtrl ctrl)
        {
            var isRunning = ctrl.isRunning;
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

        private void btnStop_Click(object sender, EventArgs e)
        {
            luaCoreCtrl?.Stop();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            luaCoreCtrl?.Start();
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

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            luaCoreCtrl?.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            luaCoreCtrl?.Stop();
        }

        private void terminateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            luaCoreCtrl?.Abort();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ctrl = luaCoreCtrl;
            if (ctrl == null)
            {
                VgcApis.Misc.UI.MsgBox(I18N.ScriptNotFound);
                return;
            }
            var cs = ctrl.GetCoreSettings();
            formMgrSvc?.CreateNewEditor(cs);
        }

        private void optionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WinForms.FormLuaCoreSettings.ShowForm(luaCoreCtrl);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var scriptName = luaCoreCtrl.name;
            if (string.IsNullOrEmpty(scriptName)
                || !VgcApis.Misc.UI.Confirm(I18N.ConfirmRemoveScript))
            {
                return;
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                if (luaServerSvc?.RemoveScriptByName(scriptName) != true)
                {
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.ScriptNotFound);
                }
            });
        }

        private void btnMenuMore_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            Point pos = new Point(control.Left, control.Top + control.Height);
            contextMenuStripMore.Show(this, pos);
        }
        #endregion
    }
}
