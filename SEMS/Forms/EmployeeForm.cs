using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SEMS.Models;

namespace SEMS.Forms
{
    public class EmployeeForm : Form
    {
        private DataGridView dgv;
        private TextBox txtName, txtDept, txtSalary, txtSearch;
        private ComboBox cmbFilter;
        private DateTimePicker dtJoining;
        private PictureBox pic;

        private Panel formPanel;

        private Button btnAddNew, btnSave, btnCancel, btnDelete, btnExport, btnUpload;

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
            this.BackColor = Color.WhiteSmoke;

            // ================= TOP PANEL =================
            Panel topPanel = new Panel();
            topPanel.Height = 60;
            topPanel.Dock = DockStyle.Top;
            topPanel.BackColor = Color.White;

            btnAddNew = CreateButton("➕ Add", 20, 10);
            btnAddNew.Click += (s, e) =>
            {
                selectedIndex = -1;
                ClearFields();
                formPanel.Visible = true;
            };

            btnDelete = CreateButton("🗑 Delete", 120, 10);
            btnDelete.Click += DeleteEmployee;

            btnExport = CreateButton("📄 Export", 220, 10);
            btnExport.Click += ExportToCsv;

            txtSearch = new TextBox();
            txtSearch.Text = "Search...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Location = new Point(350, 15);
            txtSearch.Width = 200;

            txtSearch.TextChanged += (s, e) => ApplyFilters();

            cmbFilter = new ComboBox();
            cmbFilter.Location = new Point(570, 15);
            cmbFilter.Width = 120;

            cmbFilter.Items.AddRange(new string[] { "All", "HR", "IT", "Finance" });
            cmbFilter.SelectedIndex = 0;

            cmbFilter.SelectedIndexChanged += (s, e) => ApplyFilters();

            topPanel.Controls.Add(btnAddNew);
            topPanel.Controls.Add(btnDelete);
            topPanel.Controls.Add(btnExport);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(cmbFilter);

            // ================= TABLE =================
            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = true;

            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Name", "Name");
            dgv.Columns.Add("Department", "Department");
            dgv.Columns.Add("Salary", "Salary");

            dgv.CellDoubleClick += EditEmployee;

            // ================= FORM PANEL =================
            formPanel = new Panel();
            formPanel.Width = 300;
            formPanel.Dock = DockStyle.Right;
            formPanel.BackColor = Color.White;
            formPanel.Visible = false;

            txtName = CreateTextBox("Name", 30);
            txtDept = CreateTextBox("Department", 80);
            txtSalary = CreateTextBox("Salary", 130);

            dtJoining = new DateTimePicker();
            dtJoining.Location = new Point(20, 180);

            pic = new PictureBox();
            pic.Size = new Size(120, 120);
            pic.Location = new Point(80, 220);
            pic.BorderStyle = BorderStyle.FixedSingle;
            pic.SizeMode = PictureBoxSizeMode.StretchImage;

            btnUpload = CreateButton("Upload", 90, 350);
            btnUpload.Click += UploadImage;

            btnSave = CreateButton("Save", 40, 400);
            btnSave.Click += SaveEmployee;

            btnCancel = CreateButton("Cancel", 150, 400);
            btnCancel.Click += (s, e) => formPanel.Visible = false;

            formPanel.Controls.Add(txtName);
            formPanel.Controls.Add(txtDept);
            formPanel.Controls.Add(txtSalary);
            formPanel.Controls.Add(dtJoining);
            formPanel.Controls.Add(pic);
            formPanel.Controls.Add(btnUpload);
            formPanel.Controls.Add(btnSave);
            formPanel.Controls.Add(btnCancel);

            // ================= ADD CONTROLS =================
            this.Controls.Add(dgv);
            this.Controls.Add(formPanel);
            this.Controls.Add(topPanel);
        }

