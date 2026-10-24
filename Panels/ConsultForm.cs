using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using LLama;
using LLama.Common;

namespace Obpression
{
    public class ConsultForm : Form
    {
        private Form previousForm;

        // chat controls
        private RichTextBox chatLog;
        private TextBox chatInput;
        private Button sendBtn;
        private Button partnerBtn;
        private DateTimePicker datePicker;
        private Button adviseBtn;
        private ComboBox entryBox;
        private Button newSessionBtn;
        private string currentSessionKey;   // which check-in the current chat belongs to

        // AI state
        private readonly List<ChatTurn> history = new List<ChatTurn>();
        private List<CheckIn> checkIns = new List<CheckIn>();      // earlier entries (for comparison)
        private List<CheckIn> focusEntries = new List<CheckIn>();  // entries on the picked date

        public class ChatTurn
        {
            public string Role { get; set; }      // "user" or "assistant"
            public string Content { get; set; }
            public bool Hidden { get; set; }      // hidden = sent to the AI but not shown in the chat
        }

        private class CheckIn
        {
            public string Time;
            public DateTime When;
            public int Status;      // 1 = normal ... 3 = extreme
            public int Bdi;         // 1 - 3
            public int SleepHours;  // 0 - 24
            public int ScreenTime;  // 0 - 24
        }

        public ConsultForm(Form previousForm)
        {
            this.previousForm = previousForm;

            //==============================
            // window settings
            //==============================

            this.Icon = new Icon(new MemoryStream(Properties.Resources.SoraVN));

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
            chatInput.Location = new Point(500, 495);
            chatInput.Width = 450;
            chatInput.Font = new Font("Segoe UI", 11);
            chatInput.BackColor = Color.FromArgb(45, 45, 45);
            chatInput.ForeColor = Color.White;
            chatInput.BorderStyle = BorderStyle.FixedSingle;
            chatInput.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true; // stops the "ding" sound
                    SendMessage(chatInput.Text);
                }
            };

            sendBtn = new Button();
            sendBtn.Text = "Send";
            sendBtn.Location = new Point(960, 494);
            sendBtn.Size = new Size(110, chatInput.Height + 2);
            sendBtn.FlatStyle = FlatStyle.Flat;
            sendBtn.BackColor = Color.FromArgb(60, 60, 60);
            sendBtn.ForeColor = Color.White;
            sendBtn.Click += (s, e) => SendMessage(chatInput.Text);

            // shortcut: ask the AI to help write a message to the partner
            partnerBtn = new Button();
            partnerBtn.Text = "Help me talk to my partner";
            partnerBtn.Location = new Point(30, 545);
            partnerBtn.Size = new Size(230, 36);
            partnerBtn.FlatStyle = FlatStyle.Flat;
            partnerBtn.BackColor = Color.FromArgb(60, 60, 60);
            partnerBtn.ForeColor = Color.White;
            partnerBtn.Click += (s, e) => SendMessage(
                "Please help me prepare to talk to my partner about what I'm going through. " +
                "Give me a short, calm message I could say or send to them, and a few tips for the conversation.");

            // pick a date -> the AI reads that day's check-in and gives advice
            datePicker = new DateTimePicker();
            datePicker.Location = new Point(30, 495);
            datePicker.Width = 220;
            datePicker.Font = new Font("Segoe UI", 11);
            datePicker.Format = DateTimePickerFormat.Long;
            datePicker.MaxDate = DateTime.Today;

            adviseBtn = new Button();
            adviseBtn.Text = "Get advice for this date";
            adviseBtn.Location = new Point(260, 494);
            adviseBtn.Size = new Size(230, chatInput.Height + 2);
            adviseBtn.FlatStyle = FlatStyle.Flat;
            adviseBtn.BackColor = Color.FromArgb(60, 60, 60);
            adviseBtn.ForeColor = Color.White;
            adviseBtn.Click += (s, e) => AdviseForDate(false);

            // if several check-ins were saved on the same date, choose which one (default = newest)
            entryBox = new ComboBox();
            entryBox.DropDownStyle = ComboBoxStyle.DropDownList;
            entryBox.FlatStyle = FlatStyle.Flat;
            entryBox.BackColor = Color.FromArgb(45, 45, 45);
            entryBox.ForeColor = Color.White;
            entryBox.Font = new Font("Segoe UI", 10);
            entryBox.Location = new Point(280, 550);
            entryBox.Width = 210;

