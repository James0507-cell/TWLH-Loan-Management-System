using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using FontAwesome.WPF;

namespace TWLH_Loan_Management_System
{
    class Collection
    {
        dbManager db = new dbManager();
        string sqlQuery = "";

        public DataTable getCollectionRecord()
        {
            sqlQuery = $@"SELECT ca.*, 
                                 CONCAT(c.first_name, ' ', c.last_name) as client_name,
                                 CONCAT(e.first_name, ' ', e.last_name) as collector_name,
                                 CONCAT(creator.first_name, ' ', creator.last_name) as creator_name,
                                 l.loan_amount,
                                 li.installment_amount,
                                 pda.penalty_added,
                                 (li.installment_amount + pda.penalty_added) as total_due
                          FROM tbl_collection_assignment ca
                          JOIN tbl_past_due_account pda ON ca.past_due_id = pda.past_due_id
                          JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id
                          JOIN tbl_loan l ON li.loan_id = l.loan_id
                          JOIN tbl_client c ON l.client_id = c.client_id
                          JOIN tbl_employee e ON ca.assigned_to = e.employee_id
                          JOIN tbl_employee creator ON ca.created_by = creator.employee_id
                          ORDER BY ca.created_at DESC";
            return db.displayRecords(sqlQuery);
        }

        public DataTable getCollectionRecordByCollector(int collectorID)
        {
            sqlQuery = $@"SELECT ca.*, 
                                 CONCAT(c.first_name, ' ', c.last_name) as client_name,
                                 CONCAT(e.first_name, ' ', e.last_name) as collector_name,
                                 CONCAT(creator.first_name, ' ', creator.last_name) as creator_name,
                                 l.loan_amount,
                                 li.installment_amount,
                                 pda.penalty_added,
                                 (li.installment_amount + pda.penalty_added) as total_due
                          FROM tbl_collection_assignment ca
                          JOIN tbl_past_due_account pda ON ca.past_due_id = pda.past_due_id
                          JOIN tbl_loan_installment li ON pda.installment_id = li.installment_id
                          JOIN tbl_loan l ON li.loan_id = l.loan_id
                          JOIN tbl_client c ON l.client_id = c.client_id
                          JOIN tbl_employee e ON ca.assigned_to = e.employee_id
                          JOIN tbl_employee creator ON ca.created_by = creator.employee_id
                          WHERE ca.assigned_to = '{collectorID}'
                          ORDER BY ca.created_at DESC";
            return db.displayRecords(sqlQuery);
        }

        public DataTable getCollectionRecord(int pastDueID)
        {
            sqlQuery = $@"SELECT ca.*, CONCAT(e.first_name, ' ', e.last_name) as employee_name 
                        FROM tbl_collection_assignment ca
                        JOIN tbl_employee e ON ca.assigned_to = e.employee_id
                        WHERE ca.past_due_id = '{pastDueID}'";
            return db.displayRecords(sqlQuery);
        }

        public void addCollectionAssignment(int pastDueID, int assignedTo, string status, int createdBy)
        {
            sqlQuery = $"INSERT INTO tbl_collection_assignment (past_due_id, assigned_to, assignment_status, created_by) " +
                       $"VALUES ('{pastDueID}', '{assignedTo}', '{status}', '{createdBy}')";
            db.sqlManager(sqlQuery);
        }

        public void updateCollectionAssignment(int assignmentID, int assignedTo, string status, int updatedBy)
        {
            sqlQuery = $"UPDATE tbl_collection_assignment " +
                       $"SET assigned_to = '{assignedTo}', assignment_status = '{status}', updated_by = '{updatedBy}', updated_at = NOW() " +
                       $"WHERE assignment_id = '{assignmentID}'";
            db.sqlManager(sqlQuery);
        }

