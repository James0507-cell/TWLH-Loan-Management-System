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

        public DataTable getTransactionRecords()
        {
            strQuery = $"select * from tbl_transaction";
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

        public UIElement createTransactionDetailCard(int paymentId, decimal paymentAmount, string recordedByName, DateTime createdAt)
        {
            Border card = new Border
            {
                Margin = new Thickness(0, 0, 0, 10),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(15)
            };

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Column 1: Transaction Info
            StackPanel col1 = new StackPanel();
            col1.Children.Add(new TextBlock { Text = $"Transaction ID: {paymentId}", FontWeight = FontWeights.SemiBold, FontSize = 14, Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B") });
            col1.Children.Add(new TextBlock { Text = $"Recorded By: {recordedByName}", FontSize = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), Margin = new Thickness(0, 5, 0, 0) });
            Grid.SetColumn(col1, 0);
            grid.Children.Add(col1);

            // Column 2: Payment Details
            StackPanel col2 = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
            col2.Children.Add(new TextBlock { Text = $"₱{paymentAmount:N2}", FontSize = 18, FontWeight = FontWeights.SemiBold, Foreground = (Brush)new BrushConverter().ConvertFrom("#3044FF"), HorizontalAlignment = HorizontalAlignment.Right });
            col2.Children.Add(new TextBlock { Text = $"{createdAt:MMM dd, yyyy hh:mm tt}", FontSize = 11, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 5, 0, 0) });
            Grid.SetColumn(col2, 1);
            grid.Children.Add(col2);

            card.Child = grid;
            return card;
        }
    }
}
