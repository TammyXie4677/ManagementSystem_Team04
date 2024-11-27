using System.Linq;
using System.Windows;

namespace CustomerLoyaltyManagementSystem
{
    public partial class CustomerDashboard : Window
    {
        private int userId;

        public CustomerDashboard(int userID)
        {
            InitializeComponent();
            DisplayUserEmail();
            this.userId = userID;
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
            ShowCustomerLoyaltyPage(userId);
        }

        private void OnViewLoyaltyProgramsButtonClick(object sender, RoutedEventArgs e)
        {
            ShowLoyaltyProgramsPage(userId);
        }

        private void ShowCustomerLoyaltyPage(int userId)
        {
            using (var context = new managementsystem_dbEntities())
            {
                var customer = context.Customers.SingleOrDefault(c => c.UserID == userId);
                if (customer != null)
                {
                    CustomerLoyaltyPage customerLoyaltyPage = new CustomerLoyaltyPage(customer.CustomerID);
                    customerLoyaltyPage.Show(); // Show the Customer Loyalty Page
                    this.Hide(); // Hide the dashboard window
                }
                else
                {
                    MessageBox.Show("Customer information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowLoyaltyProgramsPage(int userId)
        {
            using (var context = new managementsystem_dbEntities())
            {
                var customer = context.Customers.SingleOrDefault(c => c.UserID == userId);
                if (customer != null)
                {
                    string customerTier = customer.Tier ?? "DefaultTier";  // Default value if Tier is null
                    int customerId = customer.CustomerID;

                    // Pass CustomerID and Tier to LoyaltyPrograms page
                    LoyaltyPrograms loyaltyPage = new LoyaltyPrograms(customerId, customerTier);
                    loyaltyPage.Show(); // Show the Loyalty Programs Page
                    this.Hide(); // Hide the dashboard window
                }
                else
                {
                    MessageBox.Show("Customer information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}
