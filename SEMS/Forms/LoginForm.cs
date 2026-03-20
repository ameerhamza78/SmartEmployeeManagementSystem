using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEMS.Forms
{
    public partial class LoginForm : Form
    {
        private Label lblTitle, lblUsername, lblPassword;
        private TextBox txtUsername, txtPassword;
        private Button btnLogin;
        private List<SEMS.Models.User> users = new List<SEMS.Models.User>()
{
    new SEMS.Models.User { Username = "admin", Password = "1234", Role = "Admin" },
    new SEMS.Models.User { Username = "user", Password = "1234", Role = "User" }
};

        public LoginForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Form Settings
            this.Text = "Login - SEMS";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Title
            lblTitle = new Label();
            lblTitle.Text = "Smart EMS Login";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(100, 30);

            // Username Label
            lblUsername = new Label();
            lblUsername.Text = "Username";
            lblUsername.Location = new Point(50, 100);
            lblUsername.Font = new Font("Segoe UI", 10);

            // Username TextBox
            txtUsername = new TextBox();
            txtUsername.Location = new Point(50, 125);
            txtUsername.Size = new Size(280, 25);

            // Password Label
            lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Location = new Point(50, 160);
            lblPassword.Font = new Font("Segoe UI", 10);

            // Password TextBox
            txtPassword = new TextBox();
            txtPassword.Location = new Point(50, 185);
            txtPassword.Size = new Size(280, 25);
            txtPassword.PasswordChar = '*';

            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "Login";
            btnLogin.Size = new Size(280, 35);
            btnLogin.Location = new Point(50, 230);
            btnLogin.BackColor = Color.FromArgb(0, 120, 215);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            btnLogin.Click += BtnLogin_Click;

            // Add Controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            foreach (var user in users)
            {
                if (txtUsername.Text == user.Username && txtPassword.Text == user.Password)
                {
                    MessageBox.Show("Login Successful!");

                    DashboardForm dashboard = new DashboardForm(user.Role);
                    this.Hide();
                    dashboard.Show();

                    return;
                }
            }

            MessageBox.Show("Invalid Credentials!");
        }
    }
}
