using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TWLH_Loan_Management_System
{
    public partial class EmployeePage : Page
    {
        private DataTable allEmployees;

        public EmployeePage()
        {
            InitializeComponent();
            LoadEmployees();
            txtSearch.TextChanged += FilterChanged;
            cmbRoleFilter.SelectionChanged += FilterChanged;
            cmbStatusFilter.SelectionChanged += FilterChanged;
        }

        public void LoadEmployees()
        {
            try
            {
                allEmployees = Employee.GetAllEmployees();
                if (allEmployees != null)
                {
                    EmployeeDataGrid.ItemsSource = allEmployees.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            if (allEmployees == null) return;

            string searchText = txtSearch.Text.Trim().Replace("'", "''");
            string roleFilter = (cmbRoleFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            string statusFilter = (cmbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            List<string> filters = new List<string>();

            // Search filter
            if (!string.IsNullOrEmpty(searchText))
            {
                filters.Add($"(Name LIKE '%{searchText}%' OR Email LIKE '%{searchText}%' OR username LIKE '%{searchText}%')");
            }

            // Role filter
            if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All Roles")
            {
                filters.Add($"Role = '{roleFilter}'");
            }

            // Status filter
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All Status")
            {
                filters.Add($"Status = '{statusFilter}'");
            }

            if (filters.Count > 0)
            {
                allEmployees.DefaultView.RowFilter = string.Join(" AND ", filters);
            }
            else
            {
                allEmployees.DefaultView.RowFilter = "";
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) { } // Placeholder as we use FilterChanged

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EmployeeForm form = new EmployeeForm();
            form.Owner = Window.GetWindow(this);
            if (form.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataRowView row = btn.DataContext as DataRowView;

            if (row != null)
            {
                Employee emp = new Employee
                {
                    EmployeeId = Convert.ToInt32(row["employee_id"]),
                    FirstName = row["first_name"].ToString(),
                    MiddleName = row["middle_name"].ToString(),
                    LastName = row["last_name"].ToString(),
                    Gender = row["gender"].ToString(),
                    DateOfBirth = row["date_of_birth"] != DBNull.Value ? Convert.ToDateTime(row["date_of_birth"]) : DateTime.Now,
                    Role = row["Role"].ToString(),
                    ContactNumber = row["contact_number"].ToString(),
                    Email = row["Email"].ToString(),
                    Username = row["username"].ToString(),
                    IsEmployeeActive = Convert.ToInt32(row["IsEmployeeActive"]) == 1,
                    IsCredentialActive = Convert.ToInt32(row["IsCredentialActive"]) == 1
                };

                EmployeeForm form = new EmployeeForm(emp);
                form.Owner = Window.GetWindow(this);
                if (form.ShowDialog() == true)
                {
                    LoadEmployees();
                }
            }
        }

        private void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataRowView row = btn.DataContext as DataRowView;

            if (row != null)
            {
                int id = Convert.ToInt32(row["employee_id"]);
                string name = row["Name"].ToString();

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to deactivate {name}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (Employee.Deactivate(id, UserSession.EmployeeID))
                    {
                        MessageBox.Show("Employee record deactivated.");
                        LoadEmployees();
                    }
                }
            }
        }
    }
}
