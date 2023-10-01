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

        var balanceStatus = new BalanceStatus
        {
            IRRBalance = await _transactionService.CalculateAccountIRRTransactions(accountId, cancellationToken, request.StartDate, request.EndDate),
            USDTBalance = await _transactionService.CalculateAccountUSDTTransactions(accountId, cancellationToken, request.StartDate, request.EndDate)
        };

        balanceStatus.TotalBalanceInUSDT = balanceStatus.USDTBalance + ConvertIRRtoUSDT(balanceStatus.IRRBalance, request.B2BIRRRate);
        balanceStatus.TotalBalanceInIRR = ConvertUSDTtoIRR(balanceStatus.USDTBalance, request.B2BIRRRate) + balanceStatus.IRRBalance;

        var balancedUSDTBalance = balanceStatus.TotalBalanceInUSDT / 2;
        var balancedIRRBalance = ConvertUSDTtoIRR(balancedUSDTBalance, request.B2BIRRRate);

        balanceStatus.USDTInject = balancedUSDTBalance - balanceStatus.USDTBalance;
        balanceStatus.IRRInject = balancedIRRBalance - balanceStatus.IRRBalance;

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
