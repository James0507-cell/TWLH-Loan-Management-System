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
    /// Interaction logic for CollectorDashboard.xaml
    /// </summary>
    public partial class CollectorDashboard : Page
    {
        DashboardInfo dashBoardinfo = new DashboardInfo();
        int userID = 0;

        public CollectorDashboard(int userID)
        {
            InitializeComponent();
            this.userID = userID;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadDashboard();
        }

        private void loadDashboard()
        {
            try
            {
                txtMyAssignmentCount.Text = dashBoardinfo.getMyAssignmentCount(userID);
                txtOverdueCount.Text = dashBoardinfo.getOverdueAssignmentCount(userID);
                
                DataTable dt = dashBoardinfo.getMyAssignments(userID);
                if (dt != null)
                {
                    dgAssignments.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the dashboard: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnManage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataRowView row = btn.DataContext as DataRowView;

            if (row != null)
            {
                int assignmentID = Convert.ToInt32(row["assignment_id"]);
                CollectionDetails details = new CollectionDetails(assignmentID);
                if (details.ShowDialog() == true)
                {
                    loadDashboard();
                }
            }
        }
    }
}
