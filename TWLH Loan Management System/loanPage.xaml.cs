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
            txtSearch.TextChanged += TxtSearch_TextChanged;
            cmbStatus.SelectionChanged += CmbStatus_SelectionChanged;
            txtPlanFilter.TextChanged += TxtPlanFilter_TextChanged;
            cmbType.SelectionChanged += CmbType_SelectionChanged;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void TxtPlanFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (txtSearch == null || cmbStatus == null || txtPlanFilter == null || cmbType == null) return;

            string searchText = txtSearch.Text;
            string status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "All Statuses";
            string plan = string.IsNullOrWhiteSpace(txtPlanFilter.Text) ? "All Plans" : txtPlanFilter.Text;
            string loanType = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Standard";

            if (scrollCards.Visibility == Visibility.Visible)
            {
                loan.displayLoanCards(loanContainer, searchText, status, plan, loanType);
            }
            else
            {
                loanFrame.Navigate(new LoanTabular(searchText, status, plan, loanType));
            }
        }

        private void btnAddLoan_Click_1(object sender, RoutedEventArgs e)
        {
            LoanForm form = new LoanForm();
            if (form.ShowDialog() == true)
            {
                ApplyFilters();
            }
           
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading loans: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Card Layout
            scrollCards.Visibility = Visibility.Visible;
            loanFrame.Visibility = Visibility.Collapsed;

            // Update styles
            btnCardView.Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9");
            btnCardView.Foreground = (Brush)new BrushConverter().ConvertFrom("#3044FF");
            btnTableView.Background = Brushes.Transparent;
            btnTableView.Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B");

            ApplyFilters();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Table Layout
            scrollCards.Visibility = Visibility.Collapsed;
            loanFrame.Visibility = Visibility.Visible;

            // Update styles
            btnTableView.Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9");
            btnTableView.Foreground = (Brush)new BrushConverter().ConvertFrom("#3044FF");
            btnCardView.Background = Brushes.Transparent;
            btnCardView.Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B");

            // Navigate to Tabular View with current filters
            ApplyFilters();
        }
    }
}
