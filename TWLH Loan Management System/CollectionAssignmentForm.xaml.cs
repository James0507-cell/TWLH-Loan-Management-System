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
    /// Interaction logic for CollectionAssignmentForm.xaml
    /// </summary>
    public partial class CollectionAssignmentForm : Window
    {
        private int _pastDueID;
        private int _assignmentID = 0; // 0 means new assignment
        private Collection _collection = new Collection();
        private dbManager _db = new dbManager();

        public CollectionAssignmentForm(int pastDueID)
        {
            InitializeComponent();
            this._pastDueID = pastDueID;
            LoadCollectors();
            cmbStatus.SelectedIndex = 0; // Default to In Progress
            checkIfVoid();
        }

        // New constructor for updating
        public CollectionAssignmentForm(DataRow row)
        {
            InitializeComponent();
            this._assignmentID = Convert.ToInt32(row["assignment_id"]);
            this._pastDueID = Convert.ToInt32(row["past_due_id"]);
            
            LoadCollectors();
            
            // Set values from row
            cmbCollector.SelectedValue = row["assigned_to"];
            
            // Set status in combo box
            string currentStatus = row["assignment_status"].ToString();
            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Content.ToString() == currentStatus)
                {
                    cmbStatus.SelectedItem = item;
                    break;
                }
            }
            
            btnSave.Content = "Update Assignment";
            this.Title = "Update Collection Assignment";
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
                
                cmbCollector.IsEnabled = false;
                cmbStatus.IsEnabled = false;

                TextBlock headerTitle = (TextBlock)this.FindName("headerTitle");
                if (headerTitle != null) headerTitle.Text += " (VOIDED LOAN)";
                
                MessageBox.Show("This collection assignment is associated with a voided loan and cannot be modified.", "Voided Loan", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadCollectors()
        {
            try
            {
                // Join with tbl_collector to ensure they are active collectors
                string query = @"SELECT e.employee_id, CONCAT(e.first_name, ' ', e.last_name) as collector_display 
                                 FROM tbl_employee e
                                 JOIN tbl_collector c ON e.employee_id = c.employee_id
                                 WHERE c.is_active = 1";
                DataTable dt = _db.displayRecords(query);
                cmbCollector.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading collectors: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbCollector.SelectedValue == null)
                {
                    MessageBox.Show("Please select a collector.");
                    return;
                }

                if (cmbStatus.SelectedItem == null)
                {
                    MessageBox.Show("Please select a status.");
                    return;
                }

                int assignedTo = Convert.ToInt32(cmbCollector.SelectedValue);
                string status = ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString();
                
                if (_assignmentID == 0)
                {
                    // New assignment
                    int createdBy = UserSession.EmployeeID;
                    _collection.addCollectionAssignment(_pastDueID, assignedTo, status, createdBy);
                    MessageBox.Show("Collection assignment saved successfully.");
                }
                else
                {
                    // Update existing
                    int updatedBy = UserSession.EmployeeID;
                    _collection.updateCollectionAssignment(_assignmentID, assignedTo, status, updatedBy);
                    MessageBox.Show("Collection assignment updated successfully.");
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving collection assignment: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
    }
}
