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
    /// Interaction logic for LoanTabular.xaml
    /// </summary>
    public partial class LoanTabular : Page
    {
        Loan loan = new Loan();
        string searchText = "";
        string status = "All Statuses";
        string plan = "All Plans";
        string loanType = "Standard";

        public LoanTabular()
        {
            InitializeComponent();
            this.Loaded += LoanTabular_Loaded;
            dgLoans.MouseDoubleClick += DgLoans_MouseDoubleClick;
        }

        public LoanTabular(string searchText, string status, string plan = "All Plans", string loanType = "Standard")
        {
            InitializeComponent();
            this.searchText = searchText;
            this.status = status;
            this.plan = plan;
            this.loanType = loanType;
            this.Loaded += LoanTabular_Loaded;
            dgLoans.MouseDoubleClick += DgLoans_MouseDoubleClick;
        }

        private void DgLoans_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgLoans.SelectedItem is DataRowView row)
            {
                OpenDetails(Convert.ToInt32(row["loan_id"]));
            }
        }

        private void OpenDetails(int loanID)
        {
            LoanDetails details = new LoanDetails(loanID);
            details.ShowDialog();
            PopulateTable();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is DataRowView row)
            {
                OpenDetails(Convert.ToInt32(row["loan_id"]));
            }
        }

        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.Role != "Admin" && UserSession.Role != "Staff")
            {
                MessageBox.Show("You do not have permission to void loans.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (((Button)sender).DataContext is DataRowView row)
            {
                int idToVoid = Convert.ToInt32(row["loan_id"]);

                if (!loan.canVoidLoan(idToVoid))
                {
                    MessageBox.Show("This loan cannot be voided because there are already confirmed transactions associated with its installments.", "Action Prohibited", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Are you sure you want to void this loan? This action will mark the loan as a mistake and cancel any associated collection assignments. This cannot be undone.", "Confirm Void", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        loan.voidLoan(idToVoid);
                        MessageBox.Show("Loan has been successfully voided.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        PopulateTable();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error voiding loan: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void LoanTabular_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateTable();
        }

        private void PopulateTable()
        {
            try
            {
                dgLoans.ItemsSource = loan.getFilteredLoans(searchText, status, plan, loanType).DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table data: " + ex.Message);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
