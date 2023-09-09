namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;

public class WorkerInputDto
{
    public bool IsEnable { get; set; }

    public string? Name { get; set; }

    public int Interval { get; set; }
}
