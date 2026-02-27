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
        private Collection _collection = new Collection();
        private dbManager _db = new dbManager();

        public CollectionAssignmentForm(int pastDueID)
        {
            InitializeComponent();
            this._pastDueID = pastDueID;
            LoadCollectors();
            cmbStatus.SelectedIndex = 0; // Default to In Progress
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
                int createdBy = UserSession.EmployeeID;

                _collection.addCollectionAssignment(_pastDueID, assignedTo, status, createdBy);

                MessageBox.Show("Collection assignment saved successfully.");
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
