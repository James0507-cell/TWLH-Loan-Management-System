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
    /// Interaction logic for FollowUpForm.xaml
    /// </summary>
    public partial class FollowUpForm : Window
    {
        private int _pastDueID;
        private int _followUpID = 0;
        private FollowUp _followUp = new FollowUp();
        private bool _isEdit = false;

        public FollowUpForm(int pastDueID)
        {
            InitializeComponent();
            this._pastDueID = pastDueID;
            dpDate.SelectedDate = DateTime.Now;
            cmbType.SelectedIndex = 0;
            checkIfVoid();
        }

        public FollowUpForm(DataRow row)
        {
            InitializeComponent();
            this._isEdit = true;
            this._followUpID = Convert.ToInt32(row["follow_up_id"]);
            this._pastDueID = Convert.ToInt32(row["past_due_id"]);
            
            dpDate.SelectedDate = Convert.ToDateTime(row["follow_up_date"]);
            txtNotes.Text = row["notes"].ToString();
            
            string type = row["follow_up_type"].ToString();
            foreach (ComboBoxItem item in cmbType.Items)
            {
                if (item.Content.ToString() == type)
                {
                    cmbType.SelectedItem = item;
                    break;
                }
            }

            // Update UI for Edit
            TextBlock headerTitle = (TextBlock)this.FindName("headerTitle");
            if (headerTitle != null) headerTitle.Text = "Edit Follow Up";
            btnSave.Content = "Update Record";
            checkIfVoid();
        }

        private void checkIfVoid()
        {
            PastDueAccount pda = new PastDueAccount();
            if (pda.isLoanVoid(_pastDueID))
            {
                btnSave.IsEnabled = false;
                btnSave.Opacity = 0.5;
                btnSave.Content = "Action Restricted";
                
                dpDate.IsEnabled = false;
                cmbType.IsEnabled = false;
                txtNotes.IsReadOnly = true;
                txtNotes.Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9");

                TextBlock headerTitle = (TextBlock)this.FindName("headerTitle");
                if (headerTitle != null) headerTitle.Text += " (VOIDED LOAN)";
                
                MessageBox.Show("This follow-up is associated with a voided loan and cannot be modified.", "Voided Loan", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpDate.SelectedDate == null)
                {
                    MessageBox.Show("Please select a follow up date.");
                    return;
                }

                if (cmbType.SelectedItem == null)
                {
                    MessageBox.Show("Please select a follow up type.");
                    return;
                }

                string date = dpDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                string type = ((ComboBoxItem)cmbType.SelectedItem).Content.ToString();
                string notes = txtNotes.Text;
                int recordedBy = UserSession.EmployeeID;

                if (_isEdit)
                {
                    _followUp.updateFollowUp(_followUpID, _pastDueID, date, type, notes, recordedBy);
                    MessageBox.Show("Follow up record updated successfully.");
                }
                else
                {
                    _followUp.addFollowUp(_pastDueID, date, type, notes, recordedBy);
                    MessageBox.Show("Follow up record saved successfully.");
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving follow up: " + ex.Message);
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
