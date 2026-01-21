using System;
using System.Windows.Forms;
using Commander.Models;
using Commander.Models.Data;
using Commander.Resources.Langs;

namespace Commander.Views
{
    public partial class CmdParamsUC : UserControl, VgcApis.Interfaces.IHasIndex
    {
        private readonly FormMain formMain;
        private readonly CmderParam pkgItem;

        public CmdParamsUC(FormMain formMain, CmderParam pkgItem)
        {
            // size: 170, 26
            InitializeComponent();
            VgcApis.Misc.UI.SetTrasparentBackground(btnDelete, btnEdit);

            this.formMain = formMain;
            this.pkgItem = pkgItem;
            RefreshLabel();
        }

        #region IHasIndex
        public double GetIndex() => pkgItem.GetIndex();

        public void SetIndex(double value) => pkgItem.SetIndex(value);

        #endregion

        #region private methods
        void RefreshLabel()
        {
            var tag = $"{pkgItem.name}";
            this.lbPkgName.Text = tag;
            this.toolTip1.SetToolTip(lbPkgName, tag);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var name = this.pkgItem.name;
            if (VgcApis.Misc.UI.Confirm($"{I18N.ConfirmDelPkg}{name}"))
            {
                this.formMain.DeleteCmd(name);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.formMain.EditCmd(this.pkgItem.name);
        }

        private void lbPkgName_MouseDown(object sender, MouseEventArgs e)
        {
            this.DoDragDrop(this, DragDropEffects.Move);
        }

        private void PkgName_MouseDown(object sender, MouseEventArgs e)
        {
            this.DoDragDrop(this, DragDropEffects.Move);
        }
        #endregion
    }
}
