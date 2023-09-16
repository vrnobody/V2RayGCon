using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services;

namespace V2RayGCon.Controllers.OptionComponent
{
    public class TabCustomCoreSettings : OptionComponentController
    {
        private readonly Servers servers;
        private readonly Settings settings;
        private readonly FlowLayoutPanel flyPanel;
        private readonly ToolTip tooltip;
        private readonly Button btnAdd;
        private readonly ComboBox cboxDefCore;
        private readonly Button btnChangeAll;

        public TabCustomCoreSettings(
            FlowLayoutPanel flyPanel,
            ToolTip tooltip,
            Button btnAdd,
            ComboBox cboxDefCore,
            Button btnChangeAll
        )
        {
            this.servers = Servers.Instance;
            this.settings = Services.Settings.Instance;
            this.flyPanel = flyPanel;
            this.tooltip = tooltip;
            this.btnAdd = btnAdd;
            this.cboxDefCore = cboxDefCore;
            this.btnChangeAll = btnChangeAll;

            BindButtonAdd();
            BindDefCoreControls();
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
        string GetCurDefCoreName()
        {
            if (cboxDefCore.SelectedIndex < 1)
            {
                return string.Empty;
            }
            return cboxDefCore.Text;
        }

        void UpdateCboxDefCoreTooltip(string name)
        {
            tooltip.SetToolTip(cboxDefCore, string.IsNullOrEmpty(name) ? I18N.Default : name);
        }

        void BindDefCoreControls()
        {
            RefreshCboxDefCore();
            var defCoreName = settings.DefaultCoreName;
            SelectByText(cboxDefCore, defCoreName);
            UpdateCboxDefCoreTooltip(defCoreName);

            cboxDefCore.DropDown += (s, a) => RefreshCboxDefCore();
            cboxDefCore.SelectedValueChanged += (s, a) =>
            {
                var name = GetCurDefCoreName();
                settings.DefaultCoreName = name;
                UpdateCboxDefCoreTooltip(name);
            };

            btnChangeAll.Click += (s, a) =>
            {
                var name = GetCurDefCoreName();
                var msg = string.Format(
                    I18N.ConfirmChangeAllDefCoreTo,
                    string.IsNullOrEmpty(name) ? I18N.Default : name
                );
                if (!VgcApis.Misc.UI.Confirm(msg))
                {
                    return;
                }
                VgcApis.Misc.Utils.RunInBgSlim(() =>
                {
                    var servs = servers.GetAllServersOrderByIndex();
                    foreach (var coreServ in servs)
                    {
                        coreServ.GetCoreCtrl().SetCustomCoreName(name);
                    }
                });
            };
        }

        void RefreshCboxDefCore()
        {
            var names = settings.GetCustomCoresSetting().Select(cs => cs.name).ToArray();
            var items = cboxDefCore.Items;
            items.Clear();
            items.Add(I18N.Default);
            items.AddRange(names);
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxDefCore);
        }

        void SelectByText(ComboBox cbox, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                cbox.SelectedIndex = 0;
                return;
            }

            var items = cboxDefCore.Items;
            for (int i = 1; i < items.Count; i++)
            {
                if (items[i].ToString() == text)
                {
                    cbox.SelectedIndex = i;
                    return;
                }
            }
        }

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
            settings.ResetCustomCoresIndex(); // this will invoke menu update event

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
