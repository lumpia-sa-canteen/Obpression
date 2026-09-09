using System.Drawing;
using System.Windows.Forms;

namespace Obpression
{
    public class ConsultWithAi : Button
    {
        public ConsultWithAi()
        {
            Text = "Consult";
            Size = new Size(110, 40);
            ForeColor = Color.White;
            BackColor = Color.Gray;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.DimGray;
            FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
            TabStop = false;
        }
    }
}