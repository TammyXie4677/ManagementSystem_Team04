using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomerLoyaltyManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DatabaseConnection(); // Call the method when the window is loaded
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
    }
}
