namespace TourManagement.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>Initializes a new instance of <see cref="NotFoundException"/>.</summary>
    public NotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.")
    {
    }

    /// <summary>Initializes a new instance of <see cref="NotFoundException"/> with a custom message.</summary>
    public NotFoundException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception thrown when a validation rule is violated.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>Gets the validation errors.</summary>
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>Initializes a new instance of <see cref="ValidationException"/>.</summary>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    /// <summary>Initializes a new instance of <see cref="ValidationException"/> with a single message.</summary>
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            { "General", new[] { message } }
        };
    }
}

/// <summary>
/// Exception thrown when a duplicate entity is detected.
/// </summary>
public class DuplicateEntityException : Exception
{
    /// <summary>Initializes a new instance of <see cref="DuplicateEntityException"/>.</summary>
    public DuplicateEntityException(string entityName, string field, object value)
        : base($"Entity '{entityName}' with {field} '{value}' already exists.")
    {
    }
}
