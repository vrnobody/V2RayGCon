using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VgcApis.UserControls
{
    // https://stackoverflow.com/questions/192413/how-do-you-prevent-a-richtextbox-from-refreshing-its-display
    public sealed class RepaintController
    {

        bool isDisabled = false;

        Control control;

        public RepaintController(Control control)
        {
            this.control = control;
        }

        private const int WM_USER = 0x0400;
        private const int EM_SETEVENTMASK = (WM_USER + 69);
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int WM_SETREDRAW = 0x0b;
        private IntPtr eventMask;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region public methods
        public void EnableRepaintEvent()
        {
            if (!isDisabled)
            {
                return;
            }
            isDisabled = false;

            // turn on events
            SendMessage(control.Handle, EM_SETEVENTMASK, IntPtr.Zero, eventMask);
            // turn on redrawing
            SendMessage(control.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            // this forces a repaint, which for some reason is necessary in some cases.
            control.Invalidate();
        }

        public void DisableRepaintEvent()
        {
            if (isDisabled)
            {
                return;
            }
            isDisabled = true;

            // Stop redrawing:
            SendMessage(control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            // Stop sending of events:
            eventMask = SendMessage(control.Handle, EM_GETEVENTMASK, IntPtr.Zero, IntPtr.Zero);
        }
        #endregion


    }
}
