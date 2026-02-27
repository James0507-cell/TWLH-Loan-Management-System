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
    /// Interaction logic for FollowUpForm.xaml
    /// </summary>
    public partial class FollowUpForm : Window
    {
        private int _pastDueID;
        private FollowUp _followUp = new FollowUp();

        public FollowUpForm(int pastDueID)
        {
            InitializeComponent();
            this._pastDueID = pastDueID;
            dpDate.SelectedDate = DateTime.Now;
            cmbType.SelectedIndex = 0;
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

                _followUp.addFollowUp(_pastDueID, date, type, notes, recordedBy);

                MessageBox.Show("Follow up record saved successfully.");
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
