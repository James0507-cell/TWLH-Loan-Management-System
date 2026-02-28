using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TWLH_Loan_Management_System
{
    public partial class ClientForm : Window
    {
        Client client = new Client();
        DataRow clientData;
        bool isUpdate = false;
        int clientID;

        public ClientForm()
        {
            InitializeComponent();
            isUpdate = false;
            txtTitle.Text = "Register New Client";
            btnSave.Content = "Save Client";
        }

        public ClientForm(DataRow row)
        {
            InitializeComponent();
            clientData = row;
            isUpdate = true;
            clientID = Convert.ToInt32(row["client_id"]);
            
            txtTitle.Text = "Update Client Profile";
            btnSave.Content = "Update Profile";
            txtSubtitle.Text = $"Modifying profile for Client ID: #{clientID:D4}";

            FillForm();
        }

        private void FillForm()
        {
            txtFirstName.Text = clientData["first_name"].ToString();
            txtLastName.Text = clientData["last_name"].ToString();
            txtMiddleName.Text = clientData["middle_name"].ToString();
            txtContact.Text = clientData["contact_number"].ToString();
            txtMessenger.Text = clientData["messenger_name"].ToString();
            txtResidence.Text = clientData["current_residence"].ToString();
            
            string gender = clientData["gender"].ToString();
            foreach (ComboBoxItem item in cmbGender.Items)
            {
                if (item.Content.ToString() == gender)
                {
                    cmbGender.SelectedItem = item;
                    break;
                }
            }

            if (clientData["date_of_birth"] != DBNull.Value)
            {
                dtpBirthDate.SelectedDate = Convert.ToDateTime(clientData["date_of_birth"]);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                string fName = txtFirstName.Text;
                string lName = txtLastName.Text;
                string mName = txtMiddleName.Text;
                string gender = (cmbGender.SelectedItem as ComboBoxItem)?.Content.ToString();
                string dob = dtpBirthDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                string contact = txtContact.Text;
                string residence = txtResidence.Text;
                string messenger = txtMessenger.Text;

                try
                {
                    if (isUpdate)
                    {
                        client.updateClient(clientID, fName, mName, lName, gender, dob, contact, residence, messenger);
                        MessageBox.Show("Client profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        client.addClient(fName, mName, lName, gender, dob, contact, residence, messenger);
                        MessageBox.Show("New client registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving client: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("First name and Last name are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (cmbGender.SelectedItem == null)
            {
                MessageBox.Show("Please select a gender.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (dtpBirthDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a date of birth.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtContact.Text))
            {
                MessageBox.Show("Contact number is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
