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
            .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(200).WithMessage("Place must not exceed 200 characters.");

        RuleFor(x => x.Days)
            .GreaterThan(0).WithMessage("Days must be greater than 0.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Locations)
            .MaximumLength(500).WithMessage("Locations must not exceed 500 characters.")
            .When(x => x.Locations != null);

        RuleFor(x => x.TourInfo)
            .MaximumLength(2000).WithMessage("Tour info must not exceed 2000 characters.")
            .When(x => x.TourInfo != null);
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
            .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(200).WithMessage("Place must not exceed 200 characters.");

        RuleFor(x => x.Days)
            .GreaterThan(0).WithMessage("Days must be greater than 0.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
}
