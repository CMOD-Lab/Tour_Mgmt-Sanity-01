using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.Pages.Tours;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Tests.Pages.Tours
{
    public class ToursCreateModelTests
    {
        private readonly Mock<ITourService> _tourServiceMock;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly Mock<ILogger<CreateModel>> _loggerMock;
        private readonly CreateModel _pageModel;
        private readonly TestSession _session;

        public ToursCreateModelTests()
        {
            _tourServiceMock = new Mock<ITourService>();
            _envMock = new Mock<IWebHostEnvironment>();
            _loggerMock = new Mock<ILogger<CreateModel>>();
            _pageModel = new CreateModel(_tourServiceMock.Object, _envMock.Object, _loggerMock.Object);

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
        }

        [Fact]
        public void OnGet_WhenNotAdmin_RedirectsToAdminLogin()
        {
            // Arrange - no AdminEmail in session

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Admin/Login", redirect.PageName);
        }

        [Fact]
        public void OnGet_WhenAdmin_ReturnsPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");

            // Act
            var result = _pageModel.OnGet();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenNotAdmin_RedirectsToAdminLogin()
        {
            // Arrange - no AdminEmail in session

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Admin/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _pageModel.ModelState.AddModelError("TourName", "Required");

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidAndNoPicFile_CreatesTourAndRedirects()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _pageModel.Input = new TourCreateViewModel
            {
                TourName = "New Tour",
                Place = "New Place",
                Days = 5,
                Price = 500m,
                Locations = "Loc1",
                TourInfo = "Info",
                PicFile = null
            };
            _tourServiceMock
                .Setup(s => s.CreateTourAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Tour { Id = 1 });

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidAndSuccess_CallsCreateTourAsync()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _pageModel.Input = new TourCreateViewModel
            {
                TourName = "New Tour",
                Place = "New Place",
                Days = 5,
                Price = 500m
            };
            _tourServiceMock
                .Setup(s => s.CreateTourAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Tour { Id = 1 });

            // Act
            await _pageModel.OnPostAsync();

            // Assert
            _tourServiceMock.Verify(s => s.CreateTourAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _pageModel.Input = new TourCreateViewModel
            {
                TourName = "New Tour",
                Place = "New Place",
                Days = 5,
                Price = 500m
            };
            _tourServiceMock
                .Setup(s => s.CreateTourAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Create error"));

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidAndSuccess_SetsTourProperties()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            Tour? capturedTour = null;
            _pageModel.Input = new TourCreateViewModel
            {
                TourName = "Captured Tour",
                Place = "Captured Place",
                Days = 3,
                Price = 300m,
                Locations = "Loc",
                TourInfo = "Info"
            };
            _tourServiceMock
                .Setup(s => s.CreateTourAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
                .Callback<Tour, CancellationToken>((t, _) => capturedTour = t)
                .ReturnsAsync(new Tour { Id = 1 });

            // Act
            await _pageModel.OnPostAsync();

            // Assert
            Assert.NotNull(capturedTour);
            Assert.Equal("Captured Tour", capturedTour!.TourName);
            Assert.Equal("Captured Place", capturedTour.Place);
            Assert.Equal(3, capturedTour.Days);
            Assert.Equal(300m, capturedTour.Price);
            Assert.True(capturedTour.IsActive);
        }
    }
}
