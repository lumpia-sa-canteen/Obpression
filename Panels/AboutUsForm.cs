using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Obpression
{
    public class AboutUsForm : Form
    {
        private Form previousForm;
        public AboutUsForm(Form previousForm)
        {
            this.previousForm = previousForm;

            // ==========================================
            // Window settings
            // ==========================================

            Text = "Obpression";

            ClientSize = new Size(400, 700);

            MinimumSize = new Size(400, 700);
            MaximumSize = new Size(400, 700);

            StartPosition = FormStartPosition.CenterScreen;

            MaximizeBox = false;
            MinimizeBox = true;

            FormBorderStyle =
                FormBorderStyle.FixedSingle;


            // ==========================================
            // Background image
            // ==========================================

            BackgroundImage =
                Properties.Resources.bgpic;

            BackgroundImageLayout =
                ImageLayout.None;


            // ==========================================
            // Transparent dark overlay
            // ==========================================

            TransparentPanel overlay =
                new TransparentPanel();

            overlay.Dock =
                DockStyle.Fill;

            Controls.Add(overlay);


            // ==========================================
            // Main Menu Panel
            // ==========================================

            MenuPanel menuPanel =
                new MenuPanel();

            menuPanel.Size =
                new Size(340, 530);

            menuPanel.Location =
                new Point(
                    (ClientSize.Width - menuPanel.Width) / 2,
                    (ClientSize.Height - menuPanel.Height) / 2
                );

            Controls.Add(menuPanel);


            // ==========================================
            // About Us Content Panel
            // ==========================================

            SolidPanel aboutPanel =
                new SolidPanel();

            aboutPanel.Size =
                new Size(300, 400);

            aboutPanel.Location =
                new Point(
                    20,
                    50
                );

            aboutPanel.BackColor =
                Color.Transparent;


            // ==========================================
            // About Us Title
            // ==========================================

            Label title =
                new Label();

            title.Text =
                "ABOUT US";

            title.ForeColor =
                Color.White;

            title.Font =
                new Font(
                    "Segoe UI",
                    20F,
                    FontStyle.Bold
                );

            title.Size = new Size(aboutPanel.Width, 50);
            title.Location = new Point(0, 20);
            title.TextAlign = ContentAlignment.MiddleCenter;


            // ==========================================
            // About Us Description
            // ==========================================

            Label description =
                new Label();

            description.Text =

                "Obpression is an AI assistant designed to help individuals " +
                "better understand and navigate the challenges of relationships, " +
                "Obessive Love Disorder, and emotional well-being. " +
                "Our goal is to encourage healthier relationships, " +
                "self-awareness, and a better understanding of the emotions " +
                "that shape our connections with others.";

            description.ForeColor =
                Color.White;

            description.Font =
                new Font(
                    "Segoe UI",
                    8F,
                    FontStyle.Regular
                );

            description.AutoSize =
                false;

            description.Size =
                new Size(260, 200);

            description.TextAlign =
                ContentAlignment.MiddleCenter;

            description.Location =
                new Point(
                    20,
                    90
                );


            aboutPanel.Controls.Add(title);
            aboutPanel.Controls.Add(description);

            menuPanel.Controls.Add(aboutPanel);


            // ==========================================
            // Z-Order
            // ==========================================

            overlay.SendToBack();

            menuPanel.BringToFront();

            aboutPanel.BringToFront();

            //back button

            BackButton backbutton = new BackButton(this, previousForm);
            backbutton.Location = new Point(
                (menuPanel.Width - backbutton.Width) / 2,
                menuPanel.Height - 55);

            menuPanel.Controls.Add(backbutton);


        }


        // ==========================================
        // Draw background image as Cover
        // ==========================================

        protected override void OnPaintBackground(
            PaintEventArgs e)
        {
            if (BackgroundImage == null)
            {
                base.OnPaintBackground(e);
                return;
            }

            Image image =
                BackgroundImage;

            float scaleX =
                (float)ClientSize.Width /
                image.Width;

            float scaleY =
                (float)ClientSize.Height /
                image.Height;

            float scale =
                Math.Max(
                    scaleX,
                    scaleY
                );

            int width =
                (int)(image.Width * scale);

            int height =
                (int)(image.Height * scale);

            int x =
                (ClientSize.Width - width) / 2;

            int y =
                (ClientSize.Height - height) / 2;

            e.Graphics.InterpolationMode =
                InterpolationMode.HighQualityBicubic;

            e.Graphics.PixelOffsetMode =
                PixelOffsetMode.HighQuality;

            e.Graphics.SmoothingMode =
                SmoothingMode.HighQuality;

            e.Graphics.DrawImage(
                image,
                new Rectangle(
                    x,
                    y,
                    width,
                    height
                )
            );
        }
    }
}