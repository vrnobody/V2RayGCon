using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VgcApis.UserControls
{
    public class ExRichTextBox : RichTextBox
    {
        [DllImport(
            "kernel32.dll",
            EntryPoint = "LoadLibraryW",
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        private static extern IntPtr LoadLibraryW(string s_File);

        public static IntPtr LoadLibrary(string s_File)
        {
            var module = LoadLibraryW(s_File);
            if (module != IntPtr.Zero)
                return module;
            var error = Marshal.GetLastWin32Error();
            throw new Win32Exception(error);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                try
                {
                    if (IsWin8OrHigher())
                    {
                        LoadLibrary("MsftEdit.dll"); // Available since XP SP1
                        cp.ClassName = "RichEdit50W";
                    }
                }
                catch
                { /* Windows XP without any Service Pack.*/
                }
                return cp;
            }
        }

        bool IsWin8OrHigher()
        {
            var v = Environment.OSVersion.Version;
            if (v.Major > 6 || (v.Major == 6 && v.Minor > 1))
            {
                // win 8+
                return true;
            }
            // win7-
            return false;
        }
    }
}
