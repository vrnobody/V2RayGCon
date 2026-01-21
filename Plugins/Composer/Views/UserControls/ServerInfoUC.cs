using System;
using System.Windows.Forms;
using Composer.Views.WinForms;

namespace Composer.Views.UserControls
{
    public partial class ServerInfoUC : UserControl, VgcApis.Interfaces.IHasIndex
    {
        private readonly FormServerSelector formEditor;
        private readonly Models.ServerInfoItem servInfo;

        public ServerInfoUC(
            Views.WinForms.FormServerSelector formEditor,
            Models.ServerInfoItem servItem
        )
        {
            // size: 320, 26
            InitializeComponent();
            VgcApis.Misc.UI.SetTrasparentBackground(btnDelete);

            this.formEditor = formEditor;
            this.servInfo = servItem;
            RefreshTitle();
        }

        #region public methods
        public Models.ServerInfoItem GetServInfoItem() => this.servInfo;
        #endregion

        #region IHasIndex
        public double GetIndex() => servInfo.GetIndex();

        public void SetIndex(double value) => servInfo.SetIndex(value);
        #endregion

        #region private methods
        void RefreshTitle()
        {
            var title = $"{this.servInfo.title}";
            this.lbTitle.Text = title;
            this.toolTip1.SetToolTip(this.lbTitle, title);
        }
        #endregion

        #region UI event handlers

        private void btnDelete_Click(object sender, EventArgs e)
        {
            formEditor.DeleteServer(servInfo.uid);
        }

        private void lbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void ServerTitle_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }
        #endregion
    }
}
