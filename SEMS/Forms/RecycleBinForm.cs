using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SEMS.Models;
using SEMS.Data;

namespace SEMS.Forms
{
    public class RecycleBinForm : Form
    {
        private DataGridView dgv;
        private Button btnRestore;

        private List<Employee> employees;

        public RecycleBinForm()
        {
            InitializeUI();
            employees = FileHandler.Load();
            LoadDeletedEmployees();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;

            // DataGridView
            dgv = new DataGridView();
            dgv.Dock = DockStyle.Top;
            dgv.Height = 300;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Name", "Name");
            dgv.Columns.Add("Department", "Department");

            // Button
            btnRestore = new Button();
            btnRestore.Text = "Restore Selected";
            btnRestore.Dock = DockStyle.Bottom;
            btnRestore.Height = 40;
            btnRestore.BackColor = Color.Green;
            btnRestore.ForeColor = Color.White;

            btnRestore.Click += RestoreEmployee;

            // Add Controls
            this.Controls.Add(dgv);
            this.Controls.Add(btnRestore);
        }

        private void LoadDeletedEmployees()
        {
            dgv.Rows.Clear();

            foreach (var emp in employees)
            {
                if (emp.IsDeleted)
                {
                    dgv.Rows.Add(emp.Id, emp.Name, emp.Department);
                }
            }
        }

        private void RestoreEmployee(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee!");
                return;
            }

            int id = int.Parse(dgv.SelectedRows[0].Cells[0].Value.ToString());

            foreach (var emp in employees)
            {
                if (emp.Id == id)
                {
                    emp.IsDeleted = false;
                    break;
                }
            }

            FileHandler.Save(employees);
            LoadDeletedEmployees();

            MessageBox.Show("Employee Restored Successfully!");
        }
    }
}