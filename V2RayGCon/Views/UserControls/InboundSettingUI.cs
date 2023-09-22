using System;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class InboundSettingUI : UserControl
    {
        public event EventHandler OnRequireReload;

        private CustomInboundSettings inbS;

        public InboundSettingUI()
        {
            // this.size = 346, 27
            InitializeComponent();
        }

        #region public mehtod
        public void Reload(CustomInboundSettings inbSettings)
        {
            this.inbS = inbSettings;
            UpdateTitle();
        }

        public void SetIndex(double index)
        {
            var ctrl = inbS;
            if (ctrl == null)
            {
                return;
            }
            ctrl.index = index;
        }

        public double GetIndex() => inbS?.index ?? -1;
        #endregion

        #region private method
        void UpdateTitle()
        {
            var title = $"{inbS.index}.{inbS.name}";
            lbTitle.Text = title;
            toolTip1.SetToolTip(lbTitle, title);
            var ty = VgcApis.Misc.Utils.DetectConfigType(inbS.template).ToString();
            rlbBinding.Text = ty;
            toolTip1.SetToolTip(rlbBinding, $"{I18N.Format}: {ty}");
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
            var form = new WinForms.FormCustomInboundSettings(inbS);
            form.FormClosed += (s, a) =>
            {
                if (form.DialogResult == DialogResult.OK)
                {
                    var inbSet = form.inbS;
                    Services.Settings.Instance.AddOrReplaceCustomInboundSettings(inbSet);
                    InvokeOnRequireReloadIgnoreError();
                }
            };
            form.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var msg = string.Format(I18N.ConfirmDeleteTpl, inbS.name);
            if (VgcApis.Misc.UI.Confirm(msg))
            {
                var ok = Services.Settings.Instance.RemoveCustomInboundByName(inbS.name);
                if (ok)
                {
                    InvokeOnRequireReloadIgnoreError();
                }
            }
        }
        #endregion
    }
}
