namespace webapp.Services;

/// <summary>
/// Abstraction for time operations to enable testability.
/// </summary>
public interface ITimeProvider
{
    /// <summary>
    /// Gets the current UTC time.
    /// </summary>
    DateTime UtcNow { get; }
}
