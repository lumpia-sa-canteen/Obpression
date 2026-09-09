using System.Drawing;
using System.Windows.Forms;

namespace Obpression
{
    public class AboutUs : Button
    {
        public AboutUs()
        {
            Text = "About Us";

            Size = new Size(110, 40);

            ForeColor = Color.White;
            BackColor = Color.Gray;

            Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Regular
            );

            FlatStyle = FlatStyle.Flat;

            // Remove border
            FlatAppearance.BorderSize = 0;

            // Hover color
            FlatAppearance.MouseOverBackColor =
                Color.DimGray;

            // Pressed color
            FlatAppearance.MouseDownBackColor =
                Color.FromArgb(40, 40, 40);

            TabStop = false;
        }
    }
}