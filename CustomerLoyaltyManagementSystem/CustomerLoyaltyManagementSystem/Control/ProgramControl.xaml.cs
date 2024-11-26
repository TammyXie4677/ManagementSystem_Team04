using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CustomerLoyaltyManagementSystem.Control
{
    /// <summary>
    /// Interaction logic for ProgramControl.xaml
    /// </summary>
    /// 
    public class Program
    {
        public int Id { get; set; }
        public string ProgramName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProgramTier { get; set; }

    }

    public partial class ProgramControl : UserControl
    {
        private ObservableCollection<Program> programs;
        public ProgramControl()
        {
            InitializeComponent();
            programs = new ObservableCollection<Program>();
            LoadPrograms();
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
            LoadPrograms();
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

        // get all programs from db
        private void LoadPrograms()
        {
            try
            {
                programs.Clear();

                string connectionString = "data source=managementsystem-team04.database.windows.net;initial catalog=managementsystem_db;persist security info=True;user id=adminDb;password=5uK]Fd£C29_E;MultipleActiveResultSets=True;App=EntityFramework";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();  // open db connection

                    // get all info
                    string query = "SELECT * FROM LoyaltyProgram";

                    // Create a SqlCommand object and execute the query
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Program program = new Program
                                {
                                    ProgramName = reader["ProgramName"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    StartDate = reader["StartDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["StartDate"]),
                                    EndDate = reader["EndDate"] == DBNull.Value ? DateTime.MinValue: Convert.ToDateTime(reader["EndDate"]),
                                    ProgramTier = reader["ProgramTier"].ToString()
                                };
                                programs.Add(program);
                            }
                        }
                    }
                }

                ProgramsDataGrid.ItemsSource = programs;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前选中行的 ProgramModel
            var selectedProgram = (Program)ProgramsDataGrid.SelectedItem;
            if (selectedProgram != null)
            {
                MessageBox.Show($"Edit {selectedProgram.ProgramName} (implement your logic here)", "Edit", MessageBoxButton.OK);
                // 添加编辑逻辑，例如弹出编辑窗口
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前选中行的 ProgramModel
            var selectedProgram = (Program)ProgramsDataGrid.SelectedItem;
            if (selectedProgram != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {selectedProgram.ProgramName}?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteProgram(selectedProgram.ProgramName);
                    LoadPrograms(); // 刷新表格
                }
            }
        }
        private void DeleteProgram(string programName)
        {
            try
            {
                string connectionString = "data source=managementsystem-team04.database.windows.net;initial catalog=managementsystem_db;persist security info=True;user id=adminDb;password=5uK]Fd£C29_E;MultipleActiveResultSets=True;App=EntityFramework";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM LoyaltyProgram WHERE ProgramName = @ProgramName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProgramName", programName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
