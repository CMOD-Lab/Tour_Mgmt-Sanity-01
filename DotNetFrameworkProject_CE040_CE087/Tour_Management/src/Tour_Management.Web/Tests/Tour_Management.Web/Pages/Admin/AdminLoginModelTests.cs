using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Web.Pages.Admin;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Tests.Pages.Admin
{
    public class AdminLoginModelTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILogger<LoginModel>> _loggerMock;
        private readonly LoginModel _pageModel;
        private readonly TestSession _session;

        public AdminLoginModelTests()
        {
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<LoginModel>>();
            _pageModel = new LoginModel(_configMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void AdminLoginModel_Constructor_InitializesInputAsNew()
        {
            Assert.NotNull(_pageModel.Input);
        }

        [Fact]
        public void OnGet_WhenAdminAlreadyLoggedIn_RedirectsToDashboard()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Dashboard", redirect.PageName);
        }

        [Fact]
        public void OnGet_WhenAdminNotLoggedIn_ReturnsPage()
        {
            // Arrange - no AdminEmail in session

            // Act
            var result = _pageModel.OnGet();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void OnPost_WhenModelStateInvalid_ReturnsPage()
        {
            // Arrange
            _pageModel.ModelState.AddModelError("Email", "Required");

            // Act
            var result = _pageModel.OnPost();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void OnPost_WhenValidCredentials_SetsSessionAndRedirects()
        {
            // Arrange
            _configMock.Setup(c => c["AppSettings:AdminEmail"]).Returns("admin@gmail.com");
            _configMock.Setup(c => c["AppSettings:AdminPassword"]).Returns("admin");
            _pageModel.Input = new AdminLoginViewModel
            {
                Email = "admin@gmail.com",
                Password = "admin"
            };

            // Act
            var result = _pageModel.OnPost();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Dashboard", redirect.PageName);
            Assert.Equal("admin@gmail.com", _session.GetString("AdminEmail"));
        }

        [Fact]
        public void OnPost_WhenInvalidCredentials_ReturnsPage()
        {
            // Arrange
            _configMock.Setup(c => c["AppSettings:AdminEmail"]).Returns("admin@gmail.com");
            _configMock.Setup(c => c["AppSettings:AdminPassword"]).Returns("admin");
            _pageModel.Input = new AdminLoginViewModel
            {
                Email = "wrong@gmail.com",
                Password = "wrongpassword"
            };

            // Act
            var result = _pageModel.OnPost();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
        }

        [Fact]
        public void OnPost_WhenConfigurationIsNull_UsesDefaultCredentials()
        {
            // Arrange - configuration returns null, so defaults are used
            _configMock.Setup(c => c["AppSettings:AdminEmail"]).Returns((string?)null);
            _configMock.Setup(c => c["AppSettings:AdminPassword"]).Returns((string?)null);
            _pageModel.Input = new AdminLoginViewModel
            {
                Email = "admin@gmail.com",
                Password = "admin"
            };

            // Act
            var result = _pageModel.OnPost();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Dashboard", redirect.PageName);
        }

        [Fact]
        public void OnPost_WhenWrongEmail_ReturnsPage()
        {
            // Arrange
            _configMock.Setup(c => c["AppSettings:AdminEmail"]).Returns("admin@gmail.com");
            _configMock.Setup(c => c["AppSettings:AdminPassword"]).Returns("admin");
            _pageModel.Input = new AdminLoginViewModel
            {
                Email = "other@gmail.com",
                Password = "admin"
            };

            // Act
            var result = _pageModel.OnPost();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void OnPost_WhenWrongPassword_ReturnsPage()
        {
            // Arrange
            _configMock.Setup(c => c["AppSettings:AdminEmail"]).Returns("admin@gmail.com");
            _configMock.Setup(c => c["AppSettings:AdminPassword"]).Returns("admin");
            _pageModel.Input = new AdminLoginViewModel
            {
                Email = "admin@gmail.com",
                Password = "wrongpassword"
            };

            // Act
            var result = _pageModel.OnPost();

            // Assert
            Assert.IsType<PageResult>(result);
        }
    }
}
