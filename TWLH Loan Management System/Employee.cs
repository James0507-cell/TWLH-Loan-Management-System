using System;
using System.Data;
using System.Collections.Generic;

namespace TWLH_Loan_Management_System
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Role { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEmployeeActive { get; set; }
        public bool IsCredentialActive { get; set; }
        public int Age { get; set; }
        public int UpdatedBy { get; set; }

        private dbManager db = new dbManager();

        public bool Save()
        {
            try
            {
                int empActive = IsEmployeeActive ? 1 : 0;
                int credActive = IsCredentialActive ? 1 : 0;

                // 1. Insert into tbl_employee
                string employeeQuery = $@"INSERT INTO tbl_employee 
                    (first_name, middle_name, last_name, gender, date_of_birth, role, contact_number, email, is_active) 
                    VALUES ('{FirstName}', '{MiddleName}', '{LastName}', '{Gender}', '{DateOfBirth:yyyy-MM-dd}', '{Role}', '{ContactNumber}', '{Email}', {empActive})";
                
                db.sqlManager(employeeQuery);

                // Get the last inserted ID
                DataTable dt = db.displayRecords("SELECT LAST_INSERT_ID() as id");
                int newId = Convert.ToInt32(dt.Rows[0]["id"]);
                this.EmployeeId = newId;

                // 2. Insert into tbl_employee_credential
                string credentialQuery = $@"INSERT INTO tbl_employee_credential 
                    (employee_id, username, password_hash, is_active) 
                    VALUES ({newId}, '{Username}', '{Password}', {credActive})";
                db.sqlManager(credentialQuery);

                // 3. Insert into role-specific tables
                string roleTable = "";
                if (Role == "Admin") roleTable = "tbl_admin";
                else if (Role == "Staff") roleTable = "tbl_staff";
                else if (Role == "Loan Collector") roleTable = "tbl_collector";

                if (!string.IsNullOrEmpty(roleTable))
                {
                    string roleQuery = $"INSERT INTO {roleTable} (employee_id, is_active) VALUES ({newId}, {empActive})";
                    db.sqlManager(roleQuery);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update()
        {
            try
            {
                int empActive = IsEmployeeActive ? 1 : 0;
                int credActive = IsCredentialActive ? 1 : 0;

                string query = $@"UPDATE tbl_employee SET 
                    first_name = '{FirstName}', 
                    middle_name = '{MiddleName}', 
                    last_name = '{LastName}', 
                    gender = '{Gender}', 
                    date_of_birth = '{DateOfBirth:yyyy-MM-dd}', 
                    role = '{Role}', 
                    contact_number = '{ContactNumber}', 
                    email = '{Email}',
                    is_active = {empActive},
                    updated_by = {UpdatedBy}
                    WHERE employee_id = {EmployeeId}";
                
                db.sqlManager(query);

                // 2. Update credentials status
                string credUpdateQuery = $@"UPDATE tbl_employee_credential SET 
                    is_active = {credActive}
                    WHERE employee_id = {EmployeeId}";
                db.sqlManager(credUpdateQuery);

                // 3. Handle Role-Specific Tables
                // Deactivate in all role tables first to ensure clean state
                db.sqlManager($"UPDATE tbl_admin SET is_active = 0 WHERE employee_id = {EmployeeId}");
                db.sqlManager($"UPDATE tbl_staff SET is_active = 0 WHERE employee_id = {EmployeeId}");
                db.sqlManager($"UPDATE tbl_collector SET is_active = 0 WHERE employee_id = {EmployeeId}");

                // Activate/Insert into the target role table
                string roleTable = "";
                if (Role == "Admin") roleTable = "tbl_admin";
                else if (Role == "Staff") roleTable = "tbl_staff";
                else if (Role == "Loan Collector") roleTable = "tbl_collector";

                if (!string.IsNullOrEmpty(roleTable))
                {
                    // Check if record exists
                    DataTable dt = db.displayRecords($"SELECT 1 FROM {roleTable} WHERE employee_id = {EmployeeId}");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Update existing
                        db.sqlManager($"UPDATE {roleTable} SET is_active = {empActive} WHERE employee_id = {EmployeeId}");
                    }
                    else
                    {
                        // Insert new
                        db.sqlManager($"INSERT INTO {roleTable} (employee_id, is_active) VALUES ({EmployeeId}, {empActive})");
                    }
                }

                // 4. Optionally update username/password if provided
                if (!string.IsNullOrEmpty(Username))
                {
                    string credQuery = $@"UPDATE tbl_employee_credential SET 
                        username = '{Username}' 
                        WHERE employee_id = {EmployeeId}";
                    db.sqlManager(credQuery);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static DataTable GetAllEmployees()
        {
            dbManager db = new dbManager();
            string query = @"SELECT 
                        e.employee_id, 
                        e.first_name, 
                        e.middle_name, 
                        e.last_name,
                        e.gender,
                        e.date_of_birth,
                        e.contact_number,
                        e.role AS Role, 
                        e.email AS Email, 
                        e.is_active AS IsEmployeeActive,
                        ec.is_active AS IsCredentialActive,
                        TIMESTAMPDIFF(YEAR, e.date_of_birth, CURDATE()) AS Age,
                        e.created_at AS DateHired,
                        CONCAT(e.first_name, ' ', e.last_name) AS Name, 
                        IF(e.is_active = 1, 'Active', 'Inactive') AS Status,
                        ec.username,
                        e.updated_at,
                        IFNULL(CONCAT(e2.first_name, ' ', e2.last_name), 'System') as updated_by_name
                     FROM tbl_employee e
                     LEFT JOIN tbl_employee_credential ec ON e.employee_id = ec.employee_id
                     LEFT JOIN tbl_employee e2 ON e.updated_by = e2.employee_id
                     ORDER BY e.employee_id ASC";
            return db.displayRecords(query);
        }

        public static bool Deactivate(int id, int updatedBy)
        {
            try
            {
                dbManager db = new dbManager();
                // 1. Deactivate main employee record and set updated_by
                db.sqlManager($"UPDATE tbl_employee SET is_active = 0, updated_by = {updatedBy} WHERE employee_id = {id}");
                
                // 2. Deactivate login credentials
                db.sqlManager($"UPDATE tbl_employee_credential SET is_active = 0 WHERE employee_id = {id}");

                // 3. Deactivate role-specific access
                db.sqlManager($"UPDATE tbl_admin SET is_active = 0 WHERE employee_id = {id}");
                db.sqlManager($"UPDATE tbl_staff SET is_active = 0 WHERE employee_id = {id}");
                db.sqlManager($"UPDATE tbl_collector SET is_active = 0 WHERE employee_id = {id}");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Deactivate(int id)
        {
            try
            {
                dbManager db = new dbManager();
                // 1. Deactivate main employee record
                db.sqlManager($"UPDATE tbl_employee SET is_active = 0 WHERE employee_id = {id}");
                
                // 2. Deactivate login credentials
                db.sqlManager($"UPDATE tbl_employee_credential SET is_active = 0 WHERE employee_id = {id}");

                // 3. Deactivate role-specific access
                db.sqlManager($"UPDATE tbl_admin SET is_active = 0 WHERE employee_id = {id}");
                db.sqlManager($"UPDATE tbl_staff SET is_active = 0 WHERE employee_id = {id}");
                db.sqlManager($"UPDATE tbl_collector SET is_active = 0 WHERE employee_id = {id}");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
