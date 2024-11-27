using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Data.SqlClient;

namespace CustomerLoyaltyManagementSystem
{
    public partial class LoyaltyPrograms : Window
    {
        private int _customerId;
        private string _userTier;

        public LoyaltyPrograms(int customerId, string userTier)
        {
            InitializeComponent();
            _customerId = customerId;
            _userTier = userTier;
            DataContext = this;  // Set DataContext to the window itself to bind UserTier in XAML
            LoadLoyaltyPrograms();
            LoadCustomerLoyaltyPoints();
        }

        public string UserTier => _userTier;  // Property for binding user's tier in XAML

        private void LoadLoyaltyPrograms()
        {
            using (var context = new managementsystem_dbEntities())
            {
                // Filter loyalty programs based on user's tier
                var programs = context.LoyaltyPrograms
                    .Where(p =>
                        (_userTier == "Platinum") ||
                        (_userTier == "Gold" && (p.Tier == "Gold" || p.Tier == "Silver")) ||
                        (_userTier == "Silver" && p.Tier == "Silver"))
                    .ToList();

                // Bind the filtered list to the ListView
                ProgramsListView.ItemsSource = programs;
            }
        }

        private void LoadCustomerLoyaltyPoints()
        {
            using (var context = new managementsystem_dbEntities())
            {
                var customer = context.Customers.SingleOrDefault(c => c.CustomerID == _customerId);
                if (customer != null)
                {
                    TotalPointsTextBlock.Text = $"Total Points: {customer.LoyaltyPoints ?? 0}";
                }
                else
                {
                    MessageBox.Show("Unable to fetch customer information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void EarnPoints_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var program = (LoyaltyProgram)button.DataContext;

            // Record transaction
            RecordTransaction(program.ProgramID, (int)program.Points);
        }

        private void RecordTransaction(int programId, int pointsEarned)
        {
            using (var context = new managementsystem_dbEntities())
            {
                var customer = context.Customers.SingleOrDefault(c => c.CustomerID == _customerId);
                if (customer != null)
                {
                    customer.LoyaltyPoints = (customer.LoyaltyPoints ?? 0) + pointsEarned;

                    var transaction = new TransactionLoyalty
                    {
                        CustomerID = _customerId,
                        ProgramID = programId,
                        Date = DateTime.Now,
                        PointsEarned = pointsEarned,
                        PointsRedeemed = 0,
                        TransactionType = "Program"
                    };

                    context.TransactionLoyalties.Add(transaction);
                    context.SaveChanges();

                    // Update the total points displayed in the UI
                    TotalPointsTextBlock.Text = $"Total Points: {customer.LoyaltyPoints}";

                    MessageBox.Show($"You have earned {pointsEarned} points for completing Program {programId}!");
                }
                else
                {
                    MessageBox.Show("Customer information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        //private void UpdateCustomerPoints(int newTotalPoints)
        //{
        //    string connectionString = Environment.GetEnvironmentVariable("ENV_CONNECTION_STRING");

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            // Update the customer's total points
        //            string query = @"UPDATE Customer SET LoyaltyPoints = @NewTotalPoints WHERE CustomerID = @CustomerID";

        //            using (SqlCommand command = new SqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@CustomerID", customerId);
        //                command.Parameters.AddWithValue("@NewTotalPoints", newTotalPoints);

        //                command.ExecuteNonQuery();
        //            }

        //            // Update the UI
        //            TotalEarnedPointsText.Text = newTotalPoints.ToString();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
        // Event handler for the Return to Dashboard button
        private void ReturnToDashboard_Click(object sender, RoutedEventArgs e)
        {

            using (var context = new managementsystem_dbEntities())
            {
                var customer = context.Customers.SingleOrDefault(c => c.CustomerID == _customerId);
                if (customer != null)
                {
                    // Open the customer dashboard (assuming CustomerDashboard is a class representing the dashboard)
                    var customerDashboard = new CustomerDashboard((int)customer.UserID);  // Assuming CustomerDashboard takes userID as a parameter
                    customerDashboard.Show();
                    // Close the current window
                    this.Close();
                }
            }
        }
    }
}
