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
    /// Interaction logic for PastDueAccountTabular.xaml
    /// </summary>
    public partial class PastDueAccountTabular : Page
    {
        PastDueAccount pastDue = new PastDueAccount();
        string searchText = "";
        string status = "All Statuses";

        public PastDueAccountTabular()
        {
            InitializeComponent();
            this.Loaded += (s, e) => PopulateTable();
        }

        public PastDueAccountTabular(string searchText, string status)
        {
            InitializeComponent();
            this.searchText = searchText;
            this.status = status;
            this.Loaded += (s, e) => PopulateTable();
        }

        private void PopulateTable()
        {
            try
            {
                dgPastDue.ItemsSource = pastDue.getFilteredPastDueAccounts(searchText, status).DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading table data: " + ex.Message);
            }
        }
    }
}
