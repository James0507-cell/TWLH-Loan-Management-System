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
    /// Interaction logic for PromiseForm.xaml
    /// </summary>
    public partial class PromiseForm : Window
    {
        private int _pastDueID;
        private PromiseToPay _promise = new PromiseToPay();

        public PromiseForm(int pastDueID)
        {
            InitializeComponent();
            this._pastDueID = pastDueID;
            dpDate.SelectedDate = DateTime.Now.AddDays(7); // Suggest next week
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    MessageBox.Show("Please enter the promise amount.");
                    return;
                }

                if (dpDate.SelectedDate == null)
                {
                    MessageBox.Show("Please select a payment date.");
                    return;
                }

                decimal amount = decimal.Parse(txtAmount.Text);
                string date = dpDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                string remarks = txtRemarks.Text;
                int recordedBy = 2; // Placeholder for staff ID

                _promise.addPromise(_pastDueID, amount, date, remarks, recordedBy);

                MessageBox.Show("Promise record saved successfully.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving promise record: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            try { this.DragMove(); } catch { }
        }
    }
}
