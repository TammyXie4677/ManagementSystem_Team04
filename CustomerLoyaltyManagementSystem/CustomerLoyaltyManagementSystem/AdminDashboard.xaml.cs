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
            MainContent.Content = new ProgramControl(); // 默认加载促销活动页面
        }

        private void ProgramsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ProgramControl();
        }
        /*
        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UsersControl();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ReportsControl();
        }*/
    }
}
