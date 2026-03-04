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
    /// Interaction logic for CollectionDetails.xaml
    /// </summary>
    public partial class CollectionDetails : Window
    {
        private int assignmentID;
        private int pastDueID;
        dbManager db = new dbManager();
        Business business = new Business();
        FollowUp followUp = new FollowUp();
        PromiseToPay promise = new PromiseToPay();

        public CollectionDetails()
        {
            InitializeComponent();
        }

        public CollectionDetails(int assignmentID)
        {
            InitializeComponent();
            this.assignmentID = assignmentID;
            LoadAssignmentDetails();
        }

        private void LoadAssignmentDetails()
        {
            string query = $@"SELECT ca.*, 
                                    c.*,
                                    l.loan_id,
                                    li.installment_id, li.installment_amount, li.installment_due_date,
                                    pda.penalty_added, pda.past_due_id,
                                    CONCAT(e.first_name, ' ', e.last_name) as collector_name,
                                    CONCAT(creator.first_name, ' ', creator.last_name) as creator_name
                             FROM tbl_collection_assignment ca
                             JOIN tbl_past_due_account pda ON ca.past_due_id = pda.past_due_id
                             JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id
                             JOIN tbl_loan l ON li.loan_id = l.loan_id
                             JOIN tbl_client c ON l.client_id = c.client_id
                             JOIN tbl_employee e ON ca.assigned_to = e.employee_id
                             JOIN tbl_employee creator ON ca.created_by = creator.employee_id
                             WHERE ca.assignment_id = '{assignmentID}'";

            DataTable dt = db.displayRecords(query);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                int clientID = Convert.ToInt32(row["client_id"]);
                this.pastDueID = Convert.ToInt32(row["past_due_id"]);

                // Header
                txtAssignmentID.Text = $"Assignment #{assignmentID:D4}";

                // Client Details
                string firstName = row["first_name"].ToString();
                string middleName = row["middle_name"].ToString();
                string lastName = row["last_name"].ToString();
                txtClientFullName.Text = string.IsNullOrWhiteSpace(middleName) ? $"{firstName} {lastName}" : $"{firstName} {middleName} {lastName}";
                txtContact.Text = row["contact_number"].ToString();
                txtMessenger.Text = row["messenger_name"].ToString();
                txtGender.Text = row["gender"].ToString();
                txtBirthDate.Text = Convert.ToDateTime(row["date_of_birth"]).ToString("MMMM dd, yyyy");
                txtResidence.Text = row["current_residence"].ToString();

                // Business Details
                business.displayBusinessCards(pnlBusinesses, clientID, true);

                // Installment Details
                txtLoanID.Text = $"#{Convert.ToInt32(row["loan_id"]):D4}";
                txtInstallmentID.Text = $"#{Convert.ToInt32(row["installment_id"]):D4}";
                string dueDate = Convert.ToDateTime(row["installment_due_date"]).ToString("MMM dd, yyyy");
                txtDueDate.Text = dueDate;
                txtDueDateHeader.Text = dueDate;
                
                double instAmt = Convert.ToDouble(row["installment_amount"]);
                double penaltyAmt = Convert.ToDouble(row["penalty_added"]);
                txtInstAmount.Text = $"₱ {instAmt:N2}";
                txtPenalty.Text = $"₱ {penaltyAmt:N2}";
                txtTotalDue.Text = $"₱ {(instAmt + penaltyAmt):N2}";

                // Assignment Status Info
                string status = row["assignment_status"].ToString();
                txtStatus.Text = status;
                txtCollector.Text = row["collector_name"].ToString();
                txtCreator.Text = row["creator_name"].ToString();

                // Status Badge Color
                if (status == "Completed")
                {
                    brdStatus.Background = (Brush)new BrushConverter().ConvertFrom("#D1FAE5");
                    txtStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#10B981");
                }
                else if (status == "Canceled")
                {
                    brdStatus.Background = (Brush)new BrushConverter().ConvertFrom("#FEE2E2");
                    txtStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444");
                }
                else // In Progress
                {
                    brdStatus.Background = (Brush)new BrushConverter().ConvertFrom("#EEF2FF");
                    txtStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#6366F1");
                }

                LoadHistory();
            }
            else
            {
                MessageBox.Show("Error: Assignment record not found.");
                this.Close();
            }
        }

        private void LoadHistory()
        {
            try
            {
                dgFollowUps.ItemsSource = followUp.getFollowUpRecrods(pastDueID).DefaultView;
                dgPromises.ItemsSource = promise.getPromiseRecords(pastDueID).DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading history: " + ex.Message);
            }
        }

        private void btnAddFollowUp_Click(object sender, RoutedEventArgs e)
        {
            FollowUpForm form = new FollowUpForm(pastDueID);
            if (form.ShowDialog() == true)
            {
                LoadHistory();
            }
        }

        private void btnAddPromise_Click(object sender, RoutedEventArgs e)
        {
            PromiseForm form = new PromiseForm(pastDueID);
            if (form.ShowDialog() == true)
            {
                LoadHistory();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
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
