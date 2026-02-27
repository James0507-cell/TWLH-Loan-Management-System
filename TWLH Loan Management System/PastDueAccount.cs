using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace TWLH_Loan_Management_System
{
    class PastDueAccount
    {
        dbManager db = new dbManager();
        string sqlQuery = "";

        public DataTable getFilteredPastDueAccounts(string searchText = "", string status = "All Statuses")
        {
            sqlQuery = @"SELECT pda.*, 
                               li.installment_amount, 
                               li.installment_due_date,
                               li.loan_id,
                               CONCAT(c.first_name, ' ', c.last_name) as ClientName,
                               c.client_id,
                               (li.installment_amount + pda.penalty_added) as TotalPastDue
                        FROM tbl_past_due_account pda
                        JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id
                        JOIN tbl_loan l ON li.loan_id = l.loan_id
                        JOIN tbl_client c ON l.client_id = c.client_id
                        WHERE 1=1 ";

            if (!string.IsNullOrEmpty(searchText))
            {
                sqlQuery += $"AND (c.first_name LIKE '%{searchText}%' OR c.last_name LIKE '%{searchText}%' OR pda.past_due_id LIKE '%{searchText}%') ";
            }

            if (status != "All Statuses")
            {
                sqlQuery += $"AND pda.past_due_status = '{status}' ";
            }

            sqlQuery += " ORDER BY li.installment_due_date DESC";

            return db.displayRecords(sqlQuery);
        }

        public DataTable getPastDueAccount()
        {
            return getFilteredPastDueAccounts();
        }

        public DataTable getPastDueAccount(int pastDueID)
        {
            sqlQuery = $"SELECT pda.*, c.first_name, c.last_name FROM tbl_past_due_account pda " +
                       $"JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id " +
                       $"JOIN tbl_loan l ON li.loan_id = l.loan_id " +
                       $"JOIN tbl_client c ON l.client_id = c.client_id " +
                       $"WHERE pda.past_due_id = '{pastDueID}'";
            return db.displayRecords(sqlQuery);
        }

        public void displayPastDueCards(WrapPanel container, string searchText = "", string status = "All Statuses")
        {
            DataTable dt = getFilteredPastDueAccounts(searchText, status);
            container.Children.Clear();

            foreach (DataRow row in dt.Rows)
            {
                container.Children.Add(createPastDueCard(row));
            }
        }

        private UIElement createPastDueCard(DataRow row)
        {
            Border card = new Border
            {
                Width = 300,
                Margin = new Thickness(0, 0, 20, 20),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(15),
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(20)
            };

            // Shadow effect
            

            StackPanel stack = new StackPanel();

            // Header: Status and ID
            Grid header = new Grid();
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            TextBlock idTxt = new TextBlock
            {
                Text = $"Account #{row["past_due_id"]}",
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B")
            };
            Grid.SetColumn(idTxt, 0);
            header.Children.Add(idTxt);

            string status = row["past_due_status"].ToString();
            Border statusBadge = new Border
            {
                Padding = new Thickness(8, 3, 8, 3),
                CornerRadius = new CornerRadius(6),
                Background = (Brush)new BrushConverter().ConvertFrom(status == "Resolved" ? "#D1FAE5" : status == "Open" ? "#FEE2E2" : "#FEF3C7")
            };
            statusBadge.Child = new TextBlock
            {
                Text = status.ToUpper(),
                FontSize = 9,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)new BrushConverter().ConvertFrom(status == "Resolved" ? "#10B981" : status == "Open" ? "#EF4444" : "#D97706")
            };
            Grid.SetColumn(statusBadge, 1);
            header.Children.Add(statusBadge);
            stack.Children.Add(header);

            // Client Name
            stack.Children.Add(new TextBlock
            {
                Text = row["ClientName"].ToString(),
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B"),
                Margin = new Thickness(0, 15, 0, 2),
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            stack.Children.Add(new TextBlock
            {
                Text = $"Loan #{row["loan_id"]} | Inst. #{row["installment_id"]}",
                FontSize = 12,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"),
                Margin = new Thickness(0, 0, 0, 15)
            });

            // Financial Info
            Border infoBox = new Border
            {
                Background = (Brush)new BrushConverter().ConvertFrom("#F8FAFC"),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 5, 0, 15)
            };

            StackPanel infoStack = new StackPanel();
            
            // Amount
            Grid amtGrid = new Grid();
            amtGrid.Children.Add(new TextBlock { Text = "Past Due Amount", FontSize = 11, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B") });
            amtGrid.Children.Add(new TextBlock { Text = $"₱{Convert.ToDecimal(row["TotalPastDue"]):N2}", FontSize = 14, FontWeight = FontWeights.Bold, Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B"), HorizontalAlignment = HorizontalAlignment.Right });
            infoStack.Children.Add(amtGrid);

            // Penalty
            Grid pnlGrid = new Grid { Margin = new Thickness(0, 8, 0, 0) };
            pnlGrid.Children.Add(new TextBlock { Text = "Penalty Added", FontSize = 11, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B") });
            pnlGrid.Children.Add(new TextBlock { Text = $"₱{Convert.ToDecimal(row["penalty_added"]):N2}", FontSize = 13, FontWeight = FontWeights.SemiBold, Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444"), HorizontalAlignment = HorizontalAlignment.Right });
            infoStack.Children.Add(pnlGrid);

            infoBox.Child = infoStack;
            stack.Children.Add(infoBox);

            // Actions
            Button btnView = new Button
            {
                Content = "Manage Account",
                Height = 38,
                Background = (Brush)new BrushConverter().ConvertFrom("#3044FF"),
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                Cursor = Cursors.Hand,
                BorderThickness = new Thickness(0)
            };
            
            // Rounded button template
            ControlTemplate template = new ControlTemplate(typeof(Button));
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            FrameworkElementFactory content = new FrameworkElementFactory(typeof(ContentPresenter));
            content.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            content.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            border.AppendChild(content);
            template.VisualTree = border;
            btnView.Template = template;

            stack.Children.Add(btnView);

            card.Child = stack;
            return card;
        }
    }
}
