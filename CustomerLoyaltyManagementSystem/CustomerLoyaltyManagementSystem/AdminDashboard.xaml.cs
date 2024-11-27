using CustomerLoyaltyManagementSystem.Control;
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
            DisplayUserEmail();
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

        private void ManagePrograms_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ProgramControl();
        }

        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UsersControl();
        }

        private void GenerateReports_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ReportsControl();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help section is under construction.", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

