using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomerLoyaltyManagementSystem.Control
{
    public partial class UsersControl : UserControl
    {
        public UsersControl()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var context = new managementsystem_dbEntities())
            {
                var customers = context.Customers.Select(c => new CustomerViewModel
                {
                    CustomerID = c.CustomerID,
                    Email = c.User.Email,
                    LoyaltyPoints = (int)c.LoyaltyPoints,
                    Tier = c.Tier
                }).ToList();

                UsersListView.ItemsSource = customers;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var customer = button.CommandParameter as CustomerViewModel;

            try
            {
                using (var context = new managementsystem_dbEntities())
                {
                    var customerInDb = context.Customers.FirstOrDefault(c => c.CustomerID == customer.CustomerID);
                    if (customerInDb != null)
                    {
                        customerInDb.LoyaltyPoints = customer.LoyaltyPoints;
                        context.SaveChanges();
                        MessageBox.Show("Loyaltypoints saved successfully!", "Save Changes", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException?.InnerException as SqlException;
                if (sqlException != null && sqlException.Number == 547)
                {
                    MessageBox.Show("A foreign key constraint violation occurred. Please ensure that the data you're trying to update does not violate any constraints.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("An error occurred while updating the entries. See the inner exception for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            LoadUsers(); // Refresh the ListView
        }


        private void LoyaltyPoints_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out int result) || result < 0)
            {
                e.Handled = true;
            }
        }
    }

    public class CustomerViewModel
    {
        public int CustomerID { get; set; }
        public string Email { get; set; }
        public int LoyaltyPoints { get; set; }
        public string Tier { get; set; }
    }
}


