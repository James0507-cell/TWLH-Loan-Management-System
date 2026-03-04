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
                // Better query: includes ORDER BY for better UI organization
                string query = "SELECT * FROM tbl_promise ORDER BY promise_payment_date ASC";
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