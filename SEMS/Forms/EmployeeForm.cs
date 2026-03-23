using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SEMS.Models;

namespace SEMS.Forms
{
    public partial class EmployeeForm : Form
    {
        private DataGridView dgv;
        private Panel inputPanel;

        private TextBox txtName, txtDept, txtSalary;
        private DateTimePicker dtJoining;
        private PictureBox pic;

        private Button btnAdd, btnUpdate, btnDelete, btnUpload, btnExport, btnClear;

        private TextBox txtSearch;
        private ComboBox cmbFilter;

        private List<Employee> employees;
        private int selectedId = -1;

        public EmployeeForm()
        {
            InitializeUI();
            employees = SEMS.Data.FileHandler.Load();
            RefreshGrid();
        }

        // ================= UI =================
        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);

            CreateInputPanel();
            CreateGrid();
            CreateTopBar();
        }

        // ================= INPUT PANEL =================
        private void CreateInputPanel()
        {
            inputPanel = new Panel();
            inputPanel.Size = new Size(300, 600);
            inputPanel.Dock = DockStyle.Left;
            inputPanel.BackColor = Color.FromArgb(40, 40, 40);

            Label title = new Label()
            {
                Text = "Employee Details",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20)
            };

            txtName = CreateInput("Full Name", 60);
            txtDept = CreateInput("Department", 110);
            txtSalary = CreateInput("Salary", 160);

            dtJoining = new DateTimePicker()
            {
                Location = new Point(20, 210),
                Width = 250
            };

            pic = new PictureBox()
            {
                Size = new Size(120, 120),
                Location = new Point(20, 260),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            btnUpload = CreateButton("Upload Image", 20, 390);
            btnUpload.Click += UploadImage;

            btnAdd = CreateButton("Add", 20, 440);
            btnUpdate = CreateButton("Update", 120, 440);
            btnDelete = CreateButton("Delete", 20, 490);
            btnClear = CreateButton("Clear", 120, 490);

            btnAdd.Click += AddEmployee;
            btnUpdate.Click += UpdateEmployee;
            btnDelete.Click += DeleteEmployee;
            btnClear.Click += (s, e) => ClearFields();

            inputPanel.Controls.Add(title);
            inputPanel.Controls.Add(txtName);
            inputPanel.Controls.Add(txtDept);
            inputPanel.Controls.Add(txtSalary);
            inputPanel.Controls.Add(dtJoining);
            inputPanel.Controls.Add(pic);
            inputPanel.Controls.Add(btnUpload);
            inputPanel.Controls.Add(btnAdd);
            inputPanel.Controls.Add(btnUpdate);
            inputPanel.Controls.Add(btnDelete);
            inputPanel.Controls.Add(btnClear);

            this.Controls.Add(inputPanel);
        }

        // ================= GRID =================
        private void CreateGrid()
        {
            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Name", "Name");
            dgv.Columns.Add("Department", "Department");
            dgv.Columns.Add("Salary", "Salary");

            dgv.CellClick += Dgv_CellClick;

            dgv.BackgroundColor = Color.FromArgb(30, 30, 30);
            dgv.EnableHeadersVisualStyles = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            dgv.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgv.DefaultCellStyle.ForeColor = Color.White;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(37, 37, 38);

            this.Controls.Add(dgv);
        }

        // ================= TOP BAR =================
        private void CreateTopBar()
        {
            Panel top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 50;
            top.BackColor = Color.FromArgb(45, 45, 48);

            txtSearch = new TextBox()
            {
                Width = 200,
                Location = new Point(20, 10)
            };

            txtSearch.TextChanged += (s, e) => RefreshGrid();

            cmbFilter = new ComboBox()
            {
                Location = new Point(250, 10),
                Width = 150
            };

            cmbFilter.Items.AddRange(new string[] { "All", "HR", "IT", "Finance" });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => RefreshGrid();

            btnExport = CreateButton("Export CSV", 420, 10);
            btnExport.Click += ExportToCSV;

            top.Controls.Add(txtSearch);
            top.Controls.Add(cmbFilter);
            top.Controls.Add(btnExport);

            this.Controls.Add(top);
        }

        // ================= PLACEHOLDER INPUT =================
        private TextBox CreateInput(string placeholder, int top)
        {
            TextBox txt = new TextBox();
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;
            txt.Location = new Point(20, top);
            txt.Width = 250;

            txt.GotFocus += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };

            txt.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                }
            };

            return txt;
        }

        // ================= BUTTON =================
        private Button CreateButton(string text, int left, int top)
        {
            return new Button()
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        // ================= EVENTS =================
        private void AddEmployee(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            Employee emp = new Employee()
            {
                Id = employees.Count + 1,
                Name = txtName.Text,
                Department = txtDept.Text,
                Salary = double.Parse(txtSalary.Text),
                JoiningDate = dtJoining.Value,
                ImagePath = "",
                IsDeleted = false
            };

            employees.Add(emp);
            SEMS.Data.FileHandler.Save(employees);

            RefreshGrid();
            ClearFields();
        }

        private void UpdateEmployee(object sender, EventArgs e)
        {
            var emp = employees.FirstOrDefault(x => x.Id == selectedId);
            if (emp == null) return;

            if (!ValidateInputs()) return;

            emp.Name = txtName.Text;
            emp.Department = txtDept.Text;
            emp.Salary = double.Parse(txtSalary.Text);
            emp.JoiningDate = dtJoining.Value;

            SEMS.Data.FileHandler.Save(employees);
            RefreshGrid();
        }

        private void DeleteEmployee(object sender, EventArgs e)
        {
            var emp = employees.FirstOrDefault(x => x.Id == selectedId);
            if (emp == null) return;

            emp.IsDeleted = true;

            SEMS.Data.FileHandler.Save(employees);
            RefreshGrid();
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            selectedId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[0].Value);

            var emp = employees.FirstOrDefault(x => x.Id == selectedId);
            if (emp == null) return;

            txtName.Text = emp.Name;
            txtDept.Text = emp.Department;
            txtSalary.Text = emp.Salary.ToString();
            dtJoining.Value = emp.JoiningDate;
        }

        private void UploadImage(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pic.Image = Image.FromFile(ofd.FileName);

                var emp = employees.FirstOrDefault(x => x.Id == selectedId);
                if (emp != null)
                {
                    emp.ImagePath = ofd.FileName;
                    SEMS.Data.FileHandler.Save(employees);
                }
            }
        }

        private void ExportToCSV(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (var sw = new System.IO.StreamWriter(sfd.FileName))
                {
                    sw.WriteLine("Id,Name,Department,Salary");

                    foreach (var emp in employees.Where(x => !x.IsDeleted))
                    {
                        sw.WriteLine($"{emp.Id},{emp.Name},{emp.Department},{emp.Salary}");
                    }
                }

                MessageBox.Show("Exported Successfully!");
            }
        }

        // ================= CORE =================
        private void RefreshGrid()
        {
            dgv.Rows.Clear();

            string search = txtSearch.Text.ToLower();
            string filter = cmbFilter.SelectedItem.ToString();

            foreach (var emp in employees)
            {
                if (!emp.IsDeleted)
                {
                    if ((filter == "All" || emp.Department == filter) &&
                        (string.IsNullOrEmpty(search) || emp.Name.ToLower().Contains(search)))
                    {
                        dgv.Rows.Add(emp.Id, emp.Name, emp.Department, emp.Salary);
                    }
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtDept.Text) ||
                string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                MessageBox.Show("All fields are required!");
                return false;
            }

            if (!double.TryParse(txtSalary.Text, out _))
            {
                MessageBox.Show("Invalid Salary!");
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            txtName.Text = "Full Name";
            txtDept.Text = "Department";
            txtSalary.Text = "Salary";
            txtName.ForeColor = txtDept.ForeColor = txtSalary.ForeColor = Color.Gray;

            dtJoining.Value = DateTime.Now;
            pic.Image = null;
            selectedId = -1;
        }
    }
}