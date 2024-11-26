using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
                    // Hash the password
                    string hashedPassword = HashPassword(PasswordBox.Password);

                    // Store user data without email verification
                    StoreUserData(EmailTextBox.Text, hashedPassword, null);

                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();

                    // Generate and send the verification code
                    // string verificationCode = GenerateVerificationCode();
                    // bool emailSent = SendVerificationEmail(EmailTextBox.Text, verificationCode);

                    // if (emailSent)
                    // {
                    //     // Store the user data and verification code
                    //     StoreUserData(EmailTextBox.Text, hashedPassword, verificationCode);

                    //     // Show the email verification dialog
                    //     EmailVerificationDialog emailVerificationDialog = new EmailVerificationDialog(EmailTextBox.Text);
                    //     if (emailVerificationDialog.ShowDialog() == true)
                    //     {
                    //         if (emailVerificationDialog.IsVerified)
                    //         {
                    //             UpdateUserVerificationStatus(EmailTextBox.Text);
                    //             MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    //             this.Close();
                    //         }
                    //         else
                    //         {
                    //             MessageBox.Show("Verification failed. Please try again.", "Verification Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    //         }
                    //     }
                    // }
                    // else
                    // {
                    //     MessageBox.Show("Failed to send the verification email. Please try again later.", "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    // }
                }
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // private string GenerateVerificationCode()
        // {
        //     const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //     Random random = new Random();
        //     return new string(Enumerable.Repeat(chars, 10)
        //         .Select(s => s[random.Next(s.Length)]).ToArray());
        // }

        // private bool SendVerificationEmail(string email, string verificationCode)
        // {
        //     try
        //     {
        //         using (var smtpClient = new System.Net.Mail.SmtpClient("smtp.office365.com"))
        //         {
        //             smtpClient.Port = 587;
        //             smtpClient.Credentials = new System.Net.NetworkCredential("testverification654321@outlook.com", "#");
        //             smtpClient.EnableSsl = true;

        //             var mailMessage = new System.Net.Mail.MailMessage
        //             {
        //                 From = new System.Net.Mail.MailAddress("testverification654321@outlook.com"),
        //                 Subject = "Your Verification Code",
        //                 Body = $"Your verification code is: {verificationCode}",
        //                 IsBodyHtml = false,
        //             };

        //             mailMessage.To.Add(email);

        //             smtpClient.Send(mailMessage);
        //         }

        //         return true;
        //     }
        //     catch (SmtpException smtpEx)
        //     {
        //         // Log SMTP-specific errors
        //         Console.WriteLine($"SMTP error occurred: {smtpEx.StatusCode} - {smtpEx.Message}");
        //         MessageBox.Show($"SMTP error occurred: {smtpEx.StatusCode} - {smtpEx.Message}", "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //         return false;
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log general errors
        //         Console.WriteLine($"An error occurred while sending the email: {ex.Message}");
        //         MessageBox.Show($"An error occurred while sending the email: {ex.Message}", "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //         return false;
        //     }
        // }

        // private void UpdateUserVerificationStatus(string email)
        // {
        //     try
        //     {
        //         using (var context = new managementsystem_dbEntities())
        //         {
        //             var user = context.Users.FirstOrDefault(u => u.Email == email);
        //             if (user != null)
        //             {
        //                 user.IsVerified = true;
        //                 context.SaveChanges();
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show($"An error occurred while updating user verification status: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //     }
        // }

        private void StoreUserData(string email, string hashedPassword, string verificationCode)
        {
            try
            {
                using (var context = new managementsystem_dbEntities())
                {
                    var user = new User
                    {
                        Email = email,
                        PasswordHashed = hashedPassword,
                        Role = "Customer",
                        DateJoined = DateTime.Now,
                        // VerificationCode = verificationCode,
                        IsVerified = false // Set to false until the user verifies
                    };
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving user data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
