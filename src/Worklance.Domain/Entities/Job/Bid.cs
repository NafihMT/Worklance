namespace Worklance.Domain.Entities.Job
{
    public class Bid
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
        public int FreelancerProfileId { get; set; }
        public decimal Amount { get; set; }
    }
}
