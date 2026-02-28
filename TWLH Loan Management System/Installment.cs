using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using FontAwesome.WPF;

namespace TWLH_Loan_Management_System
{
    internal class Installment
    {
        dbManager db = new dbManager();
        string strQuery = "";

        public DataTable getInstallmentsByLoanID(int loanID)
        {
            strQuery = $"SELECT * FROM tbl_loan_installment WHERE loan_id = {loanID} ORDER BY installment_due_date ASC";
            return db.displayRecords(strQuery);
        }
        public DataTable getTotalAmountNeededToPay(int loanID)
        {
            strQuery = $"select * from vw_total_amount_installment where loan_id = {loanID}";
            return db.displayRecords(strQuery);
        }

        public void setInstallmentToPastDueWithPenalty(int installmentID, decimal penalty)
        {
            // 1. Mark installment as Past Due (This triggers tbl_past_due_account insert via DB trigger)
            strQuery = $"UPDATE tbl_loan_installment SET installment_status = 'Past Due', updated_by = '{UserSession.EmployeeID}' WHERE installment_id = {installmentID}";
            db.sqlManager(strQuery);

            // 2. Update the newly created past due account record with the penalty
            // Since the trigger just ran, we look for the most recent 'Open' record for this installment
            strQuery = $"UPDATE tbl_past_due_account SET penalty_added = {penalty}, updated_by = '{UserSession.EmployeeID}' WHERE installment_id = {installmentID} AND past_due_status = 'Open'";
            db.sqlManager(strQuery);
        }

