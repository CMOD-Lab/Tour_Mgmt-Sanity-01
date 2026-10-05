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

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        _service = new UserService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    // ─── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = new DateTime(1990, 1, 1), Street = "1 Main St", City = "NY", State = "NY" },
            new UserInfo { Email = "bob@test.com",   FirstName = "Bob",   LastName = "Jones", Gender = "Male",   Dob = new DateTime(1985, 5, 15), Street = "2 Oak Ave", City = "LA", State = "CA" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Email.Should().Be("alice@test.com");
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmpty_ShouldReturnEmptyList()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserInfo>());

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.GetAllAsync()).Should().ThrowAsync<Exception>().WithMessage("DB error");
    }

    // ─── GetByEmailAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        var user = new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = new DateTime(1990, 1, 1), Street = "1 Main St", City = "NY", State = "NY" };
        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.GetByEmailAsync("alice@test.com");

        result.Should().NotBeNull();
        result!.Email.Should().Be("alice@test.com");
        result.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task GetByEmailAsync_WithUnknownEmail_ShouldReturnNull()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("nobody@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        var result = await _service.GetByEmailAsync("nobody@test.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_WhenRepositoryThrows_ShouldRethrow()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

        await _service.Invoking(s => s.GetByEmailAsync("x@test.com")).Should().ThrowAsync<Exception>();
    }

    // ─── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateUser()
    {
        var dto = new UserCreateDto
        {
            Email = "new@test.com",
            FirstName = "New",
            LastName = "User",
            Gender = "Male",
            Password = "Password123",
            Dob = new DateTime(1995, 3, 20),
            Street = "5 Elm St",
            City = "Chicago",
            State = "IL"
        };

        _mockRepository.Setup(r => r.ExistsAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo u, CancellationToken _) => u);

        var result = await _service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Email.Should().Be("new@test.com");
        result.FirstName.Should().Be("New");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ShouldThrowDuplicateEntityException()
    {
        var dto = new UserCreateDto { Email = "existing@test.com", Password = "pass", FirstName = "A", LastName = "B", Gender = "Male", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s" };
        _mockRepository.Setup(r => r.ExistsAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await _service.Invoking(s => s.CreateAsync(dto)).Should().ThrowAsync<DuplicateEntityException>();
    }

    [Fact]
    public async Task CreateAsync_ShouldHashPassword()
    {
        var dto = new UserCreateDto { Email = "hash@test.com", Password = "PlainText", FirstName = "H", LastName = "U", Gender = "Female", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s" };
        UserInfo? captured = null;

        _mockRepository.Setup(r => r.ExistsAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .Callback<UserInfo, CancellationToken>((u, _) => captured = u)
            .ReturnsAsync((UserInfo u, CancellationToken _) => u);

        await _service.CreateAsync(dto);

        captured.Should().NotBeNull();
        captured!.Password.Should().NotBe("PlainText");
        BCrypt.Net.BCrypt.Verify("PlainText", captured.Password).Should().BeTrue();
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithValidEmail_ShouldUpdateUser()
    {
        var existing = new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = new DateTime(1990, 1, 1), Street = "1 Main St", City = "NY", State = "NY" };
        var dto = new UserUpdateDto { FirstName = "Alicia", LastName = "Smith", Gender = "Female", Dob = new DateTime(1990, 1, 1), Street = "1 Main St", City = "NY", State = "NY" };

        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo u, CancellationToken _) => u);

        var result = await _service.UpdateAsync("alice@test.com", dto);

        result.Should().NotBeNull();
        result.FirstName.Should().Be("Alicia");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentEmail_ShouldThrowNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("ghost@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        await _service.Invoking(s => s.UpdateAsync("ghost@test.com", new UserUpdateDto())).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_WithNewPassword_ShouldHashPassword()
    {
        var existing = new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s", Password = "old" };
        var dto = new UserUpdateDto { FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s", Password = "NewPass123" };

        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo u, CancellationToken _) => u);

        var result = await _service.UpdateAsync("alice@test.com", dto);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithEmptyPassword_ShouldNotChangePassword()
    {
        var existing = new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s", Password = "existingHash" };
        var dto = new UserUpdateDto { FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s", Password = null };

        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo u, CancellationToken _) => u);

        await _service.UpdateAsync("alice@test.com", dto);

        existing.Password.Should().Be("existingHash");
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithValidEmail_ShouldCallRepositoryDelete()
    {
        _mockRepository.Setup(r => r.ExistsAsync("alice@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync("alice@test.com", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _service.DeleteAsync("alice@test.com");

        _mockRepository.Verify(r => r.DeleteAsync("alice@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentEmail_ShouldThrowNotFoundException()
    {
        _mockRepository.Setup(r => r.ExistsAsync("ghost@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await _service.Invoking(s => s.DeleteAsync("ghost@test.com")).Should().ThrowAsync<NotFoundException>();
    }

    // ─── ValidateLoginAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ShouldReturnUser()
    {
        var password = "Secret123";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s", Password = hash };

        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.ValidateLoginAsync("alice@test.com", password);

        result.Should().NotBeNull();
        result!.Email.Should().Be("alice@test.com");
    }

    [Fact]
    public async Task ValidateLoginAsync_WithWrongPassword_ShouldReturnNull()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("CorrectPass");
        var user = new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s", Password = hash };

        _mockRepository.Setup(r => r.GetByEmailAsync("alice@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.ValidateLoginAsync("alice@test.com", "WrongPass");

        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidateLoginAsync_WithUnknownEmail_ShouldReturnNull()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("nobody@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        var result = await _service.ValidateLoginAsync("nobody@test.com", "pass");

        result.Should().BeNull();
    }

    // ─── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnFilteredUsers()
    {
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "alice@test.com", FirstName = "Alice", LastName = "Smith", Gender = "Female", Dob = DateTime.UtcNow, Street = "s", City = "c", State = "s" }
        };
        _mockRepository.Setup(r => r.SearchAsync("alice", It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _service.SearchAsync("alice");

        result.Should().HaveCount(1);
        result.First().Email.Should().Be("alice@test.com");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatch_ShouldReturnEmptyList()
    {
        _mockRepository.Setup(r => r.SearchAsync("zzz", It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserInfo>());

        var result = await _service.SearchAsync("zzz");

        result.Should().BeEmpty();
    }
}
