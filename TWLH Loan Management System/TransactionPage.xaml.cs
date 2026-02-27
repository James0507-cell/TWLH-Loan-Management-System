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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TWLH_Loan_Management_System
{
    /// <summary>
    /// Interaction logic for TransactionPage.xaml
    /// </summary>
    public partial class TransactionPage : Page
    {
        Transaction transaction = new Transaction();

        public TransactionPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is DataRowView row)
            {
                int transactionID = Convert.ToInt32(row["transaction_id"]);
                string clientName = row["ClientName"].ToString();
                
                installmentTransactionDetail details = new installmentTransactionDetail(transactionID, clientName);
                details.ShowDialog();
                ApplyFilters();
            }
        }

        private void btnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            TransactionForm form = new TransactionForm();
            if (form.ShowDialog() == true)
            {
                ApplyFilters();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is DataRowView row)
            {
                TransactionForm form = new TransactionForm(row.Row);
                if (form.ShowDialog() == true)
                {
                    ApplyFilters();
                }
            }
        }

        private void ApplyFilters()
        {
            try
            {
                // Guard against uninitialized UI components
                if (dgTransactions == null || txtSearch == null || cmbType == null) return;

                string searchText = txtSearch.Text;
                string type = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "All Types";

                DataTable dt = transaction.getTransactionRecords(searchText, type);
                dgTransactions.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                // Silently fail during initialization if components are not yet available
            }
        }
    }
}
