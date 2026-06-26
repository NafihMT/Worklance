using Worklance.Application.Common.Interfaces;

namespace Worklance.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}
