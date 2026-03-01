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
    /// Interaction logic for CollectionPage.xaml
    /// </summary>
    public partial class CollectionPage : Page
    {
        Collection collection = new Collection();
        DataTable fullDt;

        public CollectionPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Only Admin and Staff can add assignments
            if (UserSession.Role == "Loan Collector")
            {
                btnAddAssignment.Visibility = Visibility.Collapsed;
            }

            FetchData();
            ApplyFilters();
        }

        private void FetchData()
        {
            // Role-based filtering: Admin and Staff see all, Loan Collector sees only their own
            if (UserSession.Role == "Admin" || UserSession.Role == "Staff")
            {
                fullDt = collection.getCollectionRecord();
                txtSubtitle.Text = "Manage and monitor all active collection assignments.";
            }
            else if (UserSession.Role == "Loan Collector")
            {
                fullDt = collection.getCollectionRecordByCollector(UserSession.EmployeeID);
                txtSubtitle.Text = "Your assigned loan collection tasks.";
            }
        }

        private void ApplyFilters()
        {
            if (fullDt == null) return;

            pnlCards.Children.Clear();
            string searchText = txtSearch.Text.ToLower().Trim();
            string selectedStatus = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            foreach (DataRow row in fullDt.Rows)
            {
                bool matchesSearch = string.IsNullOrEmpty(searchText) || 
                                     row["client_name"].ToString().ToLower().Contains(searchText) ||
                                     row["past_due_id"].ToString().Contains(searchText);
                
                bool matchesStatus = selectedStatus == "All Status" || 
                                     row["assignment_status"].ToString() == selectedStatus;

                if (matchesSearch && matchesStatus)
                {
                    UIElement card = collection.collectionCards(row, BtnView_Click, BtnUpdate_Click);
                    pnlCards.Children.Add(card);
                }
            }

            if (pnlCards.Children.Count == 0)
            {
                DisplayNoRecordsMessage();
            }
        }

        private void DisplayNoRecordsMessage()
        {
            StackPanel emptyStack = new StackPanel();
            emptyStack.HorizontalAlignment = HorizontalAlignment.Center;
            emptyStack.VerticalAlignment = VerticalAlignment.Center;
            emptyStack.Margin = new Thickness(50);

            TextBlock txtNoRecords = new TextBlock();
            txtNoRecords.Text = "No collection assignments found.";
            txtNoRecords.FontSize = 18;
            txtNoRecords.FontWeight = FontWeights.Medium;
            txtNoRecords.Foreground = (Brush)new BrushConverter().ConvertFrom("#94A3B8");
            txtNoRecords.HorizontalAlignment = HorizontalAlignment.Center;
            
            emptyStack.Children.Add(txtNoRecords);
            pnlCards.Children.Add(emptyStack);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnAddAssignment_Click(object sender, RoutedEventArgs e)
        {
            // Similar to Follow-up page, assignments are tied to a past due account.
            // Redirect user to Past Due Accounts page to select an account.
            MessageBox.Show("Please select an account from the Past Due Accounts page to create a new collection assignment.", "Add Collection Assignment", MessageBoxButton.OK, MessageBoxImage.Information);

            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(new PastDueAccountPage());
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag is DataRow row)
            {
                // Open the assignment form in edit mode
                CollectionAssignmentForm form = new CollectionAssignmentForm(row);
                if (form.ShowDialog() == true)
                {
                    FetchData();
                    ApplyFilters();
                }
            }
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int assignmentID = Convert.ToInt32(btn.Tag);
                
                CollectionDetails details = new CollectionDetails(assignmentID);
                details.ShowDialog();
                
                // Refresh data in case status was updated
                FetchData();
                ApplyFilters();
            }
        }
    }
}
