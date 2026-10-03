using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

/// <summary>
/// Service interface for Booking business operations.
/// </summary>
public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default);
    Task<BookingDto> UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
