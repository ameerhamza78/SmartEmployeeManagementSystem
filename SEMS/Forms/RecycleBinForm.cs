using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SEMS.Models;
using SEMS.Data;

namespace SEMS.Forms
{
    public class RecycleBinForm : Form
    {
        private DataGridView dgv;
        private Panel topPanel;

        private Button btnRestore, btnDeletePermanent, btnClearAll;

        private List<Employee> employees;
        private bool isDark;

        // 🎨 COLORS
        private Color darkBg = Color.FromArgb(30, 30, 30);
        private Color darkGrid = Color.FromArgb(37, 37, 38);
        private Color darkHeader = Color.FromArgb(45, 45, 48);
        private Color darkText = Color.White;

        private Color lightBg = Color.White;
        private Color lightGrid = Color.FromArgb(245, 245, 245);
        private Color lightHeader = Color.FromArgb(230, 230, 230);
        private Color lightText = Color.Black;

        public RecycleBinForm(bool isDarkMode)
        {
            isDark = isDarkMode;

            InitializeUI();
            ApplyTheme();

            employees = FileHandler.Load();
            LoadDeletedEmployees();
        }

        // ================= UI =================
        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;

            // ===== TOP PANEL =====
            topPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 60
            };

            btnRestore = CreateButton("Restore", 20, 10, Color.FromArgb(76, 175, 80));
            btnDeletePermanent = CreateButton("Delete Permanently", 200, 10, Color.FromArgb(244, 67, 54));
            btnClearAll = CreateButton("Clear All", 380, 10, Color.FromArgb(255, 152, 0));a

            btnRestore.Click += RestoreEmployee;
            btnDeletePermanent.Click += DeletePermanent;
            btnClearAll.Click += ClearAll;

            topPanel.Controls.Add(btnRestore);
            topPanel.Controls.Add(btnDeletePermanent);
            topPanel.Controls.Add(btnClearAll);

            // ===== GRID =====
            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Name", "Name");
            dgv.Columns.Add("Department", "Department");

            dgv.BorderStyle = BorderStyle.None;
            dgv.RowTemplate.Height = 30;

            this.Controls.Add(dgv);
            this.Controls.Add(topPanel);
        }

        // ================= THEME =================
        private void ApplyTheme()
        {
            if (isDark)
            {
                this.BackColor = darkBg;
                topPanel.BackColor = darkHeader;

                dgv.BackgroundColor = darkBg;
                dgv.EnableHeadersVisualStyles = false;

                dgv.ColumnHeadersDefaultCellStyle.BackColor = darkHeader;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = darkText;

                dgv.DefaultCellStyle.BackColor = darkBg;
                dgv.DefaultCellStyle.ForeColor = darkText;

                dgv.AlternatingRowsDefaultCellStyle.BackColor = darkGrid;
            }
            else
            {
                this.BackColor = lightBg;
                topPanel.BackColor = lightHeader;

                dgv.BackgroundColor = lightBg;
                dgv.EnableHeadersVisualStyles = false;

                dgv.ColumnHeadersDefaultCellStyle.BackColor = lightHeader;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = lightText;

                dgv.DefaultCellStyle.BackColor = lightBg;
                dgv.DefaultCellStyle.ForeColor = lightText;

                dgv.AlternatingRowsDefaultCellStyle.BackColor = lightGrid;
            }
        }

        // ================= BUTTON =================
        private Button CreateButton(string text, int left, int top, Color color)
        {
            return new Button()
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(160, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        // ================= LOAD =================
        private void LoadDeletedEmployees()
        {
            dgv.Rows.Clear();

            foreach (var emp in employees.Where(e => e.IsDeleted))
            {
                dgv.Rows.Add(emp.Id, emp.Name, emp.Department);
            }
        }

        // ================= ACTIONS =================
        private void RestoreEmployee(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select an employee first!");
                return;
            }

            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells[0].Value);

            var emp = employees.FirstOrDefault(x => x.Id == id);
            if (emp != null)
            {
                emp.IsDeleted = false;
                FileHandler.Save(employees);

                LoadDeletedEmployees();
                MessageBox.Show("Employee restored successfully!");
            }
        }

        private void DeletePermanent(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select an employee first!");
                return;
            }

            var confirm = MessageBox.Show("Delete permanently?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells[0].Value);

            employees.RemoveAll(x => x.Id == id);

            FileHandler.Save(employees);
            LoadDeletedEmployees();

            MessageBox.Show("Employee permanently deleted!");
        }

        private void ClearAll(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Clear entire recycle bin?", "Confirm", MessageBoxButtons.YesNo);

            if (confirm != DialogResult.Yes) return;

            employees.RemoveAll(x => x.IsDeleted);

            FileHandler.Save(employees);
            LoadDeletedEmployees();

            MessageBox.Show("Recycle bin cleared!");
        }
    }
}