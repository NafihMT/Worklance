using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        InProgress = 3,
        Completed = 4,
        Closed = 5,
        Cancelled = 6
    }
}
