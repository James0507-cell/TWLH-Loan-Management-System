using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TWLH_Loan_Management_System
{
    internal class Transaction
    {
        dbManager db = new dbManager();
        string strQuery = "";

        public DataTable getTransactionRecords(string searchText = "", string type = "All Types")
        {
            strQuery = "SELECT t.*, " +
                       "CONCAT(c.first_name, ' ', c.last_name) as ClientName, " +
                       "CONCAT(e.first_name, ' ', e.last_name) as RecordedBy, " +
                       "IFNULL(CONCAT(e2.first_name, ' ', e2.last_name), 'System') as updated_by_name " +
                       "FROM tbl_transaction t " +
                       "LEFT JOIN tbl_client c ON t.client_id = c.client_id " +
                       "LEFT JOIN tbl_employee e ON t.recorded_by = e.employee_id " +
                       "LEFT JOIN tbl_employee e2 ON t.updated_by = e2.employee_id " +
                       "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(searchText))
            {
                strQuery += $"AND (c.first_name LIKE '%{searchText}%' OR c.last_name LIKE '%{searchText}%' OR t.transaction_id LIKE '%{searchText}%') ";
            }

            if (type != "All Types")
            {
                strQuery += $"AND t.transaction_type = '{type}' ";
            }

            strQuery += "ORDER BY t.transaction_id DESC";

            return db.displayRecords(strQuery);
        }

        public DataTable getInstallmentTransactons(int installmentID)
        {
            strQuery = $"select * from tbl_installment_payment where installment_id = '{installmentID}'";
            return db.displayRecords(strQuery);
        }

        public DataTable getTransactionByID(int transactonID)
        {
            strQuery = $"select * from tbl_transaction where transaction_id = '{transactonID}'";
            return db.displayRecords(strQuery);
        }

        public void addTransacton(int clientID, string transactionType, double transactionAmount, int recordedBy)
        {
            strQuery = $"insert into tbl_transaction(client_id, transaction_type, transaction_amount, recorded_by) " +
                        $"values('{clientID}', '{transactionType}', '{transactionAmount}', '{recordedBy}')";
            db.sqlManager(strQuery);
        }

        public StackPanel installmentTransactionCards(int installment_id)
        {
            StackPanel stk = new StackPanel();

            //card design here

            return stk;
        }

        public UIElement createTransactionDetailCard(int paymentId, decimal paymentAmount, string recordedByName, DateTime createdAt, string status = "Confirmed", int installmentId = 0)
        {
            Border card = new Border
            {
                Margin = new Thickness(0, 0, 0, 10),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(15),
                Opacity = status == "Void" ? 0.6 : 1.0
            };

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Column 1: Info
            StackPanel col1 = new StackPanel();
            string idLabel = installmentId > 0 ? $"Payment #{paymentId} (Inst. #{installmentId})" : $"ID: #{paymentId}";
            col1.Children.Add(new TextBlock { Text = idLabel, FontWeight = FontWeights.SemiBold, FontSize = 14, Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B") });
            col1.Children.Add(new TextBlock { Text = $"Recorded By: {recordedByName}", FontSize = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), Margin = new Thickness(0, 5, 0, 0) });
            
            // Status Badge
            Border statusBadge = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 8, 0, 0),
                Padding = new Thickness(8, 2, 8, 2),
                CornerRadius = new CornerRadius(4),
                Background = (Brush)new BrushConverter().ConvertFrom(status == "Confirmed" ? "#D1FAE5" : "#FEE2E2")
            };
            statusBadge.Child = new TextBlock
            {
                Text = status.ToUpper(),
                FontSize = 9,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)new BrushConverter().ConvertFrom(status == "Confirmed" ? "#10B981" : "#EF4444")
            };
            col1.Children.Add(statusBadge);
            
            Grid.SetColumn(col1, 0);
            grid.Children.Add(col1);

            // Column 2: Details
            StackPanel col2 = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
            col2.Children.Add(new TextBlock 
            { 
                Text = (status == "Void" ? "- " : "") + $"₱{paymentAmount:N2}", 
                FontSize = 18, 
                FontWeight = FontWeights.SemiBold, 
                Foreground = (Brush)new BrushConverter().ConvertFrom(status == "Confirmed" ? "#3044FF" : "#EF4444"), 
                HorizontalAlignment = HorizontalAlignment.Right 
            });
            col2.Children.Add(new TextBlock { Text = $"{createdAt:MMM dd, yyyy hh:mm tt}", FontSize = 11, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 5, 0, 0) });
            Grid.SetColumn(col2, 1);
            grid.Children.Add(col2);

            card.Child = grid;
            return card;
        }
    }
}
