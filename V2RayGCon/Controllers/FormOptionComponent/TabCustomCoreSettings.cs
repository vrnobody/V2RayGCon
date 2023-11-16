using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Services;

namespace V2RayGCon.Controllers.OptionComponent
{
    public class TabCustomCoreSettings : OptionComponentController
    {
        private readonly Settings settings;
        private readonly FlowLayoutPanel flyPanel;
        private readonly Button btnAdd;

        public TabCustomCoreSettings(FlowLayoutPanel flyPanel, Button btnAdd)
        {
            this.settings = Settings.Instance;
            this.flyPanel = flyPanel;
            this.btnAdd = btnAdd;
            BindButtonAdd();
            BindFlyPanelDragDropEvent();
            Refresh();
        }

        #region properties

        #endregion

        #region public methods
        public override void Cleanup()
        {
            ReleaseEventHandler();
        }

        public override bool IsOptionsChanged()
        {
            return false;
        }

        public override bool SaveOptions()
        {
            return true;
        }
        #endregion

        #region private methods

        void BindFlyPanelDragDropEvent()
        {
            flyPanel.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };

            flyPanel.DragDrop += (s, a) =>
            {
                var data = a.Data;
                if (data.GetDataPresent(typeof(Views.UserControls.CoreSettingUI)))
                {
                    SwapFlyControls(s, a);
                }
            };
        }

        private void SwapFlyControls(object sender, DragEventArgs args)
        {
            // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag
            var panel = sender as FlowLayoutPanel;
            Point p = panel.PointToClient(new Point(args.X, args.Y));
            if (
                !(
                    args.Data.GetData(typeof(Views.UserControls.CoreSettingUI))
                    is Views.UserControls.CoreSettingUI curItem
                )
                || !(panel.GetChildAtPoint(p) is Views.UserControls.CoreSettingUI destItem)
                || curItem == destItem
            )
            {
                return;
            }

            // swap index
            var destIdx = destItem.GetIndex() + 0.5;
            var curIdx = (curItem.GetIndex() > destIdx) ? destIdx - 0.1 : destIdx + 0.1;
            destItem.SetIndex(destIdx);
            curItem.SetIndex(curIdx);
            settings.ResetCustomCoresIndex();

            // !must! reset item index!!
            var destPos = panel.Controls.GetChildIndex(destItem, false);
            panel.Controls.SetChildIndex(curItem, destPos);
            panel.Invalidate();

            Refresh();
        }

        void BindButtonAdd()
        {
            btnAdd.Click += (s, a) =>
            {
                var form = new Views.WinForms.FormCustomCoreSettings();
                form.FormClosed += (_, __) =>
                {
                    if (form.DialogResult == DialogResult.OK)
                    {
                        var cs = form.coreSettings;
                        settings.AddOrReplaceCustomCoreSettings(cs);
                        Refresh();
                    }
                };
                form.Show();
            };
        }

        void OnRequireReloadHandler(object sender, EventArgs args)
        {
            Refresh();
        }

        void ReleaseEventHandler()
        {
            var coreUis = flyPanel.Controls.OfType<Views.UserControls.CoreSettingUI>();
            foreach (var cui in coreUis)
            {
                cui.OnRequireReload -= OnRequireReloadHandler;
            }
        }

        void Refresh()
        {
            VgcApis.Misc.UI.Invoke(RefreshPanelCore);
        }

        void KeepNthControls(int num)
        {
            var ctrls = flyPanel.Controls;
            while (ctrls.Count > num)
            {
                ctrls.RemoveAt(ctrls.Count - 1);
            }

            while (ctrls.Count < num)
            {
                ctrls.Add(new Views.UserControls.CoreSettingUI());
            }
        }

        void RefreshPanelCore()
        {
            ReleaseEventHandler();
            var cs = settings.GetCustomCoresSetting();
            flyPanel.SuspendLayout();
            KeepNthControls(cs.Count);
            for (var i = 0; i < cs.Count; i++)
            {
                var ui = flyPanel.Controls[i] as Views.UserControls.CoreSettingUI;
                ui.Reload(cs[i]);
                ui.OnRequireReload += OnRequireReloadHandler;
            }
            flyPanel.ResumeLayout();
        }
        #endregion

        #region protected methods

        #endregion
    }
}
