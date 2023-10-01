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
            IrrBalance = await _transactionService.CalculateAccountIrrTransactions(accountId, cancellationToken, request.StartDate, request.EndDate),
            UsdtBalance = await _transactionService.CalculateAccountUsdtTransactions(accountId, cancellationToken, request.StartDate, request.EndDate)
        };

        balanceStatus.TotalBalanceInUsdt = balanceStatus.UsdtBalance + ConvertIrrToUsdt(balanceStatus.IrrBalance, request.B2bIrrRate);
        balanceStatus.TotalBalanceInIrr = ConvertUsdtToIrr(balanceStatus.UsdtBalance, request.B2bIrrRate) + balanceStatus.IrrBalance;

        var balancedUsdtBalance = balanceStatus.TotalBalanceInUsdt / 2;
        var balancedIrrBalance = ConvertUsdtToIrr(balancedUsdtBalance, request.B2bIrrRate);

        balanceStatus.UsdtDebit = balancedUsdtBalance - balanceStatus.UsdtBalance;
        balanceStatus.IrrDebit = balancedIrrBalance - balanceStatus.IrrBalance;

        return balanceStatus;
    }

    private static decimal ConvertIrrToUsdt(decimal irrValue, decimal irrRate)
    {
        return irrValue / irrRate;
    }

    private static decimal ConvertUsdtToIrr(decimal usdtValue, decimal irrRate)
    {
        return usdtValue * irrRate;
    }

    private static void ValidateRequest(BalanceStatusInputDto request)
    {
        if (request.B2bIrrRate <= 0)
            throw new BadRequestException("Property 'B2bIrrRate' should be greater than 0");
    }
}
