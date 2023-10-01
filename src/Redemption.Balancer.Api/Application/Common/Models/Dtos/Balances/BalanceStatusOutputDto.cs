namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

public class BalanceStatusOutputDto
{
    public decimal TotalMasterBalanceInIRR { get; set; }

    public decimal TotalMasterBalanceInUSDT { get; set; }

    public decimal IRRMasterBalance { get; set; }

    public decimal USDTMasterBalance { get; set; }

    public decimal IRRInject { get; set; }

    public decimal USDTInject { get; set; }
}
