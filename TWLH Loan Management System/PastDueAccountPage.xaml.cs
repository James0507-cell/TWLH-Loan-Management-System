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
using FontAwesome.WPF;

namespace TWLH_Loan_Management_System
{
    /// <summary>
    /// Interaction logic for PastDueAccountPage.xaml
    /// </summary>
    public partial class PastDueAccountPage : Page
    {
        PastDueAccount pastDue = new PastDueAccount();

        public PastDueAccountPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            try
            {
                if (pastDueContainer == null || txtSearch == null || cmbStatus == null) return;

                string searchText = txtSearch.Text;
                string status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "All Statuses";

                if (scrollCards.Visibility == Visibility.Visible)
                {
                    pastDue.displayPastDueCards(pastDueContainer, searchText, status, ApplyFilters);
                }
                else
                {
                    pastDueFrame.Navigate(new PastDueAccountTabular(searchText, status));
                }
            }
            catch (Exception ex)
            {
                // Silent fail during init
            }
        }

        private void btnCardView_Click(object sender, RoutedEventArgs e)
        {
            scrollCards.Visibility = Visibility.Visible;
            pastDueFrame.Visibility = Visibility.Collapsed;

            // Update styles
            btnCardView.Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9");
            ((btnCardView.Content as StackPanel).Children[0] as ImageAwesome).Foreground = (Brush)new BrushConverter().ConvertFrom("#3044FF");
            ((btnCardView.Content as StackPanel).Children[1] as TextBlock).Foreground = (Brush)new BrushConverter().ConvertFrom("#3044FF");

            btnTableView.Background = Brushes.Transparent;
            ((btnTableView.Content as StackPanel).Children[0] as ImageAwesome).Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B");
            ((btnTableView.Content as StackPanel).Children[1] as TextBlock).Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B");

            ApplyFilters();
        }

        private void btnTableView_Click(object sender, RoutedEventArgs e)
        {
            scrollCards.Visibility = Visibility.Collapsed;
            pastDueFrame.Visibility = Visibility.Visible;

            // Update styles
            btnTableView.Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9");
            ((btnTableView.Content as StackPanel).Children[0] as ImageAwesome).Foreground = (Brush)new BrushConverter().ConvertFrom("#3044FF");
            ((btnTableView.Content as StackPanel).Children[1] as TextBlock).Foreground = (Brush)new BrushConverter().ConvertFrom("#3044FF");

            btnCardView.Background = Brushes.Transparent;
            ((btnCardView.Content as StackPanel).Children[0] as ImageAwesome).Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B");
            ((btnCardView.Content as StackPanel).Children[1] as TextBlock).Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B");

            ApplyFilters();
        }
    }
}
