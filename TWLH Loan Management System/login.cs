using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
namespace TWLH_Loan_Management_System
{
    class login
    {
        
        dbManager db = new dbManager();
        string sql = "";
        public string role = "";
        public string UserValidation(string username, string password_hash)
        {
            sql =   $"Select * from tbl_employee_credential " +
                    $"where username = '{username}'     " +
                    $"and password_hash = '{password_hash}'";
            DataTable dt = db.displayRecords(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                sql =   $"Select * from tbl_employee " +
                        $"where employee_id = '{dt.Rows[0][1]}'";
                dt = db.displayRecords(sql);
                role = dt.Rows[0][6].ToString();
                return role;
            } else
            {
                return null;
            }

        }

        public string getRole()
        {
            return role;
        }
    }
}
