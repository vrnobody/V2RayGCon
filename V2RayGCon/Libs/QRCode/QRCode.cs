using System;
using System.Drawing;
using System.Windows.Forms;
using ZXing;
using ZXing.QrCode;

namespace V2RayGCon.Libs.QRCode
{
    // 这段代码由 ssr-csharp 魔改而成
    public class QRCode
    {
        static readonly QrCodeEncodingOptions options = new QrCodeEncodingOptions
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

        public static void ScanQRCode(Action<string> success, Action fail)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                foreach (var screen in Screen.AllScreens)
                {
                    if (ScanScreen(screen, success))
                    {
                        return;
                    }
                }
                fail?.Invoke();
            });
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

        static bool ScanScreen(Screen screen, Action<string> success)
        {
            VgcApis.Libs.Sys.ScreenExtensions.CalcScreenScaleInfo(
                screen,
                out var bounds,
                out var scale
            );
            using (Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height))
            {
                // take a screenshot
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(
                        bounds.X,
                        bounds.Y,
                        0,
                        0,
                        bounds.Size,
                        CopyPixelOperation.SourceCopy
                    );
                }

                var luminance = new BitmapLuminanceSource(screenshot);
                var hybirdBin = new HybridBinarizer(luminance);
                var binBmp = new BinaryBitmap(hybirdBin);

                var reader = new QRCodeReader();
                var result = reader.decode(binBmp);
                if (result == null)
                {
                    return false;
                }
                ShowSplashForm(result, bounds, scale, success);
                return true;
            }
        }

        static void ShowSplashForm(
            Result result,
            Rectangle bounds,
            PointF scale,
            Action<string> success
        )
        {
            var link = result.Text;
            var qrCodeRect = TransformQrCodeRect(result, bounds, scale);
            var formBounds = TransformWinformRect(bounds, scale);

            VgcApis.Misc.UI.Invoke(() =>
            {
                var qrSplash = new QRCodeSplashForm
                {
                    Location = formBounds.Location,
                    Size = formBounds.Size,
                    TargetRect = qrCodeRect
                };
                qrSplash.FormClosed += (s, a) => success?.Invoke(link);
                qrSplash.Show();
            });
        }

        static int DivIntFloat(int value, float ratio)
        {
            return (int)(value / ratio);
        }

        static Rectangle TransformQrCodeRect(Result result, Rectangle bounds, PointF scale)
        {
            // get qrcode rect
            Point start = new Point(bounds.Size.Width, bounds.Size.Height);
            Point end = new Point(0, 0);

            foreach (var point in result.ResultPoints)
            {
                start.X = Math.Min(start.X, (int)point.X);
                start.Y = Math.Min(start.Y, (int)point.Y);
                end.X = Math.Max(end.X, (int)point.X);
                end.Y = Math.Max(end.Y, (int)point.Y);
            }

            return new Rectangle(
                DivIntFloat(start.X, scale.X),
                DivIntFloat(start.Y, scale.Y),
                DivIntFloat(end.X - start.X, scale.X),
                DivIntFloat(end.Y - start.Y, scale.Y)
            );
        }

        static Rectangle TransformWinformRect(Rectangle bounds, PointF scale)
        {
            return new Rectangle(
                bounds.X,
                bounds.Y,
                DivIntFloat(bounds.Width, scale.X),
                DivIntFloat(bounds.Height, scale.Y)
            );
        }
    }
}
