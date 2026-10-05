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
    public class UsersRegisterModelTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<RegisterModel>> _loggerMock;
        private readonly RegisterModel _pageModel;
        private readonly TestSession _session;

        public UsersRegisterModelTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<RegisterModel>>();
            _pageModel = new RegisterModel(_userServiceMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void RegisterModel_Constructor_InitializesInputAsNew()
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
        public async Task OnPostAsync_WhenValidAndSuccess_RedirectsToLogin()
        {
            // Arrange
            _pageModel.Input = new RegisterViewModel
            {
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User",
                Gender = "Male",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };
            _userServiceMock
                .Setup(s => s.RegisterUserAsync(It.IsAny<UserInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserInfo { Id = 1, Email = "newuser@example.com" });

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidationException_ReturnsPage()
        {
            // Arrange
            _pageModel.Input = new RegisterViewModel
            {
                Email = "existing@example.com",
                FirstName = "Existing",
                LastName = "User",
                Gender = "Male",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };
            _userServiceMock
                .Setup(s => s.RegisterUserAsync(It.IsAny<UserInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Tour_Management.Domain.Exceptions.ValidationException("Email already exists"));

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
        }

        [Fact]
        public async Task OnPostAsync_WhenGeneralException_ReturnsPage()
        {
            // Arrange
            _pageModel.Input = new RegisterViewModel
            {
                Email = "user@example.com",
                FirstName = "User",
                LastName = "Test",
                Gender = "Male",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };
            _userServiceMock
                .Setup(s => s.RegisterUserAsync(It.IsAny<UserInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("General error"));

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValid_MapsViewModelToUserInfo()
        {
            // Arrange
            UserInfo? capturedUser = null;
            string? capturedPassword = null;
            var dob = new DateTime(1990, 5, 15);
            _pageModel.Input = new RegisterViewModel
            {
                Email = "map@example.com",
                FirstName = "Map",
                LastName = "Test",
                Gender = "Female",
                Password = "Password123",
                ConfirmPassword = "Password123",
                DateOfBirth = dob,
                Street = "123 St",
                City = "City",
                State = "State"
            };
            _userServiceMock
                .Setup(s => s.RegisterUserAsync(It.IsAny<UserInfo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<UserInfo, string, CancellationToken>((u, p, _) => { capturedUser = u; capturedPassword = p; })
                .ReturnsAsync(new UserInfo { Id = 1 });

            // Act
            await _pageModel.OnPostAsync();

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal("map@example.com", capturedUser!.Email);
            Assert.Equal("Map", capturedUser.FirstName);
            Assert.Equal("Test", capturedUser.LastName);
            Assert.Equal("Female", capturedUser.Gender);
            Assert.Equal(dob, capturedUser.DateOfBirth);
            Assert.Equal("123 St", capturedUser.Street);
            Assert.Equal("City", capturedUser.City);
            Assert.Equal("State", capturedUser.State);
            Assert.Equal("Password123", capturedPassword);
        }
    }
}
