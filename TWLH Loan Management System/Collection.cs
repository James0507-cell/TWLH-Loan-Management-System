using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TWLH_Loan_Management_System
{
    class Collection
    {
        dbManager db = new dbManager();
        string sqlQuery = "";

        public DataTable getCollectionRecord()
        {
            sqlQuery = $"select * from tbl_collection_assignment";
            return db.displayRecords(sqlQuery);
        }

        public DataTable getCollectionRecord(int pastDueID)
        {
            sqlQuery = $@"SELECT ca.*, CONCAT(e.first_name, ' ', e.last_name) as employee_name 
                        FROM tbl_collection_assignment ca
                        JOIN tbl_employee e ON ca.assigned_to = e.employee_id
                        WHERE ca.past_due_id = '{pastDueID}'";
            return db.displayRecords(sqlQuery);
        }
    }
}
