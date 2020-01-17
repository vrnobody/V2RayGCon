using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

using System.Windows.Forms;
using ZXing;
using ZXing.QrCode;

namespace V2RayGCon.Lib.QRCode
{
    // 这段代码由 ssr-csharp 魔改而成
    public class QRCode
    {
        static QrCodeEncodingOptions options = new QrCodeEncodingOptions
        {
            DisableECI = true,
            CharacterSet = "UTF-8"
        };

        public enum WriteErrors
        {
            DataEmpty,
            DataTooBig,
            Success,
        }

        public static Tuple<Bitmap, WriteErrors> GenQRCode(string content, int size = 512)
        {
            Bitmap binCode = null;
            if (string.IsNullOrEmpty(content))
            {
                return Tuple.Create(binCode, WriteErrors.DataEmpty);
            }

            options.Width = size;
            options.Height = size;

            IBarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options,
            };

            var error = WriteErrors.Success;

            try
            {
                binCode = new Bitmap(writer.Write(content));
            }
            catch
            {
                error = WriteErrors.DataTooBig;
            }

            return Tuple.Create(binCode, error);
        }

        static Func<Rectangle, Rectangle, int> GenNearCenterCompareFunc(int centerX, int centerY)
        {
            return (a, b) =>
            {
                int distAX = a.X + a.Width / 2 - centerX;
                int distAY = a.Y + a.Height / 2 - centerY;
                int distA = distAX * distAX + distAY * distAY;

                int distBX = b.X + b.Width / 2 - centerX;
                int distBY = b.Y + b.Height / 2 - centerY;
                int distB = distBX * distBX + distBY * distBY;

                return Math.Sign(distA - distB);
            };
        }

        // 默认以屏幕高宽 约（划重点）1/5为移动单位，生成一系列边长为3/5的正方形扫描窗口
        public static List<Rectangle[]> GenSquareScanWinList(Point screenSize, int parts = 5, int scanSize = 3)
        {
            // center x/y adjustment x/y
            int unit, cx, cy, ax, ay, size;

            // 注意屏幕可能是竖着的
            unit = Math.Min(screenSize.X, screenSize.Y) / parts;
            ax = (screenSize.X % unit) / (screenSize.X / unit - scanSize); //这就是为什么上面说约1/5
            ay = (screenSize.Y % unit) / (screenSize.Y / unit - scanSize);
            cx = screenSize.X / 2;
            cy = screenSize.Y / 2;
            size = scanSize * unit;

            // 注意最后一个窗口并非和屏幕对齐,但影响不大
            // start position x/y
            int sx, sy;

            var screenWinList = new List<Rectangle>();
            for (var row = 0; row <= screenSize.Y / unit - scanSize; row++)
            {
                sy = row * (unit + ay);
                for (var file = 0; file <= screenSize.X / unit - scanSize; file++)
                {
                    sx = file * (unit + ax);
                    screenWinList.Add(new Rectangle(sx, sy, size, size));
                }
            }

            var NearCenterComparer = GenNearCenterCompareFunc(cx, cy);

            screenWinList.Sort((a, b) => NearCenterComparer(a, b));

            // List: [winRect, screeRect], [], ...
            var winRect = new Rectangle(0, 0, size, size);
            var scanList = new List<Rectangle[]>();
            foreach (var rect in screenWinList)
            {
                scanList.Add(new Rectangle[] { winRect, rect });
            }
            return scanList;
        }

        public static List<Rectangle[]> GenZoomScanWinList(Point screenSize, int factor = 5)
        {
            List<Rectangle[]> scanList = new List<Rectangle[]>();

            for (var i = 1; i < Math.Max(factor, 3); i++)
            {
                var shrink = 2.8 - Math.Pow(1.0 + 1.0 / i, i);
                scanList.Add(new Rectangle[] {
                    new Rectangle(0,0,(int)(screenSize.X*shrink),(int)(screenSize.Y*shrink)),
                    new Rectangle(0,0,screenSize.X,screenSize.Y)                    });
            }
            return scanList;
        }

