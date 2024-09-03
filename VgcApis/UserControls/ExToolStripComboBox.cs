using System.Windows.Forms;

namespace VgcApis.UserControls
{
    public class ExToolStripComboBox : ToolStripComboBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
            }
            base.OnKeyDown(e);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Tab):
                    return true;
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
    }
}
