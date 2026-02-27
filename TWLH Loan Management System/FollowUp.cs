using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLH_Loan_Management_System
{
    class FollowUp
    {
        dbManager db = new dbManager();
        string sqlQuery = "";


        public DataTable getFollowUpRecrods()
        {
            sqlQuery = $"select * from tbl_follow_up";
            return db.displayRecords(sqlQuery);
        }

        public DataTable getFollowUpRecrods(int pastDueID)
        {
            sqlQuery = $"select * from tbl_follow_up where past_due_id = '{pastDueID}'";
            return db.displayRecords(sqlQuery);
        }

    }
}
