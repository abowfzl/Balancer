using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Balances;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;
using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Infrastructure.Services.Balances;

public class BalanceService : IBalanceService
{
    private readonly ITransactionService _transactionService;

    public BalanceService(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    public async Task<BalanceStatus> GetBalanceStatus(BalanceStatusInputDto request, int accountId, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var balanceStatus = new BalanceStatus();

        var irrBalanceValue = await _transactionService.CalculateAccountIRRTransactions(accountId, cancellationToken, request.StartDate, request.EndDate);

        var usdtBalanceValue = await _transactionService.CalculateAccountUSDTTransactions(accountId, cancellationToken, request.StartDate, request.EndDate);

        balanceStatus.USDTGained = usdtBalanceValue + ConvertIRRtoUSDT(irrBalanceValue, request.B2BIRRRate);
        balanceStatus.IRRGained = ConvertUSDTtoIRR(usdtBalanceValue, request.B2BIRRRate) + irrBalanceValue;

        var balancedUSDTBalance = balanceStatus.USDTGained / 2;
        var balancedIRRBalance = ConvertUSDTtoIRR(balancedUSDTBalance, request.B2BIRRRate);

        balanceStatus.USDTInject = balancedUSDTBalance - usdtBalanceValue;
        balanceStatus.IRRInject = balancedIRRBalance - irrBalanceValue;

        return balanceStatus;
    }

    private static decimal ConvertIRRtoUSDT(decimal irrValue, decimal irrRate)
    {
        return irrValue / irrRate;
    }

    private static decimal ConvertUSDTtoIRR(decimal usdtValue, decimal irrRate)
    {
        return usdtValue * irrRate;
    }

    private static void ValidateRequest(BalanceStatusInputDto request)
    {
        if (request.B2BIRRRate <= 0)
            throw new BadRequestException("Property 'B2BIRRRate' should be greater than 0");
    }
}
