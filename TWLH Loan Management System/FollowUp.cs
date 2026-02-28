using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using FontAwesome.WPF;

namespace TWLH_Loan_Management_System
{
    class FollowUp
    {
        dbManager db = new dbManager();
        string sqlQuery = "";


        public DataTable getFollowUpRecrods()
        {
            sqlQuery = "SELECT f.*, CONCAT(c.first_name, ' ', c.last_name) as client_name, e.first_name as recorder_name " +
                       "FROM tbl_follow_up f " +
                       "JOIN tbl_past_due_account pda ON f.past_due_id = pda.past_due_id " +
                       "JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id " +
                       "JOIN tbl_loan l ON li.loan_id = l.loan_id " +
                       "JOIN tbl_client c ON l.client_id = c.client_id " +
                       "JOIN tbl_employee e ON f.recorded_by = e.employee_id";
            return db.displayRecords(sqlQuery);
        }

        public DataTable getFilteredFollowUps(string searchText = "")
        {
            sqlQuery = "SELECT f.*, CONCAT(c.first_name, ' ', c.last_name) as client_name, e.first_name as recorder_name " +
                       "FROM tbl_follow_up f " +
                       "JOIN tbl_past_due_account pda ON f.past_due_id = pda.past_due_id " +
                       "JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id " +
                       "JOIN tbl_loan l ON li.loan_id = l.loan_id " +
                       "JOIN tbl_client c ON l.client_id = c.client_id " +
                       "JOIN tbl_employee e ON f.recorded_by = e.employee_id " +
                       "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(searchText))
            {
                sqlQuery += $"AND (c.first_name LIKE '%{searchText}%' OR c.last_name LIKE '%{searchText}%' OR f.notes LIKE '%{searchText}%' OR f.follow_up_type LIKE '%{searchText}%') ";
            }

            sqlQuery += "ORDER BY f.follow_up_date DESC";
            return db.displayRecords(sqlQuery);
        }

        public DataTable getFollowUpRecrods(int pastDueID)
        {
            sqlQuery = $"select * from tbl_follow_up where past_due_id = '{pastDueID}'";
            return db.displayRecords(sqlQuery);
        }

        public void addFollowUp(int pastDueID, string date, string type, string notes, int recordedBy)
        {
            sqlQuery = $"INSERT INTO tbl_follow_up (past_due_id, follow_up_date, follow_up_type, notes, recorded_by) " +
                       $"VALUES ('{pastDueID}', '{date}', '{type}', '{notes}', '{recordedBy}')";
            db.sqlManager(sqlQuery);
        }

        public void updateFollowUp(int followUpID, int pastDueID, string date, string type, string notes, int recordedBy)
        {
            sqlQuery = $"UPDATE tbl_follow_up SET past_due_id = '{pastDueID}', follow_up_date = '{date}', follow_up_type = '{type}', notes = '{notes}', recorded_by = '{recordedBy}', updated_by = '{recordedBy}' " +
                       $"WHERE follow_up_id = '{followUpID}'";
            db.sqlManager(sqlQuery);
        }

