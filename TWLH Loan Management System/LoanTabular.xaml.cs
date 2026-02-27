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

        public LoanTabular()
        {
            InitializeComponent();
            this.Loaded += LoanTabular_Loaded;
            dgLoans.MouseDoubleClick += DgLoans_MouseDoubleClick;
        }

        public LoanTabular(string searchText, string status)
        {
            InitializeComponent();
            this.searchText = searchText;
            this.status = status;
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

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is DataRowView row)
            {
                LoanForm form = new LoanForm(row.Row);
                if (form.ShowDialog() == true)
                {
                    PopulateTable();
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
                dgLoans.ItemsSource = loan.getFilteredLoans(searchText, status).DefaultView;
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
