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
        int transactionID = 0;
        bool isTransactionBreakdown = false;
        dbManager db = new dbManager();
        Transaction transaction = new Transaction();

        // Mode 1: Log for a specific installment (repayment history)
        public installmentTransactionDetail(int installmentID)
        {
            this.installmentID = installmentID;
            InitializeComponent();
            loadInstallmentHistory();
        }

        // Mode 2: Breakdown of a specific transaction (waterfall distribution)
        public installmentTransactionDetail(int transactionID, string clientName)
        {
            this.transactionID = transactionID;
            this.isTransactionBreakdown = true;
            InitializeComponent();
            txtHeader.Text = $"Transaction #{transactionID}: {clientName}";
            loadTransactionBreakdown();
        }

        private void loadTransactionBreakdown()
        {
            string query = $@"
                SELECT 
                    ip.payment_id, 
                    ip.payment_amount, 
                    ip.installment_id,
                    t.created_at,
                    t.status as transaction_status,
                    e.first_name, 
                    e.middle_name, 
                    e.last_name
                FROM tbl_installment_payment ip
                JOIN tbl_transaction t ON ip.transaction_id = t.transaction_id
                JOIN tbl_employee e ON ip.recorded_by = e.employee_id
                WHERE ip.transaction_id = {transactionID}
                ORDER BY ip.payment_id ASC";

            DataTable dt = db.displayRecords(query);
            transactionContainer.Children.Clear();
            decimal totalPayment = 0;

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string status = row["transaction_status"].ToString();
                    UIElement transactionCard = transaction.createTransactionDetailCard(
                        Convert.ToInt32(row["payment_id"]),
                        Convert.ToDecimal(row["payment_amount"]),
                        $"{row["first_name"]} {row["middle_name"]} {row["last_name"]}",
                        Convert.ToDateTime(row["created_at"]),
                        status,
                        Convert.ToInt32(row["installment_id"]) // Show installment context
                    );
                    transactionContainer.Children.Add(transactionCard);
                    
                    if (status == "Confirmed")
                    {
                        totalPayment += Convert.ToDecimal(row["payment_amount"]);
                    }
                }
            }
            else
            {
                transactionContainer.Children.Add(new TextBlock
                {
                    Text = "No installment payments found for this transaction.",
                    FontSize = 14,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                });
            }

            txtTotalPayment.Text = $"₱{totalPayment:N2}";
        }

        private void loadInstallmentHistory()
        {
            txtHeader.Text = $"Log for Installment #{installmentID}";

            string query = $@"
                SELECT 
                    ip.payment_id, 
                    ip.payment_amount, 
                    ip.created_at,
                    t.status as transaction_status,
                    e.first_name, 
                    e.middle_name, 
                    e.last_name
                FROM tbl_installment_payment ip
                JOIN tbl_transaction t ON ip.transaction_id = t.transaction_id
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
                    string status = row["transaction_status"].ToString();
                    UIElement transactionCard = transaction.createTransactionDetailCard(
                        Convert.ToInt32(row["payment_id"]),
                        Convert.ToDecimal(row["payment_amount"]),
                        $"{row["first_name"]} {row["middle_name"]} {row["last_name"]}",
                        Convert.ToDateTime(row["created_at"]),
                        status
                    );
                    transactionContainer.Children.Add(transactionCard);
                    
                    if (status == "Confirmed")
                    {
                        totalPayment += Convert.ToDecimal(row["payment_amount"]);
                    }
                }
            }
            else
            {
                transactionContainer.Children.Add(new TextBlock
                {
                    Text = "No transaction records found for this installment.",
                    FontSize = 14,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
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
