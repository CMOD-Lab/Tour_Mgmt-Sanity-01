using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tour_Management.Application.Services;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Repositories;
using Xunit;

namespace Tour_Management.UnitTests.Services;

/// <summary>
/// Unit tests for UserService.
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _service = new UserService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_WithNewEmail_ShouldRegisterUser()
    {
        // Arrange
        var user = new UserInfo
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "password123"
        };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.RegisterUserAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@example.com");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_WithExistingEmail_ShouldThrowException()
    {
        // Arrange
        var existingUser = new UserInfo { Email = "existing@example.com" };
        _mockRepository.Setup(r => r.GetByEmailAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var newUser = new UserInfo { Email = "existing@example.com" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterUserAsync(newUser));
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var user = new UserInfo { Email = "test@example.com", Password = "password123" };
        _mockRepository.Setup(r => r.AuthenticateAsync("test@example.com", "password123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("test@example.com", "password123");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidCredentials_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.AuthenticateAsync("test@example.com", "wrongpassword", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _service.AuthenticateAsync("test@example.com", "wrongpassword");

        // Assert
        result.Should().BeNull();
    }
}
