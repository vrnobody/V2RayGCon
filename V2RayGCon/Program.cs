using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;


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
            Thread.CurrentThread.Name = VgcApis.Models.Consts.Libs.UiThreadName;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IntPtr pShcoreDll = HiResSupport();
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                var form = new Views.WinForms.FormMain();
                Application.Run(form);
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show(I18N.ExitOtherVGCFirst);
            }
            Libs.Sys.SafeNativeMethods.FreeLibrary(pShcoreDll);
        }

        #region DPI awareness
        // PROCESS_DPI_AWARENESS = 0/1/2 None/SystemAware/PerMonitorAware
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SetProcessDpiAwareness(int PROCESS_DPI_AWARENESS);
        static IntPtr HiResSupport()
        {
            // load Shcore.dll and get high resolution support
            IntPtr pDll = Libs.Sys.SafeNativeMethods.LoadLibrary(@"Shcore.DLL");
            Libs.Sys.DllLoader.CallMethod(
                pDll,
                @"SetProcessDpiAwareness",
                typeof(SetProcessDpiAwareness),
                (method) => ((SetProcessDpiAwareness)method).Invoke(2));
            return pDll;
        }
        #endregion
    }
}
