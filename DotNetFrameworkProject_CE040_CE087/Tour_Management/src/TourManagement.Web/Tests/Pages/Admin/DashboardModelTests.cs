using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.Pages.Admin;
using TourManagement.Web.ViewModels;
using TourManagement.Web.Tests.Pages.Tours;

namespace TourManagement.Web.Tests.Pages.Admin
{
    public class DashboardModelTests
    {
        private readonly Mock<ITourService> _mockTourService;
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<DashboardModel>> _mockLogger;
        private readonly DashboardModel _pageModel;

        public DashboardModelTests()
        {
            _mockTourService = new Mock<ITourService>();
            _mockBookingService = new Mock<IBookingService>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<DashboardModel>>();

            _pageModel = new DashboardModel(
                _mockTourService.Object,
                _mockBookingService.Object,
                _mockUserService.Object,
                _mockLogger.Object);

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
        public async Task OnGetAsync_WhenAdmin_LoadsDashboardData()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");

            var tours = new List<TourDto>
            {
                new TourDto { Id = 1, TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel Tower", TourInfo = "Amazing tour", IsActive = true, CreatedDate = DateTime.UtcNow },
                new TourDto { Id = 2, TourName = "Rome Tour", Place = "Rome", Days = 5, Price = 1200m, Locations = "Colosseum", TourInfo = "Great tour", IsActive = true, CreatedDate = DateTime.UtcNow }
            };
            var bookings = new List<BookingDto>
            {
                new BookingDto { Id = 1, TourName = "Paris Tour", Place = "Paris", Email = "user@test.com", FirstName = "John", IsActive = true, CreatedDate = DateTime.UtcNow },
                new BookingDto { Id = 2, TourName = "Rome Tour", Place = "Rome", Email = "user2@test.com", FirstName = "Jane", IsActive = true, CreatedDate = DateTime.UtcNow },
                new BookingDto { Id = 3, TourName = "Paris Tour", Place = "Paris", Email = "user3@test.com", FirstName = "Bob", IsActive = true, CreatedDate = DateTime.UtcNow }
            };
            var users = new List<UserDto>
            {
                new UserDto { Id = 1, Email = "user@test.com", FirstName = "John", LastName = "Doe", Gender = "Male", DateOfBirth = new DateTime(1990, 1, 1), IsAdmin = false, CreatedDate = DateTime.UtcNow }
            };

            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);
            _mockBookingService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
            _mockUserService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(2, _pageModel.TotalTours);
            Assert.Equal(3, _pageModel.TotalBookings);
            Assert.Equal(1, _pageModel.TotalUsers);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdmin_LoadsRecentBookings()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");

            var bookings = new List<BookingDto>();
            for (int i = 1; i <= 8; i++)
            {
                bookings.Add(new BookingDto
                {
                    Id = i,
                    TourName = $"Tour {i}",
                    Place = $"Place {i}",
                    Email = $"user{i}@test.com",
                    FirstName = $"User{i}",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow.AddDays(-i)
                });
            }

            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TourDto>());
            _mockBookingService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
            _mockUserService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserDto>());

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(5, ((List<BookingViewModel>)_pageModel.RecentBookings).Count);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void DashboardModel_Constructor_InitializesProperties()
        {
            // Assert
            Assert.Equal(0, _pageModel.TotalTours);
            Assert.Equal(0, _pageModel.TotalBookings);
            Assert.Equal(0, _pageModel.TotalUsers);
            Assert.NotNull(_pageModel.RecentBookings);
            Assert.Empty(_pageModel.RecentBookings);
        }

        [Fact]
        public async Task OnGetAsync_WithEmptyData_ReturnsZeroCounts()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TourDto>());
            _mockBookingService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<BookingDto>());
            _mockUserService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserDto>());

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(0, _pageModel.TotalTours);
            Assert.Equal(0, _pageModel.TotalBookings);
            Assert.Equal(0, _pageModel.TotalUsers);
            Assert.Empty(_pageModel.RecentBookings);
        }

        [Fact]
        public async Task OnGetAsync_RecentBookings_OrderedByDateDescending()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            var now = DateTime.UtcNow;
            var bookings = new List<BookingDto>
            {
                new BookingDto { Id = 1, TourName = "Old Tour", Place = "Place", Email = "u1@t.com", FirstName = "U1", IsActive = true, CreatedDate = now.AddDays(-5) },
                new BookingDto { Id = 2, TourName = "New Tour", Place = "Place", Email = "u2@t.com", FirstName = "U2", IsActive = true, CreatedDate = now.AddDays(-1) },
                new BookingDto { Id = 3, TourName = "Mid Tour", Place = "Place", Email = "u3@t.com", FirstName = "U3", IsActive = true, CreatedDate = now.AddDays(-3) }
            };

            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TourDto>());
            _mockBookingService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
            _mockUserService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserDto>());

            // Act
            await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            var recentList = (List<BookingViewModel>)_pageModel.RecentBookings;
            Assert.Equal("New Tour", recentList[0].TourName);
            Assert.Equal("Mid Tour", recentList[1].TourName);
            Assert.Equal("Old Tour", recentList[2].TourName);
        }
    }
}
