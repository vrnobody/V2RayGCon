using System;
using System.Windows.Forms;

namespace VgcApis.UserControls.AcmComboBoxComps
{
    /// <summary>
    /// Wrapper over the control like TextBox.
    /// </summary>
    public interface ITextBoxWrapper
    {
        int SelectionStart { get; }

        Control TargetControl { get; }

        string Text { get; set; }

        bool Readonly { get; }

        void Select(int pos);

        void InvokeKeyDownCallback(Keys e);

        event EventHandler LostFocus;
        event KeyEventHandler KeyDown;
        event MouseEventHandler MouseDown;
    }
}