            datePicker.ValueChanged += (s, e) => RefreshEntryList();

            newSessionBtn = new Button();
            newSessionBtn.Text = "Start new advice";
            newSessionBtn.Location = new Point(880, 545);
            newSessionBtn.Size = new Size(190, 36);
            newSessionBtn.FlatStyle = FlatStyle.Flat;
            newSessionBtn.BackColor = Color.FromArgb(60, 60, 60);
            newSessionBtn.ForeColor = Color.White;
            newSessionBtn.Click += (s, e) =>
            {
                if (MessageBox.Show(
                        "Start new advice for this check-in? The saved conversation for it will be replaced.",
                        "Obpression",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    AdviseForDate(true);
                }
            };

            menu.Controls.Add(newSessionBtn);
            menu.Controls.Add(entryBox);
            menu.Controls.Add(datePicker);
            menu.Controls.Add(adviseBtn);
            menu.Controls.Add(chatLog);
            menu.Controls.Add(chatInput);
            menu.Controls.Add(sendBtn);
            menu.Controls.Add(partnerBtn);

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
            // start: load check-ins, AI opens the chat
            //==============================

            // default the picker to the newest saved check-in date
            List<CheckIn> all = LoadAllCheckIns();
            if (all.Count > 0 && all[all.Count - 1].When.Date <= DateTime.Today)
                datePicker.Value = all[all.Count - 1].When.Date;

            RefreshEntryList();

            // diagnostic: shows exactly which file is read and how many check-ins it contains
            AppendNote("Reading check-ins from: " + Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Obpression", "obpression_data.csv") + "  (found " + all.Count + ")");

            AppendNote("This is supportive guidance only. It is not a diagnosis or medical advice.");
            AppendNote("Pick a date and press \"Get advice for this date\". I will read that day's check-in and advise you.");

            Shown += (s, e) => datePicker.Focus();
        }

        //==============================
        // read the csv written by Form1
        //==============================

        private List<CheckIn> LoadAllCheckIns()
        {
            var result = new List<CheckIn>();
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Obpression");
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "obpression_data.csv");

            try
            {
                if (!File.Exists(path)) return result;

                string[] lines;
                // FileShare.ReadWrite so it still works if the file is open in Excel
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    lines = sr.ReadToEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                }

                for (int i = 1; i < lines.Length; i++) // line 0 is the header
                {
                    string[] p = lines[i].Split(',');
                    if (p.Length < 5) continue;

                    DateTime when;
                    int status, bdi, sleep, screen;
                    if (DateTime.TryParse(p[0].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out when) &&
                        int.TryParse(p[1].Trim(), out status) &&
                        int.TryParse(p[2].Trim(), out bdi) &&
                        int.TryParse(p[3].Trim(), out sleep) &&
                        int.TryParse(p[4].Trim(), out screen))
                    {
                        result.Add(new CheckIn
                        {
                            Time = p[0].Trim(),
                            When = when,
                            Status = status,
                            Bdi = bdi,
                            SleepHours = sleep,
                            ScreenTime = screen
                        });
                    }
                }
            }
            catch (IOException)
            {
                // file locked - continue without data
            }

