using MySqlX.XDevAPI;
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
    /// Interaction logic for admin.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string role;
        private int userId;

        public MainWindow(int id, string role)
        {
            InitializeComponent();
            this.role = role;
            this.userId = id;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NavigateToDashboard();
        }

        private void NavigateToDashboard()
        {
            SetActiveButton(btnDashboard);
            if (role == "Admin")
            {
                MainFrame.Navigate(new AdminDashboard());
            }
            else if(role == "Staff")
            {
                MainFrame.Navigate(new StaffDashboard());
            } 
            else if (role == "Loan Collector")
            {
                MainFrame.Navigate(new CollectorDashboard(userId));
            }
            
        }

        private void SetActiveButton(Button selectedButton)
        {
            btnDashboard.Tag = "";
            btnTransaction.Tag = "";
            btnLoans.Tag = "";
            btnPastDueAccounts.Tag = "";
            btnCollection.Tag = "";
            btnFollowUp.Tag = "";
            btnPromiseToPay.Tag = "";
            btnClient.Tag = "";
            btnEmployee.Tag = "";

            if (selectedButton != null)
            {
                selectedButton.Tag = "Active";
            }
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            NavigateToDashboard();
        }

        private void btnTransaction_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new TransactionPage());
        }

        private void btnLoans_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new LoanPage());
        }

        private void btnPastDueAccounts_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new PastDueAccountPage());
        }

        private void btnCollection_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new CollectionPage());

        }

        private void btnClient_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new Page1());
        }

        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new EmployeePage());
        }
        private void btnFollowUp_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new followUps());
        }

        private void btnPromiseToPay_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new promisetopay());
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            UserSession.Logout();
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        
    }
}
