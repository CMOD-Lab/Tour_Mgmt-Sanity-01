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
using TourManagement.Web.Pages.Tours;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.Pages.Tours
{
    public class ToursIndexModelTests
    {
        private readonly Mock<ITourService> _mockTourService;
        private readonly Mock<ILogger<IndexModel>> _mockLogger;
        private readonly IndexModel _pageModel;

        public ToursIndexModelTests()
        {
            _mockTourService = new Mock<ITourService>();
            _mockLogger = new Mock<ILogger<IndexModel>>();
            _pageModel = new IndexModel(_mockTourService.Object, _mockLogger.Object);

            // Setup HttpContext with session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task OnGetAsync_WithNoSearchTerm_LoadsAllTours()
        {
            // Arrange
            var tours = new List<TourDto>
            {
                new TourDto { Id = 1, TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel Tower", TourInfo = "Great tour", IsActive = true, CreatedDate = DateTime.UtcNow },
                new TourDto { Id = 2, TourName = "Rome Tour", Place = "Rome", Days = 5, Price = 1200m, Locations = "Colosseum", TourInfo = "Amazing tour", IsActive = true, CreatedDate = DateTime.UtcNow }
            };
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync(null, CancellationToken.None);

            // Assert
            Assert.NotNull(_pageModel.Tours);
            Assert.Equal(2, ((List<TourViewModel>)_pageModel.Tours).Count);
            _mockTourService.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_WithSearchTerm_SearchesTours()
        {
            // Arrange
            var tours = new List<TourDto>
            {
                new TourDto { Id = 1, TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel Tower", TourInfo = "Great tour", IsActive = true, CreatedDate = DateTime.UtcNow }
            };
            _mockTourService.Setup(s => s.SearchAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync("Paris", CancellationToken.None);

            // Assert
            Assert.NotNull(_pageModel.Tours);
            Assert.Equal("Paris", _pageModel.SearchTerm);
            _mockTourService.Verify(s => s.SearchAsync("Paris", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_WithWhitespaceSearchTerm_LoadsAllTours()
        {
            // Arrange
            var tours = new List<TourDto>();
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

            // Act
            await _pageModel.OnGetAsync("   ", CancellationToken.None);

            // Assert
            _mockTourService.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockTourService.Verify(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_ReturnsEmptyList()
        {
            // Arrange
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            await _pageModel.OnGetAsync(null, CancellationToken.None);

            // Assert
            Assert.NotNull(_pageModel.Tours);
            Assert.Empty(_pageModel.Tours);
        }

        [Fact]
        public async Task OnGetAsync_SetsIsAdminFromSession()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TourDto>());

            // Act
            await _pageModel.OnGetAsync(null, CancellationToken.None);

            // Assert
            Assert.True(_pageModel.IsAdmin);
        }

        [Fact]
        public async Task OnGetAsync_WithEmptyTourList_ReturnsEmptyCollection()
        {
            // Arrange
            _mockTourService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TourDto>());

            // Act
            await _pageModel.OnGetAsync(null, CancellationToken.None);

            // Assert
            Assert.NotNull(_pageModel.Tours);
            Assert.Empty(_pageModel.Tours);
        }

        [Fact]
        public void IndexModel_Constructor_InitializesProperties()
        {
            // Assert
            Assert.NotNull(_pageModel.Tours);
            Assert.Equal(string.Empty, _pageModel.SearchTerm);
            Assert.False(_pageModel.IsAdmin);
        }
    }

    public class ToursCreateModelTests
    {
        private readonly Mock<ITourService> _mockTourService;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly Mock<ILogger<CreateModel>> _mockLogger;
        private readonly CreateModel _pageModel;

        public ToursCreateModelTests()
        {
            _mockTourService = new Mock<ITourService>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<CreateModel>>();
            _pageModel = new CreateModel(_mockTourService.Object, _mockEnvironment.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void OnGet_WhenNotAdmin_RedirectsToLogin()
        {
            // Arrange - no admin session

            // Act
            var result = _pageModel.OnGet();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public void OnGet_WhenAdmin_ReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");

            // Act
            var result = _pageModel.OnGet();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenNotAdmin_RedirectsToLogin()
        {
            // Arrange - no admin session

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
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _pageModel.ModelState.AddModelError("TourName", "Required");

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidModel_CreatesTourAndRedirects()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _pageModel.HttpContext.Session.SetString("UserEmail", "admin@test.com");
            _pageModel.Input = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing tour"
            };
            _mockTourService.Setup(s => s.CreateAsync(It.IsAny<TourCreateDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TourDto());

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
            _mockTourService.Verify(s => s.CreateAsync(It.IsAny<TourCreateDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _pageModel.Input = new TourCreateViewModel
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing tour"
            };
            _mockTourService.Setup(s => s.CreateAsync(It.IsAny<TourCreateDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
        }

        [Fact]
        public void CreateModel_Constructor_InitializesInput()
        {
            // Assert
            Assert.NotNull(_pageModel.Input);
        }
    }

    public class ToursEditModelTests
    {
        private readonly Mock<ITourService> _mockTourService;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly Mock<ILogger<EditModel>> _mockLogger;
        private readonly EditModel _pageModel;

        public ToursEditModelTests()
        {
            _mockTourService = new Mock<ITourService>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<EditModel>>();
            _pageModel = new EditModel(_mockTourService.Object, _mockEnvironment.Object, _mockLogger.Object);

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
        public async Task OnGetAsync_WhenTourNotFound_ReturnsNotFound()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult<TourDto?>(null));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenTourFound_PopulatesInputAndReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            var tourDto = new TourDto
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing tour",
                IsActive = true
            };
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tourDto);

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Paris Tour", _pageModel.Input.TourName);
            Assert.Equal("Paris", _pageModel.Input.Place);
            Assert.Equal(7, _pageModel.Input.Days);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToIndex()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

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
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _pageModel.ModelState.AddModelError("TourName", "Required");

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_WhenValidModel_UpdatesTourAndRedirects()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _pageModel.HttpContext.Session.SetString("UserEmail", "admin@test.com");
            _pageModel.Input = new TourEditViewModel
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing tour",
                IsActive = true
            };
            _mockTourService.Setup(s => s.UpdateAsync(1, It.IsAny<TourUpdateDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TourDto());

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _pageModel.Input = new TourEditViewModel
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing tour"
            };
            _mockTourService.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<TourUpdateDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync(CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
        }
    }

    public class ToursDeleteModelTests
    {
        private readonly Mock<ITourService> _mockTourService;
        private readonly Mock<ILogger<DeleteModel>> _mockLogger;
        private readonly DeleteModel _pageModel;

        public ToursDeleteModelTests()
        {
            _mockTourService = new Mock<ITourService>();
            _mockLogger = new Mock<ILogger<DeleteModel>>();
            _pageModel = new DeleteModel(_mockTourService.Object, _mockLogger.Object);

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
        public async Task OnGetAsync_WhenTourNotFound_ReturnsNotFound()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult<TourDto?>(null));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenTourFound_PopulatesTourAndReturnsPage()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            var tourDto = new TourDto
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing tour",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tourDto);

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.Tour);
            Assert.Equal("Paris Tour", _pageModel.Tour!.TourName);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToIndex()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

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
        public async Task OnPostAsync_WhenAdmin_DeletesTourAndRedirects()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockTourService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _pageModel.OnPostAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
            _mockTourService.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_WhenServiceThrowsException_RedirectsToIndexWithError()
        {
            // Arrange
            _pageModel.HttpContext.Session.SetString("IsAdmin", "true");
            _mockTourService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnPostAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }
    }

    public class ToursDetailsModelTests
    {
        private readonly Mock<ITourService> _mockTourService;
        private readonly Mock<ILogger<DetailsModel>> _mockLogger;
        private readonly DetailsModel _pageModel;

        public ToursDetailsModelTests()
        {
            _mockTourService = new Mock<ITourService>();
            _mockLogger = new Mock<ILogger<DetailsModel>>();
            _pageModel = new DetailsModel(_mockTourService.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            _pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            _pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task OnGetAsync_WhenTourNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult<TourDto?>(null));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WhenTourFound_PopulatesTourAndReturnsPage()
        {
            // Arrange
            var tourDto = new TourDto
            {
                Id = 1,
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 7,
                Price = 1500m,
                Locations = "Eiffel Tower",
                TourInfo = "Amazing tour",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tourDto);

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(_pageModel.Tour);
            Assert.Equal("Paris Tour", _pageModel.Tour!.TourName);
            Assert.Equal("Paris", _pageModel.Tour.Place);
            Assert.Equal(7, _pageModel.Tour.Days);
            Assert.Equal(1500m, _pageModel.Tour.Price);
        }

        [Fact]
        public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToIndex()
        {
            // Arrange
            _mockTourService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _pageModel.OnGetAsync(1, CancellationToken.None);

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirect.PageName);
        }

        [Fact]
        public void DetailsModel_Constructor_InitializesTourAsNull()
        {
            // Assert
            Assert.Null(_pageModel.Tour);
        }
    }

    // Mock session implementation for testing
    public class MockSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();

        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);

        public void Set(string key, byte[] value) => _store[key] = value;

        public bool TryGetValue(string key, out byte[] value)
        {
            return _store.TryGetValue(key, out value!);
        }
    }
}
