namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

public class BalanceStatusOutputDto
{
    public decimal TotalMasterBalanceInIrr { get; set; }

    public decimal TotalMasterBalanceInUsdt { get; set; }

    public decimal IrrMasterBalance { get; set; }

    public decimal UsdtMasterBalance { get; set; }

    public decimal IrrInject { get; set; }

    public decimal UsdtInject { get; set; }
}
