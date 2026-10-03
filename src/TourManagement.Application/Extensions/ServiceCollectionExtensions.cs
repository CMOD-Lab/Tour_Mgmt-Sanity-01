using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Interfaces;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Application.Validators;

namespace TourManagement.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Application layer services with the DI container.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<TourCreateDtoValidator>();

        // Business services
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}
