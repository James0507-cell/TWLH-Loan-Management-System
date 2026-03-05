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
    /// Interaction logic for LoanDetails.xaml
    /// </summary>
    public partial class LoanDetails : Window
    {
        int loanID = 0;
        Loan loan = new Loan();
        Installment installment = new Installment();

        public LoanDetails(int loanID)
        {
            this.loanID = loanID;
            InitializeComponent();
            loadLoanDetails();
        }

        private void loadLoanDetails()
        {
            DataTable dt = loan.getLoanByID(loanID);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                txtLoanID.Text = $"Loan #{row["loan_id"]}";
                txtClientID.Text = $"Client ID: {row["client_id"]}";
                txtPlan.Text = row["installment_plan"].ToString();
                txtInterest.Text = $"{row["interest_rate"]}%";

                decimal grandTotal = installment.displayInstallmentCards(installmentContainer, loanID);
                txtGrandTotal.Text = $"₱{grandTotal:N2}";
            }
            else
            {
                MessageBox.Show("This loan record is no longer available (it may have been voided).");
                this.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
