using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Persistence;

namespace Redemption.Balancer.Api.Infrastructure.Services.Transactions;

public class TransactionService : ITransactionService
{
    private readonly BalancerDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;


    public TransactionService(BalancerDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Insert(TransactionEntity transaction, CancellationToken cancellationToken)
    {
        _dbContext.Transactions.Add(transaction);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Insert(IList<TransactionEntity> transactions, CancellationToken cancellationToken)
    {
        _dbContext.Transactions.AddRange(transactions);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public TransactionEntity GetDebitTransaction(int fromAccountId, int toAccountId, string symbol, decimal referencePrice, decimal differenceAmount, string source)
    {
        if (differenceAmount < 0 is false)
        {
            throw new ArgumentOutOfRangeException(nameof(differenceAmount));
        }

        return new TransactionEntity()
        {
            Amount = Math.Abs(differenceAmount),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 0,
            Symbol = symbol,
            TotalValue = referencePrice * Math.Abs(differenceAmount),
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Source = source
        };
    }

    public TransactionEntity GetCreditTransaction(int fromAccountId, int toAccountId, string symbol, decimal referencePrice, decimal differenceAmount, string source)
    {
        if (differenceAmount > 0 is false)
        {
            throw new ArgumentOutOfRangeException(nameof(differenceAmount));
        }

        return new TransactionEntity()
        {
            Amount = Math.Abs(differenceAmount),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 0,
            Symbol = symbol,
            TotalValue = referencePrice * Math.Abs(differenceAmount),
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Source = source
        };
    }

    public async ValueTask<decimal> CalculateAccountIRRTransactions(int accountId, CancellationToken cancellationToken, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbContext.Transactions.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(s => s.CreatedAt >= _dateTimeProvider.SetKindUtc(startDate.Value));

        if (endDate.HasValue)
            query = query.Where(s => s.CreatedAt <= _dateTimeProvider.SetKindUtc(endDate.Value));

        var credit = await query.Where(q => q.FromAccountId == accountId && q.Symbol == "IRR").SumAsync(q => q.Amount, cancellationToken);
        var debit = await query.Where(q => q.ToAccountId == accountId && q.Symbol == "IRR").SumAsync(q => q.Amount, cancellationToken);

        var total = debit - credit;

        return total;
    }

    public async ValueTask<decimal> CalculateAccountUSDTTransactions(int accountId, CancellationToken cancellationToken, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbContext.Transactions.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(s => s.CreatedAt >= _dateTimeProvider.SetKindUtc(startDate.Value));

        if (endDate.HasValue)
            query = query.Where(s => s.CreatedAt <= _dateTimeProvider.SetKindUtc(endDate.Value));

        var credit = await query.Where(q => q.FromAccountId == accountId && q.Symbol != "IRR").SumAsync(q => q.TotalValue, cancellationToken);
        var debit = await query.Where(q => q.ToAccountId == accountId && q.Symbol != "IRR").SumAsync(q => q.TotalValue, cancellationToken);

        var total = debit - credit;

        return total;
    }
}