        public UIElement collectionCards(DataRow row, RoutedEventHandler viewDetailsHandler, RoutedEventHandler updateHandler = null)
        {
            Border card = new Border();
            card.Background = Brushes.White;
            card.CornerRadius = new CornerRadius(15);
            card.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0");
            card.BorderThickness = new Thickness(1);
            card.Width = 330;
            card.Margin = new Thickness(12);
            card.Padding = new Thickness(0); // Controlled by internal panels

            StackPanel mainStack = new StackPanel();

            // Header Section with Status Badge
            Grid headerGrid = new Grid();
            headerGrid.Margin = new Thickness(20, 20, 20, 15);
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock txtClient = new TextBlock();
            txtClient.Text = row["client_name"].ToString();
            txtClient.FontSize = 18;
            txtClient.FontWeight = FontWeights.Bold;
            txtClient.Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B");
            txtClient.TextWrapping = TextWrapping.Wrap;
            txtClient.MaxWidth = 180;
            Grid.SetColumn(txtClient, 0);
            headerGrid.Children.Add(txtClient);

            // Status Badge
            string status = row["assignment_status"].ToString();
            Border statusBadge = new Border();
            statusBadge.CornerRadius = new CornerRadius(6);
            statusBadge.Padding = new Thickness(8, 4, 8, 4);
            statusBadge.VerticalAlignment = VerticalAlignment.Top;

            TextBlock txtStatus = new TextBlock();
            txtStatus.Text = status;
            txtStatus.FontSize = 11;
            txtStatus.FontWeight = FontWeights.Bold;

            if (status == "Completed") {
                statusBadge.Background = (Brush)new BrushConverter().ConvertFrom("#D1FAE5");
                txtStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#10B981");
            } else if (status == "Canceled") {
                statusBadge.Background = (Brush)new BrushConverter().ConvertFrom("#FEE2E2");
                txtStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444");
            } else { // In Progress
                statusBadge.Background = (Brush)new BrushConverter().ConvertFrom("#EEF2FF");
                txtStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#6366F1");
            }

            statusBadge.Child = txtStatus;
            Grid.SetColumn(statusBadge, 1);
            headerGrid.Children.Add(statusBadge);
            mainStack.Children.Add(headerGrid);

            // Divider
            Border divider = new Border();
            divider.Height = 1;
            divider.Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9");
            divider.Margin = new Thickness(20, 0, 20, 15);
            mainStack.Children.Add(divider);

            // Body Details
            StackPanel bodyStack = new StackPanel();
            bodyStack.Margin = new Thickness(20, 0, 20, 20);

            AddIconDetail(bodyStack, FontAwesomeIcon.Hashtag, "Account ID:", $"PD-{Convert.ToInt32(row["past_due_id"]):D4}");
            AddIconDetail(bodyStack, FontAwesomeIcon.User, "Collector:", row["collector_name"].ToString());
            AddIconDetail(bodyStack, FontAwesomeIcon.UserCircle, "Assigned By:", row["creator_name"].ToString());
            AddIconDetail(bodyStack, FontAwesomeIcon.Calendar, "Date:", Convert.ToDateTime(row["created_at"]).ToString("MMM dd, yyyy"));

            mainStack.Children.Add(bodyStack);

            // Total Due Highlight
            Border totalBox = new Border();
            totalBox.Background = (Brush)new BrushConverter().ConvertFrom("#F8FAFC");
            totalBox.Padding = new Thickness(20, 12, 20, 12);
            
            Grid totalGrid = new Grid();
            totalGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            totalGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock lblTotal = new TextBlock { Text = "TOTAL DUE", FontSize = 11, FontWeight = FontWeights.Bold, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), VerticalAlignment = VerticalAlignment.Center };
            TextBlock valTotal = new TextBlock { Text = "₱ " + Convert.ToDouble(row["total_due"]).ToString("N2"), FontSize = 16, FontWeight = FontWeights.Bold, Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444"), VerticalAlignment = VerticalAlignment.Center };
            
            Grid.SetColumn(lblTotal, 0);
            Grid.SetColumn(valTotal, 1);
            totalGrid.Children.Add(lblTotal);
            totalGrid.Children.Add(valTotal);
            totalBox.Child = totalGrid;
            mainStack.Children.Add(totalBox);

            // Action Buttons
            Grid buttonsGrid = new Grid();
            buttonsGrid.Margin = new Thickness(20);
            
            // Shared Template
            ControlTemplate btnTemplate = new ControlTemplate(typeof(Button));
            FrameworkElementFactory btnBorder = new FrameworkElementFactory(typeof(Border));
            btnBorder.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
            btnBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            FrameworkElementFactory btnContent = new FrameworkElementFactory(typeof(ContentPresenter));
            btnContent.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            btnContent.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            btnBorder.AppendChild(btnContent);
            btnTemplate.VisualTree = btnBorder;

            if (UserSession.Role == "Admin" || UserSession.Role == "Staff")
            {
                buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Button btnView = CreateBtn("View Details", "#F1F5F9", "#475569", viewDetailsHandler, btnTemplate);
                btnView.Tag = row["assignment_id"];
                btnView.Margin = new Thickness(0, 0, 5, 0);
                Grid.SetColumn(btnView, 0);
                buttonsGrid.Children.Add(btnView);

                Button btnUpdate = CreateBtn("Update", "#3044FF", "#FFFFFF", updateHandler, btnTemplate);
                btnUpdate.Tag = row;
                btnUpdate.Margin = new Thickness(5, 0, 0, 0);
                Grid.SetColumn(btnUpdate, 1);
                buttonsGrid.Children.Add(btnUpdate);
            }
            else
            {
                Button btnView = CreateBtn("View Assignment Details", "#3044FF", "#FFFFFF", viewDetailsHandler, btnTemplate);
                btnView.Tag = row["assignment_id"];
                buttonsGrid.Children.Add(btnView);
            }

            mainStack.Children.Add(buttonsGrid);
            card.Child = mainStack;
            return card;
        }

