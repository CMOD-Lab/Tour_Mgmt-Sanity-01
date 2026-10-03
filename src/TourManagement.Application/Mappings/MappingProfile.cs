using AutoMapper;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Tour mappings
        CreateMap<Tour, TourDto>();
        CreateMap<TourCreateDto, Tour>();
        CreateMap<TourUpdateDto, Tour>();

        // UserInfo mappings
        CreateMap<UserInfo, UserDto>();
        CreateMap<UserCreateDto, UserInfo>();
        CreateMap<UserUpdateDto, UserInfo>();

        // Booking mappings
        CreateMap<Booking, BookingDto>();
        CreateMap<BookingCreateDto, Booking>()
            .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<BookingUpdateDto, Booking>();
    }
}
