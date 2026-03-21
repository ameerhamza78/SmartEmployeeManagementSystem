using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SEMS.Forms
{
    public class DashboardForm : Form
    {
        private Panel sidebar, header, mainPanel, footer;
        private Label lblTitle, lblTime, lblDate;
        private bool isDark = false;

        private Button btnDashboard, btnEmployees, btnRecycle;

        public DashboardForm(string role)
        {
            // Build the UI
            InitializeUI(role);

            // Start the clock (for time/date display)
            StartClock();

            // ✅ Add auto-refresh timer for dashboard stats
            Timer refreshTimer = new Timer();
            refreshTimer.Interval = 5000; // every 5 seconds
            refreshTimer.Tick += (s, e) =>
            {
                if (lblTitle.Text == "Dashboard")
                    LoadDashboard();
            };
            refreshTimer.Start();
        }

        private void InitializeUI(string role)
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "SEMS Dashboard";

            // ================= SIDEBAR =================
            sidebar = new Panel();
            sidebar.Width = 230;
            sidebar.Dock = DockStyle.Left;
            sidebar.BackColor = Color.FromArgb(30, 30, 30);

            btnDashboard = CreateSidebarButton("📊 Dashboard", 0);
            btnEmployees = CreateSidebarButton("👨‍💼 Employees", 60);
            btnRecycle = CreateSidebarButton("♻ Recycle Bin", 120);

            btnDashboard.Click += (s, e) => { Highlight(btnDashboard); LoadDashboard(); };
            btnEmployees.Click += (s, e) => { Highlight(btnEmployees); LoadForm(new EmployeeForm(), "Employees"); };
            btnRecycle.Click += (s, e) => { Highlight(btnRecycle); LoadForm(new RecycleBinForm(), "Recycle Bin"); };

            sidebar.Controls.Add(btnDashboard);
            sidebar.Controls.Add(btnEmployees);
            sidebar.Controls.Add(btnRecycle);

            // ================= HEADER =================
            header = new Panel();
            header.Height = 70;
            header.Dock = DockStyle.Top;
            header.BackColor = Color.FromArgb(0, 120, 215);

            lblTitle = new Label();
            lblTitle.Text = "Dashboard";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);

            // Profile Card
            Panel profile = new Panel();
            profile.Size = new Size(200, 50);
            profile.Location = new Point(850, 10);
            profile.BackColor = Color.FromArgb(0, 100, 200);

            Label lblUser = new Label();
            lblUser.Text = "👤 Admin";
            lblUser.ForeColor = Color.White;
            lblUser.Location = new Point(10, 15);

            profile.Controls.Add(lblUser);

            // Theme Button
            Button btnTheme = new Button();
            btnTheme.Text = "🌙";
            btnTheme.Size = new Size(40, 30);
            btnTheme.Location = new Point(1070, 20);
            btnTheme.Click += ToggleTheme;

            header.Controls.Add(lblTitle);
            header.Controls.Add(profile);
            header.Controls.Add(btnTheme);

            // ================= MAIN =================
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.WhiteSmoke;

            // ================= FOOTER =================
            footer = new Panel();
            footer.Height = 40;
            footer.Dock = DockStyle.Bottom;
            footer.BackColor = Color.White;

            lblTime = new Label();
            lblTime.Location = new Point(20, 10);

            lblDate = new Label();
            lblDate.Location = new Point(150, 10);

            Label lblWeather = new Label();
            lblWeather.Text = "🌤 24°C Partly Cloudy";
            lblWeather.Location = new Point(300, 10);

            footer.Controls.Add(lblTime);
            footer.Controls.Add(lblDate);
            footer.Controls.Add(lblWeather);

            // ADD CONTROLS
            this.Controls.Add(mainPanel);
            this.Controls.Add(header);
            this.Controls.Add(footer);
            this.Controls.Add(sidebar);

            Highlight(btnDashboard);
            LoadDashboard();
        }

        // ================= SIDEBAR BUTTON =================
        private Button CreateSidebarButton(string text, int top)
        {
            Button btn = new Button();
            btn.Text = "   " + text;
            btn.Width = 230;
            btn.Height = 60;
            btn.Location = new Point(0, top);

            btn.FlatStyle = FlatStyle.Flat;
            btn.ForeColor = Color.White;
            btn.BackColor = Color.FromArgb(30, 30, 30);
            btn.Font = new Font("Segoe UI Emoji", 11, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleLeft;

            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += (s, e) =>
            {
                if (btn.BackColor != Color.FromArgb(0, 120, 215))
                    btn.BackColor = Color.FromArgb(50, 50, 50);
            };

            btn.MouseLeave += (s, e) =>
            {
                if (btn.BackColor != Color.FromArgb(0, 120, 215))
                    btn.BackColor = Color.FromArgb(30, 30, 30);
            };

            return btn;
        }

        // ================= ACTIVE HIGHLIGHT =================
        private void Highlight(Button active)
        {
            foreach (Control c in sidebar.Controls)
            {
                if (c is Button btn)
                    btn.BackColor = Color.FromArgb(30, 30, 30);
            }

            active.BackColor = Color.FromArgb(0, 120, 215);
        }

        // ================= LOAD FORM =================
        private void LoadForm(Form form, string title)
        {
            lblTitle.Text = title;
            mainPanel.Controls.Clear();

            form.TopLevel = false;
            form.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(form);
            form.Show();
        }

        // ================= DASHBOARD =================
        private void LoadDashboard()
        {
            lblTitle.Text = "Dashboard";
            mainPanel.Controls.Clear();

            var employees = SEMS.Data.FileHandler.Load() ?? new List<SEMS.Models.Employee>();

            int total = employees.Count;
            int active = employees.FindAll(e => !e.IsDeleted).Count;
            int deleted = employees.FindAll(e => e.IsDeleted).Count;

            double activePercent = total == 0 ? 0 : (active * 100.0 / total);
            double deletedPercent = total == 0 ? 0 : (deleted * 100.0 / total);

            Panel card1 = CreateCard("👥 Total", total.ToString(), "100%", 100, Color.RoyalBlue, null);
            Panel card2 = CreateCard("✅ Active", active.ToString(), $"{activePercent:0}%", 350, Color.SeaGreen, () =>
            {
                LoadForm(new EmployeeForm(), "Employees");
            });

            Panel card3 = CreateCard("🗑 Deleted", deleted.ToString(), $"{deletedPercent:0}%", 600, Color.IndianRed, () =>
            {
                LoadForm(new RecycleBinForm(), "Recycle Bin");
            });

            mainPanel.Controls.Add(card1);
            mainPanel.Controls.Add(card2);
            mainPanel.Controls.Add(card3);
        }
        // ================= CARD =================
        private Panel CreateCard(string title, string value, string percent, int left, Color color, Action onClick)
        {
            Panel card = new Panel();
            card.Size = new Size(250, 170); // ✅ increased height
            card.Location = new Point(left, 100);
            card.BackColor = color;
            card.Cursor = Cursors.Hand;

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(15, 15);

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.ForeColor = Color.White;
            lblValue.Font = new Font("Segoe UI", 32, FontStyle.Bold); // ✅ bigger but safe
            lblValue.AutoSize = true; // ✅ prevents clipping
            lblValue.Location = new Point(15, 55);

            Label lblPercent = new Label();
            lblPercent.Text = percent;
            lblPercent.ForeColor = Color.WhiteSmoke;
            lblPercent.Font = new Font("Segoe UI", 10);
            lblPercent.AutoSize = true;
            lblPercent.Location = new Point(15, 120);

            // Hover effect
            card.MouseEnter += (s, e) => card.BackColor = ControlPaint.Light(color);
            card.MouseLeave += (s, e) => card.BackColor = color;

            // Click navigation
            if (onClick != null)
            {
                card.Click += (s, e) => onClick();
                lblTitle.Click += (s, e) => onClick();
                lblValue.Click += (s, e) => onClick();
                lblPercent.Click += (s, e) => onClick();
            }

            ToolTip tip = new ToolTip();
            tip.SetToolTip(card, $"{title} Employees");

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblPercent);

            return card;
        }
        // ================= CLOCK =================
        private void StartClock()
        {
            Timer timer = new Timer();
            timer.Interval = 1000;

            timer.Tick += (s, e) =>
            {
                lblTime.Text = DateTime.Now.ToString("hh:mm tt");
                lblDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            };

            timer.Start();
        }

        // ================= DARK MODE =================
        private void ToggleTheme(object sender, EventArgs e)
        {
            isDark = !isDark;

            if (isDark)
            {
                mainPanel.BackColor = Color.FromArgb(45, 45, 45);
                footer.BackColor = Color.FromArgb(30, 30, 30);
                header.BackColor = Color.Black;
            }
            else
            {
                mainPanel.BackColor = Color.WhiteSmoke;
                footer.BackColor = Color.White;
                header.BackColor = Color.FromArgb(0, 120, 215);
            }
        }
    }
}