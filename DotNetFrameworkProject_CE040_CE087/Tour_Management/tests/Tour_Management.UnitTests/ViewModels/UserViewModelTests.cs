using FluentAssertions;
using Tour_Management.Web.ViewModels;
using Xunit;

namespace Tour_Management.UnitTests.ViewModels;

/// <summary>
/// Unit tests for User ViewModels.
/// </summary>
public class UserViewModelTests
{
    // ─── LoginViewModel ────────────────────────────────────────────────────────

    [Fact]
    public void LoginViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new LoginViewModel();

        // Assert
        vm.Email.Should().Be(string.Empty);
        vm.Password.Should().Be(string.Empty);
    }

    [Fact]
    public void LoginViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var vm = new LoginViewModel
        {
            Email = "user@test.com",
            Password = "SecurePass123"
        };

        // Assert
        vm.Email.Should().Be("user@test.com");
        vm.Password.Should().Be("SecurePass123");
    }

    // ─── RegisterViewModel ─────────────────────────────────────────────────────

    [Fact]
    public void RegisterViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new RegisterViewModel();

        // Assert
        vm.Email.Should().Be(string.Empty);
        vm.FirstName.Should().Be(string.Empty);
        vm.LastName.Should().Be(string.Empty);
        vm.Gender.Should().Be(string.Empty);
        vm.Password.Should().Be(string.Empty);
        vm.ConfirmPassword.Should().Be(string.Empty);
        vm.DateOfBirth.Should().BeNull();
        vm.Street.Should().Be(string.Empty);
        vm.City.Should().Be(string.Empty);
        vm.State.Should().Be(string.Empty);
    }

    [Fact]
    public void RegisterViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var dob = new DateTime(1995, 3, 20);

        // Act
        var vm = new RegisterViewModel
        {
            Email = "john@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "Pass@123",
            ConfirmPassword = "Pass@123",
            DateOfBirth = dob,
            Street = "123 Main St",
            City = "Mumbai",
            State = "Maharashtra"
        };

        // Assert
        vm.Email.Should().Be("john@test.com");
        vm.FirstName.Should().Be("John");
        vm.LastName.Should().Be("Doe");
        vm.Gender.Should().Be("Male");
        vm.Password.Should().Be("Pass@123");
        vm.ConfirmPassword.Should().Be("Pass@123");
        vm.DateOfBirth.Should().Be(dob);
        vm.Street.Should().Be("123 Main St");
        vm.City.Should().Be("Mumbai");
        vm.State.Should().Be("Maharashtra");
    }

    [Fact]
    public void RegisterViewModel_DateOfBirth_CanBeNull()
    {
        // Arrange & Act
        var vm = new RegisterViewModel { DateOfBirth = null };

        // Assert
        vm.DateOfBirth.Should().BeNull();
    }

    // ─── UserProfileViewModel ──────────────────────────────────────────────────

    [Fact]
    public void UserProfileViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new UserProfileViewModel();

        // Assert
        vm.Email.Should().Be(string.Empty);
        vm.FirstName.Should().Be(string.Empty);
        vm.LastName.Should().Be(string.Empty);
        vm.Gender.Should().Be(string.Empty);
        vm.DateOfBirth.Should().BeNull();
        vm.Street.Should().Be(string.Empty);
        vm.City.Should().Be(string.Empty);
        vm.State.Should().Be(string.Empty);
    }

    [Fact]
    public void UserProfileViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var dob = new DateTime(1988, 7, 10);

        // Act
        var vm = new UserProfileViewModel
        {
            Email = "profile@test.com",
            FirstName = "Profile",
            LastName = "User",
            Gender = "Female",
            DateOfBirth = dob,
            Street = "456 Park Ave",
            City = "Delhi",
            State = "Delhi"
        };

        // Assert
        vm.Email.Should().Be("profile@test.com");
        vm.FirstName.Should().Be("Profile");
        vm.LastName.Should().Be("User");
        vm.Gender.Should().Be("Female");
        vm.DateOfBirth.Should().Be(dob);
        vm.Street.Should().Be("456 Park Ave");
        vm.City.Should().Be("Delhi");
        vm.State.Should().Be("Delhi");
    }

    [Theory]
    [InlineData("alice@example.com", "Alice", "Smith")]
    [InlineData("bob@company.org", "Bob", "Jones")]
    [InlineData("charlie@domain.net", "Charlie", "Brown")]
    public void RegisterViewModel_WithVariousUsers_ShouldRetainValues(string email, string firstName, string lastName)
    {
        // Arrange & Act
        var vm = new RegisterViewModel
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Gender = "Male",
            Password = "Pass@123",
            ConfirmPassword = "Pass@123"
        };

        // Assert
        vm.Email.Should().Be(email);
        vm.FirstName.Should().Be(firstName);
        vm.LastName.Should().Be(lastName);
    }
}
