namespace Redemption.Balancer.Api.Application.Common.Models.Balances;

public class BalanceStatus
{
    public decimal TotalBalanceInIrr { get; set; }

    public decimal TotalBalanceInUsdt { get; set; }

    public decimal IrrBalance { get; set; }

    public decimal UsdtBalance { get; set; }

    public decimal IrrDebit { get; set; }

    public decimal UsdtDebit { get; set; }

}
