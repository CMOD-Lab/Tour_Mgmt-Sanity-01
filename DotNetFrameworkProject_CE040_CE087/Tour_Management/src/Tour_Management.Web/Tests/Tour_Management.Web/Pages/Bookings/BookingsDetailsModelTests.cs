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
using Tour_Management.Web.Pages.Bookings;

namespace Tour_Management.Web.Tests.Pages.Bookings
{
    public class BookingsDetailsModelTests
    {
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<ILogger<DetailsModel>> _loggerMock;
        private readonly DetailsModel _pageModel;
        private readonly TestSession _session;

        public BookingsDetailsModelTests()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _loggerMock = new Mock<ILogger<DetailsModel>>();
            _pageModel = new DetailsModel(_bookingServiceMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        }

        [Fact]
        public void DetailsModel_Constructor_InitializesBookingAsNull()
        {
            Assert.Null(_pageModel.Booking);
        }

        [Fact]
        public async Task OnGetAsync_WhenNeitherUserNorAdminLoggedIn_RedirectsToLogin()
        {
            // Arrange - no session values

            // Act
            var result = await _pageModel.OnGetAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserLoggedIn_LoadsBooking()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            var booking = new Booking { Id = 1, TourName = "Paris Tour" };
            _bookingServiceMock
                .Setup(s => s.GetBookingByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await _pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.Booking);
            Assert.Equal("Paris Tour", _pageModel.Booking!.TourName);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdminLoggedIn_LoadsBooking()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            var booking = new Booking { Id = 2, TourName = "Rome Tour" };
            _bookingServiceMock
                .Setup(s => s.GetBookingByIdAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await _pageModel.OnGetAsync(2);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.Booking);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToMyBookings()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _bookingServiceMock
                .Setup(s => s.GetBookingByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("MyBookings", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserLoggedIn_CallsGetBookingByIdAsync()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _bookingServiceMock
                .Setup(s => s.GetBookingByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Booking { Id = 5 });

            // Act
            await _pageModel.OnGetAsync(5);

            // Assert
            _bookingServiceMock.Verify(s => s.GetBookingByIdAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
