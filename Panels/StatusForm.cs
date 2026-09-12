using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Obpression.Buttons;
using System.Xml.Serialization;


namespace Obpression
{
    public class StatusForm : Form
    {
        private Form previousForm;

        public StatusForm(Form previousForm)
        {
            this.previousForm = previousForm;

            //==========================================
            //window settings
            //==========================================

            Text = "Obpression";
            ClientSize = new Size(400, 300);
            MinimumSize = new Size(400, 300);
            MaximumSize = new Size(400, 300);
            MaximizeBox = false;
            MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;

            //==========================================
            // background image
            //==========================================

            BackgroundImage = Properties.Resources.bgpic;
            BackgroundImageLayout = ImageLayout.None;

            //==========================================
            // transparent overlay
            //==========================================

            TransparentPanel overlay = new TransparentPanel();
            overlay.Dock = DockStyle.Fill;

            Controls.Add(overlay);

            //==========================================
            // main menu panel
            //==========================================

            MenuPanel menu = new MenuPanel();
            menu.Size = new Size(340, 200);
            menu.Location = new Point(
                (ClientSize.Width - menu.Width) / 2,
                (ClientSize.Height - menu.Height) / 2
                );

            Controls.Add(menu);

            //==========================================
            // status form content
            //==========================================

            SolidPanel panel = new SolidPanel();
            panel.Size = new Size(300, 400);
            panel.Location = new Point(20, 50);
            panel.BackColor = Color.Transparent;

            //==========================================
            // normal btn
            //==========================================

            NormalButton normalBtn = new NormalButton();
            normalBtn.Location = new Point(10, 45);

            //==========================================
            // moderate btn
            //==========================================

            ModerateButton moderateBtn = new ModerateButton();
            moderateBtn.Location = new Point(120, 45);

            //==========================================
            //extreme btn
            //==========================================

            ExtremeButton extremeBtn = new ExtremeButton();
            extremeBtn.Location = new Point(230, 45);

            // action listeners

            normalBtn.Click += NormalButton_Click;
            moderateBtn.Click += ModerateButton_Click;
            extremeBtn.Click += ExtremeButton_Click;


            //==========================================
            // add buttton controls
            //==========================================

            menu.Controls.Add(normalBtn);
            menu.Controls.Add(moderateBtn);
            menu.Controls.Add(extremeBtn);

            //==========================================
            // order
            //==========================================

            overlay.SendToBack();
            menu.BringToFront();
            panel.BringToFront();

            //==========================================
            // back button
            //==========================================

            BackButton backBtn = new BackButton(this, previousForm);
            backBtn.Location = new Point(
                (menu.Width - backBtn.Width) / 2,
                (menu.Height - 55)
                );

            menu.Controls.Add(backBtn);


        }

        // action listeners

        private void NormalButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Thinking about someone often, enjoying their attention, " +
                "and wanting to spend time together can be normal.",
                "Normal Behaviors",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information 
                );
        }

        private void ModerateButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Frequently checking messages or social media, " +
                "may show some signs of jealousy and seeking reassurance " +
                "may indicate increasing preoccupation.",
                "Moderate Behaviors",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
                );
        }

        private void ExtremeButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Constantly thinking about the person on interest, " +
                "extreme cases of jealousy towards families or friends, " +
                "repeatedly contacting them despite their boundaries, " +
                "and is very wary of your actions towards other people " +
                "leading to controlling.",
                "Extreme Behaviors",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
                );
        }

        //==========================================
        // draw bg image as cover
        //==========================================

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackgroundImage == null)
            {
                base.OnPaintBackground(e);
                return;
            }

            Image img = BackgroundImage;
            float sclX = (float)ClientSize.Width / img.Width;
            float sclY = (float)ClientSize.Height / img.Height;
            float scl = Math.Max(sclX, sclY);

            int width = (int)(img.Width * scl);
            int height = (int)(img.Height * scl);
            int x = (ClientSize.Width - width) / 2;
            int y = (ClientSize.Height - height) / 2;

            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.DrawImage(img, new Rectangle(x, y, width, height));

        }
    }
}
