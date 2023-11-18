using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon
{
    static class Program
    {
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

            var app = new Services.Launcher();
            if (app.Warmup())
            {
                app.Run();
                Application.Run(app.context);
                app.Dispose();
            }
        }

        #region DPI awareness
        [DllImport("user32.dll")]
        public static extern IntPtr SetProcessDPIAware();
        #endregion
    }
}
