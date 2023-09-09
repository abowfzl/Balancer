using System.ComponentModel.DataAnnotations;

namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.AccountConfigs;

public class AccountConfigInputDto
{
    public int AccountId { get; set; }

    [Required]
    public string Symbol { get; set; }

    public decimal Value { get; set; }
}