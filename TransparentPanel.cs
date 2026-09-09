using System.Drawing;
using System.Windows.Forms;

namespace Obpression
{
    public class TransparentPanel : Panel
    {
        public TransparentPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true
            );

            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Show the form's background image first.

            base.OnPaintBackground(e);

            // Dark transparent overlay.

            using (SolidBrush brush = new SolidBrush(
                Color.FromArgb(
                    120,
                    0,
                    0,
                    0)))
            {
                e.Graphics.FillRectangle(
                    brush,
                    ClientRectangle
                );
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Nothing else to draw.
        }
    }
}
