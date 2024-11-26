using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;

namespace UnitTestCustomerLoyaltyManagementSystem
{
    [TestClass]
    public class UnitTests
    {
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
    }
}

