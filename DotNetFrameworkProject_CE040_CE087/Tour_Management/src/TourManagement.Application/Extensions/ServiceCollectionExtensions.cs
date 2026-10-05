using AutoMapper;
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
        // Register AutoMapper manually (compatible with AutoMapper 12.x)
        services.AddSingleton<IMapper>(sp =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        });

        // Register FluentValidation validators manually
        services.AddTransient<IValidator<DTOs.TourCreateDto>, TourCreateDtoValidator>();
        services.AddTransient<IValidator<DTOs.TourUpdateDto>, TourUpdateDtoValidator>();
        services.AddTransient<IValidator<DTOs.UserCreateDto>, UserCreateDtoValidator>();
        services.AddTransient<IValidator<DTOs.UserUpdateDto>, UserUpdateDtoValidator>();
        services.AddTransient<IValidator<DTOs.BookingCreateDto>, BookingCreateDtoValidator>();

        // Register application services
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}
