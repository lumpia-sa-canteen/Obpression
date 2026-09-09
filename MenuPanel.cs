using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Obpression
{
    public class MenuPanel : Panel
    {
        private int cornerRadius = 30;
        private int panelOpacity = 150;

        public MenuPanel()
        {
            BackColor = Color.Transparent;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true
            );

            UpdateRegion();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }

        private void UpdateRegion()
        {
            if (Width <= 0 || Height <= 0)
                return;

            using (GraphicsPath path = CreateRoundedPath(
                new Rectangle(0, 0, Width - 1, Height - 1),
                cornerRadius))
            {
                Region = new Region(path);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Parent == null)
                return;

            // Get the Form
            Form form = Parent as Form;

            if (form == null || form.BackgroundImage == null)
                return;

            Image image = form.BackgroundImage;

            // ==========================================
            // Draw the same "Cover" background as Form1
            // ==========================================

            float scaleX =
                (float)form.ClientSize.Width / image.Width;

            float scaleY =
                (float)form.ClientSize.Height / image.Height;

            float scale =
                Math.Max(scaleX, scaleY);

            int width =
                (int)(image.Width * scale);

            int height =
                (int)(image.Height * scale);

            int x =
                (form.ClientSize.Width - width) / 2;

            int y =
                (form.ClientSize.Height - height) / 2;

            e.Graphics.InterpolationMode =
                InterpolationMode.HighQualityBicubic;

            e.Graphics.PixelOffsetMode =
                PixelOffsetMode.HighQuality;

            e.Graphics.SmoothingMode =
                SmoothingMode.HighQuality;

            // Draw background image
            e.Graphics.DrawImage(
                image,
                new Rectangle(
                    x - Left,
                    y - Top,
                    width,
                    height
                )
            );

            // ==========================================
            // Draw the same dark overlay
            // ==========================================

            using (SolidBrush overlayBrush =
                new SolidBrush(
                    Color.FromArgb(
                        120,
                        0,
                        0,
                        0)))
            {
                e.Graphics.FillRectangle(
                    overlayBrush,
                    ClientRectangle
                );
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            e.Graphics.PixelOffsetMode =
                PixelOffsetMode.HighQuality;

            e.Graphics.CompositingQuality =
                CompositingQuality.HighQuality;

            Rectangle rect = new Rectangle(
                0,
                0,
                Width - 1,
                Height - 1
            );

            using (GraphicsPath path =
                CreateRoundedPath(
                    rect,
                    cornerRadius))
            using (SolidBrush brush =
                new SolidBrush(
                    Color.FromArgb(
                        panelOpacity,
                        0,
                        0,
                        0)))
            {
                e.Graphics.FillPath(
                    brush,
                    path
                );
            }
        }

        private GraphicsPath CreateRoundedPath(
            Rectangle rect,
            int radius)
        {
            GraphicsPath path =
                new GraphicsPath();

            int diameter = radius * 2;

            path.AddArc(
                rect.X,
                rect.Y,
                diameter,
                diameter,
                180,
                90
            );

            path.AddArc(
                rect.Right - diameter,
                rect.Y,
                diameter,
                diameter,
                270,
                90
            );

            path.AddArc(
                rect.Right - diameter,
                rect.Bottom - diameter,
                diameter,
                diameter,
                0,
                90
            );

            path.AddArc(
                rect.X,
                rect.Bottom - diameter,
                diameter,
                diameter,
                90,
                90
            );

            path.CloseFigure();

            return path;
        }
    }
}