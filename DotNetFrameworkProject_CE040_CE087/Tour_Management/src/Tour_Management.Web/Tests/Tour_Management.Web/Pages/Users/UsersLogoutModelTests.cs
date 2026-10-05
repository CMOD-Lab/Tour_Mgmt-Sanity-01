using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Web.Pages.Users;

namespace Tour_Management.Web.Tests.Pages.Users
{
    public class UsersLogoutModelTests
    {
        private readonly Mock<ILogger<LogoutModel>> _loggerMock;
        private readonly LogoutModel _pageModel;
        private readonly TestSession _session;

        public UsersLogoutModelTests()
        {
            _loggerMock = new Mock<ILogger<LogoutModel>>();
            _pageModel = new LogoutModel(_loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void OnGet_WhenUserLoggedIn_ClearsSessionAndRedirects()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _session.SetString("UserName", "John Doe");
            _session.SetInt32("UserId", 1);

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
            Assert.Null(_session.GetString("UserEmail"));
            Assert.Null(_session.GetString("UserName"));
            Assert.Null(_session.GetInt32("UserId"));
        }

        [Fact]
        public void OnGet_WhenUserNotLoggedIn_StillRedirects()
        {
            // Arrange - no session values

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
        }

        [Fact]
        public void OnGet_RemovesUserEmailFromSession()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");

            // Act
            _pageModel.OnGet();

            // Assert
            Assert.Null(_session.GetString("UserEmail"));
        }

        [Fact]
        public void OnGet_RemovesUserNameFromSession()
        {
            // Arrange
            _session.SetString("UserName", "John Doe");

            // Act
            _pageModel.OnGet();

            // Assert
            Assert.Null(_session.GetString("UserName"));
        }

        [Fact]
        public void OnGet_RemovesUserIdFromSession()
        {
            // Arrange
            _session.SetInt32("UserId", 42);

            // Act
            _pageModel.OnGet();

            // Assert
            Assert.Null(_session.GetInt32("UserId"));
        }

        [Fact]
        public void OnGet_SetsTempDataSuccessMessage()
        {
            // Act
            _pageModel.OnGet();

            // Assert
            Assert.True(_pageModel.TempData.ContainsKey("SuccessMessage"));
        }
    }
}
