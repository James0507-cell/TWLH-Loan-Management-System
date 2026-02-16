using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using FontAwesome.WPF;

namespace TWLH_Loan_Management_System
{
    class adminDashboardInfo
    {
        dbManager db = new dbManager();
        string strquery = "";
        
        public string getTotalActiveLoans()
        {
            strquery =   $"select count(loan_id) from tbl_loan " +
                         $"where loan_status = 'Active'";
            return db.displayRecords(strquery).Rows[0][0].ToString();
        }
        public string getTotalLoanAmount()
        {
            strquery =  $"select sum(loan_amount) from tbl_loan " +
                        $"where loan_status = 'Active'";
            return db.displayRecords(strquery).Rows[0][0].ToString();
        }

        public string getTotalClients()
        {
            strquery = $"select count(client_id) from tbl_client";
            return db.displayRecords(strquery).Rows[0][0].ToString();
        }
        public string getTotalEmployees()
        {
            strquery = $"select count(employee_id) from tbl_employee where is_active = '{1}'";
            return db.displayRecords(strquery).Rows[0][0].ToString();
        }

        public string getTotalPaidInstallment()
        {
            strquery = $"select count(installment_id) from tbl_loan_installment where installment_status = 'Paid'";
            return db.displayRecords(strquery).Rows[0][0].ToString();
        }
        public string getTotalPastDueAccount()
        {
            strquery = $"select count(installment_id) from tbl_past_due_account where past_due_status <> 'Resolved'";
            return db.displayRecords(strquery).Rows[0][0].ToString();
        }

        public string getTotalPastDueAmount()
        {
            strquery = $"select sum(installment_amount) from tbl_loan_installment where installment_status = 'Past Due'";
            return db.displayRecords(strquery).Rows[0][0].ToString();
        }

        public DataTable getOverDueList()
        {
            strquery =  $"select vw.installment_id, " +
                        $"(select concat(first_name, ' ', last_name) as full_name from tbl_client where client_id = vw.client_id) as full_name, " +
                        $"installment_amount, " +
                        $"timestampdiff(day, installment_due_date, curdate()) as total_overdue_day " +
                        $"from vw_total_amount_installment vw " +
                        $"where installment_status = 'Past Due';";

            return db.displayRecords(strquery);
        }

        
    }
}
