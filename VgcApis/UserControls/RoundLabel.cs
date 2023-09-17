using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VgcApis.UserControls
{
    // https://stackoverflow.com/questions/42627293/label-with-smooth-rounded-corners
    public class RoundLabel : Label
    {
        [Browsable(true)]
        public Color _BackColor { get; set; }

        public RoundLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var graphicsPath = _getRoundRectangle(this.ClientRectangle))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(_BackColor))
                    e.Graphics.FillPath(brush, graphicsPath);
                using (var pen = new Pen(_BackColor, 1.0f))
                    e.Graphics.DrawPath(pen, graphicsPath);
            }
            base.OnPaint(e);
        }

        private GraphicsPath _getRoundRectangle(Rectangle rectangle)
        {
            int cornerRadius = rectangle.Height / 3; // change this value according to your needs
            int diminisher = 1;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rectangle.X, rectangle.Y, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(
                rectangle.X + rectangle.Width - cornerRadius - diminisher,
                rectangle.Y,
                cornerRadius,
                cornerRadius,
                270,
                90
            );
            path.AddArc(
                rectangle.X + rectangle.Width - cornerRadius - diminisher,
                rectangle.Y + rectangle.Height - cornerRadius - diminisher,
                cornerRadius,
                cornerRadius,
                0,
                90
            );
            path.AddArc(
                rectangle.X,
                rectangle.Y + rectangle.Height - cornerRadius - diminisher,
                cornerRadius,
                cornerRadius,
                90,
                90
            );
            path.CloseAllFigures();
            return path;
        }
    }
}
