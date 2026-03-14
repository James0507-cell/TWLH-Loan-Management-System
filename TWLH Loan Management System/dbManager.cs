    using MySql.Data.MySqlClient;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Forms;


    namespace TWLH_Loan_Management_System
    {
        internal class dbManager
        {
            private MySqlConnection dbConn;
            private MySqlCommand dbCom;
            private MySqlDataAdapter da;
            private DataTable dt;
       

            private const string strConn = "server=localhost;user id=root;password=;database=db_twlh";


            public DataTable displayRecords(string query)
            {
                try
                {
                    dbConn = new MySqlConnection(strConn);
                    dbConn.Open();
                    da = new MySqlDataAdapter(query, dbConn);
                    dt = new DataTable();
                    da.Fill(dt);
                    dbConn.Close();
                    return dt;
                }
                catch (Exception ex)
                {
                    if (dbConn.State == ConnectionState.Open) dbConn.Close();
                    throw new Exception("Database Error (displayRecords): " + ex.Message);
                }
            }

            public void displayRecords(String strQuerry, DataGrid DG)
            {
                try
                {
                    dbConn = new MySqlConnection(strConn);
                    dbConn.Open();
                    da = new MySqlDataAdapter(strQuerry, dbConn);
                    dt = new DataTable();
                    da.Fill(dt);
                    DG.ItemsSource = dt.DefaultView;
                    dbConn.Close();
                }
                catch (Exception ex)
                {
                    if (dbConn.State == ConnectionState.Open) dbConn.Close();
                    throw new Exception("Database Error (displayRecords to DataGrid): " + ex.Message);
                }
            }

            public void sqlManager(string query)
            {
                try
                {
                    dbConn = new MySqlConnection(strConn);
                    dbConn.Open();
                    dbCom = new MySqlCommand(query, dbConn);
                    dbCom.ExecuteNonQuery();
                    dbConn.Close();
                }
                catch (Exception ex)
                {
                    if (dbConn.State == ConnectionState.Open) dbConn.Close();
                    throw new Exception("Database Error (sqlManager): " + ex.Message);
                }
            }

            public void executeQuery(string query)
            {
                try
                {
                    using (MySqlConnection dbConn = new MySqlConnection(strConn))
                    {
                        dbConn.Open();
                        MySqlCommand cmd = new MySqlCommand(query, dbConn);
                        cmd.ExecuteNonQuery();
                        dbConn.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Database Error (executeQuery): " + ex.Message);
                }
            }


    }
    }
