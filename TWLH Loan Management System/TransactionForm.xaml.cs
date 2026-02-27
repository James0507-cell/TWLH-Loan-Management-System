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
    /// <summary>
    /// Interaction logic for TransactionForm.xaml
    /// </summary>
    public partial class TransactionForm : Window
    {
        private int _transactionID = -1;
        private Transaction _transaction = new Transaction();
        private dbManager _db = new dbManager();

        public TransactionForm()
        {
            InitializeComponent();
            LoadClients();
            txtTitle.Text = "Record New Transaction";
            txtSubtitle.Text = "Enter the transaction details below.";
            cmbStatus.SelectedIndex = 0; // Default to Confirmed
            cmbStatus.IsEnabled = false; // New transactions are usually confirmed by default
        }

        public TransactionForm(DataRow row)
        {
            InitializeComponent();
            LoadClients();
            _transactionID = Convert.ToInt32(row["transaction_id"]);
            txtTitle.Text = "Update Transaction Status";
            txtSubtitle.Text = $"Updating status for Transaction #{_transactionID}.";
            
            // Pre-fill and disable most fields
            int clientID = Convert.ToInt32(row["client_id"]);
            foreach (DataRowView item in cmbClient.Items)
            {
                if (Convert.ToInt32(item["client_id"]) == clientID)
                {
                    cmbClient.SelectedItem = item;
                    break;
                }
            }
            cmbClient.IsEnabled = false;
            
            txtAmount.Text = row["transaction_amount"].ToString();
            txtAmount.IsEnabled = false;

            SelectComboBoxItem(cmbType, row["transaction_type"].ToString());
            cmbType.IsEnabled = false;

            string currentStatus = row["status"].ToString();
            SelectComboBoxItem(cmbStatus, currentStatus);

            // Business Rule: Cannot un-void a transaction
            if (currentStatus == "Void")
            {
                cmbStatus.IsEnabled = false;
                btnSave.IsEnabled = false;
                txtSubtitle.Text = $"Transaction #{_transactionID} is VOIDED and cannot be changed.";
                txtSubtitle.Foreground = Brushes.Red;
            }
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
                    MessageBox.Show("Please enter an amount.");
                    return;
                }

                if (cmbType.SelectedItem == null)
                {
                    MessageBox.Show("Please select a transaction type.");
                    return;
                }

                string status = ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString();
                int recordedBy = 1; // Default for now

                if (_transactionID == -1)
                {
                    // Adding new transaction (will trigger waterfall)
                    int clientID = Convert.ToInt32(cmbClient.SelectedValue);
                    double amount = double.Parse(txtAmount.Text);
                    string type = ((ComboBoxItem)cmbType.SelectedItem).Content.ToString();

                    string query = $"INSERT INTO tbl_transaction (client_id, transaction_type, transaction_amount, status, recorded_by) " +
                                   $"VALUES ({clientID}, '{type}', {amount}, '{status}', {recordedBy})";
                    _db.sqlManager(query);
                    MessageBox.Show("Transaction recorded successfully! Allocation has been automated.");
                }
                else
                {
                    // Updating status (will trigger trg_after_transaction_status_change)
                    string query = $"UPDATE tbl_transaction SET status = '{status}' WHERE transaction_id = {_transactionID}";
                    _db.sqlManager(query);
                    MessageBox.Show("Transaction status updated. Balances have been recalculated.");
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving transaction: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
