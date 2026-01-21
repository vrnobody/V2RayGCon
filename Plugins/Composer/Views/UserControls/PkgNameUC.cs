using System;
using System.Windows.Forms;
using Composer.Models;
using Composer.Resources.Langs;
using Composer.Views.WinForms;

namespace Composer.Views.UserControls
{
    public partial class PkgNameUC : UserControl, VgcApis.Interfaces.IHasIndex
    {
        private readonly FormMain formMain;
        private readonly PackageItem pkgItem;

        public PkgNameUC(Views.WinForms.FormMain formMain, Models.PackageItem pkgItem)
        {
            // container: 170, 26
            // btnDelete: 148, 1
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
            var tag = $"{pkgItem.GetIndex()}.{pkgItem.name}";
            this.lbPkgName.Text = tag;
            this.toolTip1.SetToolTip(lbPkgName, tag);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var name = this.pkgItem.name;
            if (VgcApis.Misc.UI.Confirm($"{I18N.ConfirmDelPkg}{name}"))
            {
                this.formMain.DeletePackageItem(name);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.formMain.LoadPackageItemToEditor(this.pkgItem);
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
