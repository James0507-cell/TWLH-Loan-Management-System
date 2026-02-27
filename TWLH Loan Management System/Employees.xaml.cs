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
    }
}