using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
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
    public async Task CreateAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Email = "existing@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123"
        };
        _mockRepository.Setup(r => r.EmailExistsAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "user@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            IsActive = true
        };
        _mockRepository.Setup(r => r.ValidateCredentialsAsync("user@test.com", "password123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("user@test.com", "password123");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("user@test.com");
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidCredentials_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.ValidateCredentialsAsync("user@test.com", "wrongpassword", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.AuthenticateAsync("user@test.com", "wrongpassword");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "user1@test.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "pass1" },
            new User { Id = 2, Email = "user2@test.com", FirstName = "Jane", LastName = "Smith", Gender = "Female", Password = "pass2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }
}
