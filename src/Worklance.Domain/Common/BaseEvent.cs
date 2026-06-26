namespace Worklance.Domain.Common;

public abstract class BaseEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
