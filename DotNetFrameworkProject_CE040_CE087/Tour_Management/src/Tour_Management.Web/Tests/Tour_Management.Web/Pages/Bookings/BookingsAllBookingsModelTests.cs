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
    public class BookingsAllBookingsModelTests
    {
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<ILogger<AllBookingsModel>> _loggerMock;
        private readonly AllBookingsModel _pageModel;
        private readonly TestSession _session;

        public BookingsAllBookingsModelTests()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _loggerMock = new Mock<ILogger<AllBookingsModel>>();
            _pageModel = new AllBookingsModel(_bookingServiceMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void AllBookingsModel_Constructor_InitializesBookingsAsEmptyList()
        {
            Assert.NotNull(_pageModel.Bookings);
            Assert.Empty(_pageModel.Bookings);
        }

        [Fact]
        public async Task OnGetAsync_WhenNotAdmin_RedirectsToAdminLogin()
        {
            // Arrange - no AdminEmail in session

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Admin/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdmin_LoadsBookingsAndReturnsPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, TourName = "Tour A" },
                new Booking { Id = 2, TourName = "Tour B" }
            };
            _bookingServiceMock
                .Setup(s => s.GetAllBookingsAsync(It.IsAny<CancellationToken>()))
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
            _session.SetString("AdminEmail", "admin@example.com");
            _bookingServiceMock
                .Setup(s => s.GetAllBookingsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostDeleteAsync_WhenNotAdmin_RedirectsToAdminLogin()
        {
            // Arrange - no AdminEmail in session

            // Act
            var result = await _pageModel.OnPostDeleteAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Admin/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostDeleteAsync_WhenAdminAndSuccess_RedirectsToPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _bookingServiceMock
                .Setup(s => s.DeleteBookingAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _pageModel.OnPostDeleteAsync(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostDeleteAsync_WhenAdminAndSuccess_CallsDeleteBookingAsync()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _bookingServiceMock
                .Setup(s => s.DeleteBookingAsync(3, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _pageModel.OnPostDeleteAsync(3);

            // Assert
            _bookingServiceMock.Verify(s => s.DeleteBookingAsync(3, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostDeleteAsync_WhenServiceThrowsException_RedirectsToPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _bookingServiceMock
                .Setup(s => s.DeleteBookingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
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
            _bookingServiceMock
                .Setup(s => s.DeleteBookingAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _pageModel.OnPostDeleteAsync(1);

            // Assert
            Assert.True(_pageModel.TempData.ContainsKey("SuccessMessage"));
        }
    }
}
