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
    public partial class ClientDetail : Window
    {
        int clientID;
        Client client = new Client();
        Business business = new Business();
        Loan loan = new Loan();

        public ClientDetail(int id)
        {
            InitializeComponent();
            this.clientID = id;
            LoadClientData();
            LoadBusinessCards();
            LoadLoanCards();
        }

        private void LoadClientData()
        {
            try
            {
                DataTable dt = client.getClient(clientID);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string firstName = row["first_name"].ToString();
                    string middleName = row["middle_name"].ToString();
                    string lastName = row["last_name"].ToString();
                    txtFullName.Text = string.IsNullOrWhiteSpace(middleName) ? $"{firstName} {lastName}" : $"{firstName} {middleName} {lastName}";
                    txtClientID.Text = $"ID: #{clientID:D4}";
                    txtGender.Text = row["gender"].ToString();
                    txtPhone.Text = row["contact_number"].ToString();
                    txtMessenger.Text = row["messenger_name"].ToString();
                    txtResidence.Text = row["current_residence"].ToString();
                    
                    if (row["date_of_birth"] != DBNull.Value)
                        txtBirthDate.Text = Convert.ToDateTime(row["date_of_birth"]).ToString("MMM dd, yyyy");
                    
                    if (row["created_at"] != DBNull.Value)
                        txtJoinedDate.Text = Convert.ToDateTime(row["created_at"]).ToString("MMM dd, yyyy");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading client details: " + ex.Message);
            }
        }

        private void LoadBusinessCards()
        {
            try
            {
                business.displayBusinessCards(businessContainer, clientID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading business cards: " + ex.Message);
            }
        }

        private void LoadLoanCards()
        {
            try
            {
                loan.displayLoanCardsByClient(loanContainer, clientID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading loan cards: " + ex.Message);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddBusiness_Click(object sender, RoutedEventArgs e)
        {
            BusinessForm form = new BusinessForm(clientID);
            if (form.ShowDialog() == true)
            {
                LoadBusinessCards();
            }
        }
    }
}
