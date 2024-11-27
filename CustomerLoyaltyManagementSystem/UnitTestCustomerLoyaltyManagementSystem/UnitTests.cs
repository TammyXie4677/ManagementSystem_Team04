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
                VerificationCode = "",
                IsVerified = true
            };

            // Assert
            Assert.AreEqual(1, user.UserID);
            Assert.AreEqual("test@example.com", user.Email);
            Assert.AreEqual("hashedpassword", user.PasswordHashed);
            Assert.IsNotNull(user.DateJoined);
            Assert.AreEqual("Admin", user.Role);
            Assert.AreEqual("", user.VerificationCode);
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
    }
}



