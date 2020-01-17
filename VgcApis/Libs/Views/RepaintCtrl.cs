using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VgcApis.Libs.Views
{
    // https://stackoverflow.com/questions/192413/how-do-you-prevent-a-richtextbox-from-refreshing-its-display
    public sealed class RepaintCtrl
    {

        Libs.Tasks.Bar bar = new Tasks.Bar();
        Control control;

        public RepaintCtrl(Control control)
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
        public void Enable()
        {
            if (!bar.Install())
            {
                return;
            }
            // turn on events
            SendMessage(control.Handle, EM_SETEVENTMASK, IntPtr.Zero, eventMask);
            // turn on redrawing
            SendMessage(control.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            // this forces a repaint, which for some reason is necessary in some cases.
            control.Invalidate();
        }

        public void Disable()
        {
            if (bar.Install())
            {
                bar.Remove();
                return;
            }
            // Stop redrawing:
            SendMessage(control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            // Stop sending of events:
            eventMask = SendMessage(control.Handle, EM_GETEVENTMASK, IntPtr.Zero, IntPtr.Zero);

            bar.Remove();
        }
        #endregion


    }
}
