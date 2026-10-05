using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.Pages.Users;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Tests.Pages.Users
{
    public class UsersLoginModelTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<LoginModel>> _loggerMock;
        private readonly LoginModel _pageModel;
        private readonly TestSession _session;

        public UsersLoginModelTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<LoginModel>>();
            _pageModel = new LoginModel(_userServiceMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void LoginModel_Constructor_InitializesInputAsNew()
        {
            Assert.NotNull(_pageModel.Input);
        }

        [Fact]
        public void OnGet_WhenUserAlreadyLoggedIn_RedirectsToProfile()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Profile", redirect.PageName);
        }

        [Fact]
        public void OnGet_WhenUserNotLoggedIn_ReturnsPage()
        {
            // Arrange - no UserEmail in session

            // Act
            var result = _pageModel.OnGet();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
        {
            // Arrange
            _pageModel.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidCredentials_SetsSessionAndRedirects()
        {
            // Arrange
            _pageModel.Input = new LoginViewModel
            {
                Email = "user@example.com",
                Password = "password"
            };
            var user = new UserInfo
            {
                Id = 1,
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe"
            };
            _userServiceMock
                .Setup(s => s.AuthenticateAsync("user@example.com", "password", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Profile", redirect.PageName);
            Assert.Equal("user@example.com", _session.GetString("UserEmail"));
            Assert.Equal("John Doe", _session.GetString("UserName"));
            Assert.Equal(1, _session.GetInt32("UserId"));
        }

        [Fact]
        public async Task OnPostAsync_WhenInvalidCredentials_ReturnsPage()
        {
            // Arrange
            _pageModel.Input = new LoginViewModel
            {
                Email = "user@example.com",
                Password = "wrongpassword"
            };
            _userServiceMock
                .Setup(s => s.AuthenticateAsync("user@example.com", "wrongpassword", It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserInfo?)null);

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPage()
        {
            // Arrange
            _pageModel.Input = new LoginViewModel
            {
                Email = "user@example.com",
                Password = "password"
            };
            _userServiceMock
                .Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidCredentials_CallsAuthenticateAsync()
        {
            // Arrange
            _pageModel.Input = new LoginViewModel
            {
                Email = "user@example.com",
                Password = "password"
            };
            _userServiceMock
                .Setup(s => s.AuthenticateAsync("user@example.com", "password", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserInfo { Id = 1, Email = "user@example.com", FirstName = "John", LastName = "Doe" });

            // Act
            await _pageModel.OnPostAsync();

            // Assert
            _userServiceMock.Verify(s => s.AuthenticateAsync("user@example.com", "password", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
