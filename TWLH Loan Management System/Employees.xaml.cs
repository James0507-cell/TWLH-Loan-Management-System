using System.Data;
using System.Windows.Controls;

namespace TWLH_Loan_Management_System
{
    public partial class Employees : Page 
    {
       
        dbManager db = new dbManager();

        public Employees()
        {
           
            InitializeComponent();
            LoadEmployeeData();
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
                dbManager db = new dbManager();

                // Use 'AS' to match the column names expected by your XAML Bindings
                string query = @"SELECT 
                            first_name AS Employee, 
                            role AS Role, 
                            email AS Email, 
                            IF(is_active = 1, 'Active', 'Inactive') AS Status, 
                            created_at AS DateHired 
                         FROM tbl_employee";

                DataTable dt = db.displayRecords(query);
                EmployeeListView.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to load employees: " + ex.Message);
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
    }
}