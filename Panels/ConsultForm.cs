using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace Obpression
{
    public class ConsultForm : Form
    {
        private Form previousForm;

        // chat controls
        private RichTextBox chatLog;
        private TextBox chatInput;
        private Button sendBtn;

        public ConsultForm(Form previousForm)
        {
            this.previousForm = previousForm;

            //==============================
            // window settings
            //==============================

            Text = "Obpression";
            ClientSize = new Size(1280, 720);
            MinimumSize = new Size(1280, 720);
            MaximumSize = new Size(1280, 720);
            MaximizeBox = false;
            MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;

            //==============================
            // bg image
            //==============================

            BackgroundImage = Properties.Resources.bgpic;
            BackgroundImageLayout = ImageLayout.None;

            //==============================
            // transparent overlay
            //==============================

            TransparentPanel overlay = new TransparentPanel();
            overlay.Dock = DockStyle.Fill;

            Controls.Add(overlay);

            //==============================
            // main menu
            //==============================

            MenuPanel menu = new MenuPanel();
            menu.Size = new Size(1100, 600);
            menu.Location = new Point(
                (ClientSize.Width - menu.Width) / 2,
                (ClientSize.Height - menu.Height) / 2
                );

            Controls.Add(menu);

            //==============================
            // chat box (lives inside menu)
            //==============================

            chatLog = new RichTextBox();
            chatLog.Location = new Point(30, 30);
            chatLog.Size = new Size(1040, 450);
            chatLog.ReadOnly = true;
            chatLog.BorderStyle = BorderStyle.None;
            chatLog.BackColor = Color.FromArgb(30, 30, 30);
            chatLog.ForeColor = Color.White;
            chatLog.Font = new Font("Segoe UI", 11);
            chatLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            chatLog.TabStop = false;

            chatInput = new TextBox();
            chatInput.Location = new Point(30, 495);
            chatInput.Width = 920;
            chatInput.Font = new Font("Segoe UI", 11);
            chatInput.BackColor = Color.FromArgb(45, 45, 45);
            chatInput.ForeColor = Color.White;
            chatInput.BorderStyle = BorderStyle.FixedSingle;
            chatInput.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true; // stops the "ding" sound
                    SendMessage();
                }
            };

            sendBtn = new Button();
            sendBtn.Text = "Send";
            sendBtn.Location = new Point(960, 494);
            sendBtn.Size = new Size(110, chatInput.Height + 2);
            sendBtn.FlatStyle = FlatStyle.Flat;
            sendBtn.BackColor = Color.FromArgb(60, 60, 60);
            sendBtn.ForeColor = Color.White;
            sendBtn.Click += (s, e) => SendMessage();

            menu.Controls.Add(chatLog);
            menu.Controls.Add(chatInput);
            menu.Controls.Add(sendBtn);

            //==============================
            // order
            //==============================

            overlay.SendToBack();
            menu.BringToFront();

            //==============================
            // back button
            //==============================

            BackButton backBtn = new BackButton(this, previousForm);
            backBtn.Location = new Point(
                (menu.Width - backBtn.Width) / 2,
                (menu.Height - 55)
                );

            menu.Controls.Add(backBtn);

            //==============================
            // greeting + focus
            //==============================

            AppendMessage("SORA", "Hello! How can I help you today?", Color.LightGreen);
            Shown += (s, e) => chatInput.Focus();
        }

        //==============================
        // chat logic
        //==============================

        private async void SendMessage()
        {
            string text = chatInput.Text.Trim();
            if (text.Length == 0) return;

            chatInput.Clear();
            AppendMessage("You", text, Color.LightSkyBlue);

            sendBtn.Enabled = false;
            chatInput.Enabled = false;

            string reply;
            try
            {
                reply = await GetAiReply(text);
            }
            catch (Exception ex)
            {
                reply = "Error: " + ex.Message;
            }

            sendBtn.Enabled = true;
            chatInput.Enabled = true;

            AppendMessage("SORA", reply, Color.LightGreen);
            chatInput.Focus();
        }

        // Replace the body of this with your real AI call (e.g. HttpClient)
        private async Task<string> GetAiReply(string userText)
        {
            await Task.Delay(500); // fake delay
            return "You said: " + userText;
        }

        private void AppendMessage(string sender, string message, Color senderColor)
        {
            chatLog.SelectionStart = chatLog.TextLength;
            chatLog.SelectionLength = 0;
            chatLog.SelectionColor = senderColor;
            chatLog.SelectionFont = new Font(chatLog.Font, FontStyle.Bold);
            chatLog.AppendText(sender + ": ");

            chatLog.SelectionColor = Color.White;
            chatLog.SelectionFont = chatLog.Font;
            chatLog.AppendText(message + "\n\n");

            chatLog.SelectionStart = chatLog.TextLength;
            chatLog.ScrollToCaret();
        }

        //==============================
        // draw bg image as cover
        //==============================

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