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
using Tour_Management.Web.Pages.Bookings;

namespace Tour_Management.Web.Tests.Pages.Bookings
{
    public class BookingsMyBookingsModelTests
    {
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<ILogger<MyBookingsModel>> _loggerMock;
        private readonly MyBookingsModel _pageModel;
        private readonly TestSession _session;

        public BookingsMyBookingsModelTests()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _loggerMock = new Mock<ILogger<MyBookingsModel>>();
            _pageModel = new MyBookingsModel(_bookingServiceMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void MyBookingsModel_Constructor_InitializesBookingsAsEmptyList()
        {
            Assert.NotNull(_pageModel.Bookings);
            Assert.Empty(_pageModel.Bookings);
        }

        [Fact]
        public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
        {
            // Arrange - no UserEmail in session

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenLoggedIn_LoadsUserBookings()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, Email = "user@example.com", TourName = "Tour A" },
                new Booking { Id = 2, Email = "user@example.com", TourName = "Tour B" }
            };
            _bookingServiceMock
                .Setup(s => s.GetBookingsByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(2, new List<Booking>(_pageModel.Bookings).Count);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _bookingServiceMock
                .Setup(s => s.GetBookingsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenLoggedIn_CallsGetBookingsByEmailAsync()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _bookingServiceMock
                .Setup(s => s.GetBookingsByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Booking>());

            // Act
            await _pageModel.OnGetAsync();

            // Assert
            _bookingServiceMock.Verify(s => s.GetBookingsByEmailAsync("user@example.com", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostCancelAsync_WhenNotLoggedIn_RedirectsToLogin()
        {
            // Arrange - no UserEmail in session

            // Act
            var result = await _pageModel.OnPostCancelAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostCancelAsync_WhenLoggedInAndSuccess_RedirectsToPage()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _bookingServiceMock
                .Setup(s => s.DeleteBookingAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _pageModel.OnPostCancelAsync(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostCancelAsync_WhenLoggedInAndSuccess_CallsDeleteBookingAsync()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _bookingServiceMock
                .Setup(s => s.DeleteBookingAsync(7, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _pageModel.OnPostCancelAsync(7);

            // Assert
            _bookingServiceMock.Verify(s => s.DeleteBookingAsync(7, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostCancelAsync_WhenServiceThrowsException_RedirectsToPage()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _bookingServiceMock
                .Setup(s => s.DeleteBookingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Cancel error"));

            // Act
            var result = await _pageModel.OnPostCancelAsync(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostCancelAsync_WhenLoggedInAndSuccess_SetsTempDataSuccessMessage()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _bookingServiceMock
                .Setup(s => s.DeleteBookingAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _pageModel.OnPostCancelAsync(1);

            // Assert
            Assert.True(_pageModel.TempData.ContainsKey("SuccessMessage"));
        }

        [Fact]
        public async Task OnPostCancelAsync_WhenServiceThrowsException_SetsTempDataErrorMessage()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _bookingServiceMock
                .Setup(s => s.DeleteBookingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Cancel error"));

            // Act
            await _pageModel.OnPostCancelAsync(1);

            // Assert
            Assert.True(_pageModel.TempData.ContainsKey("ErrorMessage"));
        }
    }
}
