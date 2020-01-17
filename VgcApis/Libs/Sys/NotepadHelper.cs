using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VgcApis.Libs.Sys
{
    // https://stackoverflow.com/questions/7613576/how-to-open-text-in-notepad-from-net
    public static class NotepadHelper
    {
#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
        private static extern int SetWindowText(IntPtr hWnd, string text);

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        public static void ShowMessage(string message = null, string title = null)
        {
            Process notepad = Process.Start(new ProcessStartInfo("notepad.exe"));
            if (notepad != null)
            {
                notepad.WaitForInputIdle();

                if (!string.IsNullOrEmpty(title))
                    SetWindowText(notepad.MainWindowHandle, title);

                if (!string.IsNullOrEmpty(message))
                {
                    IntPtr child = FindWindowEx(notepad.MainWindowHandle, new IntPtr(0), "Edit", null);
                    SendMessage(child, 0x000C, 0, message);
                }
            }
        }
    }
}
