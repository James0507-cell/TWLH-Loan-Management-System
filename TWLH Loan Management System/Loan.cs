using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Controls;
using FontAwesome.WPF;

namespace TWLH_Loan_Management_System
{
    internal class Loan
    {
        dbManager db  =new dbManager();
        string strQuery = "";


        public void addLoan(int clientID, double amount, string dueDate, string installmentPlan, 
                            double interestRate, string status, int approvedBy)
        {
            strQuery = $"Insert into tbl_loan (client_id, loan_amount, due_date, installment_plan, interest_rate, loan_status, approved_by) " +
                       $"values ({clientID}, {amount}, '{dueDate}', '{installmentPlan}', {interestRate}, '{status}', {approvedBy})";
            db.sqlManager(strQuery);
        }

        public void updateLoan(int loanID, int clientID, double amount, string dueDate, string installmentPlan,
                                double interestRate, string status, int approvedBy)
        {
            strQuery = $"Update tbl_loan set loan_amount = {amount}, due_date = '{dueDate}', installment_plan = '{installmentPlan}', " +
                       $"interest_rate = {interestRate}, loan_status = '{status}', approved_by = {approvedBy} " +
                       $"where loan_id = {loanID}";
            db.sqlManager(strQuery);
        }

        public DataTable getLoans()
        {
            strQuery = "SELECT l.*, c.first_name, c.last_name, " +
                       "(SELECT COUNT(*) FROM tbl_loan_installment WHERE loan_id = l.loan_id) as total_installments, " +
                       "(SELECT COUNT(*) FROM tbl_loan_installment WHERE loan_id = l.loan_id AND installment_status = 'Paid') as paid_installments " +
                       "FROM tbl_loan l " +
                       "JOIN tbl_client c ON l.client_id = c.client_id";
            return db.displayRecords(strQuery);
        }