        public decimal displayInstallmentCards(StackPanel container, int loanID)
        {
            DataTable dt = getTotalAmountNeededToPay(loanID);
            container.Children.Clear();
            decimal grandTotal = 0;

            foreach (DataRow row in dt.Rows)
            {
                int installmentID = Convert.ToInt32(row["installment_id"]);
                decimal amount = Convert.ToDecimal(row["installment_amount"]);
                DateTime dueDate = Convert.ToDateTime(row["installment_due_date"]);
                string status = row["installment_status"].ToString();
                decimal penalty = row["penalty_added"] != DBNull.Value ? Convert.ToDecimal(row["penalty_added"]) : 0;
                decimal totalToPay = Convert.ToDecimal(row["total_amount_to_pay"]);
                
                // Ensure remaining balance is not negative
                if (totalToPay < 0) totalToPay = 0;

                // Get partial payment status from tbl_loan_installment
                DataTable dtInst = db.displayRecords($"SELECT is_partially_paid FROM tbl_loan_installment WHERE installment_id = {installmentID}");
                bool isPartiallyPaid = dtInst.Rows.Count > 0 && Convert.ToBoolean(dtInst.Rows[0]["is_partially_paid"]);

                grandTotal += totalToPay;

                Border card = new Border
                {
                    Margin = new Thickness(0, 0, 0, 15),
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(12),
                    BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                    BorderThickness = new Thickness(1.5),
                    Padding = new Thickness(20)
                };

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Column 1: Installment Info
                StackPanel col1 = new StackPanel();
                col1.Children.Add(new TextBlock { Text = $"Installment #{installmentID}", FontWeight = FontWeights.SemiBold, FontSize = 16, Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B") });
                col1.Children.Add(new TextBlock { Text = $"Due Date: {dueDate:MMM dd, yyyy}", FontSize = 13, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), Margin = new Thickness(0, 5, 0, 0) });
                
                Border statusChip = new Border
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 10, 0, 0),
                    Padding = new Thickness(10, 4, 10, 4),
                    CornerRadius = new CornerRadius(6),
                    Background = (Brush)new BrushConverter().ConvertFrom(status == "Paid" ? "#D1FAE5" : status == "Past Due" ? "#FEE2E2" : "#E0E7FF")
                };
                statusChip.Child = new TextBlock
                {
                    Text = status,
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom(status == "Paid" ? "#10B981" : status == "Past Due" ? "#EF4444" : "#3044FF")
                };
                col1.Children.Add(statusChip);
                Grid.SetColumn(col1, 0);
                grid.Children.Add(col1);

                // Column 2: Payment Details
                StackPanel col2 = new StackPanel();
                col2.Children.Add(new TextBlock { Text = "INSTALLMENT AMOUNT", FontSize = 10, FontWeight = FontWeights.Normal, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B") });
                col2.Children.Add(new TextBlock { Text = $"₱{amount:N2}", FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B") });
                
                if (isPartiallyPaid)
                {
                    col2.Children.Add(new TextBlock { Text = "Partially Paid", FontSize = 11, Foreground = (Brush)new BrushConverter().ConvertFrom("#F59E0B"), FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 5, 0, 0) });
                }

                if (status == "Past Due" && penalty > 0)
                {
                    col2.Children.Add(new TextBlock { Text = $"Penalty: ₱{penalty:N2}", FontSize = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444"), FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 5, 0, 0) });
                }
                Grid.SetColumn(col2, 1);
                grid.Children.Add(col2);

                // Column 3: Total to Pay & Actions
                StackPanel col3 = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
                col3.Children.Add(new TextBlock { Text = "REMAINING BALANCE", FontSize = 10, FontWeight = FontWeights.Normal, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), HorizontalAlignment = HorizontalAlignment.Right });
                col3.Children.Add(new TextBlock { Text = $"₱{totalToPay:N2}", FontSize = 20, FontWeight = FontWeights.SemiBold, Foreground = (Brush)new BrushConverter().ConvertFrom("#3044FF"), HorizontalAlignment = HorizontalAlignment.Right });

                // Action Buttons container
                StackPanel btnPanel = new StackPanel 
                { 
                    Orientation = Orientation.Horizontal, 
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 12, 0, 0)
                };

                // Logic to show multiple buttons if necessary
                if (status == "Paid" || isPartiallyPaid)
                {
                    Button btnView = CreateIconButton(FontAwesomeIcon.History, "#3044FF", "View Transaction Log");
                    btnView.Tag = installmentID; // Store installmentID in the Tag for access in the event handler
                    btnView.Click += BtnViewTransactionLog_Click;
                    btnPanel.Children.Add(btnView);                }

                if (status == "Active" && DateTime.Now.Date > dueDate.Date)
                {
                    Button btnPastDue = CreateIconButton(FontAwesomeIcon.ExclamationTriangle, "#EF4444", "Mark as Past Due");
                    btnPastDue.Tag = installmentID;
                    if (btnPanel.Children.Count > 0) btnPastDue.Margin = new Thickness(10, 0, 0, 0);
                    btnPastDue.Click += (s, e) =>
                    {
                        PenaltyInputDialog dialog = new PenaltyInputDialog();
                        if (dialog.ShowDialog() == true)
                        {
                            setInstallmentToPastDueWithPenalty((int)((Button)s).Tag, dialog.PenaltyAmount);
                            displayInstallmentCards(container, loanID); // Refresh
                        }
                    };
                    btnPanel.Children.Add(btnPastDue);
                }

                if (status == "Past Due")
                {
                    Button btnFollow = CreateIconButton(FontAwesomeIcon.Calendar, "#64748B", "Schedule Follow Up");
                    if (btnPanel.Children.Count > 0)
                    {
                        btnFollow.Margin = new Thickness(10, 0, 0, 0);
                    }
                    btnPanel.Children.Add(btnFollow);
                }

                if (btnPanel.Children.Count > 0)
                {
                    col3.Children.Add(btnPanel);
                }

                Grid.SetColumn(col3, 2);
                grid.Children.Add(col3);

                card.Child = grid;
                container.Children.Add(card);
            }

            return grandTotal;
        }

        private Button CreateIconButton(FontAwesomeIcon icon, string color, string tooltip)
        {
            ImageAwesome iconImage = new ImageAwesome
            {
                Icon = icon,
                Foreground = Brushes.White,
                Width = 16,
                Height = 16
            };

            Button btn = new Button
            {
                Content = iconImage,
                Background = (Brush)new BrushConverter().ConvertFrom(color),
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                Height = 36,
                Width = 36,
                ToolTip = tooltip
            };

            // Programmatic rounded corners template
            ControlTemplate template = new ControlTemplate(typeof(Button));
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(4)); // Square with slight rounding
            FrameworkElementFactory content = new FrameworkElementFactory(typeof(ContentPresenter));
            content.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            content.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            border.AppendChild(content);
            template.VisualTree = border;
            btn.Template = template;

            return btn;
        }

        private void BtnViewTransactionLog_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int installmentID = (int)btn.Tag;

            installmentTransactionDetail transactionDetailWindow = new installmentTransactionDetail(installmentID);
            transactionDetailWindow.ShowDialog();
        }
    }
}
