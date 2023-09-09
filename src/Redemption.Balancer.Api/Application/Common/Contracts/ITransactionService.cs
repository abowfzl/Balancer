using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface ITransactionService
{
    Task Insert(TransactionEntity transaction, CancellationToken cancellationToken);

    Task Insert(IList<TransactionEntity> transactions, CancellationToken cancellationToken);

    TransactionEntity GetDebitTransaction(int fromAccountId, int toAccountId, string symbol, decimal basePrice, decimal quotePrice, decimal differenceAmount);

    TransactionEntity GetCreditTransaction(int fromAccountId, int toAccountId, string symbol, decimal basePrice, decimal quotePrice, decimal differenceAmount);

    Task<IList<TransactionEntity>> GetAccountTransactions(int accountId, CancellationToken cancellationToken, DateTime? startDate = null, DateTime? endDate = null);
}
