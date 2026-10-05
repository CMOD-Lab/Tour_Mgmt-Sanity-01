using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace TourManagement.UnitTests.Repositories;

/// <summary>
/// Integration-style unit tests for UserRepository using InMemory database.
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly UserRepository _repository;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;

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

    private UserInfo CreateSampleUser(string email = "test@test.com", string firstName = "Test", string lastName = "User") => new UserInfo
    {
        Email = email,
        FirstName = firstName,
        LastName = lastName,
        Gender = "Male",
        Password = "hashedpassword",
        Dob = new DateTime(1990, 1, 1),
        Street = "1 Main St",
        City = "New York",
        State = "NY"
    };

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        _context.UserInfos.AddRange(
            CreateSampleUser("alice@test.com", "Alice", "Smith"),
            CreateSampleUser("bob@test.com", "Bob", "Jones")
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsersOrderedByLastNameThenFirstName()
    {
        _context.UserInfos.AddRange(
            CreateSampleUser("c@t.com", "Charlie", "Zebra"),
            CreateSampleUser("a@t.com", "Alice", "Apple"),
            CreateSampleUser("b@t.com", "Bob", "Apple")
        );
        await _context.SaveChangesAsync();

        var result = (await _repository.GetAllAsync()).ToList();

        result[0].LastName.Should().Be("Apple");
        result[0].FirstName.Should().Be("Alice");
        result[1].LastName.Should().Be("Apple");
        result[1].FirstName.Should().Be("Bob");
        result[2].LastName.Should().Be("Zebra");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoUsers_ShouldReturnEmptyList()
    {
        var result = await _repository.GetAllAsync();
        result.Should().BeEmpty();
    }

    // ─── GetByEmailAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        _context.UserInfos.Add(CreateSampleUser("alice@test.com", "Alice", "Smith"));
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmailAsync("alice@test.com");

        result.Should().NotBeNull();
        result!.Email.Should().Be("alice@test.com");
        result.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task GetByEmailAsync_WithUnknownEmail_ShouldReturnNull()
    {
        var result = await _repository.GetByEmailAsync("nobody@test.com");
        result.Should().BeNull();
    }

    // ─── AddAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ShouldPersistUser()
    {
        var user = CreateSampleUser("new@test.com");

        await _repository.AddAsync(user);

        _context.UserInfos.Should().HaveCount(1);
        _context.UserInfos.First().Email.Should().Be("new@test.com");
    }

    [Fact]
    public async Task AddAsync_ShouldReturnCreatedUser()
    {
        var user = CreateSampleUser("returned@test.com");

        var result = await _repository.AddAsync(user);

        result.Should().NotBeNull();
        result.Email.Should().Be("returned@test.com");
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        var user = CreateSampleUser("update@test.com", "Original", "Name");
        _context.UserInfos.Add(user);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        user.FirstName = "Updated";
        await _repository.UpdateAsync(user);

        var updated = await _context.UserInfos.FindAsync("update@test.com");
        updated!.FirstName.Should().Be("Updated");
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser()
    {
        var user = CreateSampleUser("delete@test.com");
        _context.UserInfos.Add(user);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync("delete@test.com");

        var deleted = await _context.UserInfos.FindAsync("delete@test.com");
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentEmail_ShouldNotThrow()
    {
        Func<Task> act = async () => await _repository.DeleteAsync("nobody@test.com");
        await act.Should().NotThrowAsync();
    }

    // ─── ExistsAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ExistsAsync_WithExistingEmail_ShouldReturnTrue()
    {
        _context.UserInfos.Add(CreateSampleUser("exists@test.com"));
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync("exists@test.com");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentEmail_ShouldReturnFalse()
    {
        var result = await _repository.ExistsAsync("nobody@test.com");
        result.Should().BeFalse();
    }

    // ─── ValidateCredentialsAsync ─────────────────────────────────────────────

    [Fact]
    public async Task ValidateCredentialsAsync_WithExistingEmail_ShouldReturnUser()
    {
        _context.UserInfos.Add(CreateSampleUser("valid@test.com"));
        await _context.SaveChangesAsync();

        var result = await _repository.ValidateCredentialsAsync("valid@test.com", "anypassword");

        result.Should().NotBeNull();
        result!.Email.Should().Be("valid@test.com");
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        var result = await _repository.ValidateCredentialsAsync("nobody@test.com", "pass");
        result.Should().BeNull();
    }

    // ─── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_ByEmail_ShouldReturnMatchingUsers()
    {
        _context.UserInfos.AddRange(
            CreateSampleUser("alice@test.com", "Alice", "Smith"),
            CreateSampleUser("bob@test.com", "Bob", "Jones")
        );
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("alice");

        result.Should().HaveCount(1);
        result.First().Email.Should().Be("alice@test.com");
    }

    [Fact]
    public async Task SearchAsync_ByFirstName_ShouldReturnMatchingUsers()
    {
        _context.UserInfos.Add(CreateSampleUser("alice@test.com", "Alice", "Smith"));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("alice");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAsync_ByLastName_ShouldReturnMatchingUsers()
    {
        _context.UserInfos.Add(CreateSampleUser("alice@test.com", "Alice", "Smith"));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("smith");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatch_ShouldReturnEmptyList()
    {
        _context.UserInfos.Add(CreateSampleUser("alice@test.com"));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("zzznomatch");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_ShouldBeCaseInsensitive()
    {
        _context.UserInfos.Add(CreateSampleUser("alice@test.com", "Alice", "Smith"));
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("ALICE");

        result.Should().HaveCount(1);
    }
}