        static void ShowResult(Result result, Point screenLocation, Rectangle winRect, Rectangle screenRect, Action<string> success)
        {
            var link = result.Text;
            Debug.WriteLine("Read: " + Lib.Utils.CutStr(link, 32));

            var qrcodeRect = GetQRCodeRect(result, winRect, screenRect);

            var formRect = new Rectangle(
                screenLocation.X + screenRect.Location.X,
                screenLocation.Y + screenRect.Location.Y,
                screenRect.Width,
                screenRect.Height);

            ShowSplashForm(formRect, qrcodeRect, () => success?.Invoke(link));
        }

        static bool ScanScreen(Screen screen, List<Rectangle[]> scanRectList, Action<string> success)
        {
            using (Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height))
            {
                // take a screenshot
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(
                        screen.Bounds.X,
                        screen.Bounds.Y,
                        0,
                        0,
                        screenshot.Size,
                        CopyPixelOperation.SourceCopy);
                }

                for (int i = 0; i < scanRectList.Count; i++)
                {
                    var winRect = scanRectList[i][0];
                    var screenRect = scanRectList[i][1];

                    if (ScanWindow(screenshot, screen.Bounds.Location, winRect, screenRect, success))
                    {
                        Debug.WriteLine("Screen {0}: {1}", i, screenRect);
                        Debug.WriteLine("Window {0}: {1}", i, winRect);

                        return true;
                    }
                }
            }

            return false;
        }

        public static void ScanQRCode(Action<string> success, Action fail)
        {
            Thread.Sleep(100);

            foreach (var screen in Screen.AllScreens)
            {
                var parts = 8;
                var scanSize = (int)(0.5 * parts);

                // Debug.WriteLine("res: {0}x{1}", screen.Bounds.Width, screen.Bounds.Height);

                var scanRectList = GenSquareScanWinList(new Point(
                    screen.Bounds.Width,
                    screen.Bounds.Height),
                    parts, scanSize);

                scanRectList.AddRange(GenZoomScanWinList(new Point(
                    screen.Bounds.Width,
                    screen.Bounds.Height)));

                if (ScanScreen(screen, scanRectList, success))
                {
                    return;
                }
            }
            fail?.Invoke();
        }

        static Rectangle GetQRCodeRect(Result result, Rectangle winRect, Rectangle screenRect)
        {
            // get qrcode rect
            Point start = new Point(winRect.Width, winRect.Height);
            Point end = new Point(0, 0);

            foreach (var point in result.ResultPoints)
            {
                start.X = Math.Min(start.X, (int)point.X);
                start.Y = Math.Min(start.Y, (int)point.Y);
                end.X = Math.Max(end.X, (int)point.X);
                end.Y = Math.Max(end.Y, (int)point.Y);
            }

            double factor = 1.0 * screenRect.Width / winRect.Width;

            Rectangle qrRect = new Rectangle(
                // splashForm will add screenRect.X/Y
                (int)(start.X * factor),
                (int)(start.Y * factor),
                (int)(factor * (end.X * 1.0 - start.X)),
                (int)(factor * (end.Y * 1.0 - start.Y)));

            Debug.WriteLine("factor: {0}", factor);
            Debug.WriteLine("qrCode: {0}", qrRect);

            return qrRect;
        }

        static void ShowSplashForm(Rectangle win, Rectangle target, Action done)
        {
            void ShowFormInBackground()
            {
                var qrSplash = new QRCodeSplashForm();
                qrSplash.Location = win.Location;
                qrSplash.Size = win.Size;
                qrSplash.TargetRect = target;
                qrSplash.FormClosed += (s, a) => done();

                try
                {
                    qrSplash.Show();
                }
                catch { }
                Application.Run();
            }

            VgcApis.Libs.Utils.RunInBackground(() => ShowFormInBackground());
        }

        static bool ScanWindow(Bitmap screenshot, Point screenLocation, Rectangle winRect, Rectangle screenRect, Action<string> success)
        {
            Result result;
            using (Bitmap window = new Bitmap(winRect.Width, winRect.Height))
            {
                using (Graphics g = Graphics.FromImage(window))
                {
                    g.DrawImage(screenshot, winRect, screenRect, GraphicsUnit.Pixel);
                }

                var binBMP = new BinaryBitmap(
                    new HybridBinarizer(
                        new BitmapLuminanceSource(window)));

                QRCodeReader reader = new QRCodeReader();

                result = reader.decode(binBMP);
            }

            if (result == null)
            {
                return false;
            }

            ShowResult(result, screenLocation, winRect, screenRect, success);
            return true;
        }
    }
}
