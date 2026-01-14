using System;
using System.Windows.Forms;

namespace Pacman.Views.UserControls
{
    public partial class BeanUI : UserControl
    {
        Models.Data.Bean bean;

        public BeanUI()
        {
            InitializeComponent();
        }

        private void BeanUI_Load(object sender, EventArgs e) { }

        #region private methods
        void UpdateLabels()
        {
            VgcApis.Misc.UI.Invoke(() =>
            {
                var p = this.Parent;
                if (p == null || p.IsDisposed)
                {
                    return;
                }

                lbTitle.Text = bean.title;
                lbStatus.Text = bean.status;
                chkTitle.Checked = bean.isSelected;
            });
        }

        #endregion

        #region properties
        public bool isSelected
        {
            get => chkTitle.Checked;
            private set { }
        }

        public double index
        {
            get { return bean.index; }
            set { bean.index = value; }
        }
        #endregion

        #region public methods
        public void Reload(Models.Data.Bean bean)
        {
            this.bean = bean;
            UpdateLabels();
        }

        public void InvertSelection()
        {
            chkTitle.Checked = !chkTitle.Checked;
        }

        public void Select(bool state)
        {
            chkTitle.Checked = state;
        }

        public void SetStatus(string status)
        {
            if (bean.status == status)
            {
                return;
            }
            lbStatus.Text = status;
            bean.status = status;
        }

        public void SetTitle(string title)
        {
            if (bean.title == title)
            {
                return;
            }
            lbTitle.Text = title;
            bean.title = title;
        }

        public Models.Data.Bean GetBean()
        {
            return this.bean;
        }
        #endregion

        #region UI events
        private void BeanUI_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void chkTitle_CheckedChanged(object sender, EventArgs e)
        {
            var isSelected = chkTitle.Checked;
            if (bean.isSelected != isSelected)
            {
                bean.isSelected = isSelected;
            }
        }

        private void lbStatus_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }

        private void lbTitle_Click(object sender, EventArgs e)
        {
            chkTitle.Checked = !chkTitle.Checked;
        }

        private void lbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }
        #endregion
    }
}