        public void displayFollowUpCards(WrapPanel container, string searchText = "")
        {
            DataTable dt = getFilteredFollowUps(searchText);
            container.Children.Clear();

            foreach (DataRow row in dt.Rows)
            {
                int followUpID = Convert.ToInt32(row["follow_up_id"]);
                int pastDueID = Convert.ToInt32(row["past_due_id"]);
                string clientName = row["client_name"].ToString();
                string followUpType = row["follow_up_type"].ToString();
                string notes = row["notes"].ToString();
                DateTime followUpDate = Convert.ToDateTime(row["follow_up_date"]);
                string recorderName = row["recorder_name"].ToString();

                Border card = new Border
                {
                    Width = 300,
                    MinHeight = 220,
                    Margin = new Thickness(10),
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(12),
                    BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                    BorderThickness = new Thickness(1),
                };

                StackPanel stack = new StackPanel { Margin = new Thickness(15) };

                // Header with IDs
                Grid idGrid = new Grid();
                idGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                idGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                TextBlock txtIDs = new TextBlock
                {
                    Text = $"Follow-up #{followUpID} | PDA #{pastDueID}",
                    FontSize = 10,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#6B7280"),
                    Margin = new Thickness(0, 0, 0, 5)
                };
                stack.Children.Add(txtIDs);

                // Header with Type Tag
                DockPanel header = new DockPanel { LastChildFill = false, Margin = new Thickness(0, 0, 0, 10) };
                
                TextBlock txtClient = new TextBlock
                {
                    Text = clientName,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#111827"),
                    MaxWidth = 180,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                DockPanel.SetDock(txtClient, Dock.Left);
                header.Children.Add(txtClient);

                Border typeTag = new Border
                {
                    Padding = new Thickness(8, 3, 8, 3),
                    CornerRadius = new CornerRadius(6),
                    Background = (Brush)new BrushConverter().ConvertFrom(
                        followUpType == "Call" ? "#DBEAFE" : 
                        followUpType == "Message" ? "#DCFCE7" : "#FEF3C7")
                };
                typeTag.Child = new TextBlock
                {
                    Text = followUpType,
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom(
                        followUpType == "Call" ? "#1E40AF" : 
                        followUpType == "Message" ? "#166534" : "#92400E")
                };
                DockPanel.SetDock(typeTag, Dock.Right);
                header.Children.Add(typeTag);
                stack.Children.Add(header);

                // Date
                StackPanel dateStack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
                dateStack.Children.Add(new ImageAwesome { Icon = FontAwesomeIcon.Calendar, Width = 12, Height = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#9CA3AF"), Margin = new Thickness(0, 0, 8, 0) });
                dateStack.Children.Add(new TextBlock { Text = followUpDate.ToString("MMMM dd, yyyy"), FontSize = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#6B7280") });
                stack.Children.Add(dateStack);

                // Notes
                Border notesBorder = new Border
                {
                    Background = (Brush)new BrushConverter().ConvertFrom("#F9FAFB"),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 0, 0, 15)
                };
                notesBorder.Child = new TextBlock
                {
                    Text = notes,
                    FontSize = 13,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#374151"),
                    TextWrapping = TextWrapping.Wrap,
                    MinHeight = 40
                };
                stack.Children.Add(notesBorder);

                // Footer with Recorder and Edit Button
                Grid footer = new Grid();
                footer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                footer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                StackPanel recorderStack = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
                recorderStack.Children.Add(new ImageAwesome { Icon = FontAwesomeIcon.User, Width = 10, Height = 10, Foreground = (Brush)new BrushConverter().ConvertFrom("#9CA3AF"), Margin = new Thickness(0, 0, 5, 0) });
                recorderStack.Children.Add(new TextBlock { Text = $"By {recorderName}", FontSize = 11, Foreground = (Brush)new BrushConverter().ConvertFrom("#9CA3AF") });
                Grid.SetColumn(recorderStack, 0);
                footer.Children.Add(recorderStack);

                Button editBtn = new Button
                {
                    Content = "Edit",
                    Padding = new Thickness(12, 5, 12, 5),
                    Background = (Brush)new BrushConverter().ConvertFrom("#4F46E5"),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = row
                };
                editBtn.Resources.Add(typeof(Border), new Style(typeof(Border)) { Setters = { new Setter(Border.CornerRadiusProperty, new CornerRadius(6)) } });
                editBtn.Click += (s, e) =>
                {
                    Button btn = (Button)s;
                    DataRow rowToUpdate = (DataRow)btn.Tag;
                    FollowUpForm form = new FollowUpForm(rowToUpdate);
                    if (form.ShowDialog() == true)
                    {
                        displayFollowUpCards(container, searchText);
                    }
                };
                Grid.SetColumn(editBtn, 1);
                footer.Children.Add(editBtn);

                stack.Children.Add(footer);

                card.Child = stack;
                container.Children.Add(card);
            }
        }
    }
}
