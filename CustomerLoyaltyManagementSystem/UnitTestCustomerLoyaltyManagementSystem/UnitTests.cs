using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using System.Data.SqlClient;
using System.Collections.Generic;
using CustomerLoyaltyManagementSystem;
using System;

namespace UnitTestCustomerLoyaltyManagementSystem
{
    [TestClass]
    public class UnitTests
    {
        private string connectionString = "data source=managementsystem-team04.database.windows.net;initial catalog=managementsystem_db;persist security info=True;user id=adminDb;password=5uK]Fd£C29_E;MultipleActiveResultSets=True;App=EntityFramework";

        [TestMethod]
        public void TestSessionStoresUserIDAndEmail()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockSession = new Mock<HttpSessionStateBase>();

            mockSession.Setup(s => s["UserID"]).Returns("12345");
            mockSession.Setup(s => s["Email"]).Returns("user@example.com");

            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            // Act
            var userID = mockHttpContext.Object.Session["UserID"];
            var email = mockHttpContext.Object.Session["Email"];

            // Assert
            Assert.AreEqual("12345", userID);
            Assert.AreEqual("user@example.com", email);
        }

        [TestMethod]
        public void TestDatabaseConnection()
        {
            // Arrange
            bool connectionSuccessful = false;

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    connectionSuccessful = true;
                }
                catch (SqlException)
                {
                    connectionSuccessful = false;
                }
            }

            // Assert
            Assert.IsTrue(connectionSuccessful, "Database connection should be successful.");
        }

        [TestMethod]
        public void TestAdminAccountExists()
        {
            // Arrange
            bool adminExists = false;

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM [User] WHERE Role = 'Admin'", connection))
                {
                    int count = (int)command.ExecuteScalar();
                    adminExists = count > 0;
                }
            }

            // Assert
            Assert.IsTrue(adminExists, "Admin account should exist.");
        }

        [TestMethod]
        public void TestCustomerTierValidity()
        {
            // Arrange
            var validTiers = new HashSet<string> { "Silver", "Gold", "Platinum" };
            bool allTiersValid = true;

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Tier FROM Customer", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tier = reader["Tier"].ToString();
                            if (!validTiers.Contains(tier))
                            {
                                allTiersValid = false;
                                break;
                            }
                        }
                    }
                }
            }

            // Assert
            Assert.IsTrue(allTiersValid, "All customer tiers should be 'Silver', 'Gold', or 'Platinum'.");
        }

        [TestMethod]
        public void TestUserProperties()
        {
            // Arrange
            var user = new User
            {
                UserID = 1,
                Email = "test@example.com",
                PasswordHashed = "hashedpassword",
                DateJoined = DateTime.Now,
                Role = "Admin",
                VerificationCode = "123456",
                IsVerified = true
            };

            // Assert
            Assert.AreEqual(1, user.UserID);
            Assert.AreEqual("test@example.com", user.Email);
            Assert.AreEqual("hashedpassword", user.PasswordHashed);
            Assert.IsNotNull(user.DateJoined);
            Assert.AreEqual("Admin", user.Role);
            Assert.AreEqual("123456", user.VerificationCode);
            Assert.IsTrue(user.IsVerified);
        }

        [TestMethod]
        public void TestCustomerProperties()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerID = 1,
                UserID = 1,
                LoyaltyPoints = 100,
                Tier = "Gold"
            };

            // Assert
            Assert.AreEqual(1, customer.CustomerID);
            Assert.AreEqual(1, customer.UserID);
            Assert.AreEqual(100, customer.LoyaltyPoints);
            Assert.AreEqual("Gold", customer.Tier);
        }

        [TestMethod]
        public void TestCustomerPointsNonNegative()
        {
            // Arrange
            bool allPointsNonNegative = true;

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT LoyaltyPoints FROM Customer", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var points = reader["LoyaltyPoints"] as int?;
                            if (points.HasValue && points < 0)
                            {
                                allPointsNonNegative = false;
                                break;
                            }
                        }
                    }
                }
            }

            // Assert
            Assert.IsTrue(allPointsNonNegative, "All customer loyalty points should be non-negative.");
        }

        [TestMethod]
        public void TestUserEmailNotNullOrEmpty()
        {
            // Arrange
            bool allEmailsValid = true;

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Email FROM [User]", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var email = reader["Email"].ToString();
                            if (string.IsNullOrEmpty(email))
                            {
                                allEmailsValid = false;
                                break;
                            }
                        }
                    }
                }
            }

            // Assert
            Assert.IsTrue(allEmailsValid, "All user emails should be non-null and non-empty.");
        }

        [TestMethod]
        public void TestLoyaltyProgramDatesValidity()
        {
            // Arrange
            bool allDatesValid = true;

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT StartDate, EndDate FROM LoyaltyProgram", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var startDate = reader["StartDate"] as DateTime?;
                            var endDate = reader["EndDate"] as DateTime?;
                            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                            {
                                allDatesValid = false;
                                break;
                            }
                        }
                    }
                }
            }

            // Assert
            Assert.IsTrue(allDatesValid, "All loyalty program start dates should be before end dates.");
        }

        [TestMethod]
        public void TestAdd100PointsToUser()
        {
            // Arrange
            int userId = 1; // Example user ID
            int initialPoints = 0;
            int pointsToAdd = 100;
            int finalPoints = 0;

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Get initial points
                using (var command = new SqlCommand("SELECT LoyaltyPoints FROM Customer WHERE UserID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    initialPoints = (int)command.ExecuteScalar();
                }

                // Add points
                using (var command = new SqlCommand("UPDATE Customer SET LoyaltyPoints = LoyaltyPoints + @PointsToAdd WHERE UserID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@PointsToAdd", pointsToAdd);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.ExecuteNonQuery();
                }

                // Get final points
                using (var command = new SqlCommand("SELECT LoyaltyPoints FROM Customer WHERE UserID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    finalPoints = (int)command.ExecuteScalar();
                }
            }

            // Assert
            Assert.AreEqual(initialPoints + pointsToAdd, finalPoints, "User should have 100 points added to their loyalty points.");
        }
    }
}

