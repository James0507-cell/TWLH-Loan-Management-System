using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TWLH_Loan_Management_System
{
    public partial class BusinessForm : Window
    {
        Business business = new Business();
        int clientID;
        int businessID;
        bool isUpdate = false;
        DataRow businessData;

        // For adding new business
        public BusinessForm(int clientID)
        {
            InitializeComponent();
            this.clientID = clientID;
            this.isUpdate = false;
        }

        // For updating existing business
        public BusinessForm(DataRow row)
        {
            InitializeComponent();
            this.businessData = row;
            this.businessID = Convert.ToInt32(row["business_id"]);
            this.clientID = Convert.ToInt32(row["client_id"]);
            this.isUpdate = true;

            txtTitle.Text = "Update Business Details";
            btnSave.Content = "Update Business";
            FillForm();
        }

        private void FillForm()
        {
            txtBusinessName.Text = businessData["business_name"].ToString();
            txtRegID.Text = businessData["business_registration_id"].ToString();
            txtAddress.Text = businessData["business_address"].ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBusinessName.Text) || string.IsNullOrWhiteSpace(txtRegID.Text))
            {
                MessageBox.Show("Business Name and Registration ID are required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (isUpdate)
                {
                    business.updateBusiness(businessID, clientID, txtBusinessName.Text, txtAddress.Text, txtRegID.Text);
                    MessageBox.Show("Business record updated successfully.", "Success");
                }
                else
                {
                    business.addBusiness(clientID, txtBusinessName.Text, txtAddress.Text, txtRegID.Text);
                    MessageBox.Show("New business added successfully.", "Success");
                }
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving business: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
