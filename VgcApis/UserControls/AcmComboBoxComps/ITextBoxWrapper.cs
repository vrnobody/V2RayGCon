using System;
using System.Windows.Forms;

namespace VgcApis.UserControls.AcmComboBoxComps
{
    /// <summary>
    /// Wrapper over the control like TextBox.
    /// </summary>
    public interface ITextBoxWrapper
    {
        Control TargetControl { get; }

        string Text { get; set; }

        bool Readonly { get; }

        void Focus();

        void InvokeKeyDownCallback(Keys e);

        event EventHandler LostFocus;
        event KeyEventHandler KeyDown;
        event MouseEventHandler MouseDown;
    }
}
