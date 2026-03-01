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
                               $"remarks = '{TxtRemarks.Text}' " +
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
