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

        private void LoadEmployeeData()
        {
            string query = "SELECT CONCAT(first_name, ' ', last_name) AS Name, role AS Role, email AS Email, is_active AS Status, created_at AS DateHired FROM tbl_employee";

            DataTable dt = db.displayRecords(query);
            if (dt != null)
            {
                EmployeeListView.ItemsSource = dt.DefaultView;
            }
        }

        public void LoadEmployees()
        {
            try
            {
                string query = @"SELECT *, 
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
                MessageBox.Show("Failed to load: " + ex.Message);
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
           
        }
    }
}