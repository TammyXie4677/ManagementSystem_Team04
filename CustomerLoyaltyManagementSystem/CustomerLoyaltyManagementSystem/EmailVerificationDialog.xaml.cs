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
    /// Interaction logic for EmailVerificationDialog.xaml
    /// </summary>
    public partial class EmailVerificationDialog : Window
    {
        public EmailVerificationDialog()
        {
            InitializeComponent();
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            //For testing
            MessageBox.Show("Verify button clicked!");
        }

        private void ResendButton_Click(object sender, RoutedEventArgs e)
        {
            //For testing
            MessageBox.Show("Send button clicked!");
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e); 
        }
    }
}
