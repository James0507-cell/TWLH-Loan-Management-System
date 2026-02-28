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
    class Business
    {
        dbManager db = new dbManager();
        string sqlQuery = "";

        public DataTable getClientBusiness(int clientID)
        {
            sqlQuery = $"select * from tbl_business where client_id = '{clientID}'";
            return db.displayRecords(sqlQuery);
        }

        public void addBusiness(int clientID, string businessName, string businessAddress, string businessRegistrationID)
        {
            sqlQuery = $"insert into tbl_business (client_id, business_name, business_address, business_registration_id) values ('{clientID}', '{businessName}', '{businessAddress}', '{businessRegistrationID}')";
            db.sqlManager(sqlQuery);
        }

        public void updateBusiness(int businessID, int clientID, string businessName, string businessAddress, string businessRegistrationID)
        {
            sqlQuery = $"update tbl_business set client_id = '{clientID}', business_name = '{businessName}', business_address = '{businessAddress}', business_registration_id = '{businessRegistrationID}' where business_id = '{businessID}'";
            db.sqlManager(sqlQuery);
        }

        public void deleteBusiness(int businessID)
        {
            sqlQuery = $"delete from tbl_business where business_id = '{businessID}'";
            db.sqlManager(sqlQuery);
        }

        public void displayBusinessCards(WrapPanel container, int clientID)
        {
            DataTable dt = getClientBusiness(clientID);
            container.Children.Clear();

            if (dt.Rows.Count == 0)
            {
                container.Children.Add(new TextBlock 
                { 
                    Text = "No businesses registered for this client.", 
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"),
                    FontSize = 14,
                    Margin = new Thickness(10)
                });
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                int businessID = Convert.ToInt32(row["business_id"]);
                string name = row["business_name"].ToString();
                string address = row["business_address"].ToString();
                string regID = row["business_registration_id"].ToString();

                Border card = new Border
                {
                    Width = 300,
                    Height = 200, // Increased height for buttons
                    Margin = new Thickness(10),
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(12),
                    BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                    BorderThickness = new Thickness(1.5)
                };

                StackPanel stack = new StackPanel { Margin = new Thickness(20) };

                stack.Children.Add(new TextBlock
                {
                    Text = name,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B"),
                    TextTrimming = TextTrimming.CharacterEllipsis
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Reg ID: {regID}",
                    FontSize = 12,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"),
                    Margin = new Thickness(0, 4, 0, 10)
                });

                StackPanel addrStack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
                addrStack.Children.Add(new FontAwesome.WPF.ImageAwesome { Icon = FontAwesome.WPF.FontAwesomeIcon.MapMarker, Width = 12, Height = 12, Foreground = (Brush)new BrushConverter().ConvertFrom("#64748B"), Margin = new Thickness(0, 0, 8, 0) });
                addrStack.Children.Add(new TextBlock
                {
                    Text = address,
                    FontSize = 13,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#475569"),
                    TextTrimming = TextTrimming.CharacterEllipsis
                });
                stack.Children.Add(addrStack);

                // Action Buttons for Business
                Grid actionGrid = new Grid();
                actionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                actionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Button btnEdit = new Button
                {
                    Content = "Edit",
                    Margin = new Thickness(0, 0, 5, 0),
                    Padding = new Thickness(0, 6, 0, 6),
                    Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9"),
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#475569"),
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = row
                };
                btnEdit.Resources.Add(typeof(Border), new Style(typeof(Border)) { Setters = { new Setter(Border.CornerRadiusProperty, new CornerRadius(6)) } });
                btnEdit.Click += (s, e) => 
                {
                    BusinessForm form = new BusinessForm((DataRow)((Button)s).Tag);
                    if (form.ShowDialog() == true) displayBusinessCards(container, clientID);
                };

                Button btnDelete = new Button
                {
                    Content = "Delete",
                    Margin = new Thickness(5, 0, 0, 0),
                    Padding = new Thickness(0, 6, 0, 6),
                    Background = (Brush)new BrushConverter().ConvertFrom("#FEE2E2"),
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444"),
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = businessID
                };
                btnDelete.Resources.Add(typeof(Border), new Style(typeof(Border)) { Setters = { new Setter(Border.CornerRadiusProperty, new CornerRadius(6)) } });
                btnDelete.Click += (s, e) =>
                {
                    if (MessageBox.Show("Are you sure you want to delete this business record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        deleteBusiness((int)((Button)s).Tag);
                        displayBusinessCards(container, clientID);
                    }
                };

                Grid.SetColumn(btnEdit, 0);
                Grid.SetColumn(btnDelete, 1);
                actionGrid.Children.Add(btnEdit);
                actionGrid.Children.Add(btnDelete);
                stack.Children.Add(actionGrid);

                card.Child = stack;
                container.Children.Add(card);
            }
        }
    }
}
