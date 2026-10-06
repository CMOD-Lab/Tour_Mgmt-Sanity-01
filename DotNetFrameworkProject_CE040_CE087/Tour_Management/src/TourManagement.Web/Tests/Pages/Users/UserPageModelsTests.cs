using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.Pages.Users;
using TourManagement.Web.ViewModels;
using TourManagement.Web.Tests.Pages.Tours;

namespace TourManagement.Web.Tests.Pages.Users
{
    public class LoginModelTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<LoginModel>> _mockLogger;
        private readonly LoginModel _pageModel;

        public LoginModelTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<LoginModel>>();

            // Setup configuration
            _mockConfiguration.Setup(c => c["AppSettings:AdminEmail"]).Returns("admin@gmail.com");
            _mockConfiguration.Setup(c => c["AppSettings:AdminPassword"]).Returns("admin");

            _pageModel = new LoginModel(_mockUserService.Object, _mockConfiguration.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void OnGet_WhenAlreadyLoggedIn_RedirectsToIndex()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
        }

        [Fact]
        public void OnGet_WhenNotLoggedIn_ReturnsPage()
        {
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
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WithAdminCredentials_RedirectsToDashboard()
        {
            // Arrange
            _pageModel.Input = new LoginViewModel
            {
                Email = "admin@gmail.com",
                Password = "admin"
            };

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Admin/Dashboard", redirect.PageName);
            Assert.Equal("true", _pageModel.HttpContext.Session.GetString("IsAdmin"));
        }

        [Fact]
        public async Task OnPostAsync_WithValidUserCredentials_RedirectsToIndex()
        {
            // Arrange
            _pageModel.Input = new LoginViewModel
            {
                Email = "user@test.com",
                Password = "password123"
            };
            var userDto = new UserDto
            {
                Id = 1,
                Email = "user@test.com",
                FirstName = "John",
                LastName = "Doe",
                IsAdmin = false
            };
            _mockUserService.Setup(s => s.AuthenticateAsync("user@test.com", "password123", It.IsAny<CancellationToken>())).ReturnsAsync(userDto);

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
            Assert.Equal("user@test.com", _pageModel.HttpContext.Session.GetString("UserEmail"));
        }

        [Fact]
        public async Task OnPostAsync_WithInvalidCredentials_ReturnsPageWithError()
        {
            // Arrange
            _pageModel.Input = new LoginViewModel
            {
                Email = "user@test.com",
                Password = "wrongpassword"
            };
            _mockUserService.Setup(s => s.AuthenticateAsync("user@test.com", "wrongpassword", It.IsAny<CancellationToken>())).Returns(Task.FromResult<UserDto?>(null));

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Invalid email or password.", _pageModel.ErrorMessage);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
        {
            // Arrange
            _pageModel.Input = new LoginViewModel
            {
                Email = "user@test.com",
                Password = "password123"
            };
            _mockUserService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotEmpty(_pageModel.ErrorMessage);
        }

        [Fact]
        public async Task OnPostAsync_WithAdminUser_SetsAdminSessionToTrue()
        {
            // Arrange
            _pageModel.Input = new LoginViewModel
            {
                Email = "user@test.com",
                Password = "password123"
            };
            var userDto = new UserDto
            {
                Id = 1,
                Email = "user@test.com",
                FirstName = "John",
                LastName = "Doe",
                IsAdmin = true
            };
            _mockUserService.Setup(s => s.AuthenticateAsync("user@test.com", "password123", It.IsAny<CancellationToken>())).ReturnsAsync(userDto);

            // Act
            await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.Equal("true", _pageModel.HttpContext.Session.GetString("IsAdmin"));
        }

        [Fact]
        public void LoginModel_Constructor_InitializesProperties()
        {
            // Assert
            Assert.NotNull(_pageModel.Input);
            Assert.Equal(string.Empty, _pageModel.ErrorMessage);
        }
    }

    public class RegisterModelTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<RegisterModel>> _mockLogger;
        private readonly RegisterModel _pageModel;

        public RegisterModelTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<RegisterModel>>();
            _pageModel = new RegisterModel(_mockUserService.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void OnGet_WhenAlreadyLoggedIn_RedirectsToIndex()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
        }

        [Fact]
        public void OnGet_WhenNotLoggedIn_ReturnsPage()
        {
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
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WithValidModel_CreatesUserAndRedirectsToLogin()
        {
            // Arrange
            _pageModel.Input = new RegisterViewModel
            {
                Email = "newuser@test.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990, 1, 1),
                Street = "123 Main St",
                City = "New York",
                State = "NY"
            };
            _mockUserService.Setup(s => s.CreateAsync(It.IsAny<UserCreateDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new UserDto());

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
            _mockUserService.Verify(s => s.CreateAsync(It.IsAny<UserCreateDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_WhenDuplicateEmail_ReturnsPageWithEmailError()
        {
            // Arrange
            _pageModel.Input = new RegisterViewModel
            {
                Email = "existing@test.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990, 1, 1)
            };
            _mockUserService.Setup(s => s.CreateAsync(It.IsAny<UserCreateDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new DuplicateEntityException("User", "Email", "existing@test.com"));

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
        {
            // Arrange
            _pageModel.Input = new RegisterViewModel
            {
                Email = "user@test.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "password123",
                ConfirmPassword = "password123",
                DateOfBirth = new DateTime(1990, 1, 1)
            };
            _mockUserService.Setup(s => s.CreateAsync(It.IsAny<UserCreateDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
        }
    }

    public class ProfileModelTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<ProfileModel>> _mockLogger;
        private readonly ProfileModel _pageModel;

        public ProfileModelTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<ProfileModel>>();
            _pageModel = new ProfileModel(_mockUserService.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
        {
            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenLoggedInWithValidUserId_LoadsProfile()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.HttpContext.Session.SetString("UserId", "1");
            var userDto = new UserDto
            {
                Id = 1,
                Email = "user@test.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1),
                Street = "123 Main St",
                City = "New York",
                State = "NY",
                IsAdmin = false
            };
            _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userDto);

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.Profile);
            Assert.Equal("John", _pageModel.Profile!.FirstName);
            Assert.Equal("Doe", _pageModel.Profile.LastName);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdminWithoutUserId_LoadsAdminProfile()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "admin@gmail.com");
            // No UserId in session (admin)

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.Profile);
            Assert.Equal("Administrator", _pageModel.Profile!.FirstName);
            Assert.True(_pageModel.Profile.IsAdmin);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToIndex()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.HttpContext.Session.SetString("UserId", "1");
            _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
        }
    }

    public class UsersEditModelTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<EditModel>> _mockLogger;
        private readonly EditModel _pageModel;

        public UsersEditModelTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<EditModel>>();
            _pageModel = new EditModel(_mockUserService.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
        {
            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult<UserDto?>(null));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserFound_PopulatesInputAndReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            var userDto = new UserDto
            {
                Id = 1,
                Email = "user@test.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1),
                Street = "123 Main St",
                City = "New York",
                State = "NY"
            };
            _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userDto);

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("John", _pageModel.Input.FirstName);
            Assert.Equal("Doe", _pageModel.Input.LastName);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToProfile()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Profile", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenNotLoggedIn_RedirectsToLogin()
        {
            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.ModelState.AddModelError("FirstName", "Required");

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidModel_UpdatesUserAndRedirects()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.Input = new UserEditViewModel
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1),
                Street = "123 Main St",
                City = "New York",
                State = "NY"
            };
            _mockUserService.Setup(s => s.UpdateAsync(1, It.IsAny<UserUpdateDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new UserDto());

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Profile", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.Input = new UserEditViewModel
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1)
            };
            _mockUserService.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UserUpdateDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }
    }

    public class UsersDeleteModelTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<DeleteModel>> _mockLogger;
        private readonly DeleteModel _pageModel;

        public UsersDeleteModelTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<DeleteModel>>();
            _pageModel = new DeleteModel(_mockUserService.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task OnGetAsync_WhenNotAdmin_RedirectsToLogin()
        {
            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult<UserDto?>(null));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserFound_PopulatesUserToDeleteAndReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            var userDto = new UserDto
            {
                Id = 1,
                Email = "user@test.com",
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                City = "New York",
                State = "NY"
            };
            _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userDto);

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.UserToDelete);
            Assert.Equal("John", _pageModel.UserToDelete!.FirstName);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToIndex()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenNotAdmin_RedirectsToLogin()
        {
            // Act
            var result = await _pageModel.OnPostAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenAdmin_DeletesUserAndRedirects()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockUserService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _pageModel.OnPostAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
            _mockUserService.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_RedirectsToIndexWithError()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockUserService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }
    }

    public class UsersIndexModelTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<IndexModel>> _mockLogger;
        private readonly IndexModel _pageModel;

        public UsersIndexModelTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<IndexModel>>();
            _pageModel = new IndexModel(_mockUserService.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task OnGetAsync_WhenNotAdmin_RedirectsToLogin()
        {
            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdmin_LoadsAllUsers()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            var users = new List<UserDto>
            {
                new UserDto { Id = 1, Email = "user1@test.com", FirstName = "John", LastName = "Doe", Gender = "Male", DateOfBirth = new DateTime(1990, 1, 1), IsAdmin = false, CreatedDate = DateTime.UtcNow },
                new UserDto { Id = 2, Email = "user2@test.com", FirstName = "Jane", LastName = "Smith", Gender = "Female", DateOfBirth = new DateTime(1992, 5, 15), IsAdmin = false, CreatedDate = DateTime.UtcNow }
            };
            _mockUserService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(2, ((List<UserProfileViewModel>)_pageModel.Users).Count);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithEmptyList()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockUserService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Empty(_pageModel.Users);
        }
    }

    public class LogoutModelTests
    {
        private readonly Mock<ILogger<LogoutModel>> _mockLogger;
        private readonly LogoutModel _pageModel;

        public LogoutModelTests()
        {
            _mockLogger = new Mock<ILogger<LogoutModel>>();
            _pageModel = new LogoutModel(_mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void OnGet_ClearsSessionAndReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");

            // Act
            var result = _pageModel.OnGet();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Null(_pageModel.HttpContext.Session.GetString("UserEmail"));
            Assert.Null(_pageModel.HttpContext.Session.GetString("IsAdmin"));
        }

        [Fact]
        public void OnGet_WhenNoSession_ReturnsPage()
        {
            // Act
            var result = _pageModel.OnGet();

            // Assert
            Assert.IsType<PageResult>(result);
        }
    }
}
