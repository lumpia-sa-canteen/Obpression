using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Text;

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

            // iocn

            this.Icon = new Icon(new MemoryStream(Properties.Resources.SoraVN));

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
            // Dropdowns (live on menuPanel,
            // stacked at the top of panel3)
            // ==========================================

            int pad = 15;
            int gap = 10;
            int boxWidth = panel3.Width - pad * 2;
            int boxX = panel3.Left + pad;
            int boxY = panel3.Top + pad;

            // Status: 1 - 3
            ComboBox statusBox = CreateDropdown(
                "Enter your status here",
                new string[] { "1", "2", "3" },
                boxX, boxY, boxWidth);

            // BDI: 1 - 3
            boxY += statusBox.Height + gap;
            ComboBox bdiBox = CreateDropdown(
                "BDI",
                new string[] { "1", "2", "3" },
                boxX, boxY, boxWidth);

            // Sleep hours: 0 - 24
            boxY += bdiBox.Height + gap;
            string[] hours = new string[25];
            for (int i = 0; i <= 24; i++)
                hours[i] = i.ToString();

            ComboBox sleepBox = CreateDropdown(
                "Sleep hours (0 - 24)",
                hours,
                boxX, boxY, boxWidth);

            // screen time

            boxY += sleepBox.Height + gap;
            ComboBox screenBox = CreateDropdown("Screen time hours (0- 24)", hours, boxX, boxY, boxWidth);

            // SelectedIndex: 0 = nothing chosen
            // status -> 1..3, bdi -> 1..3, sleep -> (index - 1) hours
            statusBox.SelectedIndexChanged += (s, e) =>
            {
                int status = statusBox.SelectedIndex;
                // do something with it
            };

            bdiBox.SelectedIndexChanged += (s, e) =>
            {
                int bdi = bdiBox.SelectedIndex;
                // do something with it
            };

            sleepBox.SelectedIndexChanged += (s, e) =>
            {
                if (sleepBox.SelectedIndex > 0)
                {
                    int sleepHours = sleepBox.SelectedIndex - 1;   // 0 - 24
                    // do something with it
                }
            };

            screenBox.SelectedIndexChanged += (s, e) =>
            {
                if (screenBox.SelectedIndex > 0)
                {
                    int screenTime = screenBox.SelectedIndex - 1; // 0 - 24
                    // do something with it
                }
            };

            menuPanel.Controls.Add(statusBox);
            menuPanel.Controls.Add(bdiBox);
            menuPanel.Controls.Add(sleepBox);
            menuPanel.Controls.Add(screenBox);

            // ok button

            Button ok = new Button();

            ok.Text = "OK";
            ok.FlatStyle = FlatStyle.Flat;
            ok.FlatAppearance.BorderColor = Color.White;
            ok.FlatAppearance.BorderSize = 1;
            ok.BackColor = Color.Transparent;
            ok.ForeColor = Color.White;
            ok.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            ok.Cursor = Cursors.Hand;
            ok.FlatAppearance.MouseOverBackColor = Color.Gray;
            ok.FlatAppearance.MouseDownBackColor = Color.DarkGray;

            ok.Size = new Size(boxWidth, 36);
            ok.Location = new Point(panel3.Left + pad, panel3.Bottom - ok.Height - pad);

            ok.Click += (s, e) =>
            {
                if (statusBox.SelectedIndex <= 0 ||
                bdiBox.SelectedIndex <= 0 ||
                sleepBox.SelectedIndex <= 0 ||
                screenBox.SelectedIndex <= 0)
                {
                    MessageBox.Show(
                        "Please Choose a value for status, BDI, sleep hours and screen time.",
                        "Obpression",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Warning);
                    return;
                }

                int status = statusBox.SelectedIndex; // 1- 3
                int bdi = bdiBox.SelectedIndex; // 1 - 3
                int sleepHours = sleepBox.SelectedIndex - 1; // 0 - 24
                int screenTime = screenBox.SelectedIndex - 1; // 0 - 24

                if (SaveToCsv(status, bdi, sleepHours, screenTime))
                {
                    MessageBox.Show(
                        "Saved!",
                        "Obpression",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // reset the dropdowms for the next entry

                    statusBox.SelectedIndex = 0;
                    bdiBox.SelectedIndex = 0;
                    sleepBox.SelectedIndex = 0;
                    screenBox.SelectedIndex = 0;
                }
            };

            menuPanel.Controls.Add(ok);


            // ==========================================
            // Z-Order
            // ==========================================

            overlay.SendToBack();

            menuPanel.BringToFront();

            panel1.BringToFront();
            panel2.BringToFront();
            panel3.BringToFront();

            // must be last so they draw on top of panel3
            statusBox.BringToFront();
            bdiBox.BringToFront();
            sleepBox.BringToFront();
            screenBox.BringToFront();
            ok.BringToFront();
        }

        // ==========================================
        // save to csv file
        // Saved in the user's AppData folder so it works on any
        // computer. ConsultForm reads the same file:
        // C:\Users\<name>\AppData\Local\Obpression\obpression_data.csv
        // ==========================================

        private bool SaveToCsv(int status, int bdi, int sleepHours, int screenTime)
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Obpression");

            string path = Path.Combine(folder, "obpression_data.csv");

            try
            {
                Directory.CreateDirectory(folder);

                if (!File.Exists(path))
                {
                    File.WriteAllText(
                        path, "timestamp, status, bdi, sleep_hours, screen_time" + Environment.NewLine,
                        Encoding.UTF8);
                }

                string line = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}, {1}, {2}, {3}, {4}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    status,
                    bdi,
                    sleepHours,
                    screenTime);

                File.AppendAllText(
                    path, line + Environment.NewLine,
                    Encoding.UTF8);

                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    "Could not write to the file. Is it open in Excel?\n\n" + ex.Message,
                    "Obpression",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    "No permission to write to:\n" + path + "\n\n" + ex.Message,
                    "Obpression",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        // ==========================================
        // Helper: builds a styled dropdown whose first
        // item is the placeholder text
        // ==========================================

        private ComboBox CreateDropdown(
            string placeholder,
            string[] items,
            int x,
            int y,
            int width)
        {
            ComboBox box = new ComboBox();

            box.DropDownStyle = ComboBoxStyle.DropDownList;
            box.FlatStyle = FlatStyle.Flat;
            box.BackColor = Color.FromArgb(30, 30, 30);
            box.ForeColor = Color.White;
            box.Font = new Font("Segoe UI", 10f);
            box.MaxDropDownItems = 8;

            box.Items.Add(placeholder);
            box.Items.AddRange(items);
            box.SelectedIndex = 0;

            box.Width = width;
            box.Location = new Point(x, y);

            return box;
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