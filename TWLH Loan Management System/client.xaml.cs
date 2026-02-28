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
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        Client client = new Client();

        public Page1()
        {
            InitializeComponent();
            this.Loaded += Page1_Loaded;
        }

        private void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                client.displayClientCards(clientContainer, txtSearch.Text == (string)txtSearch.Tag ? "" : txtSearch.Text);
            }
            catch (Exception ex)
            {
                // Silence error if it's just initialization noise, or show if it's critical
                // MessageBox.Show("Error loading clients: " + ex.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadClients();
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            ClientForm form = new ClientForm();
            if (form.ShowDialog() == true)
            {
                LoadClients(); // Refresh list after adding
            }
        }
    }
}
