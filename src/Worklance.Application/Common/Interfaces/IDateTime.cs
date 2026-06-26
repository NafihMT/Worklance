namespace Worklance.Application.Common.Interfaces;

public interface IDateTime
{
    DateTime OffsetNow => DateTime.UtcNow; // Or DateTimeOffset if preferred, let's use DateTime UtcNow.
    DateTime UtcNow { get; }
}
