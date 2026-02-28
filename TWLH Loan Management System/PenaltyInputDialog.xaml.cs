using System;
using System.Windows;

namespace TWLH_Loan_Management_System
{
    public partial class PenaltyInputDialog : Window
    {
        public decimal PenaltyAmount { get; private set; }

        public PenaltyInputDialog()
        {
            InitializeComponent();
            txtPenalty.Focus();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtPenalty.Text, out decimal amount))
            {
                if (amount < 0)
                {
                    MessageBox.Show("Penalty amount cannot be negative.");
                    return;
                }
                PenaltyAmount = amount;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric amount.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
