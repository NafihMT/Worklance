        
namespace Worklance.Domain.Enums.Job
{
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
        Verified = 6,
        NotVerified = 7,
        Completed = 8,
        Cancelled = 9
    }
}
