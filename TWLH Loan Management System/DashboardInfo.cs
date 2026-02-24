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
    class DashboardInfo
    {
        int id = 0;

        public DashboardInfo()
        {

        }
        public DashboardInfo(int id)
        {
            this.id = id;
        }

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

        public double getCollectionRate()
        {
            strquery =  "select " +
                        "ifnull((select count(li.installment_id) " +
                        " from tbl_loan_installment li " +
                        " inner join tbl_loan lo on li.loan_id = lo.loan_id " +
                        " where lo.loan_status <> 'Paid' " +
                        " and li.installment_status = 'Paid') " +
                        "/ " +
                        "(select count(li.installment_id) " +
                        " from tbl_loan_installment li " +
                        " inner join tbl_loan lo on li.loan_id = lo.loan_id " +
                        " where lo.loan_status <> 'Paid'), 0)";


            DataTable dt = db.displayRecords(strquery);
            if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
            {
                return Convert.ToDouble(dt.Rows[0][0]);
            }
            return 0;
        }

        public string getCollectedAmount()
        {
            strquery =  $"select sum(li.installment_amount) " +
                        $"from tbl_loan_installment li inner join tbl_loan lo " +
                        $"on li.loan_id = lo.loan_id " +
                        $"where lo.loan_status <> 'Paid' " +
                        $"and " +
                        $"li.installment_status = 'Paid'";
                        return db.displayRecords(strquery).Rows[0][0].ToString();
        }

        public string getMyAssignmentCount(int userId)
        {
            strquery = $"select count(assignment_id) from tbl_collection_assignment " +
                       $"where assigned_to = {userId} and assignment_status = 'In Progress'";
            DataTable dt = db.displayRecords(strquery);
            return (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value) ? dt.Rows[0][0].ToString() : "0";
        }

        public string getOverdueAssignmentCount(int userId)
        {
            strquery = $"select count(assignment_id) from tbl_collection_assignment " +
                       $"where assigned_to = {userId} and assignment_status = 'Overdue'";
            DataTable dt = db.displayRecords(strquery);
            return (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value) ? dt.Rows[0][0].ToString() : "0";
        }

        public DataTable getMyAssignments(int userId)
        {
            strquery = $@"SELECT 
                            ca.assignment_id,
                            CONCAT(c.first_name, ' ', c.last_name) AS client_name,
                            vw.total_amount_to_pay,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
                            ca.due_date,
                            ca.assignment_status
                        FROM tbl_collection_assignment ca
                        JOIN tbl_past_due_account pda ON ca.past_due_id = pda.past_due_id
                        JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id
                        JOIN tbl_loan l ON li.loan_id = l.loan_id
                        JOIN tbl_client c ON l.client_id = c.client_id
                        JOIN vw_total_amount_installment vw ON li.installment_id = vw.installment_id
                        WHERE ca.assigned_to = {userId} 
                        AND ca.assignment_status IN ('In Progress', 'Overdue')";

            return db.displayRecords(strquery);
        }
        
        public StackPanel collectionAssignmentCard(int userID)
        {
            StackPanel container = new StackPanel();
            DataTable dt = getMyAssignments(userID);

            if (dt.Rows.Count == 0)
            {
                TextBlock noData = new TextBlock
                {
                    Text = "No active assignments found.",
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0),
                    Foreground = Brushes.Gray,
                    FontStyle = FontStyles.Italic
                };
                container.Children.Add(noData);
                return container;
            }

            foreach (DataRow row in dt.Rows)
            {
                Border card = new Border
                {
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(15),
                    BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFDDDDDD"),
                    BorderThickness = new Thickness(1)
                };

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                // Icon
                ImageAwesome icon = new ImageAwesome
                {
                    Icon = FontAwesomeIcon.UserCircle,
                    Width = 30,
                    Height = 30,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF2196F3"),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 15, 0)
                };
                Grid.SetColumn(icon, 0);
                grid.Children.Add(icon);

                // Details
                StackPanel details = new StackPanel { VerticalAlignment = System.Windows.VerticalAlignment.Center };
                TextBlock txtClient = new TextBlock
                {
                    Text = row["client_name"].ToString(),
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#333")
                };
                TextBlock txtDueDate = new TextBlock
                {
                    Text = "Due: " + Convert.ToDateTime(row["due_date"]).ToString("MMM dd, yyyy"),
                    FontSize = 12,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF757575"),
                    Margin = new Thickness(0, 2, 0, 0)
                };
                details.Children.Add(txtClient);
                details.Children.Add(txtDueDate);
                Grid.SetColumn(details, 1);
                grid.Children.Add(details);

                // Amount and Status
                StackPanel amountStatus = new StackPanel { VerticalAlignment = System.Windows.VerticalAlignment.Center, HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
                TextBlock txtAmount = new TextBlock
                {
                    Text = "₱" + string.Format("{0:N2}", row["total_amount_to_pay"]),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF44336"),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right
                };
                
                string status = row["assignment_status"].ToString();
                Border badge = new Border
                {
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(8, 2, 8, 2),
                    Margin = new Thickness(0, 5, 0, 0),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(status == "Overdue" ? "#FFFFEBEE" : "#FFE3F2FD")
                };
                TextBlock txtStatus = new TextBlock
                {
                    Text = status.ToUpper(),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(status == "Overdue" ? "#FFC62828" : "#FF1565C0")
                };
                badge.Child = txtStatus;

                amountStatus.Children.Add(txtAmount);
                amountStatus.Children.Add(badge);
                Grid.SetColumn(amountStatus, 2);
                grid.Children.Add(amountStatus);

                card.Child = grid;
                container.Children.Add(card);
            }

            return container;
        }
    }
}
