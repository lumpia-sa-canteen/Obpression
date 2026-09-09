using System;
using System.Drawing;
using System.Windows.Forms;

namespace Obpression
{
    public class BackButton : Button
    {
        private Form currentForm;
        private Form previousForm;

        public BackButton(Form currentForm, Form previousForm)
        {
            this.currentForm = currentForm;
            this.previousForm = previousForm;

            Text = "<-";

            Size = new Size(60, 40);

            ForeColor = Color.White;
            BackColor = Color.Transparent;

            Font = new Font(
                "Segoe UI",
                16F,
                FontStyle.Regular
            );

            FlatStyle = FlatStyle.Flat;

            FlatAppearance.BorderSize = 0;

            FlatAppearance.MouseOverBackColor =
                Color.Transparent;

            FlatAppearance.MouseDownBackColor =
                Color.Transparent;

            TabStop = false;

            // ==========================================
            // Back
            // ==========================================

            Click += BackButton_Click;
        }

        private void BackButton_Click(
            object sender,
            EventArgs e)
        {
            currentForm.Hide();

            previousForm.Show();
        }
    }
}