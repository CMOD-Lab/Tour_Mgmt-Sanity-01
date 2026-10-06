using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.Pages;
using TourManagement.Web.ViewModels;
using TourManagement.Web.Tests.Pages.Tours;

namespace TourManagement.Web.Tests.Pages
{
    public class HomeIndexModelTests
    {
        private readonly Mock<ITourService> _mockTourService;
        private readonly Mock<ILogger<IndexModel>> _mockLogger;
        private readonly IndexModel _pageModel;

        public HomeIndexModelTests()
        {
            _mockTourService = new Mock<ITourService>();
            _mockLogger = new Mock<ILogger<IndexModel>>();
            _pageModel = new IndexModel(_mockTourService.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task OnGetAsync_LoadsFeaturedTours()
        {
            // Arrange
            var tours = new List<TourDto>
            {
                new TourDto { Id = 1, TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel Tower", TourInfo = "Amazing tour", IsActive = true, CreatedDate = DateTime.UtcNow },
                new TourDto { Id = 2, TourName = "Rome Tour", Place = "Rome", Days = 5, Price = 1200m, Locations = "Colosseum", TourInfo = "Great tour", IsActive = true, CreatedDate = DateTime.UtcNow },
                new TourDto { Id = 3, TourName = "London Tour", Place = "London", Days = 4, Price = 1100m, Locations = "Big Ben", TourInfo = "Wonderful tour", IsActive = true, CreatedDate = DateTime.UtcNow }
            };
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(_pageModel.FeaturedTours);
            Assert.Equal(3, ((List<TourViewModel>)_pageModel.FeaturedTours).Count);
        }

        [Fact]
        public async Task OnGetAsync_LimitsToSixFeaturedTours()
        {
            // Arrange
            var tours = new List<TourDto>();
            for (int i = 1; i <= 10; i++)
            {
                tours.Add(new TourDto
                {
                    Id = i,
                    TourName = $"Tour {i}",
                    Place = $"Place {i}",
                    Days = 5,
                    Price = 1000m,
                    Locations = $"Location {i}",
                    TourInfo = $"Info {i}",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                });
            }
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.Equal(6, ((List<TourViewModel>)_pageModel.FeaturedTours).Count);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsEmptyList()
        {
            // Arrange
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(_pageModel.FeaturedTours);
            Assert.Empty(_pageModel.FeaturedTours);
        }

        [Fact]
        public async Task OnGetAsync_WithEmptyTourList_ReturnsEmptyFeaturedTours()
        {
            // Arrange
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TourDto>());

            // Act
            await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(_pageModel.FeaturedTours);
            Assert.Empty(_pageModel.FeaturedTours);
        }

        [Fact]
        public async Task OnGetAsync_MapsAllTourDtoFieldsCorrectly()
        {
            // Arrange
            var createdDate = new DateTime(2024, 1, 15);
            var tours = new List<TourDto>
            {
                new TourDto
                {
                    Id = 42,
                    TourName = "Special Tour",
                    Place = "Special Place",
                    Days = 10,
                    Price = 2500m,
                    Locations = "Special Location",
                    TourInfo = "Special Info",
                    PicturePath = "special.jpg",
                    IsActive = true,
                    CreatedDate = createdDate
                }
            };
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync(CancellationToken.None);

            // Assert
            var featuredList = (List<TourViewModel>)_pageModel.FeaturedTours;
            Assert.Single(featuredList);
            var vm = featuredList[0];
            Assert.Equal(42, vm.Id);
            Assert.Equal("Special Tour", vm.TourName);
            Assert.Equal("Special Place", vm.Place);
            Assert.Equal(10, vm.Days);
            Assert.Equal(2500m, vm.Price);
            Assert.Equal("Special Location", vm.Locations);
            Assert.Equal("Special Info", vm.TourInfo);
            Assert.Equal("special.jpg", vm.PicturePath);
            Assert.True(vm.IsActive);
            Assert.Equal(createdDate, vm.CreatedDate);
        }

        [Fact]
        public void IndexModel_Constructor_InitializesFeaturedToursAsEmpty()
        {
            // Assert
            Assert.NotNull(_pageModel.FeaturedTours);
            Assert.Empty(_pageModel.FeaturedTours);
        }
    }

    public class ErrorModelTests
    {
        private readonly Mock<ILogger<ErrorModel>> _mockLogger;
        private readonly ErrorModel _pageModel;

        public ErrorModelTests()
        {
            _mockLogger = new Mock<ILogger<ErrorModel>>();
            _pageModel = new ErrorModel(_mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void OnGet_SetsRequestId()
        {
            // Act
            _pageModel.OnGet();

            // Assert
            // RequestId should be set (either from Activity or TraceIdentifier)
            Assert.NotNull(_pageModel.RequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsNotEmpty_ReturnsTrue()
        {
            // Arrange
            _pageModel.RequestId = "test-request-id";

            // Assert
            Assert.True(_pageModel.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
        {
            // Arrange
            _pageModel.RequestId = null;

            // Assert
            Assert.False(_pageModel.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
        {
            // Arrange
            _pageModel.RequestId = string.Empty;

            // Assert
            Assert.False(_pageModel.ShowRequestId);
        }

        [Fact]
        public void ErrorModel_Constructor_InitializesRequestIdAsNull()
        {
            // Assert
            Assert.Null(_pageModel.RequestId);
        }

        [Fact]
        public void OnGet_WithTraceIdentifier_UsesTraceIdentifier()
        {
            // Arrange
            _pageModel.HttpContext.TraceIdentifier = "trace-123";

            // Act
            _pageModel.OnGet();

            // Assert
            // When no Activity.Current, should use TraceIdentifier
            Assert.NotNull(_pageModel.RequestId);
        }
    }
}
