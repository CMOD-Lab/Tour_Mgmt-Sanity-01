using FluentValidation;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Validators;

/// <summary>
/// Validator for TourCreateDto.
/// </summary>
public class TourCreateDtoValidator : AbstractValidator<TourCreateDto>
{
    public TourCreateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(20).WithMessage("Tour name must not exceed 20 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(20).WithMessage("Place must not exceed 20 characters.");

        RuleFor(x => x.Days)
            .GreaterThan(0).WithMessage("Days must be greater than 0.")
            .LessThanOrEqualTo(99).WithMessage("Days must not exceed 99.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Locations)
            .NotEmpty().WithMessage("Locations are required.")
            .MaximumLength(100).WithMessage("Locations must not exceed 100 characters.");

        RuleFor(x => x.TourInfo)
            .NotEmpty().WithMessage("Tour information is required.")
            .MaximumLength(200).WithMessage("Tour information must not exceed 200 characters.");
    }
}

/// <summary>
/// Validator for TourUpdateDto.
/// </summary>
public class TourUpdateDtoValidator : AbstractValidator<TourUpdateDto>
{
    public TourUpdateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(20).WithMessage("Tour name must not exceed 20 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(20).WithMessage("Place must not exceed 20 characters.");

        RuleFor(x => x.Days)
            .GreaterThan(0).WithMessage("Days must be greater than 0.")
            .LessThanOrEqualTo(99).WithMessage("Days must not exceed 99.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Locations)
            .NotEmpty().WithMessage("Locations are required.")
            .MaximumLength(100).WithMessage("Locations must not exceed 100 characters.");

        RuleFor(x => x.TourInfo)
            .NotEmpty().WithMessage("Tour information is required.")
            .MaximumLength(200).WithMessage("Tour information must not exceed 200 characters.");
    }
}
