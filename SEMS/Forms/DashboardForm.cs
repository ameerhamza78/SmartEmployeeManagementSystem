using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SEMS.Forms
{
    public class DashboardForm : Form
    {
        private Panel sidebar, header, mainPanel, footer;
        private Label lblTitle, lblTime, lblDate, lblWeather;
        private bool isDark = false;

        private Button btnDashboard, btnEmployees, btnRecycle;
        private Timer clockTimer, refreshTimer;

        // 🎨 LIGHT THEME
        private Color lightSidebar = Color.FromArgb(245, 245, 245);
        private Color lightHover = Color.FromArgb(224, 224, 224);
        private Color lightText = Color.FromArgb(51, 51, 51);

        private Color lightTotal = Color.FromArgb(77, 182, 172);
        private Color lightActive = Color.FromArgb(102, 187, 106);
        private Color lightDeleted = Color.FromArgb(239, 83, 80);

        // 🎨 DARK THEME
        private Color darkBg = Color.FromArgb(18, 18, 18);
        private Color darkCard = Color.FromArgb(30, 30, 30);
        private Color darkText = Color.FromArgb(224, 224, 224);
        private Color darkHover = Color.FromArgb(44, 44, 44);

        // 🎯 ACCENT
        private Color accent = Color.FromArgb(63, 81, 181);

        public DashboardForm(string role)
        {
            InitializeUI(role);
            ApplyTheme(); // ✅ FIX: Apply theme at startup
            StartClock();
            StartAutoRefresh();
        }

        private void InitializeUI(string role)
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "SEMS Dashboard";
            this.Font = new Font("Segoe UI", 10);

            // ===== SIDEBAR =====
            sidebar = new Panel
            {
                Width = 230,
                Dock = DockStyle.Left
            };

            btnDashboard = CreateSidebarButton("📊 Dashboard", 0);
            btnEmployees = CreateSidebarButton("👨‍💼 Employees", 60);
            btnRecycle = CreateSidebarButton("♻ Recycle Bin", 120);

            btnDashboard.Click += (s, e) => { SetActive(btnDashboard); LoadDashboard(); };
            btnEmployees.Click += (s, e) => { SetActive(btnEmployees); LoadForm(new EmployeeForm(isDark), "Employees"); };
            btnRecycle.Click += (s, e) => { SetActive(btnRecycle); LoadForm(new RecycleBinForm(isDark), "Recycle Bin"); };

            sidebar.Controls.AddRange(new Control[] { btnDashboard, btnEmployees, btnRecycle });

            // ===== HEADER =====
            header = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top
            };

            lblTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Button btnTheme = new Button
            {
                Text = "🌙",
                Size = new Size(40, 30),
                Location = new Point(1100, 20),
                FlatStyle = FlatStyle.Flat
            };
            btnTheme.FlatAppearance.BorderSize = 0;
            btnTheme.Click += ToggleTheme;

            header.Controls.Add(lblTitle);
            header.Controls.Add(btnTheme);

            // ===== MAIN =====
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            // ===== FOOTER =====
            footer = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom
            };

            lblTime = new Label { Location = new Point(20, 10), AutoSize = true };
            lblDate = new Label { Location = new Point(150, 10), AutoSize = true };
            lblWeather = new Label { Location = new Point(320, 10), AutoSize = true };

            footer.Controls.AddRange(new Control[] { lblTime, lblDate, lblWeather });

            this.Controls.Add(mainPanel);
            this.Controls.Add(header);
            this.Controls.Add(footer);
            this.Controls.Add(sidebar);

            SetActive(btnDashboard);
            LoadDashboard();
        }

        // ================= THEME APPLY =================
        private void ApplyTheme()
        {
            // Backgrounds
            this.BackColor = isDark ? darkBg : Color.White;
            mainPanel.BackColor = isDark ? darkBg : Color.WhiteSmoke;
            sidebar.BackColor = isDark ? darkCard : lightSidebar;
            footer.BackColor = isDark ? darkCard : Color.FromArgb(245, 245, 245);
            header.BackColor = isDark ? darkCard : Color.FromArgb(0, 120, 215);

            // Text
            lblTitle.ForeColor = isDark ? darkText : Color.White;
            lblTime.ForeColor = isDark ? darkText : Color.FromArgb(60, 60, 60);
            lblDate.ForeColor = isDark ? darkText : Color.FromArgb(60, 60, 60);
            lblWeather.ForeColor = isDark ? darkText : Color.FromArgb(60, 60, 60);

            // Sidebar buttons fix (IMPORTANT)
            foreach (Control c in sidebar.Controls)
            {
                if (c is Button btn)
                {
                    btn.ForeColor = isDark ? Color.Gainsboro : lightText;
                    btn.BackColor = sidebar.BackColor;
                }
            }

            SetActive(GetActiveButton() ?? btnDashboard);
        }

        // ================= SIDEBAR =================
        private Button CreateSidebarButton(string text, int top)
        {
            Button btn = new Button
            {
                Text = "   " + text,
                Width = 230,
                Height = 60,
                Location = new Point(0, top),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Emoji", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += (s, e) =>
            {
                if (btn != GetActiveButton())
                    btn.BackColor = isDark ? darkHover : lightHover;
            };

            btn.MouseLeave += (s, e) =>
            {
                if (btn != GetActiveButton())
                    btn.BackColor = sidebar.BackColor;
            };

            return btn;
        }

        private void SetActive(Button active)
        {
            foreach (Control c in sidebar.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = sidebar.BackColor;
                    btn.ForeColor = isDark ? Color.Gainsboro : lightText;
                    btn.Padding = new Padding(10, 0, 0, 0);
                }
            }

            active.BackColor = accent;
            active.ForeColor = Color.White;
            active.Padding = new Padding(20, 0, 0, 0);
        }

        private Button GetActiveButton()
        {
            foreach (Control c in sidebar.Controls)
                if (c is Button btn && btn.BackColor == accent)
                    return btn;

            return null;
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

            mainPanel.Controls.Add(CreateCard("👥 Total", total.ToString(), 100, 100, lightTotal));
            mainPanel.Controls.Add(CreateCard("✅ Active", active.ToString(), activePercent, 380, lightActive));
            mainPanel.Controls.Add(CreateCard("🗑 Deleted", deleted.ToString(), deletedPercent, 660, lightDeleted));
        }

        private Panel CreateCard(string title, string value, double percent, int left, Color color)
        {
            Panel card = new Panel
            {
                Size = new Size(260, 180),
                Location = new Point(left, 100),
                BackColor = isDark ? darkCard : color
            };

            ApplyRoundedCorners(card, 20);

            Color textColor = isDark ? darkText : Color.FromArgb(40, 40, 40);

            Label lblT = new Label
            {
                Text = title,
                ForeColor = textColor,
                Location = new Point(12, 12),
                AutoSize = true
            };

            Label lblV = new Label
            {
                Text = value,
                ForeColor = textColor,
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                Location = new Point(12, 45),
                AutoSize = true
            };

            Panel track = new Panel
            {
                Location = new Point(12, 125),
                Size = new Size(220, 10),
                BackColor = isDark ? Color.FromArgb(44, 44, 44) : Color.FromArgb(230, 230, 230)
            };

            Panel fill = new Panel
            {
                Size = new Size((int)(220 * percent / 100), 10),
                BackColor = color
            };

            track.Controls.Add(fill);

            card.Controls.Add(lblT);
            card.Controls.Add(lblV);
            card.Controls.Add(track);

            return card;
        }

        private void ApplyRoundedCorners(Control control, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            control.Region = new Region(path);
        }

        // ================= CLOCK =================
        private void StartClock()
        {
            clockTimer = new Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) =>
            {
                lblTime.Text = "🕒 " + DateTime.Now.ToString("hh:mm tt");
                lblDate.Text = "📅 " + DateTime.Now.ToString("dd/MM/yyyy");
                lblWeather.Text = DateTime.Now.Hour < 18 ? "☀ Sunny" : "🌙 Night";
            };
            clockTimer.Start();
        }

        private void StartAutoRefresh()
        {
            refreshTimer = new Timer { Interval = 5000 };
            refreshTimer.Tick += (s, e) =>
            {
                if (lblTitle.Text == "Dashboard")
                    LoadDashboard();
            };
            refreshTimer.Start();
        }

        // ================= THEME TOGGLE =================
        private void ToggleTheme(object sender, EventArgs e)
        {
            isDark = !isDark;
            ApplyTheme();
            LoadDashboard();
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
    }
}