using FluentValidation;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Validators;

/// <summary>
/// Validator for UserCreateDto.
/// </summary>
public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    /// <summary>Initializes a new instance of <see cref="UserCreateDtoValidator"/>.</summary>
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
    }
}

/// <summary>
/// Validator for BookingCreateDto.
/// </summary>
public class BookingCreateDtoValidator : AbstractValidator<BookingCreateDto>
{
    /// <summary>Initializes a new instance of <see cref="BookingCreateDtoValidator"/>.</summary>
    public BookingCreateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");
    }
}
