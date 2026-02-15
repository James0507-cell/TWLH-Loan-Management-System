using Google.Protobuf;
using System.Text;
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
    
    public partial class LoginWindow : Window
    {
        MainWindow main = new MainWindow();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (username == "admin" && password == "admin123")
            {
                main.Show();
                this.Close();
            }
            else if (username == "staff" && password == "staff123")
            {
                main.Show();
                main.btnClient.Visibility = Visibility.Collapsed;
                main.btnEmployee.Visibility = Visibility.Collapsed;
                this.Close();
            }
            else if (username == "collector" && password == "collector123")
            {
                main.Show();
                main.btnTransaction.Visibility = Visibility.Collapsed;
                main.btnLoans.Visibility = Visibility.Collapsed;
                main.btnClient.Visibility = Visibility.Collapsed;
                main.btnEmployee.Visibility = Visibility.Collapsed;
                this.Close();
            } else
            {
                MessageBox.Show("Invalid Credentials");
            }
        }
    }
}