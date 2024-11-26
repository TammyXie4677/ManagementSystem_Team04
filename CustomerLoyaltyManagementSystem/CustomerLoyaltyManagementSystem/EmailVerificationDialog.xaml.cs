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
        private string userEmail;
        public bool IsVerified { get; private set; }
        public EmailVerificationDialog(string email)
        {
            InitializeComponent();
            userEmail = email;
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredCode = VerificationCodeTextBox.Text;

            using (var context = new managementsystem_dbEntities())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user != null && user.VerificationCode == enteredCode)
                {
                    IsVerified = true;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid verification code. Please try again.", "Verification Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
