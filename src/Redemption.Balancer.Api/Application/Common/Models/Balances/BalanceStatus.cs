namespace Redemption.Balancer.Api.Application.Common.Models.Balances;

public class BalanceStatus
{
    public decimal TotalBalanceInIRR { get; set; }

    public decimal TotalBalanceInUSDT { get; set; }

    public decimal IRRBalance { get; set; }

    public decimal USDTBalance { get; set; }

    public decimal IRRInject { get; set; }

    public decimal USDTInject { get; set; }

}
