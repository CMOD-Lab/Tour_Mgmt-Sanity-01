using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tour_Management.Application.Services;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Exceptions;
using Tour_Management.Domain.Interfaces.Repositories;
using Xunit;

namespace Tour_Management.UnitTests.Services;

/// <summary>
/// Extended unit tests for UserService covering all methods and edge cases.
/// </summary>
public class UserServiceExtendedTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceExtendedTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _service = new UserService(_mockRepository.Object, _mockLogger.Object);
    }

    // ─── GetAllUsersAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith" },
            new UserInfo { Email = "bob@test.com", FirstName = "Bob", LastName = "Jones" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "alice@test.com");
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenRepositoryReturnsEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfo>());

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllUsersAsync());
    }

    // ─── GetUserByEmailAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetUserByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var user = new UserInfo { Email = "alice@test.com", FirstName = "Alice" };
        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByEmailAsync("alice@test.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("alice@test.com");
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("nobody@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _service.GetUserByEmailAsync("nobody@test.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetUserByEmailAsync("test@test.com"));
    }

    // ─── RegisterUserAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task RegisterUserAsync_ShouldSetCreatedDateAndIsActive()
    {
        // Arrange
        var user = new UserInfo { Email = "new@test.com", FirstName = "New", Password = "pass123" };
        _mockRepository.Setup(r => r.GetByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo u, CancellationToken _) => u);

        // Act
        var result = await _service.RegisterUserAsync(user);

        // Assert
        result.IsActive.Should().BeTrue();
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task RegisterUserAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        var user = new UserInfo { Email = "new@test.com" };
        _mockRepository.Setup(r => r.GetByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.RegisterUserAsync(user));
    }

    [Fact]
    public async Task RegisterUserAsync_ExceptionMessageShouldContainEmail()
    {
        // Arrange
        var email = "existing@test.com";
        var existingUser = new UserInfo { Email = email };
        _mockRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.RegisterUserAsync(new UserInfo { Email = email }));
        ex.Message.Should().Contain(email);
    }

    // ─── UpdateUserAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateUserAsync_WithValidUser_ShouldUpdateAndReturn()
    {
        // Arrange
        var existing = new UserInfo { Email = "alice@test.com", FirstName = "Alice" };
        var updated = new UserInfo { Email = "alice@test.com", FirstName = "Alicia", LastName = "Smith" };
        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        // Act
        var result = await _service.UpdateUserAsync(updated);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Alicia");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var user = new UserInfo { Email = "ghost@test.com" };
        _mockRepository.Setup(r => r.GetByEmailAsync("ghost@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateUserAsync(user));
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var existing = new UserInfo { Email = "alice@test.com" };
        var user = new UserInfo { Email = "alice@test.com", FirstName = "Updated" };
        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo u, CancellationToken _) => u);

        // Act
        var result = await _service.UpdateUserAsync(user);

        // Assert
        result.ModifiedDate.Should().NotBeNull();
        result.ModifiedDate!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateUserAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        var existing = new UserInfo { Email = "alice@test.com" };
        var user = new UserInfo { Email = "alice@test.com" };
        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.UpdateUserAsync(user));
    }

    // ─── DeleteUserAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteUserAsync_WithValidEmail_ShouldDeleteUser()
    {
        // Arrange
        var user = new UserInfo { Email = "alice@test.com" };
        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockRepository.Setup(r => r.DeleteAsync("alice@test.com", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteUserAsync("alice@test.com");

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync("alice@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentEmail_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("ghost@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteUserAsync("ghost@test.com"));
    }

    [Fact]
    public async Task DeleteUserAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteUserAsync("test@test.com"));
    }

    // ─── AuthenticateAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task AuthenticateAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        // Arrange
        _mockRepository.Setup(r => r.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.AuthenticateAsync("test@test.com", "pass"));
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldCallRepositoryWithCorrectCredentials()
    {
        // Arrange
        var email = "user@test.com";
        var password = "secret123";
        _mockRepository.Setup(r => r.AuthenticateAsync(email, password, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        // Act
        await _service.AuthenticateAsync(email, password);

        // Assert
        _mockRepository.Verify(r => r.AuthenticateAsync(email, password, It.IsAny<CancellationToken>()), Times.Once);
    }
}
