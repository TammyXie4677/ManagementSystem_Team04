using System;
using System.Collections.Generic;
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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private bool isExitButtonClicked = false;
        public Registration()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            EmailVerificationDialog emailVerificationDialog = new EmailVerificationDialog(); 
            emailVerificationDialog.ShowDialog();
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            isExitButtonClicked = true;
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!isExitButtonClicked)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }
    } 
}
