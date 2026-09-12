using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Configuration;

namespace Obpression
{
    public class OldForm : Form
    {
        private Form previousForm;

        public OldForm(Form previousForm)
        {
            this.previousForm = previousForm;

            //======================
            //window settings
            //======================

            Text = "Obpression";

            ClientSize = new Size(400, 700);
            MinimumSize = new Size(400, 700);
            MaximumSize = new Size(400, 700);
            MaximizeBox = false;
            MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;

            //======================
            //background image
            //======================

            BackgroundImage = Properties.Resources.bgpic;
            BackgroundImageLayout = ImageLayout.None;

            //=======================
            //transparent dark overlay
            //=======================

            TransparentPanel overlay = new TransparentPanel();
            overlay.Dock = DockStyle.Fill;
            Controls.Add(overlay);

            //=======================
            //main menu
            //=======================

            MenuPanel menuPanel = new MenuPanel();
            menuPanel.Size = new Size(340, 530);
            menuPanel.Location = new Point(
                (ClientSize.Width - menuPanel.Width) / 2,
                (ClientSize.Height - menuPanel.Height) / 2
                );

            Controls.Add(menuPanel);

            //=======================
            //old form panel
            //=======================

            SolidPanel oldPanel = new SolidPanel();
            oldPanel.Size = new Size(300, 400);
            oldPanel.Location = new Point(20, 50);
            oldPanel.BackColor = Color.Transparent;

            //=======================
            // old
            //=======================

            Label title = new Label();
            title.Text = "OLD";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            title.Size = new Size(oldPanel.Width, 50);
            title.Location = new Point(0, 20);
            title.TextAlign = ContentAlignment.MiddleCenter;

            //=======================
            // description
            //=======================

            Label desc = new Label();
            desc.Text =
                "Obsessive Love Disorder (OLD) refers to an intense preoccupation " +
                "with another person that can become difficult to control. " +
                "It may involve persistent thoughts, a strong need for reassurance, " +
                "and difficulty managing emotions when separated from the person. " +
                "Understanding these patterns can help encourage healthier relationships " +
                "and emotional well-being.";

            desc.ForeColor = Color.White;
            desc.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            desc.AutoSize = false;
            desc.Size = new Size(260, 200);
            desc.TextAlign =ContentAlignment.MiddleCenter;
            desc.Location = new Point(20, 90);

            oldPanel.Controls.Add(title);
            oldPanel.Controls.Add(desc);

            menuPanel.Controls.Add(oldPanel);

            //=======================
            // order
            //=======================

            overlay.SendToBack();
            menuPanel.BringToFront();
            oldPanel.BringToFront();

            //=======================
            //back button
            //=======================

            BackButton btn = new BackButton(this, previousForm);
            btn.Location = new Point(
                (menuPanel.Width - btn.Width) / 2,
                menuPanel.Height - 55);

            menuPanel.Controls.Add(btn);

            //=======================
            // draw bg image
            //=======================

            
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackgroundImage == null)
            {
                base.OnPaintBackground(e);
                return;
            }

            Image img = BackgroundImage;

            float scaleX = (float)ClientSize.Width / img.Width;
            float scaleY = (float)ClientSize.Height / img.Height;
            float scale = Math.Max(scaleX, scaleY);

            int width = (int)(img.Width * scale);
            int height = (int)(img.Height * scale);
            int x = (ClientSize.Width - width) / 2;
            int y = (ClientSize.Height - height) / 2;

            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.DrawImage(img, new Rectangle(x, y, width, height));
        }
    }
}
