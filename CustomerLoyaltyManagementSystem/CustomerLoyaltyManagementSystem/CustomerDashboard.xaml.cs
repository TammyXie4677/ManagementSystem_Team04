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
        public CustomerDashboard()
        {
            InitializeComponent();
            DisplayUserEmail();
        }

        private void DisplayUserEmail()
        { 
            // Fetch email from session
            string userEmail = Application.Current.Properties["Email"] as string; 
            if (!string.IsNullOrEmpty(userEmail)) 
            { 
                emailTextBlock.Text = $"Logged in as: {userEmail}";
            } 
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
            int customerId = 1; // Example: You can pass the actual customer ID here (dynamically assigned in your case)
            ShowCustomerLoyaltyPage(customerId);
        }

        // This method will create and show the CustomerLoyaltyPage
        private void ShowCustomerLoyaltyPage(int customerId)
        {
            CustomerLoyaltyPage customerLoyaltyPage = new CustomerLoyaltyPage(customerId);
            customerLoyaltyPage.Show(); // Show the Customer Loyalty Page
            this.Hide(); // Optionally hide the main window
        }
    }
}
