using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon
{
    static class Program
    {
        #region single instance
        // https://stackoverflow.com/questions/19147/what-is-the-correct-way-to-create-a-single-instance-application

        static readonly Mutex mutex = new Mutex(true,
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

            SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                var app = new Services.Launcher();
                if (app.Warmup())
                {
                    app.Run();
                    Application.Run(app.context);
                    app.Dispose();
                }
                mutex.ReleaseMutex();
            }
            else
            {
                VgcApis.Misc.UI.MsgBox(I18N.ExitOtherVGCFirst);
            }
        }

        #region DPI awareness
        [DllImport("user32.dll")]
        public static extern IntPtr SetProcessDPIAware();
        #endregion
    }
}
