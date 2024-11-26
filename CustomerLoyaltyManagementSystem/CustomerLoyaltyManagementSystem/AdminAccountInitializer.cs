using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CustomerLoyaltyManagementSystem
{
    public static class AdminAccountInitializer
    {
        public static void EnsureAdminAccount()
        {
            try
            {
                using (var context = new managementsystem_dbEntities()) 
                {
                    // Check if an admin account already exists
                    var existingAdmin = context.Users.FirstOrDefault(u => u.Role == "Admin");

                    if (existingAdmin != null)
                    {
                        Console.WriteLine($"Admin account already exists: {existingAdmin.Email}");
                        return;
                    }

                    // Create a new admin account
                    var adminUser = new User
                    {
                        Email = "admin@example.com", 
                        PasswordHashed = HashPassword("Admin@123"), 
                        DateJoined = DateTime.Now,
                        Role = "Admin",
                        VerificationCode = null,
                        IsVerified = false
                    };

                    // Add the new admin account to the database
                    context.Users.Add(adminUser);
                    context.SaveChanges();

                    Console.WriteLine("Admin account created successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring admin account: {ex.Message}");
            }
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

