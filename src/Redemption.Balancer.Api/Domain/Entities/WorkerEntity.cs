namespace Redemption.Balancer.Api.Domain.Entities;

public class WorkerEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Interval { get; set; }
    public bool IsRunning { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }
}