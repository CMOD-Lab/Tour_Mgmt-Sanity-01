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
using Tour_Management.Web.Pages;

namespace Tour_Management.Web.Tests.Pages
{
    public class IndexModelTests
    {
        private readonly Mock<ITourService> _tourServiceMock;
        private readonly Mock<ILogger<IndexModel>> _loggerMock;
        private readonly IndexModel _pageModel;

        public IndexModelTests()
        {
            _tourServiceMock = new Mock<ITourService>();
            _loggerMock = new Mock<ILogger<IndexModel>>();
            _pageModel = new IndexModel(_tourServiceMock.Object, _loggerMock.Object);

            // Set up a default HttpContext
            var httpContext = new DefaultHttpContext();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task OnGetAsync_WhenToursExist_SetsFeaturedToursToFirst6()
        {
            // Arrange
            var tours = new List<Tour>
            {
                new Tour { Id = 1, TourName = "Tour 1" },
                new Tour { Id = 2, TourName = "Tour 2" },
                new Tour { Id = 3, TourName = "Tour 3" },
                new Tour { Id = 4, TourName = "Tour 4" },
                new Tour { Id = 5, TourName = "Tour 5" },
                new Tour { Id = 6, TourName = "Tour 6" },
                new Tour { Id = 7, TourName = "Tour 7" },
                new Tour { Id = 8, TourName = "Tour 8" }
            };
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(_pageModel.FeaturedTours);
            Assert.Equal(6, ((List<Tour>)new List<Tour>(_pageModel.FeaturedTours)).Count);
        }

        [Fact]
        public async Task OnGetAsync_WhenFewerThan6Tours_SetsAllTours()
        {
            // Arrange
            var tours = new List<Tour>
            {
                new Tour { Id = 1, TourName = "Tour 1" },
                new Tour { Id = 2, TourName = "Tour 2" }
            };
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(_pageModel.FeaturedTours);
            var featuredList = new List<Tour>(_pageModel.FeaturedTours);
            Assert.Equal(2, featuredList.Count);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_SetsFeaturedToursToEmpty()
        {
            // Arrange
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            await _pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(_pageModel.FeaturedTours);
            Assert.Empty(_pageModel.FeaturedTours);
        }

        [Fact]
        public async Task OnGetAsync_WhenNoTours_SetsFeaturedToursToEmpty()
        {
            // Arrange
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Tour>());

            // Act
            await _pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(_pageModel.FeaturedTours);
            Assert.Empty(_pageModel.FeaturedTours);
        }

        [Fact]
        public void IndexModel_Constructor_InitializesFeaturedToursAsEmptyList()
        {
            // Assert
            Assert.NotNull(_pageModel.FeaturedTours);
            Assert.Empty(_pageModel.FeaturedTours);
        }

        [Fact]
        public async Task OnGetAsync_CallsTourService_GetAllToursAsync()
        {
            // Arrange
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Tour>());

            // Act
            await _pageModel.OnGetAsync();

            // Assert
            _tourServiceMock.Verify(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
