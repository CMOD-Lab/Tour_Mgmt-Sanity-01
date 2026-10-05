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
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Tests.Pages.Bookings
{
    public class BookingsCreateModelTests
    {
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<ITourService> _tourServiceMock;
        private readonly Mock<ILogger<CreateModel>> _loggerMock;
        private readonly CreateModel _pageModel;
        private readonly TestSession _session;

        public BookingsCreateModelTests()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _tourServiceMock = new Mock<ITourService>();
            _loggerMock = new Mock<ILogger<CreateModel>>();
            _pageModel = new CreateModel(_bookingServiceMock.Object, _tourServiceMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void CreateModel_Constructor_InitializesInputAsNew()
        {
            Assert.NotNull(_pageModel.Input);
            Assert.Null(_pageModel.SelectedTour);
        }

        [Fact]
        public async Task OnGetAsync_WithNoTourId_ReturnsPage()
        {
            // Act
            var result = await _pageModel.OnGetAsync(null);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WithValidTourId_SetsSelectedTourAndInput()
        {
            // Arrange
            var tour = new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris" };
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tour);

            // Act
            var result = await _pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.SelectedTour);
            Assert.Equal("Paris Tour", _pageModel.Input.TourName);
            Assert.Equal("Paris", _pageModel.Input.Place);
            Assert.Equal(1, _pageModel.Input.TourId);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserLoggedIn_PreFillsEmail()
        {
            // Arrange
            _session.SetString("UserEmail", "user@example.com");
            _session.SetString("UserName", "John Doe");

            // Act
            var result = await _pageModel.OnGetAsync(null);

            // Assert
            Assert.Equal("user@example.com", _pageModel.Input.Email);
            Assert.Equal("John", _pageModel.Input.FirstName);
        }

        [Fact]
        public async Task OnGetAsync_WhenUserNotLoggedIn_DoesNotPreFillEmail()
        {
            // Arrange - no UserEmail in session

            // Act
            await _pageModel.OnGetAsync(null);

            // Assert
            Assert.Equal(string.Empty, _pageModel.Input.Email);
        }

        [Fact]
        public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
        {
            // Arrange
            _pageModel.ModelState.AddModelError("TourName", "Required");

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidAndSuccess_RedirectsToMyBookings()
        {
            // Arrange
            _pageModel.Input = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "John",
                TourId = 1
            };
            _bookingServiceMock
                .Setup(s => s.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Booking { Id = 1 });

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("MyBookings", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPage()
        {
            // Arrange
            _pageModel.Input = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "John"
            };
            _bookingServiceMock
                .Setup(s => s.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Create error"));

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValid_MapsViewModelToBooking()
        {
            // Arrange
            Booking? capturedBooking = null;
            _pageModel.Input = new BookingCreateViewModel
            {
                TourName = "Rome Tour",
                Place = "Rome",
                Email = "bob@example.com",
                FirstName = "Bob",
                TourId = 2
            };
            _bookingServiceMock
                .Setup(s => s.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .Callback<Booking, CancellationToken>((b, _) => capturedBooking = b)
                .ReturnsAsync(new Booking { Id = 1 });

            // Act
            await _pageModel.OnPostAsync();

            // Assert
            Assert.NotNull(capturedBooking);
            Assert.Equal("Rome Tour", capturedBooking!.TourName);
            Assert.Equal("Rome", capturedBooking.Place);
            Assert.Equal("bob@example.com", capturedBooking.Email);
            Assert.Equal("Bob", capturedBooking.FirstName);
            Assert.Equal(2, capturedBooking.TourId);
            Assert.True(capturedBooking.IsActive);
        }

        [Fact]
        public async Task OnPostAsync_WhenModelInvalidWithTourId_LoadsSelectedTour()
        {
            // Arrange
            _pageModel.ModelState.AddModelError("TourName", "Required");
            _pageModel.Input = new BookingCreateViewModel { TourId = 3 };
            var tour = new Tour { Id = 3, TourName = "Test Tour" };
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tour);

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.SelectedTour);
        }
    }
}
