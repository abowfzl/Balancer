using System.ComponentModel.DataAnnotations;

namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

public class BalanceStatusInputDto
{
    [Required]
    public decimal B2bIrrRate { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
