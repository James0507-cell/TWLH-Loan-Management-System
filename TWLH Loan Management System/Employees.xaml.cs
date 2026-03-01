using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TWLH_Loan_Management_System
{
    public partial class Employees : Page 
    {
       
        dbManager db = new dbManager();

        public Employees()
        {
           
            InitializeComponent();
            LoadEmployees();

        }


        public void LoadEmployees()
        {
            try
            {
                // Add middle_name and any other columns needed by the Edit window
                string query = @"SELECT 
                            employee_id, 
                            first_name, 
                            middle_name, 
                            last_name,
                            gender,
                            contact_number,
                            role AS Role, 
                            email AS Email, 
                            created_at AS DateHired,
                            CONCAT(first_name, ' ', last_name) AS Name, 
                            IF(is_active = 1, 'Active', 'Inactive') AS Status 
                         FROM tbl_employee";

                DataTable dt = db.displayRecords(query);

                if (dt != null)
                {
                    EmployeeListView.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            addEmployee registrationWindow = new addEmployee();

            
            registrationWindow.Owner = System.Windows.Window.GetWindow(this);

            bool? result = registrationWindow.ShowDialog();

            
            if (result == true)
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
                editEmployee editWin = new editEmployee(row); // This calls your window
                editWin.Owner = Window.GetWindow(this);
                editWin.ShowDialog(); // This displays the window
            }
        }

        private void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected row data from the button's context
            Button btn = sender as Button;
            DataRowView row = btn.DataContext as DataRowView;

            if (row != null)
            {
                string id = row["employee_id"].ToString(); // Required from your SELECT query
                string name = row["Name"].ToString();

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to deactivate {name}? This will prevent them from logging in.",
                    "Confirm Deactivation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        dbManager db = new dbManager();

                        // 1. Update Employee Table for UI status
                        string deactivateEmployee = $"UPDATE tbl_employee SET is_active = 0 WHERE employee_id = '{id}'";

                        // 2. Update Credentials Table to block login
                        // Note: Ensure your credentials table has 'is_active' or similar status column
                        string deactivateLogin = $"UPDATE tbl_employee_credential SET is_active = 0 WHERE employee_id = '{id}'";

                        // Execute both (Assuming executeQuery returns true on success)
                        db.executeQuery(deactivateEmployee);
                        db.executeQuery(deactivateLogin);

                        MessageBox.Show($"{name} has been successfully deactivated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Refresh the table to show 'Inactive' status
                        LoadEmployees();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Deactivation failed: " + ex.Message);
                    }
                }
            }
        }
    }
}