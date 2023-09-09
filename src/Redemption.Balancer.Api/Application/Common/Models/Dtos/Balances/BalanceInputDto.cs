using System.ComponentModel.DataAnnotations;

namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

public class BalanceInputDto
{
    [Required]
    public string Symbol { get; set; }

    public decimal Value { get; set; }
}
