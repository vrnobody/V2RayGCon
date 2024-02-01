using System;
using System.Collections.Generic;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class ConfigTemplateUI : UserControl
    {
        CustomConfigTemplate tplS;

        public ConfigTemplateUI(CustomConfigTemplate tplSettings)
        {
            // this.size = 347, 28
            InitializeComponent();

            this.tplS = tplSettings;
            Reload();
        }

        #region public mehtod

        public CustomConfigTemplate GetTemplateSettings() => tplS;

        public void SetIndex(double index)
        {
            tplS.index = index;
            Reload();
        }
        #endregion

        #region private method
        void Reload()
        {
            chkIsInject.Checked = tplS.isInject;

            var title = $"{tplS.index}.{tplS.name}";
            lbTitle.Text = title;
            toolTip1.SetToolTip(lbTitle, title);
            var ty = VgcApis.Misc.Utils.DetectConfigType(tplS.template);
            rlbBinding.Text = ty.ToString();
            var tip = $"{I18N.Format}: {ty}";

            var tails = new List<string>();
            if (ty == VgcApis.Models.Datas.Enums.ConfigType.json)
            {
                tails.Add(tplS.GetJsonArrMergeOption());
            }
            if (tplS.isSocks5Inbound)
            {
                tails.Add("SOCKS5");
            }

            if (tails.Count > 0)
            {
                var t = string.Join(",", tails);
                tip = $"{tip} ({t})";
            }

            toolTip1.SetToolTip(rlbBinding, tip);
        }

        #endregion

        #region UI event handler
        private void CoreSettingUI_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void lbTitle_Click(object sender, EventArgs e)
        {
            chkIsInject.Checked = !chkIsInject.Checked;
        }

        private void rlbBinding_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void chkIsInject_CheckedChanged(object sender, EventArgs e)
        {
            tplS.isInject = chkIsInject.Checked;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var form = new WinForms.FormCustomConfigTemplates(tplS);
            form.FormClosed += (s, a) =>
            {
                if (form.DialogResult == DialogResult.OK)
                {
                    this.tplS = form.inbS;
                    Reload();
                }
            };
            form.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var msg = string.Format(I18N.ConfirmDeleteTpl, tplS.name);
            if (VgcApis.Misc.UI.Confirm(msg))
            {
                Parent.Controls.Remove(this);
            }
        }
        #endregion
    }
}
