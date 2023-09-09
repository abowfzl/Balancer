using System.ComponentModel.DataAnnotations;

namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;

public class RunWorkerInputDto
{
    [Required]
    public string? Name { get; set; }
}