        private void AddIconDetail(StackPanel parent, FontAwesomeIcon icon, string label, string value)
        {
            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            
            ImageAwesome img = new ImageAwesome { Icon = icon, Width = 12, Height = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#94A3B8"), Margin = new Thickness(0, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center };
            
            TextBlock txtLabel = new TextBlock { Text = label, FontSize = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), Width = 80, VerticalAlignment = VerticalAlignment.Center };
            
            TextBlock txtValue = new TextBlock { Text = value, FontSize = 12, FontWeight = FontWeights.SemiBold, Foreground = (Brush)new BrushConverter().ConvertFrom("#334155"), VerticalAlignment = VerticalAlignment.Center };

            row.Children.Add(img);
            row.Children.Add(txtLabel);
            row.Children.Add(txtValue);
            parent.Children.Add(row);
        }

        private Button CreateBtn(string text, string bg, string fg, RoutedEventHandler handler, ControlTemplate template)
        {
            Button btn = new Button
            {
                Content = text,
                Height = 38,
                Background = (Brush)new BrushConverter().ConvertFrom(bg),
                Foreground = (Brush)new BrushConverter().ConvertFrom(fg),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand,
                Template = template
            };
            btn.Click += handler;
            return btn;
        }

        private void AddDetailRow(Grid grid, int rowIdx, string label, string value, string valueColor = "#64748B")
        {
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            TextBlock txtLabel = new TextBlock();
            txtLabel.Text = label;
            txtLabel.Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B");
            txtLabel.FontSize = 13;
            txtLabel.Margin = new Thickness(0, 4, 15, 4);
            Grid.SetRow(txtLabel, rowIdx);
            Grid.SetColumn(txtLabel, 0);
            grid.Children.Add(txtLabel);

            TextBlock txtValue = new TextBlock();
            txtValue.Text = value;
            txtValue.Foreground = (Brush)new BrushConverter().ConvertFrom(valueColor);
            txtValue.FontSize = 13;
            txtValue.FontWeight = FontWeights.Medium;
            txtValue.Margin = new Thickness(0, 4, 0, 4);
            Grid.SetRow(txtValue, rowIdx);
            Grid.SetColumn(txtValue, 1);
            grid.Children.Add(txtValue);
        }
    }
}
