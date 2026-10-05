using FluentValidation;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Validators;

/// <summary>
/// Validator for TourCreateDto.
/// </summary>
public class TourCreateDtoValidator : AbstractValidator<TourCreateDto>
{
    /// <summary>Initializes a new instance of <see cref="TourCreateDtoValidator"/>.</summary>
    public TourCreateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(200).WithMessage("Place must not exceed 200 characters.");

        RuleFor(x => x.Days)
            .GreaterThan(0).WithMessage("Days must be greater than 0.")
            .LessThanOrEqualTo(365).WithMessage("Days must not exceed 365.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Locations)
            .NotEmpty().WithMessage("Locations are required.")
            .MaximumLength(500).WithMessage("Locations must not exceed 500 characters.");

        RuleFor(x => x.TourInfo)
            .NotEmpty().WithMessage("Tour information is required.")
            .MaximumLength(2000).WithMessage("Tour information must not exceed 2000 characters.");
    }
}

/// <summary>
/// Validator for TourUpdateDto.
/// </summary>
public class TourUpdateDtoValidator : AbstractValidator<TourUpdateDto>
{
    /// <summary>Initializes a new instance of <see cref="TourUpdateDtoValidator"/>.</summary>
    public TourUpdateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(200).WithMessage("Place must not exceed 200 characters.");

        RuleFor(x => x.Days)
            .GreaterThan(0).WithMessage("Days must be greater than 0.")
            .LessThanOrEqualTo(365).WithMessage("Days must not exceed 365.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
}
