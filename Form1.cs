using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Obpression
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            

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
            // Main menu
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
            // Panel 1
            // ==========================================

            SolidPanel panel1 =
                new SolidPanel();

            panel1.Size =
                new Size(140, 140);

            panel1.Location =
                new Point(
                    20,
                    30
                );

            panel1.BackColor =
                Color.Transparent;

            AboutUs aboutUsButton = new AboutUs();
            aboutUsButton.Size = new Size(110, 40);
            aboutUsButton.Location = new Point(15, 15);

            aboutUsButton.Click += AboutUsButton_Click;

            OldBtn oldBtn = new OldBtn();
            oldBtn.Size = new Size(110, 40);
            oldBtn.Location = new Point(15, 85);

            oldBtn.Click += OldBtn_Click;

            panel1.Controls.Add(aboutUsButton);
            panel1.Controls.Add(oldBtn);

            menuPanel.Controls.Add(panel1);


            // ==========================================
            // Panel 2
            // ==========================================

            SolidPanel panel2 =
                new SolidPanel();

            panel2.Size =
                new Size(140, 140);

            panel2.Location =
                new Point(
                    180,
                    30
                );

            panel2.BackColor =
                Color.Transparent;

            RelationshipGoals relBtn = new RelationshipGoals();
            relBtn.Size = new Size(110, 40);
            relBtn.Location = new Point(15, 15);

            relBtn.Click += RelationshipGoals_Click;

            ConsultWithAi consBtn = new ConsultWithAi();
            consBtn.Size = new Size(110, 40);
            consBtn.Location = new Point(15, 85);

            consBtn.Click += ConsultWithAi_Click;

            panel2.Controls.Add(consBtn);
            panel2.Controls.Add(relBtn);

            menuPanel.Controls.Add(panel2);


            // ==========================================
            // Panel 3
            // ==========================================

            SolidPanel panel3 =
                new SolidPanel();

            panel3.Size =
                new Size(300, 280);

            panel3.Location =
                new Point(
                    20,
                    200
                );

            panel3.BackColor =
                Color.Transparent;

            menuPanel.Controls.Add(panel3);


            // ==========================================
            // Z-Order
            // ==========================================

            overlay.SendToBack();

            menuPanel.BringToFront();

            panel1.BringToFront();
            panel2.BringToFront();
            panel3.BringToFront();
        }

        //action listener
        private void AboutUsButton_Click(object sender, EventArgs e)
        {
            AboutUsForm aboutUsForm = new AboutUsForm(this);
            aboutUsForm.FormClosed += (s, args) =>
            {
                this.Show();
            };

            this.Hide();
            aboutUsForm.Show(); 
        }

        // action listener

        private void OldBtn_Click(object sender, EventArgs e)
        {
            OldForm oldForm = new OldForm(this);
            oldForm.FormClosed += (s, args) =>
            {
                this.Show();
            };


            this.Hide();
            oldForm.Show();
        }

        // action listener

        private void RelationshipGoals_Click(object sender, EventArgs e)
        {
            StatusForm statusForm = new StatusForm(this);
            statusForm.FormClosed += (s, args) =>
            {
                this.Show();
            };

            this.Hide();
            statusForm.Show();
        }

        // action listener

        private void ConsultWithAi_Click(object sender, EventArgs e)
        {
            ConsultForm consForm = new ConsultForm(this);
            consForm.FormClosed += (s, args) =>
            {
                this.Show();
            };

            this.Hide();
            consForm.Show();
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
