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
            try
            {
                SqlConnection connection = new SqlConnection("Data Source=managementsystem-team04.database.windows.net;initial catalog=managementsystem_db;persist security info=True;user id=adminDb;password=5uK]Fd£C29_E;MultipleActiveResultSets=True;App=EntityFramework");
                connection.Open();

                // Load customer data (email, total points, tier)
                SqlCommand command = new SqlCommand("SELECT C.CustomerID, C.UserID, C.LoyaltyPoints, C.Tier, U.Email FROM Customer C INNER JOIN [User] U ON C.UserID = U.UserID WHERE C.CustomerID = @CustomerID", connection);
                command.Parameters.AddWithValue("@CustomerID", customerId);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int loyaltyPoints = Convert.ToInt32(reader["LoyaltyPoints"]);
                    string tier = reader["Tier"].ToString();
                    string email = reader["Email"].ToString();

                    TotalPointsText.Text = $"{loyaltyPoints}";
                    TierText.Text = $"{tier}";
                    EmailText.Text = $"{email}";
                }

                // Load transaction history (including registration points)
                List<TransactionRecord> transactions = new List<TransactionRecord>();
                SqlCommand transactionCommand = new SqlCommand("SELECT PointsEarned, Date, 'Registration Bonus' AS Details FROM TransactionLoyalty WHERE CustomerID = @CustomerID AND TransactionType = 'Program'", connection);
                transactionCommand.Parameters.AddWithValue("@CustomerID", customerId);
                SqlDataReader transactionReader = transactionCommand.ExecuteReader();

                while (transactionReader.Read())
                {
                    transactions.Add(new TransactionRecord
                    {
                        PointsEarned = Convert.ToInt32(transactionReader["PointsEarned"]),
                        Details = transactionReader["Details"].ToString(),
                        Date = Convert.ToDateTime(transactionReader["Date"]).ToString("MM/dd/yyyy")
                    });
                }

                // Bind transaction data to the ListBox
                TransactionHistoryList.ItemsSource = transactions;

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Data Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class TransactionRecord
        {
            public int PointsEarned { get; set; }
            public string Details { get; set; }
            public string Date { get; set; }
        }
    }
}
