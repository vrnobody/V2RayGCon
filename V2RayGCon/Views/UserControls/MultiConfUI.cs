using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class MultiConfUI : UserControl
    {
        readonly Action OnDeleted;

        public MultiConfUI(Models.Datas.MultiConfItem multiConfItem, Action OnDeleted)
        {
            InitializeComponent();

            lbIndex.Text = "";
            tboxUrl.Text = multiConfItem.path;
            tboxAlias.Text = multiConfItem.alias;

            this.OnDeleted = OnDeleted;
        }

        public Models.Datas.MultiConfItem GetValue()
        {
            return new Models.Datas.MultiConfItem { alias = tboxAlias.Text, path = tboxUrl.Text, };
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
            if (!Misc.UI.Confirm(I18N.ConfirmDeleteControl))
            {
                return;
            }

            var flyPanel = this.Parent as FlowLayoutPanel;
            flyPanel.Controls.Remove(this);

            this.OnDeleted?.Invoke();
        }

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
        #endregion

        #region private method
        private void UrlListItem_MouseDown(object sender, MouseEventArgs e)
        {
            // Cursor.Current = Misc.UI.CreateCursorIconFromUserControl(this);
            DoDragDrop((MultiConfUI)sender, DragDropEffects.Move);
        }
        #endregion
    }
}
