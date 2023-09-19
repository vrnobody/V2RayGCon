using System;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class InboundSettingUI : UserControl
    {
        public event EventHandler OnRequireReload;

        private CustomInboundSettings inbSettings;

        public InboundSettingUI()
        {
            // this.size = 347, 27
            InitializeComponent();
        }

        #region public mehtod
        public void Reload(CustomInboundSettings inbSettings)
        {
            this.inbSettings = inbSettings;
            UpdateTitle();
        }

        public void SetIndex(double index)
        {
            var ctrl = inbSettings;
            if (ctrl == null)
            {
                return;
            }
            ctrl.index = index;
        }

        public double GetIndex() => inbSettings?.index ?? -1;
        #endregion

        #region private method
        void UpdateTitle()
        {
            var title = $"{inbSettings.index}.{inbSettings.name}";
            lbTitle.Text = title;
            toolTip1.SetToolTip(lbTitle, title);
            rlbBinding.Text = inbSettings.format;
            toolTip1.SetToolTip(rlbBinding, $"{I18N.Format}: {inbSettings.format}");
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
            var form = new WinForms.FormCustomInboundSettings(inbSettings);
            form.FormClosed += (s, a) =>
            {
                if (form.DialogResult == DialogResult.OK)
                {
                    var inbSet = form.inbSettings;
                    Services.Settings.Instance.AddOrReplaceCustomInboundSettings(inbSet);
                    InvokeOnRequireReloadIgnoreError();
                }
            };
            form.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var msg = string.Format(I18N.ConfirmDeleteTpl, inbSettings.name);
            if (VgcApis.Misc.UI.Confirm(msg))
            {
                var ok = Services.Settings.Instance.RemoveCustomInboundByName(inbSettings.name);
                if (ok)
                {
                    InvokeOnRequireReloadIgnoreError();
                }
            }
        }
        #endregion
    }
}
