using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Tour business operations.
/// </summary>
public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TourService> _logger;

    public TourService(ITourRepository tourRepository, IMapper mapper, ILogger<TourService> logger)
    {
        _tourRepository = tourRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all tours");
            var tours = await _tourRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tours");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving tour with ID {TourId}", id);
            var tour = await _tourRepository.GetByIdAsync(id, cancellationToken);
            return tour == null ? null : _mapper.Map<TourDto>(tour);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new tour: {TourName}", dto.TourName);
            var tour = _mapper.Map<Tour>(dto);
            var created = await _tourRepository.AddAsync(tour, cancellationToken);
            _logger.LogInformation("Tour created successfully with ID {TourId}", created.Id);
            return _mapper.Map<TourDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", dto.TourName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TourDto> UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating tour with ID {TourId}", id);
            var existing = await _tourRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Tour), id);

            _mapper.Map(dto, existing);
            existing.Id = id;
            var updated = await _tourRepository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("Tour updated successfully with ID {TourId}", id);
            return _mapper.Map<TourDto>(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with ID {TourId}", id);
            var exists = await _tourRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
                throw new NotFoundException(nameof(Tour), id);

            await _tourRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Tour deleted successfully with ID {TourId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching tours with term: {SearchTerm}", searchTerm);
            var tours = await _tourRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tours with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
