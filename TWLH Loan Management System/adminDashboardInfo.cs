using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