            return result.OrderBy(c => c.When).ToList(); // oldest -> newest
        }

        //==============================
        // user picks a date -> AI advises
        //==============================

        // fills the "which check-in" dropdown with the times saved on the picked date
        private void RefreshEntryList()
        {
            DateTime day = datePicker.Value.Date;

            List<CheckIn> dayList = LoadAllCheckIns()
                .Where(c => c.When.Date == day)
                .OrderBy(c => c.When)
                .ToList();

            entryBox.Items.Clear();
            entryBox.Tag = null;

            if (dayList.Count == 0)
            {
                entryBox.Items.Add("No check-in this date");
                entryBox.SelectedIndex = 0;
                entryBox.Tag = "empty";
                entryBox.Enabled = false;
                return;
            }

            foreach (CheckIn c in dayList)
                entryBox.Items.Add(c.When.ToString("HH:mm:ss", CultureInfo.InvariantCulture));

            entryBox.SelectedIndex = entryBox.Items.Count - 1; // newest by default
            entryBox.Enabled = true;
        }

        //==============================
        // saved chat sessions (one per check-in)
        // C:\Users\<name>\AppData\Local\Obpression\chat_sessions.json
        //==============================

        private static string SessionsFile()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Obpression");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, "chat_sessions.json");
        }

        private Dictionary<string, List<ChatTurn>> ReadAllSessions()
        {
            try
            {
                string file = SessionsFile();
                if (File.Exists(file))
                {
                    var data = JsonSerializer.Deserialize<Dictionary<string, List<ChatTurn>>>(
                        File.ReadAllText(file, Encoding.UTF8));
                    if (data != null) return data;
                }
            }
            catch (Exception)
            {
                // unreadable file - start with an empty list
            }

            return new Dictionary<string, List<ChatTurn>>();
        }

        private void WriteAllSessions(Dictionary<string, List<ChatTurn>> sessions)
        {
            try
            {
                File.WriteAllText(
                    SessionsFile(),
                    JsonSerializer.Serialize(sessions, new JsonSerializerOptions { WriteIndented = true }),
                    Encoding.UTF8);
            }
            catch (Exception ex)
            {
                AppendNote("Could not save the conversation: " + ex.Message);
            }
        }

        private void SaveCurrentSession()
        {
            if (currentSessionKey == null) return;

            Dictionary<string, List<ChatTurn>> sessions = ReadAllSessions();
            sessions[currentSessionKey] = history
                .Select(h => new ChatTurn { Role = h.Role, Content = h.Content, Hidden = h.Hidden })
                .ToList();
            WriteAllSessions(sessions);
        }

        private void StartSessionView(string title)
        {
            chatLog.Clear();
            AppendNote("This is supportive guidance only. It is not a diagnosis or medical advice.");
            AppendNote(title);
        }

        private void ShowTranscript()
        {
            foreach (ChatTurn t in history)
            {
                if (t.Hidden) continue;

                if (t.Role == "user")
                    AppendMessage("You", t.Content, Color.LightSkyBlue);
                else
                    AppendMessage("SORA", t.Content, Color.LightGreen);
            }
        }

        private async void AdviseForDate(bool forceNew)
        {
            DateTime day = datePicker.Value.Date;
            string dayText = day.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);

            List<CheckIn> all = LoadAllCheckIns();

            List<CheckIn> dayList = all
                .Where(c => c.When.Date == day)
                .OrderBy(c => c.When)
                .ToList();

            if (dayList.Count == 0)
            {
                AppendNote("No check-in saved for " + dayText +
                           ". Pick another date, or save a check-in on the main screen first.");
                return;
            }

            // use ONLY the check-in the user selected (newest if nothing selected)
            CheckIn chosen = dayList.FirstOrDefault(c =>
                c.When.ToString("HH:mm:ss", CultureInfo.InvariantCulture) == entryBox.Text)
                ?? dayList[dayList.Count - 1];

            focusEntries = new List<CheckIn> { chosen };

            // everything before the chosen entry (including earlier ones the same day) is comparison only
            List<CheckIn> earlier = all.Where(c => c.When < chosen.When).ToList();
            checkIns = earlier.Skip(Math.Max(0, earlier.Count - 4)).ToList();

            currentSessionKey = chosen.Time;
            string title = dayText + " at " +
                           chosen.When.ToString("HH:mm", CultureInfo.InvariantCulture);

            Dictionary<string, List<ChatTurn>> sessions = ReadAllSessions();

            // a conversation already exists for this check-in -> restore it
            if (!forceNew && sessions.ContainsKey(chosen.Time) && sessions[chosen.Time].Count > 0)
            {
                history.Clear();
                history.AddRange(sessions[chosen.Time]);

                StartSessionView("Saved conversation for " + title);
                ShowTranscript();
                return;
            }

            // new advice (or "Start new advice"): drop the old saved chat for this check-in
            if (forceNew && sessions.Remove(chosen.Time))
                WriteAllSessions(sessions);

            history.Clear();
            StartSessionView("Advice for " + title);

            await AskAi("Please look at my check-in for " + dayText +
                        " and advise me about my relationship.", false);
        }

        //==============================
        // system prompt (the AI's rules)
        //==============================

        private string BuildSystemPrompt()
        {
            var sb = new StringBuilder();

            sb.AppendLine("You are a warm, calm support companion inside a wellbeing app called Obpression.");
            sb.AppendLine("The app helps people who may be struggling with obsessive love or obsessive attachment in a relationship, and helps couples build a healthier relationship.");
            sb.AppendLine();
            sb.AppendLine("YOUR GOAL:");
            sb.AppendLine("- The user only tells you a date. You take the initiative: read the check-in data for that date and give the advice yourself. Do not ask the user to re-enter their numbers.");
            sb.AppendLine("- In your first reply: (1) briefly say what the numbers suggest, (2) give 2 or 3 small things to try, (3) offer one short sentence they could say to their partner.");
            sb.AppendLine("- Help the user understand how their sleep, screen time, mood (BDI) and status may be affecting their feelings and the relationship.");
            sb.AppendLine("- Gently encourage the user to share what they are going through with their partner, and help them prepare for that conversation (calm timing, 'I feel...' statements, listening, agreeing on small boundaries together).");
            sb.AppendLine("- Suggest small, realistic habits: regular sleep, less late-night screen time, less checking of the partner's messages/location/social media, time with friends and hobbies, short breaks when urges to check or call feel strong.");
            sb.AppendLine("- If the user is the partner of someone with obsessive behavior, help them set kind, clear boundaries and start a calm conversation.");
            sb.AppendLine();
            sb.AppendLine("STRICT RULES:");
            sb.AppendLine("- NEVER recommend, name or discuss medications, supplements or dosages. If asked, say you cannot advise on that and suggest a doctor or licensed mental health professional.");
            sb.AppendLine("- NEVER diagnose. Say clearly that you are not a doctor and this is not a diagnosis. 'Obsessive love' is not an official clinical diagnosis, so describe patterns and feelings, not disorders.");
            sb.AppendLine("- Do not blame either partner, and never encourage controlling behavior (monitoring, tracking, ultimatums, threats).");
            sb.AppendLine("- If status is 3 or BDI is 3, or the user describes ongoing distress, gently recommend a licensed therapist or couples counselor.");
            sb.AppendLine("- If the user mentions self-harm, suicide, harming their partner, or being in danger, respond with care, encourage them to contact local emergency services or a crisis line right now and to reach out to someone they trust, and keep your reply short and calm. Do not give any methods.");
            sb.AppendLine("- If the user describes being abused or afraid of their partner, prioritize their safety and suggest a local support service.");
            sb.AppendLine();
            sb.AppendLine("STYLE:");
            sb.AppendLine("- Plain text only. No markdown, no asterisks, no headings, no bullet symbols other than a simple dash.");
            sb.AppendLine("- Keep replies under about 200 words, kind and non-judgmental.");
            sb.AppendLine("- Ask at most one question per reply.");
            sb.AppendLine();
            sb.AppendLine("Scales: status 1 = normal, 2 = elevated, 3 = extreme. BDI 1 = low, 3 = high. Sleep and screen time are hours per day.");
            sb.AppendLine();
            sb.AppendLine("THE ONE CHECK-IN THE USER CHOSE (base your advice on this entry only; do not mix it up with the others):");

            foreach (CheckIn c in focusEntries)
            {
                sb.AppendLine(string.Format(
                    "- {0}: status {1}, BDI {2}, sleep {3}h, screen time {4}h",
                    c.Time, c.Status, c.Bdi, c.SleepHours, c.ScreenTime));
            }

            if (checkIns.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("EARLIER CHECK-INS FOR COMPARISON (oldest to newest). Mention improvement or decline only if it is clear:");
                foreach (CheckIn c in checkIns)
                {
                    sb.AppendLine(string.Format(
                        "- {0}: status {1}, BDI {2}, sleep {3}h, screen time {4}h",
                        c.Time, c.Status, c.Bdi, c.SleepHours, c.ScreenTime));
                }
            }

            sb.AppendLine("Helpful context: very little sleep, or screen time above 8 hours, can make worry and the urge to check on a partner feel stronger.");

            return sb.ToString();
        }

        //==============================
        // chat logic
        //==============================

        private async void SendMessage(string text)
        {
            text = (text ?? "").Trim();
            if (text.Length == 0) return;

            chatInput.Clear();
            await AskAi(text, true);
        }

        private async Task AskAi(string userText, bool showUserText)
        {
            if (showUserText)
                AppendMessage("You", userText, Color.LightSkyBlue);

            history.Add(new ChatTurn { Role = "user", Content = userText, Hidden = !showUserText });

            SetBusy(true);

            try
            {
                string reply = await GetAiReply();
                history.Add(new ChatTurn { Role = "assistant", Content = reply });
                AppendMessage("SORA", reply, Color.LightGreen);
                SaveCurrentSession();   // keep the whole conversation for this check-in
            }
            catch (Exception ex)
            {
                // remove the failed user turn so the user can simply try again
                history.RemoveAt(history.Count - 1);
                AppendNote("Could not reach the AI: " + ex.Message);
            }

            SetBusy(false);
            chatInput.Focus();
        }

        private void SetBusy(bool busy)
        {
            sendBtn.Enabled = !busy;
            adviseBtn.Enabled = !busy;
            newSessionBtn.Enabled = !busy;
            datePicker.Enabled = !busy;
            entryBox.Enabled = !busy && entryBox.Items.Count > 0 && entryBox.Tag == null;
            partnerBtn.Enabled = !busy;
            chatInput.Enabled = !busy;
        }

        // ==========================================
        // Built-in AI (LLamaSharp). The model file ships with the app:
        //   <app folder>\Models\Llama-3.2-3B-Instruct-Q4_K_M.gguf
        // ==========================================

        private const string ModelFile = "Llama-3.2-3B-Instruct-Q4_K_M.gguf";
        private const string EndToken = "<|eot_id|>";

        private static LLamaWeights modelWeights;   // loaded once, reused
        private static ModelParams modelParams;

        private async Task EnsureModelLoaded()
        {
            if (modelWeights != null) return;

            string path = Path.Combine(Application.StartupPath, "Models", ModelFile);
            if (!File.Exists(path))
                throw new FileNotFoundException("AI model file not found: " + path);

            AppendNote("Loading the AI model (the first time takes a little while)...");

            modelParams = new ModelParams(path) { ContextSize = 4096 };
            modelWeights = await Task.Run(() => LLamaWeights.LoadFromFile(modelParams));
        }

        // Builds the prompt in the Llama 3 chat format: system + recent conversation
        private string BuildLlamaPrompt()
        {
            var sb = new StringBuilder();

            sb.Append("<|start_header_id|>system<|end_header_id|>\n\n");
            sb.Append(BuildSystemPrompt());
            sb.Append(EndToken);

            // keep only the latest turns so the prompt always fits the context size
            List<ChatTurn> recent = history.Skip(Math.Max(0, history.Count - 6)).ToList();
            while (recent.Count > 0 && recent[0].Role == "assistant")
                recent.RemoveAt(0);

            foreach (ChatTurn h in recent)
            {
                sb.Append("<|start_header_id|>" + h.Role + "<|end_header_id|>\n\n");
                sb.Append(h.Content);
                sb.Append(EndToken);
            }

            sb.Append("<|start_header_id|>assistant<|end_header_id|>\n\n");
            return sb.ToString();
        }

        private async Task<string> GetAiReply()
        {
            await EnsureModelLoaded();

            string prompt = BuildLlamaPrompt();

            // run on a background thread so the window never freezes
            return await Task.Run(async () =>
            {
                var executor = new StatelessExecutor(modelWeights, modelParams);

                var inferParams = new InferenceParams
                {
                    MaxTokens = 450,
                    AntiPrompts = new List<string> { EndToken }
                };

                var reply = new StringBuilder();
                await foreach (string piece in executor.InferAsync(prompt, inferParams))
                    reply.Append(piece);

                return reply.ToString().Replace(EndToken, "").Trim();
            });
        }

        //==============================
        // chat display
        //==============================

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

        private void AppendNote(string note)
        {
            chatLog.SelectionStart = chatLog.TextLength;
            chatLog.SelectionLength = 0;
            chatLog.SelectionColor = Color.Gray;
            chatLog.SelectionFont = new Font(chatLog.Font, FontStyle.Italic);
            chatLog.AppendText(note + "\n\n");

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