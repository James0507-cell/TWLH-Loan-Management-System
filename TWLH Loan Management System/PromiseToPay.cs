using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLH_Loan_Management_System
{
    class PromiseToPay
    {
        dbManager db = new dbManager();
        string sqlQuery = "";

        public DataTable getPromiseRecords()
        {
            sqlQuery = $"select * from tbl_promise";
            return db.displayRecords(sqlQuery);

        }

        public DataTable getPromiseRecords(int pastDueID)
        {
            sqlQuery = $"select * from tbl_promise where past_due_id = '{pastDueID}'";
            return db.displayRecords(sqlQuery);

        }

        public void addPromise(int pastDueID, decimal amount, string date, string remarks, int recordedBy)
        {
            sqlQuery = $"INSERT INTO tbl_promise (past_due_id, promise_amount, promise_payment_date, remarks, recorded_by) " +
                       $"VALUES ('{pastDueID}', '{amount}', '{date}', '{remarks}', '{recordedBy}')";
            db.sqlManager(sqlQuery);
        }
    }
}
