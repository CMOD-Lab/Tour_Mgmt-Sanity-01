using FluentValidation;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Validators;

/// <summary>
/// Validator for BookingCreateDto.
/// </summary>
public class BookingCreateDtoValidator : AbstractValidator<BookingCreateDto>
{
    public BookingCreateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(50).WithMessage("Tour name must not exceed 50 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(50).WithMessage("Place must not exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(50).WithMessage("Email must not exceed 50 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
    }
}
