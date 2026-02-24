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
using System.Windows.Shapes;

namespace TWLH_Loan_Management_System
{
    /// <summary>
    /// Interaction logic for installmentTransactionDetail.xaml
    /// </summary>
    public partial class installmentTransactionDetail : Window
    {
        int installmentID = 0;
        dbManager db = new dbManager();
        Transaction transaction = new Transaction(); // Assuming Transaction class exists for card creation

        public installmentTransactionDetail(int installmentID)
        {
            this.installmentID = installmentID;
            InitializeComponent();
            loadTransactionDetails();
        }

        private void loadTransactionDetails()
        {
            txtHeader.Text = $"Transaction Log for Installment #{installmentID}";

            string query = $@"
                SELECT 
                    ip.payment_id, 
                    ip.payment_amount, 
                    ip.created_at,
                    e.first_name, 
                    e.middle_name, 
                    e.last_name
                FROM tbl_installment_payment ip
                JOIN tbl_employee e ON ip.recorded_by = e.employee_id
                WHERE ip.installment_id = {installmentID}
                ORDER BY ip.created_at ASC";

            DataTable dt = db.displayRecords(query);
            transactionContainer.Children.Clear();
            decimal totalPayment = 0;

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    // Assuming Transaction.cs has a method to create a UI element for a transaction card
                    // This method needs to be created or identified next.
                    UIElement transactionCard = transaction.createTransactionDetailCard(
                        Convert.ToInt32(row["payment_id"]),
                        Convert.ToDecimal(row["payment_amount"]),
                        $"{row["first_name"]} {row["middle_name"]} {row["last_name"]}",
                        Convert.ToDateTime(row["created_at"])
                    );
                    transactionContainer.Children.Add(transactionCard);
                    totalPayment += Convert.ToDecimal(row["payment_amount"]);
                }
            }
            else
            {
                // Display a message if no transactions are found
                transactionContainer.Children.Add(new TextBlock
                {
                    Text = "No transaction records found for this installment.",
                    FontSize = 14,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                });
            }

            txtTotalPayment.Text = $"₱{totalPayment:N2}";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
