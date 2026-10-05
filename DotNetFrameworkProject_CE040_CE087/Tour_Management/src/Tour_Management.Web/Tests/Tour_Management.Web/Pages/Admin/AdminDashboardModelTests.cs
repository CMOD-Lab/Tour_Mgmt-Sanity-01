using System;
using System.Collections.Generic;
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
using Tour_Management.Web.Pages.Admin;

namespace Tour_Management.Web.Tests.Pages.Admin
{
    public class AdminDashboardModelTests
    {
        private readonly Mock<ITourService> _tourServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<DashboardModel>> _loggerMock;
        private readonly DashboardModel _pageModel;
        private readonly TestSession _session;

        public AdminDashboardModelTests()
        {
            _tourServiceMock = new Mock<ITourService>();
            _bookingServiceMock = new Mock<IBookingService>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<DashboardModel>>();
            _pageModel = new DashboardModel(
                _tourServiceMock.Object,
                _bookingServiceMock.Object,
                _userServiceMock.Object,
                _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        }

        [Fact]
        public void DashboardModel_Constructor_InitializesCountsToZero()
        {
            Assert.Equal(0, _pageModel.TotalTours);
            Assert.Equal(0, _pageModel.TotalBookings);
            Assert.Equal(0, _pageModel.TotalUsers);
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
        public async Task OnGetAsync_WhenAdmin_LoadsCountsAndReturnsPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Tour> { new Tour(), new Tour(), new Tour() });
            _bookingServiceMock
                .Setup(s => s.GetAllBookingsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Booking> { new Booking(), new Booking() });
            _userServiceMock
                .Setup(s => s.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserInfo> { new UserInfo() });

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(3, _pageModel.TotalTours);
            Assert.Equal(2, _pageModel.TotalBookings);
            Assert.Equal(1, _pageModel.TotalUsers);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithZeroCounts()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(0, _pageModel.TotalTours);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdmin_CallsAllThreeServices()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Tour>());
            _bookingServiceMock
                .Setup(s => s.GetAllBookingsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Booking>());
            _userServiceMock
                .Setup(s => s.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserInfo>());

            // Act
            await _pageModel.OnGetAsync();

            // Assert
            _tourServiceMock.Verify(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()), Times.Once);
            _bookingServiceMock.Verify(s => s.GetAllBookingsAsync(It.IsAny<CancellationToken>()), Times.Once);
            _userServiceMock.Verify(s => s.GetAllUsersAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdmin_WithEmptyData_ReturnsZeroCounts()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Tour>());
            _bookingServiceMock
                .Setup(s => s.GetAllBookingsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Booking>());
            _userServiceMock
                .Setup(s => s.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserInfo>());

            // Act
            await _pageModel.OnGetAsync();

            // Assert
            Assert.Equal(0, _pageModel.TotalTours);
            Assert.Equal(0, _pageModel.TotalBookings);
            Assert.Equal(0, _pageModel.TotalUsers);
        }
    }
}
