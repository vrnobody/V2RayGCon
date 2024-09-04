using System.Drawing;
using System.Windows.Forms;

namespace VgcApis.UserControls
{
    public class AcmComboBox : ToolStripComboBox
    {
        public AcmComboBox()
            : base() { }

        #region Acm
        public event KeyEventHandler OnAcmKeyDown;

        private bool _readonly = false;

        public bool ReadOnly
        {
            get { return _readonly; }
            set { _readonly = value; }
        }

        public Point GetPositionFromCharIndex(int index)
        {
            return Point.Empty;
        }

        public Form FindForm()
        {
            return this.GetCurrentParent().FindForm();
        }

        public void InvokeKeyDown(Keys keyCode)
        {
            base.OnKeyDown(new KeyEventArgs(keyCode));
        }
        #endregion

        #region disable keys
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                return true;
            }
            var action = OnAcmKeyDown;
            if (action != null && action.GetInvocationList().Length > 0)
            {
                action.Invoke(this, new KeyEventArgs(keyData));
                switch (keyData)
                {
                    case Keys.Enter:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Escape:
                        return true;
                }
            }
            return base.ProcessCmdKey(ref m, keyData);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
            }
            base.OnKeyUp(e);
        }

        #endregion
    }
}
