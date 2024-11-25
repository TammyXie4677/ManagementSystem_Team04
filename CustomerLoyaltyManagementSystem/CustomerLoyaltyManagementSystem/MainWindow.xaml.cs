using System;
using System.Data.SqlClient;
using System.Windows;

namespace CustomerLoyaltyManagementSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           // DatabaseConnection(); // Call the method when the window is loaded
        }

        /*
        private void DatabaseConnection()
        {
            try
            {

               
                // Get the environment variable
                string envConnectionString = Environment.GetEnvironmentVariable("ENV_CONNECTION_STRING");

                if (string.IsNullOrEmpty(envConnectionString))
                {
                    MessageBox.Show($"Environment Variable Value: {envConnectionString}", "Check Environment Variable", MessageBoxButton.OK, MessageBoxImage.Information);
                    throw new InvalidOperationException("Environment variable 'ENV_CONNECTION_STRING' is not set.");
                }

                // Display success message
                MessageBox.Show("Database connection was successful.", "Database Connection Succeeded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Display error message if something goes wrong
                MessageBox.Show("Error: " + ex.Message, "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        */

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close(); 
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }
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
