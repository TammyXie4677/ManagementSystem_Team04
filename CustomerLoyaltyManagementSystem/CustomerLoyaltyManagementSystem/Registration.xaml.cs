using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            string emailErrorMessage = ValidateEmail(EmailTextBox.Text);
            string passwordErrorMessage = ValidatePassword(PasswordBox.Password);
            string confirmPasswordErrorMessage = ValidateConfirmPassword(PasswordBox.Password, ConfirmPasswordBox.Password);

            EmailErrorTextBlock.Text = emailErrorMessage;
            PasswordErrorTextBlock.Text = passwordErrorMessage;
            ConfirmPasswordErrorTextBlock.Text = confirmPasswordErrorMessage;

            if (string.IsNullOrEmpty(emailErrorMessage) && string.IsNullOrEmpty(passwordErrorMessage) && string.IsNullOrEmpty(confirmPasswordErrorMessage))
            {
                if (IsUserExisting(EmailTextBox.Text))
                {
                    MessageBox.Show("This email is already registered.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    EmailVerificationDialog emailVerificationDialog = new EmailVerificationDialog();
                    emailVerificationDialog.ShowDialog();
                    this.Close();
                }
            }
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

        private string ValidateEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return "Invalid email format.";
            }
            return string.Empty;
        }

        private string ValidatePassword(string password)
        {
            if (password.Length < 8)
            {
                return "Password must be at least 8 characters long.";
            }

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigits = password.Any(char.IsDigit);
            bool hasSpecialChars = password.Any(ch => !char.IsLetterOrDigit(ch));

            if (!hasUpperCase)
            {
                return "Password must contain at least one uppercase letter.";
            }
            if (!hasLowerCase)
            {
                return "Password must contain at least one lowercase letter.";
            }
            if (!hasDigits && !hasSpecialChars)
            {
                return "Password must contain at least one number or special character.";
            }

            return string.Empty;
        }

        private string ValidateConfirmPassword(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return "Passwords do not match.";
            }
            return string.Empty;
        }

        private bool IsUserExisting(string email)
        {
            using (var context = new managementsystem_dbEntities())
            {
                return context.Users.Any(u => u.Email == email);
            }
        }
    }
}
