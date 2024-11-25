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
            DatabaseConnection(); // Call the method when the window is loaded
        }

        private void DatabaseConnection2(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection("Data Source=managementsystem-team04.database.windows.net;initial catalog=managementsystem_db;persist security info=True;user id=adminDb;password=5uK]Fd£C29_E;MultipleActiveResultSets=True;App=EntityFramework");
            connection.Open();
            SqlCommand command = new SqlCommand("select C.CustomerID,C.UserID,C.LoyaltyPoints,C.Tier,U.Email from Customer C inner join [User] U ON C.UserID=U.UserID where C.CustomerID =1;", connection);
            //var customerId = Convert.ToInt32(txtCtmID.Text);
            //command.Parameters.AddWithValue("@CustomerID", customerId);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                MessageBox.Show(reader["CustomerID"].ToString());
                MessageBox.Show(reader["UserID"].ToString());
                MessageBox.Show(reader["LoyaltyPoints"].ToString());
                MessageBox.Show(reader["Tier"].ToString());
                MessageBox.Show(reader["Email"].ToString());

            }
        }
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
