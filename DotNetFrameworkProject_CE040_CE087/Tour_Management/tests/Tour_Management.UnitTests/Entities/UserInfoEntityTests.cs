using FluentAssertions;
using Tour_Management.Domain.Entities;
using Xunit;

namespace Tour_Management.UnitTests.Entities;

/// <summary>
/// Unit tests for UserInfo entity.
/// </summary>
public class UserInfoEntityTests
{
    [Fact]
    public void UserInfo_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var user = new UserInfo();

        // Assert
        user.Email.Should().Be(string.Empty);
        user.FirstName.Should().Be(string.Empty);
        user.LastName.Should().Be(string.Empty);
        user.Gender.Should().Be(string.Empty);
        user.Password.Should().Be(string.Empty);
        user.DateOfBirth.Should().BeNull();
        user.Street.Should().Be(string.Empty);
        user.City.Should().Be(string.Empty);
        user.State.Should().Be(string.Empty);
        user.IsActive.Should().BeTrue();
        user.ModifiedDate.Should().BeNull();
    }

    [Fact]
    public void UserInfo_CreatedDate_ShouldBeSetToUtcNow()
    {
        // Act
        var before = DateTime.UtcNow.AddSeconds(-1);
        var user = new UserInfo();
        var after = DateTime.UtcNow.AddSeconds(1);

        // Assert
        user.CreatedDate.Should().BeAfter(before);
        user.CreatedDate.Should().BeBefore(after);
    }

    [Fact]
    public void UserInfo_SetProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var dob = new DateTime(1990, 5, 15);
        var user = new UserInfo
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "SecurePass123",
            DateOfBirth = dob,
            Street = "123 Main St",
            City = "Mumbai",
            State = "Maharashtra",
            IsActive = true
        };

        // Assert
        user.Email.Should().Be("john.doe@example.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Gender.Should().Be("Male");
        user.Password.Should().Be("SecurePass123");
        user.DateOfBirth.Should().Be(dob);
        user.Street.Should().Be("123 Main St");
        user.City.Should().Be("Mumbai");
        user.State.Should().Be("Maharashtra");
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UserInfo_IsActive_CanBeSetToFalse()
    {
        // Arrange
        var user = new UserInfo { IsActive = true };

        // Act
        user.IsActive = false;

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UserInfo_ModifiedDate_CanBeSet()
    {
        // Arrange
        var user = new UserInfo();
        var modifiedDate = DateTime.UtcNow;

        // Act
        user.ModifiedDate = modifiedDate;

        // Assert
        user.ModifiedDate.Should().Be(modifiedDate);
    }

    [Fact]
    public void UserInfo_DateOfBirth_CanBeNull()
    {
        // Arrange & Act
        var user = new UserInfo { DateOfBirth = null };

        // Assert
        user.DateOfBirth.Should().BeNull();
    }

    [Theory]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("Other")]
    public void UserInfo_Gender_ShouldAcceptVariousValues(string gender)
    {
        // Arrange & Act
        var user = new UserInfo { Gender = gender };

        // Assert
        user.Gender.Should().Be(gender);
    }

    [Fact]
    public void UserInfo_FullAddress_ShouldCombineStreetCityState()
    {
        // Arrange
        var user = new UserInfo
        {
            Street = "456 Park Ave",
            City = "Delhi",
            State = "Delhi"
        };

        // Assert
        user.Street.Should().Be("456 Park Ave");
        user.City.Should().Be("Delhi");
        user.State.Should().Be("Delhi");
    }
}