        public DataTable getLoanByID(int loanID)
        {
            strQuery = $"select * from tbl_loan where loan_id = {loanID}";
            return db.displayRecords(strQuery);
        }
        public void displayLoanCards(WrapPanel container)
        {
            DataTable dt = getLoans();
            container.Children.Clear();

            foreach (DataRow row in dt.Rows)
            {
                // Extract data from row, handling potential nulls or data type differences
                int loanID = Convert.ToInt32(row["loan_id"]);
                decimal loanAmount = Convert.ToDecimal(row["loan_amount"]);
                int clientID = Convert.ToInt32(row["client_id"]);
                string clientName = $"{row["first_name"]} {row["last_name"]}";
                DateTime dueDate = Convert.ToDateTime(row["due_date"]);
                string installmentPlan = row["installment_plan"].ToString();
                string loanStatus = row["loan_status"].ToString();
                int totalInstallments = Convert.ToInt32(row["total_installments"]);
                int paidInstallments = Convert.ToInt32(row["paid_installments"]);

                // Create Card Border
                Border card = new Border
                {
                    Width = 340,
                    Height = 350,
                    Margin = new System.Windows.Thickness(12),
                    Background = Brushes.White,
                    CornerRadius = new System.Windows.CornerRadius(15),
                    BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                    BorderThickness = new System.Windows.Thickness(1.5)
                };

                StackPanel stack = new StackPanel { Margin = new System.Windows.Thickness(25) };

                // Header: Loan ID
                stack.Children.Add(new TextBlock
                {
                    Text = $"Loan #{loanID}",
                    FontWeight = System.Windows.FontWeights.Bold,
                    FontSize = 18,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B")
                });

                // Client Name & ID
                stack.Children.Add(new TextBlock
                {
                    Text = clientName,
                    FontSize = 14,
                    FontWeight = System.Windows.FontWeights.SemiBold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B"),
                    Margin = new System.Windows.Thickness(0, 4, 0, 0)
                });
                stack.Children.Add(new TextBlock
                {
                    Text = $"Client ID: {clientID}",
                    FontSize = 12,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"),
                    Margin = new System.Windows.Thickness(0, 0, 0, 15)
                });

                // Top Info Grid (Amount and Status)
                Grid infoGrid = new Grid();
                infoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                infoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                StackPanel amountStack = new StackPanel();
                amountStack.Children.Add(new TextBlock
                {
                    Text = "LOAN AMOUNT",
                    FontSize = 10,
                    FontWeight = System.Windows.FontWeights.SemiBold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B")
                });
                amountStack.Children.Add(new TextBlock
                {
                    Text = $"₱{loanAmount:N2}",
                    FontSize = 22,
                    FontWeight = System.Windows.FontWeights.Bold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B")
                });
                Grid.SetColumn(amountStack, 0);
                infoGrid.Children.Add(amountStack);

                // Status Chip inside the grid
                Border statusBorder = new Border
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new System.Windows.Thickness(12, 6, 12, 6),
                    CornerRadius = new System.Windows.CornerRadius(8),
                    Background = (Brush)new BrushConverter().ConvertFrom(
                        loanStatus == "Active" ? "#E0E7FF" : 
                        loanStatus == "Paid" ? "#D1FAE5" : "#FEE2E2")
                };
                statusBorder.Child = new TextBlock
                {
                    Text = loanStatus,
                    FontSize = 11,
                    FontWeight = System.Windows.FontWeights.Bold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom(
                        loanStatus == "Active" ? "#3044FF" : 
                        loanStatus == "Paid" ? "#10B981" : "#EF4444")
                };
                Grid.SetColumn(statusBorder, 1);
                infoGrid.Children.Add(statusBorder);

                stack.Children.Add(infoGrid);

                // Divider
                stack.Children.Add(new Separator { Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9"), Margin = new System.Windows.Thickness(0, 15, 0, 15) });

                // Details: Due Date & Plan
                Grid detailsGrid = new Grid();
                detailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                detailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                StackPanel dateStack = new StackPanel();
                dateStack.Children.Add(new TextBlock { Text = "DUE DATE", FontSize = 9, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), FontWeight = FontWeights.SemiBold });
                dateStack.Children.Add(new TextBlock { Text = dueDate.ToString("MMM dd, yyyy"), FontSize = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B"), FontWeight = FontWeights.Medium });
                Grid.SetColumn(dateStack, 0);
                detailsGrid.Children.Add(dateStack);

                StackPanel planStack = new StackPanel();
                planStack.Children.Add(new TextBlock { Text = "PLAN", FontSize = 9, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), FontWeight = FontWeights.SemiBold });
                planStack.Children.Add(new TextBlock { Text = installmentPlan, FontSize = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B"), FontWeight = FontWeights.Medium });
                Grid.SetColumn(planStack, 1);
                detailsGrid.Children.Add(planStack);

                stack.Children.Add(detailsGrid);

                // Progress Section
                StackPanel progressStack = new StackPanel { Margin = new System.Windows.Thickness(0, 15, 0, 0) };
                
                Grid progressLabels = new Grid();
                progressLabels.Children.Add(new TextBlock
                {
                    Text = "Repayment Progress",
                    FontSize = 10,
                    FontWeight = System.Windows.FontWeights.SemiBold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B")
                });
                progressLabels.Children.Add(new TextBlock
                {
                    Text = $"{paidInstallments}/{totalInstallments} paid",
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontWeight = System.Windows.FontWeights.Bold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B")
                });
                progressStack.Children.Add(progressLabels);

                // Custom Border-based Progress Bar
                Border pbContainer = new Border
                {
                    Height = 10,
                    Background = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                    CornerRadius = new CornerRadius(5),
                    Margin = new System.Windows.Thickness(0, 5, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                double progressPercentage = totalInstallments > 0 ? (double)paidInstallments / totalInstallments : 0;
                double fillWidth = 290 * progressPercentage; // Card width 340 - 2*25 margins

                Border pbFill = new Border
                {
                    Height = 10,
                    Background = (Brush)new BrushConverter().ConvertFrom("#3044FF"),
                    CornerRadius = new CornerRadius(5),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = fillWidth
                };

                pbContainer.Child = pbFill;
                progressStack.Children.Add(pbContainer);
                stack.Children.Add(progressStack);

                // Buttons Container
                Grid buttonsGrid = new Grid { Margin = new System.Windows.Thickness(0, 15, 0, 0) };
                buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Apply rounded button style from resources
                Style cardBtnStyle = (Style)container.FindResource("CardButtonStyle");

                // View Button with Icon
                Button viewBtn = new Button
                {
                    Style = cardBtnStyle,
                    Margin = new System.Windows.Thickness(0, 0, 5, 0),
                    Background = (Brush)new BrushConverter().ConvertFrom("#F3F4F6"),
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B")
                };
                StackPanel viewBtnContent = new StackPanel { Orientation = Orientation.Horizontal };
                viewBtnContent.Children.Add(new ImageAwesome { Icon = FontAwesomeIcon.Eye, Width = 14, Height = 14, Foreground = viewBtn.Foreground, Margin = new Thickness(0, 0, 8, 0) });
                viewBtnContent.Children.Add(new TextBlock { Text = "View", VerticalAlignment = VerticalAlignment.Center });
                viewBtn.Content = viewBtnContent;
                viewBtn.Click += (s, e) =>
                {
                    LoanDetails details = new LoanDetails(loanID);
                    details.ShowDialog();
                };

                // Update Button with Icon
                Button updateBtn = new Button
                {
                    Style = cardBtnStyle,
                    Tag = row, // Store row for updating
                    Margin = new System.Windows.Thickness(5, 0, 0, 0),
                    Background = (Brush)new BrushConverter().ConvertFrom("#3044FF"),
                    Foreground = Brushes.White
                };
                StackPanel updateBtnContent = new StackPanel { Orientation = Orientation.Horizontal };
                updateBtnContent.Children.Add(new ImageAwesome { Icon = FontAwesomeIcon.Edit, Width = 14, Height = 14, Foreground = updateBtn.Foreground, Margin = new Thickness(0, 0, 8, 0) });
                updateBtnContent.Children.Add(new TextBlock { Text = "Update", VerticalAlignment = VerticalAlignment.Center });
                updateBtn.Content = updateBtnContent;

                updateBtn.Click += (s, e) =>
                {
                    Button btn = (Button)s;
                    DataRow rowToUpdate = (DataRow)btn.Tag;
                    LoanForm form = new LoanForm(rowToUpdate);
                    if (form.ShowDialog() == true)
                    {
                        displayLoanCards(container); // Refresh cards
                    }
                };

                Grid.SetColumn(viewBtn, 0);
                Grid.SetColumn(updateBtn, 1);
                buttonsGrid.Children.Add(viewBtn);
                buttonsGrid.Children.Add(updateBtn);
                stack.Children.Add(buttonsGrid);

                card.Child = stack;
                container.Children.Add(card);
            }
        }
    }
}
