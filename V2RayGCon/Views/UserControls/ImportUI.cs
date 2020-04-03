using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class ImportUI : UserControl
    {
        Action OnDeleted;

        public ImportUI(Models.Datas.ImportItem subItem, Action OnDeleted)
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

        public Models.Datas.ImportItem GetValue()
        {
            return new Models.Datas.ImportItem
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
        private void btnBrowseLocalFile_Click(object sender, EventArgs e)
        {
            var path = VgcApis.Misc.UI.ShowSelectFileDialog(VgcApis.Models.Consts.Files.JsonExt);

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            try
            {
                tboxUrl.Text = path;
                if (string.IsNullOrEmpty(tboxAlias.Text))
                {
                    tboxAlias.Text = System.IO.Path.GetFileNameWithoutExtension(path);
                }
            }
            catch { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!Misc.UI.Confirm(I18N.ConfirmDeleteControl))
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
            // Cursor.Current = Misc.UI.CreateCursorIconFromUserControl(this);
            DoDragDrop((ImportUI)sender, DragDropEffects.Move);
        }
        #endregion


    }
}
