using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace TWLH_Loan_Management_System
{
    public partial class editEmployee : Window
    {
        private string employeeId;

        public editEmployee(DataRowView row)
        {
            InitializeComponent();

            // Mapping from tbl_employee columns
            employeeId = row["employee_id"].ToString();
            TxtFirstName.Text = row["first_name"].ToString();
            TxtMiddleName.Text = row["middle_name"].ToString();
            TxtLastName.Text = row["last_name"].ToString();
            CboRole.Text = row["Role"].ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dbManager db = new dbManager();

                // Ensure there are no trailing commas and all fields are included
                string updateQuery = $"UPDATE tbl_employee SET " +
                     $"first_name = '{TxtFirstName.Text}', " +
                     $"middle_name = '{TxtMiddleName.Text}', " +
                     $"last_name = '{TxtLastName.Text}', " +
                     $"role = '{CboRole.Text}' " + // Ensure this isn't empty!
                     $"WHERE employee_id = '{employeeId}'";

                db.executeQuery(updateQuery);
                MessageBox.Show("Employee updated successfully!");

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Failed: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => this.Close();

        private void CbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}