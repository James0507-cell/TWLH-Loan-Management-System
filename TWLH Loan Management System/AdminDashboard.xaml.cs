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
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Page
    {
        DashboardInfo dashboardInfo =new DashboardInfo();
        
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

            dtgOverdueList.ItemsSource = dashboardInfo.getOverDueList().DefaultView;
            double collectionRate = dashboardInfo.getCollectionRate();
            txtblCollectionRate.Text = collectionRate.ToString("P2") + " Collection Rate";
            prgCollectionRate.Value = collectionRate * 100;
            txtblCollectedAmount.Text = dashboardInfo.getCollectedAmount().ToString();
        }

        private void btnViewOverdue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn != null)
                {
                    DataRowView row = (DataRowView)btn.DataContext;
                    if (row != null)
                    {
                        int pastDueID = Convert.ToInt32(row["past_due_id"]);
                        PastDueAccountForm form = new PastDueAccountForm(pastDueID);
                        if (form.ShowDialog() == true)
                        {
                            // Refresh dashboard data if needed
                            Page_Loaded(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening past due account: " + ex.Message);
            }
        }
    }
}
