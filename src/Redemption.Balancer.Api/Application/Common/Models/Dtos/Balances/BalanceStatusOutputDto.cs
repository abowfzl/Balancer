namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

public class BalanceStatusOutputDto
{
    public decimal IRRGained { get; set; }

    public decimal USDTGained { get; set; }

    public decimal IRRInject { get; set; }

    public decimal USDTInject { get; set; }
}
