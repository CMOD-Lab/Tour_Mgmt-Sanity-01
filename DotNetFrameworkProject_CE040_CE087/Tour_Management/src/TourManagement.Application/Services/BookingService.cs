using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Booking business operations.
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IBookingRepository bookingRepository, IMapper mapper, ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all bookings.
    /// </summary>
    public async Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings");
            var bookings = await _bookingRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a booking by its ID.
    /// </summary>
    public async Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving booking with ID {BookingId}", id);
            var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            return booking == null ? null : _mapper.Map<BookingDto>(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all bookings for a specific email.
    /// </summary>
    public async Task<IEnumerable<BookingDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for email: {Email}", email);
            var bookings = await _bookingRepository.GetByEmailAsync(email, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    public async Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for tour: {TourName}", dto.TourName);
            var booking = _mapper.Map<Booking>(dto);
            booking.CreatedDate = DateTime.UtcNow;
            booking.IsActive = true;
            var created = await _bookingRepository.AddAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created with ID {BookingId}", created.Id);
            return _mapper.Map<BookingDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourName}", dto.TourName);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing booking.
    /// </summary>
    public async Task<BookingDto> UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID {BookingId}", id);
            var existing = await _bookingRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Booking), id);

            _mapper.Map(dto, existing);
            existing.ModifiedDate = DateTime.UtcNow;
            var updated = await _bookingRepository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("Booking with ID {BookingId} updated successfully", id);
            return _mapper.Map<BookingDto>(updated);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a booking by its ID.
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID {BookingId}", id);
            if (!await _bookingRepository.ExistsAsync(id, cancellationToken))
                throw new NotFoundException(nameof(Booking), id);

            await _bookingRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Booking with ID {BookingId} deleted successfully", id);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <summary>
    /// Searches bookings by a search term.
    /// </summary>
    public async Task<IEnumerable<BookingDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching bookings with term: {SearchTerm}", searchTerm);
            var bookings = await _bookingRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookings with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
