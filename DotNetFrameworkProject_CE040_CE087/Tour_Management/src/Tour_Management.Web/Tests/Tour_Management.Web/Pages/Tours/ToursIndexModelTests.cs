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
using Tour_Management.Web.Pages.Tours;

namespace Tour_Management.Web.Tests.Pages.Tours
{
    public class ToursIndexModelTests
    {
        private readonly Mock<ITourService> _tourServiceMock;
        private readonly Mock<ILogger<IndexModel>> _loggerMock;
        private readonly IndexModel _pageModel;

        public ToursIndexModelTests()
        {
            _tourServiceMock = new Mock<ITourService>();
            _loggerMock = new Mock<ILogger<IndexModel>>();
            _pageModel = new IndexModel(_tourServiceMock.Object, _loggerMock.Object);

            var httpContext = new DefaultHttpContext();
            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        }

        [Fact]
        public void IndexModel_Constructor_InitializesToursAsEmptyList()
        {
            Assert.NotNull(_pageModel.Tours);
            Assert.Empty(_pageModel.Tours);
            Assert.Equal(string.Empty, _pageModel.SearchTerm);
        }

        [Fact]
        public async Task OnGetAsync_WithNoSearchTerm_LoadsAllTours()
        {
            // Arrange
            var tours = new List<Tour>
            {
                new Tour { Id = 1, TourName = "Tour A" },
                new Tour { Id = 2, TourName = "Tour B" }
            };
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync(null);

            // Assert
            Assert.Equal(2, new List<Tour>(_pageModel.Tours).Count);
            Assert.Equal(string.Empty, _pageModel.SearchTerm);
            _tourServiceMock.Verify(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_WithSearchTerm_CallsSearchToursAsync()
        {
            // Arrange
            var tours = new List<Tour> { new Tour { Id = 1, TourName = "Paris Tour" } };
            _tourServiceMock
                .Setup(s => s.SearchToursAsync("Paris", It.IsAny<CancellationToken>()))
                .ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync("Paris");

            // Assert
            Assert.Equal("Paris", _pageModel.SearchTerm);
            Assert.Single(_pageModel.Tours);
            _tourServiceMock.Verify(s => s.SearchToursAsync("Paris", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_WithWhitespaceSearchTerm_LoadsAllTours()
        {
            // Arrange
            var tours = new List<Tour> { new Tour { Id = 1, TourName = "Tour A" } };
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync("   ");

            // Assert
            _tourServiceMock.Verify(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()), Times.Once);
            _tourServiceMock.Verify(s => s.SearchToursAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_SetsToursToEmpty()
        {
            // Arrange
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            await _pageModel.OnGetAsync(null);

            // Assert
            Assert.NotNull(_pageModel.Tours);
            Assert.Empty(_pageModel.Tours);
        }

        [Fact]
        public async Task OnGetAsync_WithEmptySearchTerm_LoadsAllTours()
        {
            // Arrange
            var tours = new List<Tour>();
            _tourServiceMock
                .Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync(string.Empty);

            // Assert
            _tourServiceMock.Verify(s => s.GetAllToursAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
