using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace TWLH_Loan_Management_System
{
    public partial class ScheduleFollowUpDialog : Window
    {
        public DateTime SelectedDateTime { get; private set; }

        public ScheduleFollowUpDialog()
        {
            InitializeComponent();
            dpDate.SelectedDate = DateTime.Now.AddDays(1);
        }

        private void Time_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnHourUp_Click(object sender, RoutedEventArgs e)
        {
            int hour = int.Parse(txtHour.Text);
            hour = hour >= 12 ? 1 : hour + 1;
            txtHour.Text = hour.ToString("D2");
        }

        private void btnHourDown_Click(object sender, RoutedEventArgs e)
        {
            int hour = int.Parse(txtHour.Text);
            hour = hour <= 1 ? 12 : hour - 1;
            txtHour.Text = hour.ToString("D2");
        }

        private void btnMinUp_Click(object sender, RoutedEventArgs e)
        {
            int min = int.Parse(txtMin.Text);
            min = min >= 59 ? 0 : min + 1;
            txtMin.Text = min.ToString("D2");
        }

        private void btnMinDown_Click(object sender, RoutedEventArgs e)
        {
            int min = int.Parse(txtMin.Text);
            min = min <= 0 ? 59 : min - 1;
            txtMin.Text = min.ToString("D2");
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpDate.SelectedDate == null)
                {
                    MessageBox.Show("Please select a date.");
                    return;
                }

                int hour = int.Parse(txtHour.Text);
                int minute = int.Parse(txtMin.Text);

                if (hour < 1 || hour > 12 || minute < 0 || minute > 59)
                {
                    MessageBox.Show("Please enter a valid time.");
                    return;
                }

                bool isPM = rbPM.IsChecked == true;
                if (isPM && hour < 12) hour += 12;
                if (!isPM && hour == 12) hour = 0;

                DateTime date = dpDate.SelectedDate.Value;
                SelectedDateTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Input Error: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
