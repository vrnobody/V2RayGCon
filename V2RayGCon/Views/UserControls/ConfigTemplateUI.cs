using System;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class ConfigTemplateUI : UserControl
    {
        public event EventHandler OnRequireReload;

        private CustomConfigTemplate tplS;

        public ConfigTemplateUI()
        {
            // this.size = 346, 27
            InitializeComponent();
        }

        #region public mehtod
        public void Reload(CustomConfigTemplate tplSettings)
        {
            this.tplS = tplSettings;
            UpdateTitle();
        }

        public void SetIndex(double index)
        {
            var ctrl = tplS;
            if (ctrl == null)
            {
                return;
            }
            ctrl.index = index;
        }

        public double GetIndex() => tplS?.index ?? -1;
        #endregion

        #region private method
        void UpdateTitle()
        {
            var title = $"{tplS.index}.{tplS.name}";
            lbTitle.Text = title;
            toolTip1.SetToolTip(lbTitle, title);
            var ty = VgcApis.Misc.Utils.DetectConfigType(tplS.template);
            rlbBinding.Text = ty.ToString();
            var tip = $"{I18N.Format}: {ty}";
            if (ty == VgcApis.Models.Datas.Enums.ConfigType.json)
            {
                tip += $" ({tplS.GetJsonArrMergeOption()})";
            }
            toolTip1.SetToolTip(rlbBinding, tip);
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
            var form = new WinForms.FormCustomConfigTemplates(tplS);
            form.FormClosed += (s, a) =>
            {
                if (form.DialogResult == DialogResult.OK)
                {
                    var inbSet = form.inbS;
                    Services.Settings.Instance.AddOrReplaceCustomConfigTemplateSettings(inbSet);
                    InvokeOnRequireReloadIgnoreError();
                }
            };
            form.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var msg = string.Format(I18N.ConfirmDeleteTpl, tplS.name);
            if (VgcApis.Misc.UI.Confirm(msg))
            {
                var ok = Services.Settings.Instance.RemoveCustomConfigTemplateByName(tplS.name);
                if (ok)
                {
                    InvokeOnRequireReloadIgnoreError();
                }
            }
        }
        #endregion
    }
}
