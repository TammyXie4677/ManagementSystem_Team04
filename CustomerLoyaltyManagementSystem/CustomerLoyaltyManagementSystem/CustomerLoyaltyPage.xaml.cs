using System;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.Generic;

namespace CustomerLoyaltyManagementSystem
{
    public partial class CustomerLoyaltyPage : Window
    {
        private int customerId;

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
                                TotalPointsText.Text = reader["LoyaltyPoints"].ToString();
                                TierText.Text = reader["Tier"].ToString();
                                EmailText.Text = reader["Email"].ToString();
                            }
                        }
                    }

                    // Load transaction history
                    string transactionQuery = @"SELECT PointsEarned, Date, 'Registration Bonus' AS Details 
                                        FROM TransactionLoyalty 
                                        WHERE CustomerID = @CustomerID AND TransactionType = 'Program'";

                    List<TransactionRecord> transactions = new List<TransactionRecord>();

                    using (SqlCommand transactionCommand = new SqlCommand(transactionQuery, connection))
                    {
                        transactionCommand.Parameters.AddWithValue("@CustomerID", customerId);

                        using (SqlDataReader transactionReader = transactionCommand.ExecuteReader())
                        {
                            while (transactionReader.Read())
                            {
                                transactions.Add(new TransactionRecord
                                {
                                    PointsEarned = Convert.ToInt32(transactionReader["PointsEarned"]),
                                    Details = transactionReader["Details"].ToString(),
                                    Date = Convert.ToDateTime(transactionReader["Date"]).ToString("MM/dd/yyyy")
                                });
                            }
                        }
                    }

                    // Bind transaction data
                    TransactionHistoryList.ItemsSource = transactions;

                    // Calculate and display redeemable value
                    int loyaltyPoints = int.Parse(TotalPointsText.Text);
                    string tier = TierText.Text;
                    double redeemableValue = CalculateRedeemableValue(loyaltyPoints, tier);
                    RedeemableValueText.Text = $"${redeemableValue:F2}";
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


        // Helper method to calculate redeemable value
        private double CalculateRedeemableValue(int points, string tier)
        {
            switch (tier.ToLower())
            {
                case "silver":
                    return points / 10.0;
                case "gold":
                    return points * 1.5 / 10.0;
                case "platinum":
                    return points * 2.0 / 10.0;
                default:
                    return 0.0;
            }
        }


        public class TransactionRecord
        {
            public int PointsEarned { get; set; }
            public string Details { get; set; }
            public string Date { get; set; }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
