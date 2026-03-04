using System;
using System.Windows;
using System.Windows.Controls;

namespace TWLH_Loan_Management_System
{
    public partial class EmployeeForm : Window
    {
        private Employee currentEmployee;
        private bool isEditMode = false;

        public EmployeeForm()
        {
            InitializeComponent();
            currentEmployee = new Employee();
            isEditMode = false;
        }

        public EmployeeForm(Employee employee)
        {
            InitializeComponent();
            currentEmployee = employee;
            isEditMode = true;
            PopulateForm();
            TitleText.Text = "Edit Employee";
            pnlPassword.Visibility = Visibility.Collapsed;
        }

        private void PopulateForm()
        {
            txtFirstName.Text = currentEmployee.FirstName;
            txtMiddleName.Text = currentEmployee.MiddleName;
            txtLastName.Text = currentEmployee.LastName;
            cmbGender.Text = currentEmployee.Gender;
            dpBirthDate.SelectedDate = currentEmployee.DateOfBirth;
            cmbRole.Text = currentEmployee.Role;
            txtContact.Text = currentEmployee.ContactNumber;
            txtEmail.Text = currentEmployee.Email;
            txtUsername.Text = currentEmployee.Username;
            cmbEmployeeStatus.Text = currentEmployee.IsEmployeeActive ? "Active" : "Inactive";
            cmbCredentialStatus.Text = currentEmployee.IsCredentialActive ? "Active" : "Inactive";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            currentEmployee.FirstName = txtFirstName.Text;
            currentEmployee.MiddleName = txtMiddleName.Text;
            currentEmployee.LastName = txtLastName.Text;
            currentEmployee.Gender = cmbGender.Text;
            currentEmployee.DateOfBirth = dpBirthDate.SelectedDate ?? DateTime.Now;
            currentEmployee.Role = cmbRole.Text;
            currentEmployee.ContactNumber = txtContact.Text;
            currentEmployee.Email = txtEmail.Text;
            currentEmployee.Username = txtUsername.Text;
            currentEmployee.Password = txtPassword.Password;
            currentEmployee.IsEmployeeActive = cmbEmployeeStatus.Text == "Active";
            currentEmployee.IsCredentialActive = cmbCredentialStatus.Text == "Active";

            bool success;
            if (isEditMode)
            {
                success = currentEmployee.Update();
            }
            else
            {
                success = currentEmployee.Save();
            }

            if (success)
            {
                MessageBox.Show(isEditMode ? "Employee updated successfully!" : "Employee added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("An error occurred while saving the employee record.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
