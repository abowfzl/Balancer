namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

public class BalanceStatusOutputDto
{
    public decimal Profit { get; set; }

    public decimal Loss { get; set; }

    public decimal Inject { get; set; }

    public decimal Withdraw { get; set; }
}
