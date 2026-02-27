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
    /// Interaction logic for PastDueAccountForm.xaml
    /// </summary>
    public partial class PastDueAccountForm : Window
    {
        private int _pastDueID;
        private dbManager _db = new dbManager();
        private PastDueAccount _pda = new PastDueAccount();
        private FollowUp _followUp = new FollowUp();
        private Collection _collection = new Collection();

        public PastDueAccountForm(int pastDueID)
        {
            InitializeComponent();
            this._pastDueID = pastDueID;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // 1. Load Account Summary
                DataTable dtAccount = _pda.getPastDueAccount(_pastDueID);
                if (dtAccount != null && dtAccount.Rows.Count > 0)
                {
                    DataRow row = dtAccount.Rows[0];
                    txtClientName.Text = $"{row["first_name"]} {row["last_name"]}";
                    
                    // We need installment_amount which is in the joined view/table
                    // Re-fetching with more details
                    DataTable dtDetails = _pda.getFilteredPastDueAccounts(row["past_due_id"].ToString());
                    if (dtDetails.Rows.Count > 0)
                    {
                        DataRow details = dtDetails.Rows[0];
                        txtOriginalAmount.Text = $"₱{Convert.ToDecimal(details["installment_amount"]):N2}";
                        txtPenalty.Text = details["penalty_added"].ToString();
                        txtTotalDue.Text = $"₱{Convert.ToDecimal(details["TotalPastDue"]):N2}";

                        // Lock form if Resolved
                        if (details["past_due_status"].ToString() == "Resolved")
                        {
                            txtTitle.Text = "Past Due Account (RESOLVED)";
                            txtSubtitle.Text = "This account is resolved. Information is now read-only.";
                            txtSubtitle.Foreground = Brushes.Green;
                            
                            txtPenalty.IsEnabled = false;
                            btnSave.Visibility = Visibility.Collapsed;
                            
                            btnScheduleFollowUp.IsEnabled = false;
                            btnAddFollowUp.IsEnabled = false;
                            btnAddCollection.IsEnabled = false;

                            btnScheduleFollowUp.Opacity = 0.5;
                            btnAddFollowUp.Opacity = 0.5;
                            btnAddCollection.Opacity = 0.5;
                        }
                    }
                }

                // 2. Load Follow Ups
                dgFollowUps.ItemsSource = _followUp.getFollowUpRecrods(_pastDueID).DefaultView;

                // 3. Load Collections
                dgCollections.ItemsSource = _collection.getCollectionRecord(_pastDueID).DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading past due details: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal penalty = 0;
                if (!decimal.TryParse(txtPenalty.Text, out penalty))
                {
                    MessageBox.Show("Please enter a valid penalty amount.");
                    return;
                }

                string query = $"UPDATE tbl_past_due_account SET penalty_added = {penalty}, updated_by = 1 WHERE past_due_id = {_pastDueID}";
                _db.sqlManager(query);

                MessageBox.Show("Account updated successfully.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating penalty: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            try { this.DragMove(); } catch { }
        }
    }
}
