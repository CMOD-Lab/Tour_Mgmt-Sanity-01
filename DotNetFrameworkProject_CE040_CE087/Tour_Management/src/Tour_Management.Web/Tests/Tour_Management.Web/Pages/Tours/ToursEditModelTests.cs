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
    public class ToursEditModelTests
    {
        private readonly Mock<ITourService> _tourServiceMock;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly Mock<ILogger<EditModel>> _loggerMock;
        private readonly EditModel _pageModel;
        private readonly TestSession _session;

        public ToursEditModelTests()
        {
            _tourServiceMock = new Mock<ITourService>();
            _envMock = new Mock<IWebHostEnvironment>();
            _loggerMock = new Mock<ILogger<EditModel>>();
            _pageModel = new EditModel(_tourServiceMock.Object, _envMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void EditModel_Constructor_InitializesInputAsNew()
        {
            Assert.NotNull(_pageModel.Input);
        }

        [Fact]
        public async Task OnGetAsync_WhenNotAdmin_RedirectsToAdminLogin()
        {
            // Arrange - no AdminEmail in session

            // Act
            var result = await _pageModel.OnGetAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Admin/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdminAndTourExists_ReturnsPageWithInput()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            var tour = new Tour
            {
                Id = 1,
                TourName = "Test Tour",
                Place = "Test Place",
                Days = 5,
                Price = 500m,
                Locations = "Loc1",
                TourInfo = "Info",
                Pic = "pic.jpg",
                IsActive = true
            };
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tour);

            // Act
            var result = await _pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Test Tour", _pageModel.Input.TourName);
            Assert.Equal("Test Place", _pageModel.Input.Place);
            Assert.Equal(5, _pageModel.Input.Days);
            Assert.Equal(500m, _pageModel.Input.Price);
            Assert.Equal("pic.jpg", _pageModel.Input.ExistingPic);
            Assert.True(_pageModel.Input.IsActive);
        }

        [Fact]
        public async Task OnGetAsync_WhenAdminAndTourNotFound_ReturnsNotFound()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Tour?)null);

            // Act
            var result = await _pageModel.OnGetAsync(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToIndex()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _pageModel.OnGetAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
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
        public async Task OnPostAsync_WhenValidAndSuccess_RedirectsToIndex()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _pageModel.Input = new TourEditViewModel
            {
                Id = 1,
                TourName = "Updated Tour",
                Place = "Updated Place",
                Days = 7,
                Price = 700m,
                IsActive = true
            };
            _tourServiceMock
                .Setup(s => s.UpdateTourAsync(1, It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Tour { Id = 1 });

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenNotFoundException_ReturnsNotFound()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _pageModel.Input = new TourEditViewModel
            {
                Id = 1,
                TourName = "Tour",
                Place = "Place",
                Days = 5,
                Price = 100m
            };
            _tourServiceMock
                .Setup(s => s.UpdateTourAsync(It.IsAny<int>(), It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Tour_Management.Domain.Exceptions.NotFoundException("Tour not found"));

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenGeneralException_ReturnsPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _pageModel.Input = new TourEditViewModel
            {
                Id = 1,
                TourName = "Tour",
                Place = "Place",
                Days = 5,
                Price = 100m
            };
            _tourServiceMock
                .Setup(s => s.UpdateTourAsync(It.IsAny<int>(), It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("General error"));

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }
    }
}
