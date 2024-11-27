using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CustomerLoyaltyManagementSystem
{
    /// <summary>
    /// Interaction logic for CustomerDashboard.xaml
    /// </summary>
    public partial class CustomerDashboard : Window
    {
        private int userId;
        public CustomerDashboard(int userID)
        {
            InitializeComponent();
            this.userId = userID;

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help section is under construction.", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // This is the event handler for the button click
        private void OnViewCustomerLoyaltyButtonClick(object sender, RoutedEventArgs e)
        {
            ShowCustomerLoyaltyPage(userId);
        }

        // This method will create and show the CustomerLoyaltyPage
        private void ShowCustomerLoyaltyPage(int userId)
        {
            using (var context = new managementsystem_dbEntities())
            {
                var customer = context.Customers.SingleOrDefault(c => c.UserID == userId);
                if (customer != null)
                {
                    CustomerLoyaltyPage customerLoyaltyPage = new CustomerLoyaltyPage(customer.CustomerID);
                    customerLoyaltyPage.Show(); // Show the Customer Loyalty Page
                    this.Hide(); // Optionally hide the dashboard window
                }
                else
                {
                    MessageBox.Show("Customer information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
