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
using Tour_Management.Web.Pages.Tours;

namespace Tour_Management.Web.Tests.Pages.Tours
{
    public class ToursDetailsModelTests
    {
        private readonly Mock<ITourService> _tourServiceMock;
        private readonly Mock<ILogger<DetailsModel>> _loggerMock;
        private readonly DetailsModel _pageModel;

        public ToursDetailsModelTests()
        {
            _tourServiceMock = new Mock<ITourService>();
            _loggerMock = new Mock<ILogger<DetailsModel>>();
            _pageModel = new DetailsModel(_tourServiceMock.Object, _loggerMock.Object);

            var httpContext = new DefaultHttpContext();
            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        }

        [Fact]
        public void DetailsModel_Constructor_InitializesTourAsNull()
        {
            Assert.Null(_pageModel.Tour);
        }

        [Fact]
        public async Task OnGetAsync_WithValidId_SetsTour()
        {
            // Arrange
            var tour = new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris" };
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tour);

            // Act
            await _pageModel.OnGetAsync(1);

            // Assert
            Assert.NotNull(_pageModel.Tour);
            Assert.Equal(1, _pageModel.Tour!.Id);
            Assert.Equal("Paris Tour", _pageModel.Tour.TourName);
        }

        [Fact]
        public async Task OnGetAsync_WhenTourNotFound_SetsTourToNull()
        {
            // Arrange
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Tour?)null);

            // Act
            await _pageModel.OnGetAsync(999);

            // Assert
            Assert.Null(_pageModel.Tour);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_TourRemainsNull()
        {
            // Arrange
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            await _pageModel.OnGetAsync(1);

            // Assert
            Assert.Null(_pageModel.Tour);
        }

        [Fact]
        public async Task OnGetAsync_CallsTourService_GetTourByIdAsync()
        {
            // Arrange
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Tour { Id = 5 });

            // Act
            await _pageModel.OnGetAsync(5);

            // Assert
            _tourServiceMock.Verify(s => s.GetTourByIdAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
