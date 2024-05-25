using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VgcApis.Libs.Sys
{
    public static class ScreenExtensions
    {
        static float Round(float value)
        {
            const float step = 0.25f;
            return (float)(Math.Round(value / step) * step);
        }

        public static void CalcScreenScaleInfo(
            Screen screen,
            out Rectangle bounds,
            out PointF scale
        )
        {
            const float dpi = 96.0f;

            GetDpi(screen, DpiType.Angular, out var dpiX, out var dpiY);

            var sx = Round(dpi / dpiX);
            var sy = Round(dpi / dpiY);

            scale = new PointF(sx, sy);

            var width = scale.X * screen.Bounds.Width;
            var height = scale.Y * screen.Bounds.Height;
            var size = new Size((int)width, (int)height);
            bounds = new Rectangle(screen.Bounds.Location, size);
        }

        public static void GetDpi(Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(
                pnt,
                2 /*MONITOR_DEFAULT TONEAREST*/
            );
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In] Point pt, [In] uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor(
            [In] IntPtr hmonitor,
            [In] DpiType dpiType,
            [Out] out uint dpiX,
            [Out] out uint dpiY
        );
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }
}
