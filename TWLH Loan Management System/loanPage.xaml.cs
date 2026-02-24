using System;
using System.Collections.Generic;
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
    /// Interaction logic for loanPage.xaml
    /// </summary>
    public partial class LoanPage : Page
    {
        Loan loan = new Loan();

        public LoanPage()
        {
            InitializeComponent();
        }

        private void btnAddLoan_Click_1(object sender, RoutedEventArgs e)
        {
            LoanForm form = new LoanForm();
            if (form.ShowDialog() == true)
            {
                loan.displayLoanCards(loanContainer);
            }
           
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                loan.displayLoanCards(loanContainer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading loans: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
