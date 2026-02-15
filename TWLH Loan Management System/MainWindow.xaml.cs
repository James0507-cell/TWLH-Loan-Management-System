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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetActiveButton(Button selectedButton)
        {
            btnDashboard.Tag = "";
            btnTransaction.Tag = "";
            btnLoans.Tag = "";
            btnCollection.Tag = "";
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
        }

        private void btnTransaction_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
        }

        private void btnLoans_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new LoanPage());
        }

        private void btnCollection_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
        }

        private void btnClient_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
        }

        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
        }

        

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
