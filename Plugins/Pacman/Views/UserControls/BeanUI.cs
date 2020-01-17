using System;
using System.Data;
using System.Windows.Forms;

namespace Pacman.Views.UserControls
{
    public partial class BeanUI : UserControl
    {
        Models.Data.Bean bean;

        public BeanUI(Models.Data.Bean bean)
        {
            this.bean = bean ?? throw new NoNullAllowedException("Bean must not null.");
            InitializeComponent();
        }

        private void BeanUI_Load(object sender, EventArgs e)
        {
            chkTitle.Text = bean.title;
            lbStatus.Text = bean.status;
            chkTitle.Checked = bean.isSelected;
        }

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

        #region public event
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
            chkTitle.Text = title;
            bean.title = title;
        }

        public Models.Data.Bean GetBean()
        {
            return this.bean;
        }
        #endregion

        #region UI
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
        }

        #endregion

        private void lbStatus_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }
    }
}
