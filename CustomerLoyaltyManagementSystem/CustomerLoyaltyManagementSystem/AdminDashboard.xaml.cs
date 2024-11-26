using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CustomerLoyaltyManagementSystem
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }
        // Click event for saving a promotion
        private void SavePromotionButton_Click(object sender, RoutedEventArgs e)
        {
            string promotionName = ProgramNameTextBox.Text;
            string description = DescriptionTextBox.Text;
            var startDate = StartDatePicker.SelectedDate;
            var endDate = EndDatePicker.SelectedDate;
            var selectedItem = TierComboBox.SelectedItem as ComboBoxItem;
            var selectedTier = selectedItem?.Content.ToString();

            if (string.IsNullOrWhiteSpace(promotionName) ||
                string.IsNullOrWhiteSpace(description) ||
                startDate == null || endDate == null ||
                string.IsNullOrWhiteSpace(selectedTier))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // get current date
            DateTime today = DateTime.Today;

            if (startDate < today)
            {
                MessageBox.Show("Start Date cannot be earlier than today.", "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (startDate > endDate)
            {
                MessageBox.Show("Start date cannot be after the end date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            // check validation
            if (selectedTier == "Select a tier" || string.IsNullOrEmpty(selectedTier))
            {
                MessageBox.Show("Please select a valid program tier.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // save to db
            SaveProgramToDatabase(promotionName, description, startDate.Value, endDate.Value, selectedTier);
        }

        private void SaveProgramToDatabase(string promotionName, string description, DateTime startDate, DateTime endDate, string tier)
        {
            try
            {
                string connectionString = "data source=managementsystem-team04.database.windows.net;initial catalog=managementsystem_db;persist security info=True;user id=adminDb;password=5uK]Fd£C29_E;MultipleActiveResultSets=True;App=EntityFramework";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO LoyaltyProgram (ProgramName, Description, StartDate, EndDate, ProgramTier) " +
                                   "VALUES (@ProgramName, @Description, @StartDate, @EndDate, @ProgramTier)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProgramName", promotionName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);
                        cmd.Parameters.AddWithValue("@ProgramTier", tier);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Program saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // catch exception
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear all input fields
            ProgramNameTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            TierComboBox.SelectedIndex = 0; // Reset dropdown to "Select a Tier"
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

    }
}
