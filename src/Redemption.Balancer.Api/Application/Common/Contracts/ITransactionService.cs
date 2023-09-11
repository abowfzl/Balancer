using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface ITransactionService
{
    Task Insert(TransactionEntity transaction, CancellationToken cancellationToken);

    Task Insert(IList<TransactionEntity> transactions, CancellationToken cancellationToken);

    TransactionEntity GetDebitTransaction(int fromAccountId, int toAccountId, string symbol, decimal referencePrice, decimal differenceAmount, string source);

    TransactionEntity GetCreditTransaction(int fromAccountId, int toAccountId, string symbol, decimal referencePrice, decimal differenceAmount, string source);

    Task<IList<TransactionEntity>> GetAccountTransactions(int accountId, CancellationToken cancellationToken, DateTime? startDate = null, DateTime? endDate = null);
}
