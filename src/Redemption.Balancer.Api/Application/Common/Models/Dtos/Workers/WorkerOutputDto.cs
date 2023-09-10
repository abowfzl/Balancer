namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;

public class WorkerOutputDto
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool IsEnable { get; set; }

    public int Interval { get; set; }
}
