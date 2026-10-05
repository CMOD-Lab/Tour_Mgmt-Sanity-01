using FluentAssertions;
using Tour_Management.Domain.Exceptions;
using Xunit;

namespace Tour_Management.UnitTests.Exceptions;

/// <summary>
/// Unit tests for NotFoundException.
/// </summary>
public class NotFoundExceptionTests
{
    [Fact]
    public void NotFoundException_WithEntityNameAndKey_ShouldFormatMessageCorrectly()
    {
        // Arrange & Act
        var ex = new NotFoundException("Tour", 42);

        // Assert
        ex.Message.Should().Contain("Tour");
        ex.Message.Should().Contain("42");
    }

    [Fact]
    public void NotFoundException_WithStringKey_ShouldFormatMessageCorrectly()
    {
        // Arrange & Act
        var ex = new NotFoundException("UserInfo", "alice@test.com");

        // Assert
        ex.Message.Should().Contain("UserInfo");
        ex.Message.Should().Contain("alice@test.com");
    }

    [Fact]
    public void NotFoundException_WithCustomMessage_ShouldUseProvidedMessage()
    {
        // Arrange
        var message = "Custom not found message";

        // Act
        var ex = new NotFoundException(message);

        // Assert
        ex.Message.Should().Be(message);
    }

    [Fact]
    public void NotFoundException_ShouldInheritFromException()
    {
        // Arrange & Act
        var ex = new NotFoundException("Entity", 1);

        // Assert
        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void NotFoundException_WithEntityNameAndKey_MessageShouldContainNotFound()
    {
        // Arrange & Act
        var ex = new NotFoundException("Booking", 99);

        // Assert
        ex.Message.ToLower().Should().Contain("not found");
    }

    [Fact]
    public void NotFoundException_CanBeThrownAndCaught()
    {
        // Arrange & Act & Assert
        Action action = () => throw new NotFoundException("Tour", 1);
        action.Should().Throw<NotFoundException>()
            .WithMessage("*Tour*");
    }

    [Fact]
    public void NotFoundException_WithZeroKey_ShouldFormatMessageCorrectly()
    {
        // Arrange & Act
        var ex = new NotFoundException("Tour", 0);

        // Assert
        ex.Message.Should().Contain("Tour");
        ex.Message.Should().Contain("0");
    }

    [Fact]
    public void NotFoundException_WithEmptyMessage_ShouldNotThrow()
    {
        // Arrange & Act
        var ex = new NotFoundException(string.Empty);

        // Assert
        ex.Message.Should().Be(string.Empty);
    }

    [Theory]
    [InlineData("Tour", 1)]
    [InlineData("Booking", 100)]
    [InlineData("UserInfo", 999)]
    public void NotFoundException_WithVariousEntityNamesAndKeys_ShouldContainBothInMessage(string entityName, int key)
    {
        // Arrange & Act
        var ex = new NotFoundException(entityName, key);

        // Assert
        ex.Message.Should().Contain(entityName);
        ex.Message.Should().Contain(key.ToString());
    }
}
