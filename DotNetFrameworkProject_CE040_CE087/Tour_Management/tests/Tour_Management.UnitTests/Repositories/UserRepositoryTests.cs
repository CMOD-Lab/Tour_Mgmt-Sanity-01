using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Tour_Management.Domain.Entities;
using Tour_Management.Infrastructure.Data;
using Tour_Management.Infrastructure.Repositories;
using Xunit;

namespace Tour_Management.UnitTests.Repositories;

/// <summary>
/// Unit tests for UserRepository using InMemory database.
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _repository = new UserRepository(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    // ─── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        _context.UserInfos.AddRange(
            new UserInfo { Email = "active@test.com", FirstName = "Active", LastName = "User", Password = "pass", IsActive = true },
            new UserInfo { Email = "inactive@test.com", FirstName = "Inactive", LastName = "User", Password = "pass", IsActive = false }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Email.Should().Be("active@test.com");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsersOrderedByFirstName()
    {
        // Arrange
        _context.UserInfos.AddRange(
            new UserInfo { Email = "z@test.com", FirstName = "Zara", LastName = "A", Password = "pass", IsActive = true },
            new UserInfo { Email = "a@test.com", FirstName = "Alice", LastName = "B", Password = "pass", IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        result[0].FirstName.Should().Be("Alice");
        result[1].FirstName.Should().Be("Zara");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoUsers_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    // ─── GetByEmailAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        _context.UserInfos.Add(new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith", Password = "pass", IsActive = true });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("alice@test.com");

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nobody@test.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        _context.UserInfos.Add(new UserInfo { Email = "inactive@test.com", FirstName = "Inactive", LastName = "User", Password = "pass", IsActive = false });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("inactive@test.com");

        // Assert
        result.Should().BeNull();
    }

    // ─── AddAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ShouldPersistUser()
    {
        // Arrange
        var user = new UserInfo { Email = "new@test.com", FirstName = "New", LastName = "User", Password = "pass", IsActive = true };

        // Act
        var result = await _repository.AddAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("new@test.com");
        _context.UserInfos.Should().HaveCount(1);
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserInDatabase()
    {
        // Arrange
        var user = new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Old", Password = "pass", IsActive = true };
        _context.UserInfos.Add(user);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var updatedUser = new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "New", Password = "pass", IsActive = true };

        // Act
        var result = await _repository.UpdateAsync(updatedUser);

        // Assert
        result.LastName.Should().Be("New");
        var fromDb = await _context.UserInfos.FindAsync("alice@test.com");
        fromDb!.LastName.Should().Be("New");
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var user = new UserInfo { Email = "todelete@test.com", FirstName = "Delete", LastName = "Me", Password = "pass", IsActive = true };
        _context.UserInfos.Add(user);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        await _repository.DeleteAsync("todelete@test.com");

        // Assert
        var fromDb = await _context.UserInfos.FindAsync("todelete@test.com");
        fromDb!.IsActive.Should().BeFalse();
        fromDb.ModifiedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentEmail_ShouldNotThrow()
    {
        // Act & Assert
        var action = async () => await _repository.DeleteAsync("ghost@test.com");
        await action.Should().NotThrowAsync();
    }

    // ─── ExistsAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ExistsAsync_WithExistingActiveUser_ShouldReturnTrue()
    {
        // Arrange
        _context.UserInfos.Add(new UserInfo { Email = "exists@test.com", FirstName = "Exists", LastName = "User", Password = "pass", IsActive = true });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync("exists@test.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentEmail_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsAsync("nobody@test.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        _context.UserInfos.Add(new UserInfo { Email = "inactive@test.com", FirstName = "Inactive", LastName = "User", Password = "pass", IsActive = false });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync("inactive@test.com");

        // Assert
        result.Should().BeFalse();
    }

    // ─── AuthenticateAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        _context.UserInfos.Add(new UserInfo { Email = "auth@test.com", FirstName = "Auth", LastName = "User", Password = "correctpass", IsActive = true });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.AuthenticateAsync("auth@test.com", "correctpass");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("auth@test.com");
    }

    [Fact]
    public async Task AuthenticateAsync_WithWrongPassword_ShouldReturnNull()
    {
        // Arrange
        _context.UserInfos.Add(new UserInfo { Email = "auth@test.com", FirstName = "Auth", LastName = "User", Password = "correctpass", IsActive = true });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.AuthenticateAsync("auth@test.com", "wrongpass");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AuthenticateAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        // Act
        var result = await _repository.AuthenticateAsync("nobody@test.com", "anypass");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AuthenticateAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        _context.UserInfos.Add(new UserInfo { Email = "inactive@test.com", FirstName = "Inactive", LastName = "User", Password = "pass", IsActive = false });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.AuthenticateAsync("inactive@test.com", "pass");

        // Assert
        result.Should().BeNull();
    }
}
