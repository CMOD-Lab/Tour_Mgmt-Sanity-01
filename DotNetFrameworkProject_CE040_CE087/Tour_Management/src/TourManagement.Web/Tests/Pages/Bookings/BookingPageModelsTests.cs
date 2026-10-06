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
using TourManagement.Web.Pages.Bookings;
using TourManagement.Web.ViewModels;
using TourManagement.Web.Tests.Pages.Tours;

namespace TourManagement.Web.Tests.Pages.Bookings
{
    public class BookingsIndexModelTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<ILogger<IndexModel>> _mockLogger;
        private readonly IndexModel _pageModel;

        public BookingsIndexModelTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockLogger = new Mock<ILogger<IndexModel>>();
            _pageModel = new IndexModel(_mockBookingService.Object, _mockLogger.Object);

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
        public async Task OnGetAsync_WhenAdmin_LoadsAllBookings()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            var bookings = new List<BookingDto>
            {
                new BookingDto { Id = 1, TourName = "Paris Tour", Place = "Paris", Email = "user@test.com", FirstName = "John", IsActive = true, CreatedDate = DateTime.UtcNow },
                new BookingDto { Id = 2, TourName = "Rome Tour", Place = "Rome", Email = "user2@test.com", FirstName = "Jane", IsActive = true, CreatedDate = DateTime.UtcNow }
            };
            _mockBookingService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(2, ((List<BookingViewModel>)_pageModel.Bookings).Count);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithEmptyList()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockBookingService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Empty(_pageModel.Bookings);
        }

        [Fact]
        public void IndexModel_Constructor_InitializesBookingsAsEmpty()
        {
            // Assert
            Assert.NotNull(_pageModel.Bookings);
            Assert.Empty(_pageModel.Bookings);
        }
    }

    public class BookingsCreateModelTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<ITourService> _mockTourService;
        private readonly Mock<ILogger<CreateModel>> _mockLogger;
        private readonly CreateModel _pageModel;

        public BookingsCreateModelTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockTourService = new Mock<ITourService>();
            _mockLogger = new Mock<ILogger<CreateModel>>();
            _pageModel = new CreateModel(_mockBookingService.Object, _mockTourService.Object, _mockLogger.Object);

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
            var result = await _pageModel.OnGetAsync(null, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenLoggedIn_ReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.HttpContext.Session.SetString("UserName", "John Doe");

            // Act
            var result = await _pageModel.OnGetAsync(null, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("user@test.com", _pageModel.Input.Email);
            Assert.Equal("John Doe", _pageModel.Input.FirstName);
        }

        [Fact]
        public async Task OnGetAsync_WithTourId_PreFillsTourDetails()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            var tourDto = new TourDto { Id = 1, TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel Tower", TourInfo = "Amazing tour" };
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tourDto);

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Paris Tour", _pageModel.Input.TourName);
            Assert.Equal("Paris", _pageModel.Input.Place);
            Assert.Equal(1, _pageModel.Input.TourId);
        }

        [Fact]
        public async Task OnGetAsync_WithInvalidTourId_ReturnsPageWithoutTourDetails()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _mockTourService.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>())).Returns(Task.FromResult<TourDto?>(null));

            // Act
            var result = await _pageModel.OnGetAsync(999, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenNotLoggedIn_RedirectsToLogin()
        {
            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.ModelState.AddModelError("TourName", "Required");

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidModel_CreatesBookingAndRedirects()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.HttpContext.Session.SetString("UserId", "1");
            _pageModel.Input = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@test.com",
                FirstName = "John"
            };
            _mockBookingService.Setup(s => s.CreateAsync(It.IsAny<BookingCreateDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new BookingDto());

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("MyBookings", redirect.PageName);
            _mockBookingService.Verify(s => s.CreateAsync(It.IsAny<BookingCreateDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.Input = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@test.com",
                FirstName = "John"
            };
            _mockBookingService.Setup(s => s.CreateAsync(It.IsAny<BookingCreateDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WithInvalidUserId_CreatesBookingWithNullUserId()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _pageModel.HttpContext.Session.SetString("UserId", "not-a-number");
            _pageModel.Input = new BookingCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@test.com",
                FirstName = "John"
            };
            BookingCreateDto? capturedDto = null;
            _mockBookingService.Setup(s => s.CreateAsync(It.IsAny<BookingCreateDto>(), It.IsAny<CancellationToken>()))
                .Callback<BookingCreateDto, CancellationToken>((dto, _) => capturedDto = dto)
                .ReturnsAsync(new BookingDto());

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.NotNull(capturedDto);
            Assert.Null(capturedDto!.UserId);
        }
    }

    public class BookingsDeleteModelTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<ILogger<DeleteModel>> _mockLogger;
        private readonly DeleteModel _pageModel;

        public BookingsDeleteModelTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockLogger = new Mock<ILogger<DeleteModel>>();
            _pageModel = new DeleteModel(_mockBookingService.Object, _mockLogger.Object);

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
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenBookingNotFound_ReturnsNotFound()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockBookingService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult<BookingDto?>(null));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenBookingFound_PopulatesBookingAndReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            var bookingDto = new BookingDto
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@test.com",
                FirstName = "John",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            _mockBookingService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bookingDto);

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.Booking);
            Assert.Equal("Paris Tour", _pageModel.Booking!.TourName);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToIndex()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockBookingService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

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
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenAdmin_DeletesBookingAndRedirects()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockBookingService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _pageModel.OnPostAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
            _mockBookingService.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_RedirectsToIndexWithError()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockBookingService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }
    }

    public class BookingsDetailsModelTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<ILogger<DetailsModel>> _mockLogger;
        private readonly DetailsModel _pageModel;

        public BookingsDetailsModelTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockLogger = new Mock<ILogger<DetailsModel>>();
            _pageModel = new DetailsModel(_mockBookingService.Object, _mockLogger.Object);

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
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenBookingNotFound_ReturnsNotFound()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _mockBookingService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult<BookingDto?>(null));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenBookingFound_PopulatesBookingAndReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            var bookingDto = new BookingDto
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@test.com",
                FirstName = "John",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            _mockBookingService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bookingDto);

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.Booking);
            Assert.Equal("Paris Tour", _pageModel.Booking!.TourName);
            Assert.Equal("user@test.com", _pageModel.Booking.Email);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToMyBookings()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _mockBookingService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("MyBookings", redirect.PageName);
        }
    }

    public class MyBookingsModelTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<ILogger<MyBookingsModel>> _mockLogger;
        private readonly MyBookingsModel _pageModel;

        public MyBookingsModelTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockLogger = new Mock<ILogger<MyBookingsModel>>();
            _pageModel = new MyBookingsModel(_mockBookingService.Object, _mockLogger.Object);

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
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenLoggedIn_LoadsUserBookings()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            var bookings = new List<BookingDto>
            {
                new BookingDto { Id = 1, TourName = "Paris Tour", Place = "Paris", Email = "user@test.com", FirstName = "John", IsActive = true, CreatedDate = DateTime.UtcNow }
            };
            _mockBookingService.Setup(s => s.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Single(_pageModel.Bookings);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithEmptyList()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "user@test.com");
            _mockBookingService.Setup(s => s.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Empty(_pageModel.Bookings);
        }

        [Fact]
        public async Task OnGetAsync_WithEmptyEmail_RedirectsToLogin()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("UserEmail", "");

            // Act
            var result = await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirect.PageName);
        }
    }
}
