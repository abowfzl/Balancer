using System.ComponentModel.DataAnnotations;

namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;

public class WorkerInputDto
{
    [Required]
    public string? Name { get; set; }
}
