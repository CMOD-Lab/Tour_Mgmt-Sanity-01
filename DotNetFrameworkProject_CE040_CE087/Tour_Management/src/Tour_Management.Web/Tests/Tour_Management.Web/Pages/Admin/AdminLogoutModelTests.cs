using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Web.Pages.Admin;

namespace Tour_Management.Web.Tests.Pages.Admin
{
    public class AdminLogoutModelTests
    {
        private readonly Mock<ILogger<LogoutModel>> _loggerMock;
        private readonly LogoutModel _pageModel;
        private readonly TestSession _session;

        public AdminLogoutModelTests()
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
        public void OnGet_WhenAdminLoggedIn_ClearsSessionAndRedirects()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
            Assert.Null(_session.GetString("AdminEmail"));
        }

        [Fact]
        public void OnGet_WhenAdminNotLoggedIn_StillRedirects()
        {
            // Arrange - no session values

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
        }

        [Fact]
        public void OnGet_RemovesAdminEmailFromSession()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");

            // Act
            _pageModel.OnGet();

            // Assert
            Assert.Null(_session.GetString("AdminEmail"));
        }

        [Fact]
        public void OnGet_SetsTempDataSuccessMessage()
        {
            // Act
            _pageModel.OnGet();

            // Assert
            Assert.True(_pageModel.TempData.ContainsKey("SuccessMessage"));
        }

        [Fact]
        public void OnGet_ReturnsRedirectToPageResult()
        {
            // Act
            var result = _pageModel.OnGet();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }
    }
}
