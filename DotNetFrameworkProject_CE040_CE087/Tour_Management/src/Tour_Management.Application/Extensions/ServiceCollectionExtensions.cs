using Microsoft.Extensions.DependencyInjection;
using Tour_Management.Application.Services;
using Tour_Management.Domain.Interfaces.Services;

namespace Tour_Management.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
