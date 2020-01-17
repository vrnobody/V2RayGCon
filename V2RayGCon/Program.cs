using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

/*
// upgraded to dot net 4.5 

#region Support CallerMemberName on .net 4.0
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
    }
}
#endregion
*/

namespace V2RayGCon
{
    static class Program
    {

        #region single instance
        // https://stackoverflow.com/questions/19147/what-is-the-correct-way-to-create-a-single-instance-application

        static Mutex mutex = new Mutex(
            true,
#if DEBUG
            "{a4333801-a206-4061-9e20-1f03e2deaf7f}"
#else
            "{84d287ae-c0b0-4c1a-9ecc-d98c26577c02}"
#endif
            );
        #endregion

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IntPtr pShcoreDll = HiResSupport();
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                var app = new Service.Launcher();
                if (app.Run())
                {
                    Application.Run();
                }
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show(I18N.ExitOtherVGCFirst);
            }
            Lib.Sys.SafeNativeMethods.FreeLibrary(pShcoreDll);
        }

        #region DPI awareness
        // PROCESS_DPI_AWARENESS = 0/1/2 None/SystemAware/PerMonitorAware
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SetProcessDpiAwareness(int PROCESS_DPI_AWARENESS);
        static IntPtr HiResSupport()
        {
            // load Shcore.dll and get high resolution support
            IntPtr pDll = Lib.Sys.SafeNativeMethods.LoadLibrary(@"Shcore.DLL");
            Lib.Sys.DllLoader.CallMethod(
                pDll,
                @"SetProcessDpiAwareness",
                typeof(SetProcessDpiAwareness),
                (method) => ((SetProcessDpiAwareness)method).Invoke(2));
            return pDll;
        }
        #endregion
    }
}