        // ================= FILTER =================
        private void ApplyFilters()
        {
            string keyword = txtSearch.Text.ToLower();
            string dept = cmbFilter.SelectedItem.ToString();

            dgv.Rows.Clear();

            foreach (var emp in employees)
            {
                if (!emp.IsDeleted &&
                    emp.Name.ToLower().Contains(keyword) &&
                    (dept == "All" || emp.Department == dept))
                {
                    dgv.Rows.Add(emp.Id, emp.Name, emp.Department, emp.Salary);
                }
            }
        }

        // ================= SAVE =================
        private void SaveEmployee(object sender, EventArgs e)
        {
            if (!double.TryParse(txtSalary.Text, out double salary))
            {
                MessageBox.Show("Invalid Salary!");
                return;
            }

            if (selectedIndex == -1)
            {
                employees.Add(new Employee
                {
                    Id = employees.Count + 1,
                    Name = txtName.Text,
                    Department = txtDept.Text,
                    Salary = salary,
                    JoiningDate = dtJoining.Value
                });
            }
            else
            {
                employees[selectedIndex].Name = txtName.Text;
                employees[selectedIndex].Department = txtDept.Text;
                employees[selectedIndex].Salary = salary;
                employees[selectedIndex].JoiningDate = dtJoining.Value;
            }

            SEMS.Data.FileHandler.Save(employees);

            RefreshGrid();
            formPanel.Visible = false;
            ClearFields();

            MessageBox.Show("Saved Successfully!");
        }

        // ================= EDIT =================
        private void EditEmployee(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = int.Parse(dgv.Rows[e.RowIndex].Cells[0].Value.ToString());

            for (int i = 0; i < employees.Count; i++)
            {
                if (employees[i].Id == id)
                {
                    selectedIndex = i;

                    txtName.Text = employees[i].Name;
                    txtDept.Text = employees[i].Department;
                    txtSalary.Text = employees[i].Salary.ToString();
                    dtJoining.Value = employees[i].JoiningDate;

                    if (!string.IsNullOrEmpty(employees[i].ImagePath))
                        pic.Image = Image.FromFile(employees[i].ImagePath);

                    break;
                }
            }

            formPanel.Visible = true;
        }

        // ================= DELETE =================
        private void DeleteEmployee(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int id = int.Parse(dgv.SelectedRows[0].Cells[0].Value.ToString());

            foreach (var emp in employees)
            {
                if (emp.Id == id)
                {
                    emp.IsDeleted = true;
                    break;
                }
            }

            SEMS.Data.FileHandler.Save(employees);
            RefreshGrid();

            MessageBox.Show("Moved to Recycle Bin!");
        }

        // ================= IMAGE =================
        private void UploadImage(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images|*.jpg;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pic.Image = Image.FromFile(ofd.FileName);

                if (selectedIndex >= 0)
                {
                    employees[selectedIndex].ImagePath = ofd.FileName;
                }
            }
        }

        // ================= GRID =================
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
        }

        // ================= EXPORT =================
        private void ExportToCsv(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (*.csv)|*.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                List<string> lines = new List<string>();
                lines.Add("ID,Name,Department,Salary");

                foreach (var emp in employees)
                {
                    if (!emp.IsDeleted)
                        lines.Add($"{emp.Id},{emp.Name},{emp.Department},{emp.Salary}");
                }

                System.IO.File.WriteAllLines(sfd.FileName, lines);

                MessageBox.Show("Exported Successfully!");
            }
        }

        // ================= HELPERS =================
        private TextBox CreateTextBox(string text, int top)
        {
            TextBox txt = new TextBox();
            txt.Text = text;
            txt.Location = new Point(20, top);
            txt.Width = 240;
            return txt;
        }

        private Button CreateButton(string text, int left, int top)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(left, top);
            btn.Size = new Size(90, 35);
            btn.BackColor = Color.FromArgb(0, 120, 215);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            return btn;
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtDept.Text = "";
            txtSalary.Text = "";
            dtJoining.Value = DateTime.Now;
            pic.Image = null;
            selectedIndex = -1;
        }
    }
}