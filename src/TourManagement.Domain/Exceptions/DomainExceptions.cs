namespace TourManagement.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of NotFoundException.
    /// </summary>
    public NotFoundException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of NotFoundException with entity name and key.
    /// </summary>
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }
}

/// <summary>
/// Exception thrown when a validation error occurs.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of ValidationException.
    /// </summary>
    public ValidationException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when a duplicate entity is detected.
/// </summary>
public class DuplicateEntityException : Exception
{
    /// <summary>
    /// Initializes a new instance of DuplicateEntityException.
    /// </summary>
    public DuplicateEntityException(string message) : base(message) { }
}
