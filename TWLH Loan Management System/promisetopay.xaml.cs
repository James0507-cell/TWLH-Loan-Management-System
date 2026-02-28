using System;
using System.Data;
using System.Windows.Controls;

namespace TWLH_Loan_Management_System
{
    public partial class promisetopay : Page
    {
        dbManager db = new dbManager();

        public promisetopay()
        {
            InitializeComponent();
            LoadPromises();
        }

        private void LoadPromises()
        {
            try
            {
                
                string query = "SELECT promise_id, promise_amount, promise_payment_date, remarks, past_due_id, recorded_by FROM tbl_promise ORDER BY promise_payment_date ASC";

                DataTable dt = db.displayRecords(query);

               
                PromiseCardsControl.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                
                System.Windows.MessageBox.Show("Failed to load records: " + ex.Message);
            }
        }

        private void BtnAddPromise_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
            PromiseForm addForm = new PromiseForm();

            
            addForm.Owner = System.Windows.Window.GetWindow(this);

            bool? result = addForm.ShowDialog();

            if (result == true)
            {
                LoadPromises();
            }
        }
    }
}