using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VgcApis.UserControls
{
    // https://stackoverflow.com/questions/3708113/round-shaped-buttons
    public class RoundButton : Button
    {
        public RoundButton()
            : base()
        {
            this.FlatAppearance.BorderSize = 0;
            this.FlatStyle = FlatStyle.Flat;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var graphicsPath = _getRoundRectangle(this.ClientRectangle))
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
                using (var pen = new Pen(Color.Gray, 1.0f))
                    e.Graphics.DrawPath(pen, graphicsPath);
            }
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
