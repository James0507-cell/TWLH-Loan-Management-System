using System;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;

namespace TWLH_Loan_Management_System
{
    public partial class LoanForm : Window
    {
        private int _loanID = -1;
        private Loan _loan = new Loan();
        private dbManager _db = new dbManager();

        public LoanForm()
        {
            InitializeComponent();
            LoadClients();
            txtTitle.Text = "Create New Loan";
            txtSubtitle.Text = "Enter the loan details below to proceed.";
            statusPanel.Visibility = Visibility.Collapsed;
        }

        public LoanForm(DataRow row)
        {
            InitializeComponent();
            LoadClients();
            _loanID = Convert.ToInt32(row["loan_id"]);
            txtTitle.Text = "Update Loan Details";
            txtSubtitle.Text = $"Updating details for Loan #{_loanID}.";
            statusPanel.Visibility = Visibility.Visible;

            // Pre-fill data
            cmbClient.SelectedValue = Convert.ToInt32(row["client_id"]);
            txtAmount.Text = row["loan_amount"].ToString();
            dtpDueDate.SelectedDate = Convert.ToDateTime(row["due_date"]);
            txtInterestRate.Text = row["interest_rate"].ToString();

            // Select Status and Plan
            SelectComboBoxItem(cmbInstallmentPlan, row["installment_plan"].ToString());
            SelectComboBoxItem(cmbStatus, row["loan_status"].ToString());
        }

        private void LoadClients()
        {
            try
            {
                string query = "SELECT client_id, CONCAT(first_name, ' ', last_name, ' (ID: ', client_id, ')') as client_display FROM tbl_client ORDER BY first_name ASC";
                DataTable dt = _db.displayRecords(query);
                cmbClient.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading clients: " + ex.Message);
            }
        }

        private void SelectComboBoxItem(ComboBox cmb, string value)
        {
            foreach (object item in cmb.Items)
            {
                if (item is ComboBoxItem cbItem)
                {
                    if (cbItem.Content.ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        cmb.SelectedItem = cbItem;
                        break;
                    }
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            try { this.DragMove(); } catch { }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbClient.SelectedValue == null)
                {
                    MessageBox.Show("Please select a client.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    MessageBox.Show("Please enter a loan amount.");
                    return;
                }

                if (dtpDueDate.SelectedDate == null)
                {
                    MessageBox.Show("Please select a due date.");
                    return;
                }

                if (cmbInstallmentPlan.SelectedItem == null)
                {
                    MessageBox.Show("Please select an installment plan.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtInterestRate.Text))
                {
                    MessageBox.Show("Please enter an interest rate.");
                    return;
                }

                int clientID = Convert.ToInt32(cmbClient.SelectedValue);
                double amount = double.Parse(txtAmount.Text);
                string dueDate = dtpDueDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                string plan = ((ComboBoxItem)cmbInstallmentPlan.SelectedItem).Content.ToString();
                double interestRate = double.Parse(txtInterestRate.Text);
                string status = _loanID == -1 ? "Active" : ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString();
                int approvedBy = UserSession.EmployeeID; 

                if (_loanID == -1)
                {
                    _loan.addLoan(clientID, amount, dueDate, plan, interestRate, status, approvedBy);
                    MessageBox.Show("Loan added successfully!");
                }
                else
                {
                    _loan.updateLoan(_loanID, clientID, amount, dueDate, plan, interestRate, status, approvedBy);
                    MessageBox.Show("Loan updated successfully!");
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving loan: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
