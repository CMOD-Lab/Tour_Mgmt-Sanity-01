using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Application.Validators;
using TourManagement.Domain.DTOs;
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
        // Register AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // Register services
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBookingService, BookingService>();

        // Register validators
        services.AddScoped<IValidator<TourCreateDto>, TourCreateDtoValidator>();
        services.AddScoped<IValidator<TourUpdateDto>, TourUpdateDtoValidator>();
        services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
        services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
        services.AddScoped<IValidator<BookingCreateDto>, BookingCreateDtoValidator>();

        return services;
    }
}
