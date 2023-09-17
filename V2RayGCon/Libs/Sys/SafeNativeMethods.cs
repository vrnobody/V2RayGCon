using System;
using System.Runtime.InteropServices;
using System.Security;

namespace V2RayGCon.Libs.Sys
{
    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        #region dll loader

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Globalization",
            "CA2101:SpecifyMarshalingForPInvokeStringArguments",
            MessageId = "0"
        )]
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Globalization",
            "CA2101:SpecifyMarshalingForPInvokeStringArguments",
            MessageId = "1"
        )]
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
        #endregion

        #region ui


        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);
        #endregion
    }
}
