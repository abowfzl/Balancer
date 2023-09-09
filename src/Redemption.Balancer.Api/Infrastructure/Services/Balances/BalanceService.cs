using Redemption.Balancer.Api.Application.Common.Contracts;
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
        var balanceStatus = new BalanceStatus();

        var transactions = await _transactionService.GetAccountTransactions(accountId, cancellationToken, request.StartDate, request.EndDate);

        var irrBalanceValue = CalculateIRRBalance(accountId, transactions);
        var usdtBalanceValue = CalculateUSDTValueBalance(accountId, transactions);

        balanceStatus.USDTGained = usdtBalanceValue + ConvertIRRtoUSDT(irrBalanceValue, request.B2BRate);
        balanceStatus.IRRGained = ConvertUSDTtoIRR(usdtBalanceValue, request.B2BRate) + irrBalanceValue;

        var balancedUSDTBalance = balanceStatus.USDTGained / 2;
        var balancedIRRBalance = ConvertUSDTtoIRR(balancedUSDTBalance, request.B2BRate);

        balanceStatus.USDTInject = usdtBalanceValue - balancedUSDTBalance;
        balanceStatus.IRRInject = irrBalanceValue - balancedIRRBalance;

        return balanceStatus;
    }

    private static decimal ConvertIRRtoUSDT(decimal value, decimal rate)
    {
        return value / rate;
    }

    private static decimal ConvertUSDTtoIRR(decimal value, decimal rate)
    {
        return value * rate;
    }

    private static decimal CalculateUSDTValueBalance(int accountId, IList<TransactionEntity> transactions)
    {
        decimal totalUSDT = 0;

        foreach (var otherTransaction in transactions.Where(t => t.Symbol != "IRR"))
        {
            if (otherTransaction.FromAccountId == accountId)
                totalUSDT -= otherTransaction.TotalValue;

            if (otherTransaction.ToAccountId == accountId)
                totalUSDT += otherTransaction.TotalValue;
        }

        return totalUSDT;
    }

    private static decimal CalculateIRRBalance(int accountId, IList<TransactionEntity> transactions)
    {
        decimal totalIRR = 0;

        foreach (var irrTransaction in transactions.Where(t => t.Symbol == "IRR").ToList())
        {
            if (irrTransaction.FromAccountId == accountId)
                totalIRR -= irrTransaction.Amount;

            if (irrTransaction.ToAccountId == accountId)
                totalIRR += irrTransaction.Amount;
        }

        return totalIRR;
    }
}
