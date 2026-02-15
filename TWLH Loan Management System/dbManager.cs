using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;


namespace TWLH_Loan_Management_System
{
    internal class dbManager
    {
        private MySqlConnection dbConn;
        private MySqlCommand dbCom;
        private MySqlDataAdapter da;
        private DataTable dt;

        private const string strConn = "server=localhost;user id=root;password=;database;db_twlh";


        public DataTable displayRecords(string query)
        {
            dbConn = new MySqlConnection(strConn);
            dbConn.Open();
            da = new MySqlDataAdapter(query, dbConn);
            dt = new DataTable();
            da.Fill(dt);
            dbConn.Close();
            return dt;
        }

        public void sqlManager(string query)
        {
            dbConn = new MySqlConnection(strConn);
            dbConn.Open();
            dbCom = new MySqlCommand(query, dbConn);
            dbCom.ExecuteNonQuery();
            dbConn.Close();
        }


    }
}
