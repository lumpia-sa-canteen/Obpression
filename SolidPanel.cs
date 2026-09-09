using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Obpression
{
    public class SolidPanel : Panel
    {
        private int cornerRadius = 15;
        private int borderThickness = 2;

        public SolidPanel()
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
            Invalidate();
        }

        private void UpdateRegion()
        {
            if (Width <= 0 || Height <= 0)
                return;

            using (GraphicsPath path = CreateRoundedPath(
                new Rectangle(
                    0,
                    0,
                    Width - 1,
                    Height - 1
                ),
                cornerRadius))
            {
                Region = new Region(path);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // IMPORTANT:
            // Repaint the area behind this panel.
            if (Parent != null)
            {
                GraphicsState state = e.Graphics.Save();

                try
                {
                    e.Graphics.TranslateTransform(
                        -Left,
                        -Top
                    );

                    PaintEventArgs parentArgs =
                        new PaintEventArgs(
                            e.Graphics,
                            new Rectangle(
                                Left,
                                Top,
                                Width,
                                Height
                            )
                        );

                    InvokePaintBackground(
                        Parent,
                        parentArgs
                    );
                }
                finally
                {
                    e.Graphics.Restore(state);
                }
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
                borderThickness / 2,
                borderThickness / 2,
                Width - borderThickness - 1,
                Height - borderThickness - 1
            );

            using (GraphicsPath path =
                CreateRoundedPath(
                    rect,
                    cornerRadius))
            using (Pen pen =
                new Pen(
                    Color.White,
                    borderThickness))
            {
                e.Graphics.DrawPath(
                    pen,
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