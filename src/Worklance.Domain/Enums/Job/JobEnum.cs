namespace Worklance.Domain.Enums.Job;

public enum JobType
{
    Contract = 1,
    Hourly = 2
}

public enum JobStatus
{
    Draft = 1,
    Open = 2,
    Assigned = 3,
    InProgress = 4,
    Delivered = 5,
    Completed = 6,
    Cancelled = 7
}
