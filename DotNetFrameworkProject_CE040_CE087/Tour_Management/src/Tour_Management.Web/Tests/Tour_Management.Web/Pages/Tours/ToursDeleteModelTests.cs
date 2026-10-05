using System;
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
using Tour_Management.Web.Pages.Tours;

namespace Tour_Management.Web.Tests.Pages.Tours
{
    public class ToursDeleteModelTests
    {
        private readonly Mock<ITourService> _tourServiceMock;
        private readonly Mock<ILogger<DeleteModel>> _loggerMock;
        private readonly DeleteModel _pageModel;
        private readonly TestSession _session;

        public ToursDeleteModelTests()
        {
            _tourServiceMock = new Mock<ITourService>();
            _loggerMock = new Mock<ILogger<DeleteModel>>();
            _pageModel = new DeleteModel(_tourServiceMock.Object, _loggerMock.Object);

            _session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _session;

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _pageModel.PageContext = new PageContext { HttpContext = httpContext };
            _pageModel.TempData = tempData;
        }

        [Fact]
        public void DeleteModel_Constructor_InitializesTourAsNull()
        {
            Assert.Null(_pageModel.Tour);
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
        public async Task OnGetAsync_WhenAdminAndTourExists_ReturnsPage()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            var tour = new Tour { Id = 1, TourName = "Test Tour" };
            _tourServiceMock
                .Setup(s => s.GetTourByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tour);

            // Act
            var result = await _pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.Tour);
            Assert.Equal("Test Tour", _pageModel.Tour!.TourName);
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
            var result = await _pageModel.OnPostAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Admin/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenAdminAndSuccess_RedirectsToIndex()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _tourServiceMock
                .Setup(s => s.DeleteTourAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _pageModel.OnPostAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_RedirectsToIndex()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _tourServiceMock
                .Setup(s => s.DeleteTourAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Delete error"));

            // Act
            var result = await _pageModel.OnPostAsync(1);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenAdminAndSuccess_CallsDeleteTourAsync()
        {
            // Arrange
            _session.SetString("AdminEmail", "admin@example.com");
            _tourServiceMock
                .Setup(s => s.DeleteTourAsync(5, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _pageModel.OnPostAsync(5);

            // Assert
            _tourServiceMock.Verify(s => s.DeleteTourAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
