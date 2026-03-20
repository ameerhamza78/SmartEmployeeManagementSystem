using System;
using System.Drawing;
using System.Windows.Forms;

namespace SEMS.Forms
{
    public partial class DashboardForm : Form
    {
        private Panel sidebar, header, mainPanel;
        private Button btnDashboard, btnEmployees, btnRecycleBin, btnLogout;
        private Label lblTitle;
        private bool isDarkMode = false;   // ✅ Only defined once
        private string role;               // ✅ Role passed from LoginForm

        public DashboardForm(string role)
        {
            this.role = role;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Form Settings
            this.Text = "Dashboard - SEMS";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // ================= SIDEBAR =================
            sidebar = new Panel();
            sidebar.Size = new Size(200, this.Height);
            sidebar.BackColor = Color.FromArgb(30, 30, 30);
            sidebar.Dock = DockStyle.Left;

            // Sidebar Buttons
            btnDashboard = CreateSidebarButton("Dashboard", 50);
            btnEmployees = CreateSidebarButton("Employees", 100);
            btnRecycleBin = CreateSidebarButton("Recycle Bin", 150);
            btnLogout = CreateSidebarButton("Logout", 500);

            btnDashboard.Click += (s, e) => LoadDashboard();
            btnEmployees.Click += (s, e) => LoadEmployees();
            btnRecycleBin.Click += (s, e) => LoadRecycleBin();
            btnLogout.Click += (s, e) => Logout();

            sidebar.Controls.Add(btnDashboard);
            sidebar.Controls.Add(btnEmployees);
            sidebar.Controls.Add(btnRecycleBin);
            sidebar.Controls.Add(btnLogout);

            // Restrict access for User role
            if (role == "User")
            {
                btnEmployees.Enabled = false;
                btnRecycleBin.Enabled = false;
            }

            // ================= HEADER =================
            header = new Panel();
            header.Height = 60;
            header.Dock = DockStyle.Top;
            header.BackColor = Color.FromArgb(0, 120, 215);

            lblTitle = new Label();
            lblTitle.Text = "Dashboard";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            header.Controls.Add(lblTitle);

            // 🌙 Theme Toggle Button
            Button btnTheme = new Button();
            btnTheme.Text = "🌙";
            btnTheme.Size = new Size(50, 30);
            btnTheme.Location = new Point(700, 15);
            btnTheme.FlatStyle = FlatStyle.Flat;
            btnTheme.FlatAppearance.BorderSize = 0;
            btnTheme.BackColor = Color.White;
            btnTheme.ForeColor = Color.Black;
            btnTheme.Click += ToggleTheme;

            header.Controls.Add(btnTheme);

            // ================= MAIN PANEL =================
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.WhiteSmoke;

            // Add Panels to Form
            this.Controls.Add(mainPanel);
            this.Controls.Add(header);
            this.Controls.Add(sidebar);

            // Load default view
            LoadDashboard();
        }

        // Sidebar Button Creator
        private Button CreateSidebarButton(string text, int top)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(200, 40);
            btn.Location = new Point(0, top);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.White;
            btn.BackColor = Color.FromArgb(30, 30, 30);
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(50, 50, 50);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(30, 30, 30);

            return btn;
        }

        // ================= NAVIGATION METHODS =================
        private void LoadDashboard()
        {
            lblTitle.Text = "Dashboard";
            mainPanel.Controls.Clear();

            var employees = SEMS.Data.FileHandler.Load();

            int total = employees.Count;
            int active = employees.FindAll(e => !e.IsDeleted).Count;
            int deleted = employees.FindAll(e => e.IsDeleted).Count;

            // Create Cards
            Panel card1 = CreateCard("Total Employees", total.ToString(), 100);
            Panel card2 = CreateCard("Active Employees", active.ToString(), 300);
            Panel card3 = CreateCard("Deleted Employees", deleted.ToString(), 500);

            mainPanel.Controls.Add(card1);
            mainPanel.Controls.Add(card2);
            mainPanel.Controls.Add(card3);
        }

        private Panel CreateCard(string title, string value, int left)
        {
            Panel panel = new Panel();
            panel.Size = new Size(180, 120);
            panel.Location = new Point(left, 150);
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTitle.Location = new Point(10, 20);
            lblTitle.AutoSize = true;

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblValue.Location = new Point(10, 60);
            lblValue.AutoSize = true;

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblValue);

            return panel;
        }

        private void LoadEmployees()
        {
            lblTitle.Text = "Employee Management";
            mainPanel.Controls.Clear();

            EmployeeForm empForm = new EmployeeForm();
            empForm.TopLevel = false;
            empForm.FormBorderStyle = FormBorderStyle.None;
            empForm.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(empForm);
            empForm.Show();
        }

        private void LoadRecycleBin()
        {
            lblTitle.Text = "Recycle Bin";
            mainPanel.Controls.Clear();

            RecycleBinForm form = new RecycleBinForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(form);
            form.Show();
        }

        private void Logout()
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        // ================= THEME TOGGLE =================
        private void ToggleTheme(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;

            if (isDarkMode)
            {
                this.BackColor = Color.FromArgb(45, 45, 45);
                mainPanel.BackColor = Color.FromArgb(60, 60, 60);
                header.BackColor = Color.Black;
                sidebar.BackColor = Color.FromArgb(30, 30, 30);
            }
            else
            {
                this.BackColor = Color.White;
                mainPanel.BackColor = Color.WhiteSmoke;
                header.BackColor = Color.FromArgb(0, 120, 215);
                sidebar.BackColor = Color.FromArgb(30, 30, 30);
            }
        }
    }
}