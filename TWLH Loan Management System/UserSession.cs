using System;

namespace TWLH_Loan_Management_System
{
    public static class UserSession
    {
        public static int EmployeeID { get; set; } = 0;
        public static string Role { get; set; } = string.Empty;

        public static void Login(int employeeID, string role)
        {
            EmployeeID = employeeID;
            Role = role;
        }

        public static void Logout()
        {
            EmployeeID = 0;
            Role = string.Empty;
        }

        public static bool IsLoggedIn()
        {
            return EmployeeID != 0;
        }
    }
}
