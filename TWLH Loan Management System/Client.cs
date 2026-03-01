using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FontAwesome.WPF;

namespace TWLH_Loan_Management_System
{
    class Client
    {
        dbManager db = new dbManager();
        string sqlQuery = "";

        public DataTable getClient()
        {
            sqlQuery = "SELECT c.*, GROUP_CONCAT(b.business_name SEPARATOR ', ') as business_names FROM tbl_client c LEFT JOIN tbl_business b ON c.client_id = b.client_id GROUP BY c.client_id";
            return db.displayRecords(sqlQuery);
        }

        public DataTable getClient(int clientID)
        {
            sqlQuery = $"SELECT c.*, GROUP_CONCAT(b.business_name SEPARATOR ', ') as business_names FROM tbl_client c LEFT JOIN tbl_business b ON c.client_id = b.client_id WHERE c.client_id = '{clientID}' GROUP BY c.client_id";
            return db.displayRecords(sqlQuery);
        }

        public DataTable getFilteredClients(string searchText = "")
        {
            sqlQuery = "SELECT c.*, GROUP_CONCAT(b.business_name SEPARATOR ', ') as business_names FROM tbl_client c LEFT JOIN tbl_business b ON c.client_id = b.client_id WHERE 1=1 ";
            if (!string.IsNullOrEmpty(searchText))
            {
                sqlQuery += $"AND (c.first_name LIKE '%{searchText}%' OR c.last_name LIKE '%{searchText}%' OR b.business_name LIKE '%{searchText}%' OR c.client_id LIKE '%{searchText}%') ";
            }
            sqlQuery += " GROUP BY c.client_id";
            return db.displayRecords(sqlQuery);
        }

        public void addClient(string firstName, string middleName, string lastName, string gender, string dateOfBirth, string contactNumber, string currentResidence, string messengerName)
        {
            sqlQuery = $"insert into tbl_client (first_name, middle_name, last_name, gender, date_of_birth, contact_number, current_residence, messenger_name) values ('{firstName}', '{middleName}', '{lastName}', '{gender}', '{dateOfBirth}', '{contactNumber}', '{currentResidence}', '{messengerName}')";
            db.sqlManager(sqlQuery);
        }

        public void updateClient(int clientID, string firstName, string middleName, string lastName, string gender, string dateOfBirth, string contactNumber, string currentResidence, string messengerName)
        {
            sqlQuery = $"update tbl_client set first_name = '{firstName}', middle_name = '{middleName}', last_name = '{lastName}', gender = '{gender}', date_of_birth = '{dateOfBirth}', contact_number = '{contactNumber}', current_residence = '{currentResidence}', messenger_name = '{messengerName}' where client_id = '{clientID}'";
            db.sqlManager(sqlQuery);
        }

        public void deleteClient(int clientID)
        {
            sqlQuery = $"delete from tbl_client where client_id = '{clientID}'";
            db.sqlManager(sqlQuery);
        }

        public void displayClientCards(WrapPanel container, string searchText = "")
        {
            DataTable dt = getFilteredClients(searchText);
            container.Children.Clear();

            foreach (DataRow row in dt.Rows)
            {
                int clientID = Convert.ToInt32(row["client_id"]);
                string firstName = row["first_name"].ToString();
                string middleName = row["middle_name"].ToString();
                string lastName = row["last_name"].ToString();
                string fullName = string.IsNullOrWhiteSpace(middleName) ? $"{firstName} {lastName}" : $"{firstName} {middleName} {lastName}";
                string businessNames = row["business_names"] != DBNull.Value ? row["business_names"].ToString() : "No Business Registered";
                string contactNumber = row["contact_number"].ToString();
                string residence = row["current_residence"].ToString();
                string gender = row["gender"].ToString();

                // Card Border
                Border card = new Border
                {
                    Width = 320,
                    Height = 250,
                    Margin = new Thickness(12),
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(12),
                    BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                    BorderThickness = new Thickness(1.5)
                };

                StackPanel stack = new StackPanel { Margin = new Thickness(20) };

                // Header: Client Name
                Grid headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                stack.Children.Add(new TextBlock
                {
                    Text = fullName,
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B"),
                    TextTrimming = TextTrimming.CharacterEllipsis
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"ID: #{clientID:D4}",
                    FontSize = 12,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"),
                    Margin = new Thickness(0, 2, 0, 10)
                });

                // Business Names with Icon
                StackPanel businessStack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
                businessStack.Children.Add(new ImageAwesome { Icon = FontAwesomeIcon.Briefcase, Width = 14, Height = 14, Foreground = (Brush)new BrushConverter().ConvertFrom("#6366F1"), Margin = new Thickness(0, 0, 8, 0) });
                businessStack.Children.Add(new TextBlock
                {
                    Text = businessNames,
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#334155"),
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    ToolTip = businessNames // Add tooltip so user can see all names if truncated
                });
                stack.Children.Add(businessStack);

                // Contact with Icon
                StackPanel contactStack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
                contactStack.Children.Add(new ImageAwesome { Icon = FontAwesomeIcon.Phone, Width = 14, Height = 14, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), Margin = new Thickness(0, 0, 8, 0) });
                contactStack.Children.Add(new TextBlock
                {
                    Text = contactNumber,
                    FontSize = 13,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#475569")
                });
                stack.Children.Add(contactStack);

                // Residence with Icon
                StackPanel residenceStack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
                residenceStack.Children.Add(new ImageAwesome { Icon = FontAwesomeIcon.MapMarker, Width = 14, Height = 14, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), Margin = new Thickness(0, 0, 8, 0) });
                residenceStack.Children.Add(new TextBlock
                {
                    Text = residence,
                    FontSize = 13,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#475569"),
                    TextTrimming = TextTrimming.CharacterEllipsis
                });
                stack.Children.Add(residenceStack);

                // Buttons Container
                Grid buttonsGrid = new Grid();
                buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Button viewBtn = new Button
                {
                    Content = "View Details",
                    Margin = new Thickness(0, 0, 5, 0),
                    Padding = new Thickness(0, 8, 0, 8),
                    Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9"),
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#475569"),
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                // Rounded corners for button
                viewBtn.Resources.Add(typeof(Border), new Style(typeof(Border)) { Setters = { new Setter(Border.CornerRadiusProperty, new CornerRadius(6)) } });
                viewBtn.Click += (s, ev) =>
                {
                    ClientDetail detail = new ClientDetail(clientID);
                    detail.ShowDialog();
                };

                Button editBtn = new Button
                {
                    Content = "Edit Profile",
                    Tag = row, // Store row for updating
                    Margin = new Thickness(5, 0, 0, 0),
                    Padding = new Thickness(0, 8, 0, 8),
                    Background = (Brush)new BrushConverter().ConvertFrom("#FF3044FF"),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                editBtn.Resources.Add(typeof(Border), new Style(typeof(Border)) { Setters = { new Setter(Border.CornerRadiusProperty, new CornerRadius(6)) } });
                editBtn.Click += (s, ev) =>
                {
                    Button btn = (Button)s;
                    DataRow rowToUpdate = (DataRow)btn.Tag;
                    ClientForm form = new ClientForm(rowToUpdate);
                    if (form.ShowDialog() == true)
                    {
                        displayClientCards(container); // Refresh cards after edit
                    }
                };

                Grid.SetColumn(viewBtn, 0);
                Grid.SetColumn(editBtn, 1);
                buttonsGrid.Children.Add(viewBtn);
                buttonsGrid.Children.Add(editBtn);
                stack.Children.Add(buttonsGrid);

                card.Child = stack;
                container.Children.Add(card);
            }
        }
    }
}
