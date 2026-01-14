namespace webapp.Services;

/// <summary>
/// Default implementation of ITimeProvider that returns the actual system time.
/// </summary>
public class SystemTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
