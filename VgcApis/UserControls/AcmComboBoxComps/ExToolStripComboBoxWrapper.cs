using System;
using System.Windows.Forms;

namespace VgcApis.UserControls.AcmComboBoxComps
{
    public class ExToolStripComboBoxWrapper : ITextBoxWrapper
    {
        public AcmComboBox target;

        public ExToolStripComboBoxWrapper(AcmComboBox target)
        {
            this.target = target;
        }

        public bool Readonly
        {
            get => target.ReadOnly;
        }

        public Control TargetControl
        {
            get => target.GetCurrentParent();
        }

        public string Text
        {
            get => target.Text ?? "";
            set => target.Text = value ?? "";
        }

        public void Focus()
        {
            target.Focus();
            var p = Math.Max(0, target.Text.Length);
            target.Select(p, 0);
        }

        public void InvokeKeyDownCallback(Keys key)
        {
            target.InvokeKeyDownCallback(key);
        }

        //Events
        public virtual event KeyEventHandler KeyDown
        {
            add { target.OnAcmKeyDown += value; }
            remove { target.OnAcmKeyDown -= value; }
        }
        public virtual event EventHandler LostFocus
        {
            add { target.LostFocus += value; }
            remove { target.LostFocus -= value; }
        }
        public virtual event MouseEventHandler MouseDown
        {
            add { target.MouseDown += value; }
            remove { target.MouseDown -= value; }
        }
    }
}
