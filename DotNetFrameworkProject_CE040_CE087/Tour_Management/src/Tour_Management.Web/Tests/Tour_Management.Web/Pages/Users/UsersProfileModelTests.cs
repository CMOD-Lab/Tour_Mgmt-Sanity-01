using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.Pages.Users;

namespace Tour_Management.Web.Tests.Pages.Users
{
    public class UsersProfileModelTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<ProfileModel>> _loggerMock;
        private readonly ProfileModel _pageModel;
        private readonly TestSession _session;

        public UsersProfileModelTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<ProfileModel>>();
            _pageModel = new ProfileModel(_userServiceMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        }

        [Fact]
        public void ProfileModel_Constructor_InitializesCurrentUserAsNull()
        {
            Assert.Null(_pageModel.CurrentUser);
        }

        [Fact]
        public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
        {
            // Arrange - no UserEmail in session

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenLoggedIn_LoadsCurrentUser()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            var user = new UserInfo { Id = 1, Email = "user@example.com", FirstName = "John" };
            _userServiceMock
                .Setup(s => s.GetUserByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.CurrentUser);
            Assert.Equal("user@example.com", _pageModel.CurrentUser!.Email);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _userServiceMock
                .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenLoggedIn_CallsGetUserByEmailAsync()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _userServiceMock
                .Setup(s => s.GetUserByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserInfo { Id = 1, Email = "user@example.com" });

            // Act
            await _pageModel.OnGetAsync();

            // Assert
            _userServiceMock.Verify(s => s.GetUserByEmailAsync("user@example.com", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserNotFound_ReturnsPageWithNullCurrentUser()
        {
            // Arrange
            _session.SetString("UserEmail", "notfound@example.com");
            _userServiceMock
                .Setup(s => s.GetUserByEmailAsync("notfound@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserInfo?)null);

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Null(_pageModel.CurrentUser);
        }
    }
}
