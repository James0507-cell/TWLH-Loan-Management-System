using System.Data;
using System.Windows;
using System.Windows.Controls;


namespace TWLH_Loan_Management_System
{
    public partial class promisetopay : Page
    {
        // One global instance is enough
        dbManager db = new dbManager();

        public promisetopay()
        {
            InitializeComponent();
            LoadPromises(); // Initial load
        }

        private void LoadPromises()
        {
            try
            {
                // Better query: includes ORDER BY for better UI organization, joins for client and employee info
                string query = @"SELECT p.*, 
                               CONCAT(c.first_name, ' ', c.last_name) as client_name, 
                               CONCAT(e1.first_name, ' ', e1.last_name) as recorder_name,
                               IFNULL(CONCAT(e2.first_name, ' ', e2.last_name), 'System') as updated_by_name
                               FROM tbl_promise p
                               JOIN tbl_past_due_account pda ON p.past_due_id = pda.past_due_id
                               JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id
                               JOIN tbl_loan l ON li.loan_id = l.loan_id
                               JOIN tbl_client c ON l.client_id = c.client_id
                               JOIN tbl_employee e1 ON p.recorded_by = e1.employee_id
                               LEFT JOIN tbl_employee e2 ON p.updated_by = e2.employee_id
                               ORDER BY p.promise_id DESC";
                DataTable dt = db.displayRecords(query);
                PromiseCardsControl.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load records: " + ex.Message);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataRowView row = btn.DataContext as DataRowView;

            if (row != null)
            {
                // This will now work because you fixed the constructor in the other file!
                UpdatePromisetopay updateWin = new UpdatePromisetopay(row);
                updateWin.Owner = Window.GetWindow(this);
                updateWin.ShowDialog();

                // Refresh using the single, primary method
                LoadPromises();
            }

        }
        private void BtnAddPromise_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please select an account from the Past Due Accounts page to create a new promise to pay record.", "Add Promise to Pay", MessageBoxButton.OK, MessageBoxImage.Information);

            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(new PastDueAccountPage());
            }
        }

        // You can now delete the extra 'LoadPromiseData()' method
    }
}