using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Web.Pages;

namespace Tour_Management.Web.Tests.Pages
{
    public class ErrorModelTests
    {
        private readonly Mock<ILogger<ErrorModel>> _loggerMock;
        private readonly ErrorModel _pageModel;

        public ErrorModelTests()
        {
            _loggerMock = new Mock<ILogger<ErrorModel>>();
            _pageModel = new ErrorModel(_loggerMock.Object);

            var httpContext = new DefaultHttpContext();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public void ErrorModel_Constructor_InitializesCorrectly()
        {
            // Assert
            Assert.NotNull(_pageModel);
            Assert.Null(_pageModel.RequestId);
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
        public void ShowRequestId_WhenRequestIdHasValue_ReturnsTrue()
        {
            // Arrange
            _pageModel.RequestId = "some-request-id";

            // Assert
            Assert.True(_pageModel.ShowRequestId);
        }

        [Fact]
        public void OnGet_SetsRequestId_FromHttpContextTraceIdentifier()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "trace-123";
            _pageModel.PageContext = new PageContext { HttpContext = httpContext };

            // Act
            _pageModel.OnGet();

            // Assert
            Assert.NotNull(_pageModel.RequestId);
            Assert.Equal("trace-123", _pageModel.RequestId);
        }

        [Fact]
        public void OnGet_WhenTraceIdentifierIsSet_ShowRequestIdIsTrue()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "trace-abc";
            _pageModel.PageContext = new PageContext { HttpContext = httpContext };

            // Act
            _pageModel.OnGet();

            // Assert
            Assert.True(_pageModel.ShowRequestId);
        }

        [Fact]
        public void RequestId_CanBeSetManually()
        {
            // Arrange & Act
            _pageModel.RequestId = "manual-id";

            // Assert
            Assert.Equal("manual-id", _pageModel.RequestId);
            Assert.True(_pageModel.ShowRequestId);
        }
    }
}
