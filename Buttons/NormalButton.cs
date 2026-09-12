using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Obpression.Buttons
{
    internal class NormalButton : Button
    {
        public NormalButton()
        {
            Text = "NORMAL";
            Size = new Size(100, 45);
            ForeColor = Color.White;
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.DimGray;
            FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
            TabStop = false;
        }
    }
}
