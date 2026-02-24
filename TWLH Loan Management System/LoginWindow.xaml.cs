using Google.Protobuf;
using Org.BouncyCastle.Bcpg;
using System.Data;
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


        login login = new login();

        string userRole = "";
        int userID = 0;
        public string getRole()
        {
            return userRole;
        }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            
            var data = login.UserValidation(username, password);
            if (data == null)
            {
                MessageBox.Show("Invalid Credentials");
            } else
            {
                userRole = data.Value.Role;
                userID = data.Value.EmployeeID;

                MainWindow main = new MainWindow(userID, userRole);
               if (userRole == "Admin")
                {
                    main.Show();
                    this.Close();
                }
               else if (userRole == "Staff")
                {
                    main.Show();
                    main.btnClient.Visibility = Visibility.Collapsed;
                    main.btnEmployee.Visibility = Visibility.Collapsed;
                    this.Close();
                }
               else if (userRole == "Loan Collector")
                {
                    main.Show();
                    main.btnTransaction.Visibility = Visibility.Collapsed;
                    main.btnLoans.Visibility = Visibility.Collapsed;
                    main.btnClient.Visibility = Visibility.Collapsed;
                    main.btnEmployee.Visibility = Visibility.Collapsed;
                    this.Close();
                }
            }

           
        }
    }
}