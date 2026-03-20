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
        private TextBox txtName, txtDept, txtSalary;
        private DateTimePicker dtJoining;
        private PictureBox pic;
        private Button btnAdd, btnUpdate, btnDelete, btnUpload;
        private TextBox txtSearch;
        private ComboBox cmbFilter;

        private List<Employee> employees;
        private int selectedIndex = -1;

        public EmployeeForm()
        {
            InitializeUI();
            employees = SEMS.Data.FileHandler.Load();
            RefreshGrid();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;

            // ================= TABLE =================
            dgv = new DataGridView();
            dgv.Size = new Size(600, 300);
            dgv.Location = new Point(20, 20);
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;

            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Name", "Name");
            dgv.Columns.Add("Department", "Department");
            dgv.Columns.Add("Salary", "Salary");

            dgv.CellClick += Dgv_CellClick;

            // ================= INPUTS =================
            txtName = CreateTextBox("Name", 350);
            txtDept = CreateTextBox("Department", 390);
            txtSalary = CreateTextBox("Salary", 430);

            dtJoining = new DateTimePicker();
            dtJoining.Location = new Point(20, 470);
            dtJoining.Size = new Size(200, 25);

            // ================= IMAGE =================
            pic = new PictureBox();
            pic.Size = new Size(120, 120);
            pic.Location = new Point(650, 20);
            pic.BorderStyle = BorderStyle.FixedSingle;
            pic.SizeMode = PictureBoxSizeMode.StretchImage;

            btnUpload = new Button();
            btnUpload.Text = "Upload Image";
            btnUpload.Location = new Point(650, 150);
            btnUpload.Click += UploadImage;

            // ================= BUTTONS =================
            btnAdd = CreateButton("Add", 20, 520);
            btnUpdate = CreateButton("Update", 120, 520);
            btnDelete = CreateButton("Delete", 220, 520);

            btnAdd.Click += AddEmployee;
            btnUpdate.Click += UpdateEmployee;
            btnDelete.Click += DeleteEmployee;

            // ================= SEARCH BOX =================
            txtSearch = new TextBox();
            txtSearch.Text = "Search...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Location = new Point(250, 350);
            txtSearch.Width = 200;

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                string keyword = txtSearch.Text.ToLower();
                dgv.Rows.Clear();

                foreach (var emp in employees)
                {
                    if (!emp.IsDeleted && emp.Name.ToLower().Contains(keyword))
                    {
                        dgv.Rows.Add(emp.Id, emp.Name, emp.Department, emp.Salary);
                    }
                }
            };

            // ================= FILTER COMBOBOX =================
            cmbFilter = new ComboBox();
            cmbFilter.Location = new Point(470, 350);
            cmbFilter.Width = 150;

            cmbFilter.Items.Add("All");
            cmbFilter.Items.Add("HR");
            cmbFilter.Items.Add("IT");
            cmbFilter.Items.Add("Finance");

            cmbFilter.SelectedIndex = 0;

            cmbFilter.SelectedIndexChanged += (s, e) =>
            {
                string selected = cmbFilter.SelectedItem.ToString();
                dgv.Rows.Clear();

                foreach (var emp in employees)
                {
                    if (!emp.IsDeleted)
                    {
                        if (selected == "All" || emp.Department == selected)
                        {
                            dgv.Rows.Add(emp.Id, emp.Name, emp.Department, emp.Salary);
                        }
                    }
                }
            };

            // Add Controls
            this.Controls.Add(dgv);
            this.Controls.Add(txtName);
            this.Controls.Add(txtDept);
            this.Controls.Add(txtSalary);
            this.Controls.Add(dtJoining);
            this.Controls.Add(pic);
            this.Controls.Add(btnUpload);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnUpdate);
            this.Controls.Add(btnDelete);
            this.Controls.Add(txtSearch);
            this.Controls.Add(cmbFilter);
        }

        // ================= HELPERS =================
        private TextBox CreateTextBox(string placeholder, int top)
        {
            TextBox txt = new TextBox();
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;
            txt.Location = new Point(20, top);
            txt.Size = new Size(200, 25);

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

        private Button CreateButton(string text, int left, int top)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(left, top);
            btn.Size = new Size(80, 35);
            btn.BackColor = Color.FromArgb(0, 120, 215);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            return btn;
        }

        // ================= EVENTS =================
        private void AddEmployee(object sender, EventArgs e)
        {
            if (txtName.Text == "Name" || txtDept.Text == "Department" || txtSalary.Text == "Salary")
            {
                MessageBox.Show("Please fill all fields!");
                return;
            }

            if (!double.TryParse(txtSalary.Text, out double salary))
            {
                MessageBox.Show("Invalid Salary!");
                return;
            }

            Employee emp = new Employee()
            {
                Id = employees.Count + 1,
                Name = txtName.Text,
                Department = txtDept.Text,
                Salary = salary,
                JoiningDate = dtJoining.Value,
                ImagePath = "",   // ✅ initialize
                IsDeleted = false
            };

            employees.Add(emp);
            SEMS.Data.FileHandler.Save(employees);

            RefreshGrid();
            ClearFields();

            MessageBox.Show("Employee Added Successfully!");
        }

        private void UpdateEmployee(object sender, EventArgs e)
        {
            if (selectedIndex < 0) return;

            employees[selectedIndex].Name = txtName.Text;
            employees[selectedIndex].Department = txtDept.Text;
            employees[selectedIndex].Salary = double.Parse(txtSalary.Text);
            employees[selectedIndex].JoiningDate = dtJoining.Value;

            SEMS.Data.FileHandler.Save(employees);
            RefreshGrid();
            ClearFields();

            MessageBox.Show("Employee Updated Successfully!");
        }

        private void DeleteEmployee(object sender, EventArgs e)
        {
            if (selectedIndex < 0) return;

            employees[selectedIndex].IsDeleted = true;

            SEMS.Data.FileHandler.Save(employees);
            RefreshGrid();
            ClearFields();

            MessageBox.Show("Employee moved to Recycle Bin!");
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedIndex = e.RowIndex;

            if (selectedIndex >= 0 && selectedIndex < employees.Count)
            {
                txtName.Text = employees[selectedIndex].Name;
                txtDept.Text = employees[selectedIndex].Department;
                txtSalary.Text = employees[selectedIndex].Salary.ToString();
                dtJoining.Value = employees[selectedIndex].JoiningDate;

                // ✅ Load profile picture
                if (!string.IsNullOrEmpty(employees[selectedIndex].ImagePath)
                    && System.IO.File.Exists(employees[selectedIndex].ImagePath))
                {
                    pic.Image = Image.FromFile(employees[selectedIndex].ImagePath);
                }
                else
                {
                    pic.Image = null;
                }
            }
        }

        private void UploadImage(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images|*.jpg;*.png;*.jpeg;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pic.Image = Image.FromFile(ofd.FileName);

                // ✅ Save path to selected employee
                if (selectedIndex >= 0 && selectedIndex < employees.Count)
                {
                    employees[selectedIndex].ImagePath = ofd.FileName;
                    SEMS.Data.FileHandler.Save(employees);
                }
            }
        }

        private void RefreshGrid()
        {
            dgv.Rows.Clear();

            foreach (var emp in employees)
            {
                if (!emp.IsDeleted)
                {
                    dgv.Rows.Add(emp.Id, emp.Name, emp.Department, emp.Salary);
                }
            }

            // Reset picture when grid refreshes
            pic.Image = null;
        }

        private void ClearFields()
        {
            txtName.Text = "Name";
            txtDept.Text = "Department";
            txtSalary.Text = "Salary";
            dtJoining.Value = DateTime.Now;
            pic.Image = null;
            selectedIndex = -1;
        }
    }
}