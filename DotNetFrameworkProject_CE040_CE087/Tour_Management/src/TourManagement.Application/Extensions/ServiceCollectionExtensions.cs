using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces.Services;

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
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        // Register application services
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}
