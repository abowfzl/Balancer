using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface ITransactionService
{
    Task Add(TransactionEntity transaction, CancellationToken cancellationToken);

    Task Add(IList<TransactionEntity> transactions, CancellationToken cancellationToken);

    TransactionEntity GetDebitTransaction(int fromAccountId, int toAccountId, string symbol, decimal referencePrice, decimal differenceAmount, string source);

    TransactionEntity GetCreditTransaction(int fromAccountId, int toAccountId, string symbol, decimal referencePrice, decimal differenceAmount, string source);

    ValueTask<decimal> CalculateAccountIrrTransactions(int accountId, CancellationToken cancellationToken, DateTime? startDate = null, DateTime? endDate = null);

    ValueTask<decimal> CalculateAccountUsdtTransactions(int accountId, CancellationToken cancellationToken, DateTime? startDate = null, DateTime? endDate = null);
}
