using System;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace CustomerLoyaltyManagementSystem
{
    public partial class CustomerLoyaltyPage : Window
    {
        private int customerId;
        private int userId;
        private int customerLoyaltyPoints;
        private string customerTier;


        public CustomerLoyaltyPage(int customerId)
        {
            InitializeComponent();
            this.customerId = customerId;
            DatabaseConnection(); // Ensure the database connection is established
            LoadCustomerData(); // Load data for the customer

            var conversionData = new List<ConversionChart>
        {
            new ConversionChart { Tier = "Silver", ConversionRate = "100 points = $10", DollarValue = "1.0x" },
            new ConversionChart { Tier = "Gold", ConversionRate = "100 points = $15", DollarValue = "1.5x" },
            new ConversionChart { Tier = "Platinum", ConversionRate = "100 points = $20", DollarValue = "2.0x" }
        };

            // Set DataContext or directly assign to DataGrid
            ConversionChartDataGrid.ItemsSource = conversionData;
        }

        private void DatabaseConnection()
        {
            try
            {
                string envConnectionString = Environment.GetEnvironmentVariable("ENV_CONNECTION_STRING");

                if (string.IsNullOrEmpty(envConnectionString))
                {
                    MessageBox.Show($"Environment Variable Value: {envConnectionString}", "Check Environment Variable", MessageBoxButton.OK, MessageBoxImage.Information);
                    throw new InvalidOperationException("Environment variable 'ENV_CONNECTION_STRING' is not set.");
                }

                MessageBox.Show("Database connection was successful.", "Database Connection Succeeded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Inside the LoadCustomerData method, after loading the redeemed points
        private void LoadCustomerData()
        {
            string connectionString = Environment.GetEnvironmentVariable("ENV_CONNECTION_STRING");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Load customer data
                    string customerQuery = @"SELECT C.LoyaltyPoints, C.Tier, U.Email 
                                      FROM Customer C 
                                      INNER JOIN [User] U ON C.UserID = U.UserID 
                                      WHERE C.CustomerID = @CustomerID";

                    using (SqlCommand customerCommand = new SqlCommand(customerQuery, connection))
                    {
                        customerCommand.Parameters.AddWithValue("@CustomerID", customerId);

                        using (SqlDataReader reader = customerCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                customerLoyaltyPoints = Convert.ToInt32(reader["LoyaltyPoints"]);
                                customerTier = reader["Tier"].ToString();
                                string email = reader["Email"].ToString();

                                TotalEarnedPointsText.Text = customerLoyaltyPoints.ToString();
                                TierText.Text = customerTier;
                                EmailText.Text = email;

                                // Display points needed for the next tier
                                DisplayPointsNeededForNextTier(customerLoyaltyPoints);
                            }
                        }
                    }

                    // Load total earned and redeemed points, and transaction history
                    string transactionQuery = @"
        SELECT 
            SUM(CASE WHEN PointsEarned IS NOT NULL THEN PointsEarned ELSE 0 END) AS TotalEarnedPoints,
            SUM(CASE WHEN PointsRedeemed IS NOT NULL THEN PointsRedeemed ELSE 0 END) AS TotalRedeemedPoints,
            PointsEarned, PointsRedeemed, Date, TransactionType
        FROM TransactionLoyalty
        WHERE CustomerID = @CustomerID
        GROUP BY Date, PointsEarned, PointsRedeemed, TransactionType";

                    List<TransactionLoyalty> transactions = new List<TransactionLoyalty>();
                    decimal totalRedeemed = 0;
                    decimal totalEarned = 0;

                    using (SqlCommand transactionCommand = new SqlCommand(transactionQuery, connection))
                    {
                        transactionCommand.Parameters.AddWithValue("@CustomerID", customerId);

                        using (SqlDataReader transactionReader = transactionCommand.ExecuteReader())
                        {
                            while (transactionReader.Read())
                            {
                                totalEarned += transactionReader.IsDBNull(0) ? 0 : Convert.ToDecimal(transactionReader["TotalEarnedPoints"]);
                                totalRedeemed += transactionReader.IsDBNull(1) ? 0 : Convert.ToDecimal(transactionReader["TotalRedeemedPoints"]);

                                transactions.Add(new TransactionLoyalty
                                {
                                    PointsEarned = transactionReader.IsDBNull(2) ? 0 : Convert.ToInt32(transactionReader["PointsEarned"]),
                                    PointsRedeemed = transactionReader.IsDBNull(3) ? 0 : Convert.ToInt32(transactionReader["PointsRedeemed"]),
                                    TransactionType = transactionReader["TransactionType"].ToString(),
                                    Date = transactionReader.IsDBNull(4) ? (DateTime?)null : transactionReader.GetDateTime(4)
                                });
                            }
                        }
                    }

                    // Bind transaction data
                    TransactionHistoryList.ItemsSource = transactions;

                    // Set Total Earned and Redeemed Points in the UI
                    TotalRedeemedPointsText.Text = totalRedeemed.ToString();

                    // Calculate and display redeemable value
                    double redeemableValue = CalculateRedeemableValue(customerLoyaltyPoints, customerTier);
                    RedeemableValueText.Text = $"${redeemableValue:F2}";

                    // Display the total value redeemed
                    DisplayTotalValueRedeemed(totalRedeemed, customerTier);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DisplayPointsNeededForNextTier(int currentPoints)
{
    int pointsNeeded = 0;
    string nextTier = string.Empty;

    // Determine next tier based on current points
    if (currentPoints < 200)
    {
        nextTier = "Gold";
        pointsNeeded = 200 - currentPoints;  // 200 points required to reach Gold
    }
    else if (currentPoints >= 200 && currentPoints < 400)
    {
        nextTier = "Platinum";
        pointsNeeded = 400 - currentPoints;  // 400 points required to reach Platinum
    }
    else
    {
        nextTier = "Max Tier (Platinum)";  // Already at Platinum or above
        pointsNeeded = 0;  // No more points needed
    }

    // Update the UI with the points needed for the next tier
    PointsNeededForNextTierText.Text = $"{pointsNeeded} points needed to reach {nextTier}.";
}



        // Helper method to calculate redeemable value
        private double CalculateRedeemableValue(int points, string tier)
        {
            switch (tier.ToLower())
            {
                case "silver":
                    return points / 10.0; // Example: 100 points = $10
                case "gold":
                    return points * 1.5 / 10.0; // Example: 100 points = $15
                case "platinum":
                    return points * 2.0 / 10.0; // Example: 100 points = $20
                default:
                    return 0.0;
            }
        }

        // Method to display the total value redeemed based on points and tier
        private void DisplayTotalValueRedeemed(decimal totalRedeemedPoints, string tier)
        {
            double totalRedeemedValue = CalculateRedeemableValue((int)totalRedeemedPoints, tier);
            TotalRedeemedValueText.Text = $"${totalRedeemedValue:F2}";
        }



        private void RedeemButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input for points to redeem
            int pointsToRedeem = 0;
            if (string.IsNullOrWhiteSpace(RedeemPointsTextBox.Text) ||
                !int.TryParse(RedeemPointsTextBox.Text, out pointsToRedeem) ||
                pointsToRedeem <= 0)
            {
                MessageBox.Show("Please enter a valid number of points to redeem.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int totalPoints = 0;
            // Validate input for total points
            if (string.IsNullOrWhiteSpace(TotalEarnedPointsText.Text) ||
                !int.TryParse(TotalEarnedPointsText.Text, out totalPoints) ||
                totalPoints < 0)
            {
                MessageBox.Show("Total points value is invalid. Please check the system data.", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Define the minimum redeemable value (example: 100 points)
            int minimumRedeemablePoints = 100;

            if (pointsToRedeem < minimumRedeemablePoints)
            {
                MessageBox.Show($"You need at least {minimumRedeemablePoints} points to redeem.", "Minimum Points Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (pointsToRedeem > totalPoints)
            {
                MessageBox.Show("You don't have enough points to redeem this amount.", "Insufficient Points", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create a new transaction record for the redemption
            CreateRedemptionTransaction(pointsToRedeem);

            // Update the total points and redeemable value
            int newTotalPoints = totalPoints - pointsToRedeem;
            UpdateCustomerPoints(newTotalPoints);
            UpdateRedeemableValue(newTotalPoints); // Assuming this method updates the redeemable value text

            // Update the total redeemed points in the UI
            int totalRedeemedPoints = int.Parse(TotalRedeemedPointsText.Text);
            TotalRedeemedPointsText.Text = (totalRedeemedPoints + pointsToRedeem).ToString();

            // Update the UI to reflect the new total points
            TotalEarnedPointsText.Text = newTotalPoints.ToString();
        }

        private void CreateRedemptionTransaction(int pointsRedeemed)
        {
            string connectionString = Environment.GetEnvironmentVariable("ENV_CONNECTION_STRING");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Calculate the redeemable amount based on points and tier
                    string tier = TierText.Text; // Get the current tier of the customer
                    double amount = CalculateRedeemableValue(pointsRedeemed, tier); // Calculate the redeemable amount

                    // Create a new transaction record for the redemption
                    string query = @"INSERT INTO TransactionLoyalty 
                             (CustomerID, PointsRedeemed, TransactionType, Date, Amount) 
                             VALUES (@CustomerID, @PointsRedeemed, @TransactionType, @Date, @Amount)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Set parameters to avoid NULL values
                        command.Parameters.AddWithValue("@CustomerID", customerId); // Ensure CustomerID is not NULL
                        command.Parameters.AddWithValue("@PointsRedeemed", pointsRedeemed); // Points redeemed, ensure it's not NULL
                        command.Parameters.AddWithValue("@TransactionType", "Redemption"); // Redemption type is set
                        command.Parameters.AddWithValue("@Date", DateTime.Now); // Set current date/time
                        command.Parameters.AddWithValue("@Amount", amount); // Calculated redeemable amount

                        command.ExecuteNonQuery(); // Execute the query
                    }

                    MessageBox.Show("Points redeemed successfully!", "Redemption Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateCustomerPoints(int newTotalPoints)
        {
            string connectionString = Environment.GetEnvironmentVariable("ENV_CONNECTION_STRING");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Update the customer's total points
                    string query = @"UPDATE Customer SET LoyaltyPoints = @NewTotalPoints WHERE CustomerID = @CustomerID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        command.Parameters.AddWithValue("@NewTotalPoints", newTotalPoints);

                        command.ExecuteNonQuery();
                    }

                    // Update the UI
                    TotalEarnedPointsText.Text = newTotalPoints.ToString();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRedeemableValue(int newTotalPoints)
        {
            string tier = TierText.Text;
            double redeemableValue = CalculateRedeemableValue(newTotalPoints, tier);
            RedeemableValueText.Text = $"${redeemableValue:F2}";
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomerData(); // Reload the customer data from the database
        }

        // Event handler for the Return to Dashboard button
        private void ReturnToDashboard_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new managementsystem_dbEntities())
            {
                var customer = context.Customers.SingleOrDefault(c => c.CustomerID == customerId);
                if (customer != null)
                {

                    // Open the customer dashboard (assuming CustomerDashboard is a class representing the dashboard)
                    var customerDashboard = new CustomerDashboard((int)customer.UserID);
                    customerDashboard.Show();
                    // Close the current window
                    this.Close();
                }
            }

        }
    }
}
