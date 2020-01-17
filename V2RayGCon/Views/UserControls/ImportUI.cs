using System;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class ImportUI : UserControl
    {
        Action OnDeleted;

        public ImportUI(Model.Data.ImportItem subItem, Action OnDeleted)
        {
            InitializeComponent();

            lbIndex.Text = "";
            tboxUrl.Text = subItem.url;
            tboxAlias.Text = subItem.alias;
            chkMergeWhenStart.Checked = subItem.isUseOnActivate;
            chkMergeWhenSpeedTest.Checked = subItem.isUseOnSpeedTest;
            chkMergeWhenPacking.Checked = subItem.isUseOnPackage;

            this.OnDeleted = OnDeleted;
        }

        public Model.Data.ImportItem GetValue()
        {
            return new Model.Data.ImportItem
            {
                isUseOnActivate = chkMergeWhenStart.Checked,
                isUseOnSpeedTest = chkMergeWhenSpeedTest.Checked,
                isUseOnPackage = chkMergeWhenPacking.Checked,
                alias = tboxAlias.Text,
                url = tboxUrl.Text,
            };
        }

        #region public method
        public void SetIndex(int index)
        {
            lbIndex.Text = index.ToString();
        }
        #endregion

        #region UI event
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!Lib.UI.Confirm(I18N.ConfirmDeleteControl))
            {
                return;
            }

            var flyPanel = this.Parent as FlowLayoutPanel;
            flyPanel.Controls.Remove(this);

            this.OnDeleted?.Invoke();
        }
        #endregion

        #region private method
        private void UrlListItem_MouseDown(object sender, MouseEventArgs e)
        {
            // Cursor.Current = Lib.UI.CreateCursorIconFromUserControl(this);
            DoDragDrop((ImportUI)sender, DragDropEffects.Move);
        }
        #endregion
    }
}
