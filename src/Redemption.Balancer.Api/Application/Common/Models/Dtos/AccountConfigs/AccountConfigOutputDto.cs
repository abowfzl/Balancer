namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.AccountConfigs;

public class AccountConfigOutputDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string? Symbol { get; set; }
    public decimal Value { get; set; }
}