using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for addEmployee.xaml
    /// </summary>
    public partial class addEmployee : Window
    {
        public addEmployee()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fn = txtFirstName.Text;
                string mn = txtMiddleName.Text;
                string ln = txtLastName.Text;
                string gender = (cmbGender.SelectedItem as ComboBoxItem)?.Content.ToString();
                string dob = dpBirthDate.SelectedDate?.ToString("yyyy-MM-dd");
                string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();
                string contact = txtContact.Text;
                string email = txtEmail.Text;
                // The value below is collected from the UI, but cannot be saved yet
                // because the column 'username' is missing in your DB.
                string user = txtUsername.Text;

                // REMOVED 'username' from the columns and '{user}' from the VALUES
                string query = $@"INSERT INTO tbl_employee 
            (first_name, middle_name, last_name, gender, date_of_birth, role, contact_number, email, is_active) 
            VALUES ('{fn}', '{mn}', '{ln}', '{gender}', '{dob}', '{role}', '{contact}', '{email}', 1)";

                dbManager db = new dbManager();
                db.sqlManager(query);

                System.Windows.MessageBox.Show("Employee saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
