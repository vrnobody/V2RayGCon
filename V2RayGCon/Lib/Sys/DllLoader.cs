using System;
using System.Runtime.InteropServices;

namespace V2RayGCon.Lib.Sys
{
    static class DllLoader
    {

        /*
         * Suppose we call SetProcessDpiAwareness from Shcore.dll.
         * 
         * Define a delegate first.
         * [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
         * private delegate int SetProcessDpiAwareness(int PROCESS_DPI_AWARENESS);
         * 
         * Then parameter lambda should look like this:
         * (method) => ((SetProcessDpiAwareness)method).Invoke(0);  // PROCESS_DPI_AWARENESS=0
         */
        public static bool CallMethod(IntPtr pDll, string methodName, Type methodType, Action<Delegate> lambda)
        {
            var method = GetMethod(pDll, methodName, methodType);
            if (method != null)
            {
                lambda(method);
                return true;
            }
            return false;
        }

        public static Delegate GetMethod(IntPtr pDll, string methodName, Type methodType)
        {
            if (pDll == IntPtr.Zero)
            {
                return null;
            }

            IntPtr pMethod = Lib.Sys.SafeNativeMethods.GetProcAddress(pDll, methodName);
            if (pMethod != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer(pMethod, methodType);
            }

            return null;
        }
    }
}
