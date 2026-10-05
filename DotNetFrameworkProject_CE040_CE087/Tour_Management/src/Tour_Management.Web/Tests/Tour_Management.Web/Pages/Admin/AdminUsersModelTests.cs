using System;
using System.Collections.Generic;
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
using Tour_Management.Web.Pages.Admin;

namespace Tour_Management.Web.Tests.Pages.Admin
{
    public class AdminUsersModelTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<UsersModel>> _loggerMock;
        private readonly UsersModel _pageModel;
        private readonly TestSession _session;

        public AdminUsersModelTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UsersModel>>();
            _pageModel = new UsersModel(_userServiceMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void UsersModel_Constructor_InitializesUsersAsEmptyList()
        {
            Assert.NotNull(_pageModel.Users);
            Assert.Empty(_pageModel.Users);
        }

        [Fact]
        public async Task OnGetAsync_WhenNotAdmin_RedirectsToLogin()
        {
            // Arrange - no AdminEmail in session

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdmin_LoadsUsersAndReturnsPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            var users = new List<UserInfo>
            {
                new UserInfo { Id = 1, Email = "user1@example.com" },
                new UserInfo { Id = 2, Email = "user2@example.com" }
            };
            _userServiceMock
                .Setup(s => s.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(2, new List<UserInfo>(_pageModel.Users).Count);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _userServiceMock
                .Setup(s => s.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostDeleteAsync_WhenNotAdmin_RedirectsToLogin()
        {
            // Arrange - no AdminEmail in session

            // Act
            var result = await _pageModel.OnPostDeleteAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostDeleteAsync_WhenAdminAndSuccess_RedirectsToPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _userServiceMock
                .Setup(s => s.DeleteUserAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _pageModel.OnPostDeleteAsync(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostDeleteAsync_WhenAdminAndSuccess_CallsDeleteUserAsync()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _userServiceMock
                .Setup(s => s.DeleteUserAsync(5, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _pageModel.OnPostDeleteAsync(5);

            // Assert
            _userServiceMock.Verify(s => s.DeleteUserAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostDeleteAsync_WhenServiceThrowsException_RedirectsToPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _userServiceMock
                .Setup(s => s.DeleteUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Delete error"));

            // Act
            var result = await _pageModel.OnPostDeleteAsync(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostDeleteAsync_WhenAdminAndSuccess_SetsTempDataSuccessMessage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _userServiceMock
                .Setup(s => s.DeleteUserAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _pageModel.OnPostDeleteAsync(1);

            // Assert
            Assert.True(_pageModel.TempData.ContainsKey("SuccessMessage"));
        }
    }
}
