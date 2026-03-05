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
using System.Data;

namespace TWLH_Loan_Management_System
{
    /// <summary>
    /// Interaction logic for UpdatePromisetopay.xaml
    /// </summary>
    public partial class UpdatePromisetopay : Window
    {

        private string currentPromiseId;
        public UpdatePromisetopay(DataRowView row)
        {
            InitializeComponent();
            currentPromiseId = row["promise_id"].ToString();
            TxtPromiseID.Text = "PROMISE #" + currentPromiseId;
            TxtAmount.Text = row["promise_amount"].ToString();
            DpDate.SelectedDate = Convert.ToDateTime(row["promise_payment_date"]);
            TxtRemarks.Text = row["remarks"].ToString();
            checkIfVoid();
        }

        private void checkIfVoid()
        {
            dbManager db = new dbManager();
            string query = $@"SELECT l.is_void FROM tbl_promise p 
                            JOIN tbl_past_due_account pda ON p.past_due_id = pda.past_due_id
                            JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id
                            JOIN tbl_loan l ON li.loan_id = l.loan_id
                            WHERE p.promise_id = {currentPromiseId}";
            DataTable dt = db.displayRecords(query);
            if (dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["is_void"]))
            {
                BtnSave.IsEnabled = false;
                BtnSave.Opacity = 0.5;
                BtnSave.Content = "Action Restricted";
                
                TxtAmount.IsReadOnly = true;
                TxtRemarks.IsReadOnly = true;
                DpDate.IsEnabled = false;
                TxtAmount.Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9");
                TxtRemarks.Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9");

                MessageBox.Show("This promise record is associated with a voided loan and cannot be modified.", "Voided Loan", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dbManager db = new dbManager();
                string query = $"UPDATE tbl_promise SET " +
                               $"promise_amount = '{TxtAmount.Text}', " +
                               $"remarks = '{TxtRemarks.Text}', " +
                               $"updated_by = '{UserSession.EmployeeID}', " +
                               $"updated_at = NOW() " +
                               $"WHERE promise_id = '{currentPromiseId}'";

                
                db.executeQuery(query);

                MessageBox.Show("Update Successful!");
                this.Close();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

    }
}
