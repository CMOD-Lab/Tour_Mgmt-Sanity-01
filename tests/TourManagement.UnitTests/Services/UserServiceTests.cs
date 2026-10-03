using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

/// <summary>
/// Unit tests for UserService.
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new UserService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_WithNewEmail_ShouldCreateUser()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Dob = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "Mumbai",
            State = "Maharashtra"
        };

        _mockRepository.Setup(r => r.ExistsAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo u, CancellationToken _) => u);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@example.com");
        result.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task CreateAsync_WithExistingEmail_ShouldThrowDuplicateEntityException()
    {
        // Arrange
        var dto = new UserCreateDto { Email = "existing@example.com", FirstName = "Jane", LastName = "Doe", Gender = "Female", Password = "pass", Dob = DateTime.Today.AddYears(-20), Street = "St", City = "City", State = "State" };
        _mockRepository.Setup(r => r.ExistsAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(dto))
            .Should().ThrowAsync<DuplicateEntityException>();
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var user = new UserInfo { Email = "user@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "password", Dob = DateTime.Today.AddYears(-25), Street = "St", City = "City", State = "State" };
        _mockRepository.Setup(r => r.AuthenticateAsync("user@example.com", "password", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("user@example.com", "password");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("user@example.com");
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidCredentials_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.AuthenticateAsync("user@example.com", "wrongpassword", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _service.AuthenticateAsync("user@example.com", "wrongpassword");

        // Assert
        result.Should().BeNull();
    }
}
