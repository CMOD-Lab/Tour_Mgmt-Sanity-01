using TourManagement.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace TourManagement.UnitTests.Domain;

/// <summary>
/// Unit tests for domain exception classes.
/// </summary>
public class DomainExceptionsTests
{
    // ─── NotFoundException ────────────────────────────────────────────────────

    [Fact]
    public void NotFoundException_WithMessage_ShouldSetMessage()
    {
        var ex = new NotFoundException("Entity not found");

        ex.Message.Should().Be("Entity not found");
    }

    [Fact]
    public void NotFoundException_WithEntityNameAndKey_ShouldFormatMessage()
    {
        var ex = new NotFoundException("Tour", 42);

        ex.Message.Should().Be("Tour with key '42' was not found.");
    }

    [Fact]
    public void NotFoundException_WithStringKey_ShouldFormatMessage()
    {
        var ex = new NotFoundException("UserInfo", "user@test.com");

        ex.Message.Should().Be("UserInfo with key 'user@test.com' was not found.");
    }

    [Fact]
    public void NotFoundException_ShouldInheritFromException()
    {
        var ex = new NotFoundException("test");

        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void NotFoundException_CanBeCaughtAsException()
    {
        Action act = () => throw new NotFoundException("Tour", 1);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void NotFoundException_CanBeCaughtAsNotFoundException()
    {
        Action act = () => throw new NotFoundException("Tour", 1);

        act.Should().Throw<NotFoundException>();
    }

    // ─── ValidationException ──────────────────────────────────────────────────

    [Fact]
    public void ValidationException_WithMessage_ShouldSetMessage()
    {
        var ex = new TourManagement.Domain.Exceptions.ValidationException("Validation failed");

        ex.Message.Should().Be("Validation failed");
    }

    [Fact]
    public void ValidationException_ShouldInheritFromException()
    {
        var ex = new TourManagement.Domain.Exceptions.ValidationException("test");

        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void ValidationException_CanBeCaughtAsException()
    {
        Action act = () => throw new TourManagement.Domain.Exceptions.ValidationException("invalid");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void ValidationException_CanBeCaughtAsValidationException()
    {
        Action act = () => throw new TourManagement.Domain.Exceptions.ValidationException("invalid");

        act.Should().Throw<TourManagement.Domain.Exceptions.ValidationException>();
    }

    // ─── DuplicateEntityException ─────────────────────────────────────────────

    [Fact]
    public void DuplicateEntityException_WithMessage_ShouldSetMessage()
    {
        var ex = new DuplicateEntityException("Duplicate entity detected");

        ex.Message.Should().Be("Duplicate entity detected");
    }

    [Fact]
    public void DuplicateEntityException_ShouldInheritFromException()
    {
        var ex = new DuplicateEntityException("test");

        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void DuplicateEntityException_CanBeCaughtAsException()
    {
        Action act = () => throw new DuplicateEntityException("duplicate");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void DuplicateEntityException_CanBeCaughtAsDuplicateEntityException()
    {
        Action act = () => throw new DuplicateEntityException("duplicate");

        act.Should().Throw<DuplicateEntityException>();
    }

    [Fact]
    public void DuplicateEntityException_WithEmailMessage_ShouldFormatCorrectly()
    {
        var email = "user@test.com";
        var ex = new DuplicateEntityException($"A user with email '{email}' already exists.");

        ex.Message.Should().Contain(email);
    }
}
