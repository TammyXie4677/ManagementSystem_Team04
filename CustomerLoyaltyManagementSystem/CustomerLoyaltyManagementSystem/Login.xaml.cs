using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace CustomerLoyaltyManagementSystem
{
    public partial class Login : Window
    {
        private bool isExitButtonClicked = false;
        private bool isLoginSuccessful = false; 

        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            EmailErrorTextBlock.Text = string.Empty;
            PasswordErrorTextBlock.Text = string.Empty;

            if (!IsValidEmail(email))
            {
                EmailErrorTextBlock.Text = "Invalid email format.";
                return;
            }

            using (var context = new managementsystem_dbEntities())
            {
                var user = context.Users.SingleOrDefault(u => u.Email == email);

                if (user == null)
                {
                    EmailErrorTextBlock.Text = "User does not exist.";
                    return;
                }

                if (!VerifyPassword(password, user.PasswordHashed))
                {
                    PasswordErrorTextBlock.Text = "Incorrect password.";
                    return;
                }

                // Successful login, set the flag to true
                isLoginSuccessful = true;

                // Store UserID and Email in session
                Application.Current.Properties["UserID"] = user.UserID; 
                Application.Current.Properties["Email"] = user.Email;
                Navigation(user.Role);
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
            if (!isExitButtonClicked && !isLoginSuccessful) // Check the login status
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private void Navigation(string role)
        {
            if (role == "Admin")
            {
                // Navigate to Admin Dashboard
                AdminDashboard adminDashboard = new AdminDashboard();
                adminDashboard.Show();
            }
            else
            {
                int userId = (int)Application.Current.Properties["UserID"];
                CustomerDashboard customerDashboard = new CustomerDashboard(userId);
                customerDashboard.Show();
            }

            this.Close();
        }
    }
}

