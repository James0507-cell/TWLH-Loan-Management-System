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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TWLH_Loan_Management_System
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Page
    {
        adminDashboardInfo dashboardInfo =new adminDashboardInfo();
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtTotalActiveLoans.Text = dashboardInfo.getTotalActiveLoans();
            txtTotalLoanAmount.Text = dashboardInfo.getTotalLoanAmount();
            txtTotalClients.Text = dashboardInfo.getTotalClients();
            txtTotalEmployees.Text = dashboardInfo.getTotalEmployees();
            txtPaidInstallments.Text = dashboardInfo.getTotalPaidInstallment();
            txtPastDueCount.Text = dashboardInfo.getTotalPastDueAccount();
            txtPastDueAmount.Text = dashboardInfo.getTotalPastDueAmount();
        }
    }
}
