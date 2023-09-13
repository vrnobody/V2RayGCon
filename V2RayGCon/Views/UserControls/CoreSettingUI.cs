using System;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class CoreSettingUI : UserControl
    {
        public event EventHandler OnRequireReload;

        private CustomCoreSettings coreSettings;

        public CoreSettingUI()
        {
            InitializeComponent();
        }

        #region public mehtod
        public void Reload(CustomCoreSettings coreSettings)
        {
            this.coreSettings = coreSettings;

            UpdateTitle();
        }

        public void SetIndex(double index)
        {
            var ctrl = coreSettings;
            if (ctrl == null)
            {
                return;
            }
            ctrl.index = index;
        }

        public double GetIndex() => coreSettings?.index ?? -1;
        #endregion

        #region private method
        void UpdateTitle()
        {
            var title = $"{coreSettings.index}.[{coreSettings.name}]";
            if (!string.IsNullOrEmpty(coreSettings.protocols))
            {
                title += $" ({coreSettings.protocols})";
            }
            lbTitle.Text = title;

            var tag = coreSettings.isBindToShareLinkProtocol ? "S" : "";
            tag += coreSettings.isBindToConfigProtocol ? "C" : "";
            rlbBinding.Text = tag;
            rlbBinding.Visible = !string.IsNullOrEmpty(tag);
            rlbBinding.Left = lbTitle.Right + lbTitle.Left;
        }

        private void InvokeOnRequireReloadIgnoreError()
        {
            try
            {
                OnRequireReload?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }
        #endregion

        #region UI event handler
        private void CoreSettingUI_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void lbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void rlbBinding_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var form = new WinForms.FormCustomCoreSettings(coreSettings);
            form.FormClosed += (s, a) =>
            {
                if (form.DialogResult == DialogResult.OK)
                {
                    var coreSet = form.coreSettings;
                    Services.Settings.Instance.AddOrReplaceCustomCoreSettings(coreSet);
                    InvokeOnRequireReloadIgnoreError();
                }
            };
            form.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var msg = string.Format(I18N.ConfirmDeleteCoreSetting, coreSettings.name);
            if (VgcApis.Misc.UI.Confirm(msg))
            {
                var ok = Services.Settings.Instance.RemoveCoreSettingsByName(coreSettings.name);
                if (ok)
                {
                    InvokeOnRequireReloadIgnoreError();
                }
            }
        }
        #endregion
    }
}
