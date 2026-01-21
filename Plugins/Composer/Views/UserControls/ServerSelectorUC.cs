using System;
using System.Windows.Forms;
using Composer.Models;
using Composer.Resources.Langs;
using Composer.Views.WinForms;

namespace Composer.Views.UserControls
{
    public partial class ServerSelectorUC : UserControl, VgcApis.Interfaces.IHasIndex
    {
        private readonly FormMain formMain;
        private readonly ServerSelectorItem nodeFilterItem;

        public ServerSelectorUC(
            Views.WinForms.FormMain formMain,
            Models.ServerSelectorItem nodeFilterItem
        )
        {
            // size: 170, 26
            InitializeComponent();
            VgcApis.Misc.UI.SetTrasparentBackground(btnDelete, btnEdit);

            this.formMain = formMain;
            this.nodeFilterItem = nodeFilterItem;

            RefreshLabel();
        }

        #region private methods
        void RefreshLabel()
        {
            var item = this.nodeFilterItem;
            var mkC = item.servInfos.Count;
            var hasFilter = !string.IsNullOrEmpty(item.filter);
            var mkF1 = hasFilter ? "f" : "";

            this.lbNodeFilterTag.Text = $"{item.GetIndex()}.({mkF1}{mkC}) {item.tag}";

            var mkF2 = hasFilter ? $"\n{I18N.HasFilter}" : "";
            var tip = $"tag: {item.tag}\n{I18N.Nodes}: {mkC}{mkF2}";
            this.toolTip1.SetToolTip(this.lbNodeFilterTag, tip);
        }
        #endregion

        #region IHasIndex
        public double GetIndex() => nodeFilterItem.GetIndex();

        public void SetIndex(double value)
        {
            nodeFilterItem.SetIndex(value);
        }
        #endregion

        #region public methods
        public Models.ServerSelectorItem GetNodeFilterItem() => this.nodeFilterItem;
        #endregion

        #region UI event handlers
        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.formMain.DeleteNodeFilterItem(this.nodeFilterItem.id);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.formMain.ShowModifyNodeFilterForm(this.nodeFilterItem);
        }

        private void lbNodeFilterName_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void NodeFilter_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }
        #endregion
    }
}
