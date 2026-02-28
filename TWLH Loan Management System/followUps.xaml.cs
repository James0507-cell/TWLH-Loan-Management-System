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
    /// Interaction logic for followUps.xaml
    /// </summary>
    public partial class followUps : Page
    {
        FollowUp _followUp = new FollowUp();

        public followUps()
        {
            InitializeComponent();
            LoadCards();
        }

        private void LoadCards(string search = "")
        {
            _followUp.displayFollowUpCards(FollowUpCardsContainer, search);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCards(txtSearch.Text);
        }

        private void btnAddFollowUp_Click(object sender, RoutedEventArgs e)
        {
            // Usually follow-ups are tied to a past due account.
            // Redirect user to Past Due Accounts page to select a an account.
            MessageBox.Show("Please select an account from the Past Due Accounts page to log a new follow-up interaction.", "Add Follow-up", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // If this is in a NavigationService context:
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(new PastDueAccountPage());
            }
        }
    }
}
