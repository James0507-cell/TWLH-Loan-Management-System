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

        // Concrete class to avoid anonymous type binding issues
        public class LoanRecord
        {
            public int loan_id { get; set; }
            public int client_id { get; set; }
            public string FullName { get; set; }
            public decimal loan_amount { get; set; }
            public double interest_rate { get; set; }
            public DateTime due_date { get; set; }
            public DateTime created_at { get; set; }
            public string installment_plan { get; set; }
            public string loan_status { get; set; }
            public string ProgressText { get; set; }
            public double ProgressValue { get; set; }
            public DataRow SourceRow { get; set; } // Store original row for updates
        }

        public LoanTabular()
        {
            InitializeComponent();
            this.Loaded += LoanTabular_Loaded;
            dgLoans.MouseDoubleClick += DgLoans_MouseDoubleClick;
        }

        private void DgLoans_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgLoans.SelectedItem is LoanRecord selectedLoan)
            {
                OpenDetails(selectedLoan.loan_id);
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
            if (((Button)sender).DataContext is LoanRecord record)
            {
                OpenDetails(record.loan_id);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is LoanRecord record)
            {
                LoanForm form = new LoanForm(record.SourceRow);
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
                DataTable dt = loan.getLoans();
                if (dt != null)
                {
                    var loanItems = dt.AsEnumerable().Select(row => new LoanRecord
                    {
                        loan_id = Convert.ToInt32(row["loan_id"]),
                        client_id = Convert.ToInt32(row["client_id"]),
                        FullName = $"{row["first_name"]} {row["last_name"]}",
                        loan_amount = Convert.ToDecimal(row["loan_amount"]),
                        interest_rate = Convert.ToDouble(row["interest_rate"]),
                        due_date = Convert.ToDateTime(row["due_date"]),
                        created_at = Convert.ToDateTime(row["created_at"]),
                        installment_plan = row["installment_plan"].ToString(),
                        loan_status = row["loan_status"].ToString(),
                        ProgressText = $"{row["paid_installments"]}/{row["total_installments"]} paid",
                        ProgressValue = CalculateProgress(row["paid_installments"], row["total_installments"]),
                        SourceRow = row
                    }).ToList();

                    dgLoans.ItemsSource = loanItems;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table data: " + ex.Message);
            }
        }

        private double CalculateProgress(object paid, object total)
        {
            try
            {
                double p = Convert.ToDouble(paid);
                double t = Convert.ToDouble(total);
                return t > 0 ? (p / t) * 100 : 0;
            }
            catch { return 0; }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
