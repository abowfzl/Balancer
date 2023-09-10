using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Common.Extensions;
using Redemption.Balancer.Api.Infrastructure.Persistence;

namespace Redemption.Balancer.Api.Infrastructure.Services.AccountConfigs;

public class BalanceAccountConfigService : IBalanceAccountConfigService
{
    private readonly IPriceService _priceService;
    private readonly ICurrencyService _currencyService;
    private readonly IStexchangeService _stexchangeService;
    private readonly ITransactionService _transactionService;
    private readonly BalancerDbContext _dbContext;
    private readonly ILogger<BalanceAccountConfigService> _logger;

    public BalanceAccountConfigService(IPriceService priceService,
        ICurrencyService currencyService,
        IStexchangeService stexchangeService,
        ITransactionService transactionService,
        BalancerDbContext dbContext,
        ILogger<BalanceAccountConfigService> logger)
    {
        _priceService = priceService;
        _currencyService = currencyService;
        _stexchangeService = stexchangeService;
        _transactionService = transactionService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task BalanceInsertAccountConfig(int trackingId, AccountConfigEntity newAccountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var accountBalances = await _stexchangeService.GetBalanceQueries(trackingId, accountEntity.StemeraldUserId, cancellationToken, newAccountConfigEntity.Symbol!);
            var balance = accountBalances.First();

            var symbol = balance.Key;

            var currency = await _currencyService.GetBySymbol(symbol, cancellationToken);

            var balanceValue = PriceExtensions.Normalize(decimal.Parse(balance.Value!.Available), currency.NormalizationScale);

            var differenceBalance = newAccountConfigEntity.Value - balanceValue;

            var transactions = await CreateInsertAccountConfigTransactions(accountEntity.Id, symbol, newAccountConfigEntity.Value, cancellationToken);

            await _transactionService.Insert(transactions, cancellationToken);

            var parameterTransaction = transactions.First(t => t.FromAccountId == Account.MasterId || t.ToAccountId == Account.MasterId);

            var businessDetail = new BusinessDetailModel<TransactionBusinessModel>() 
            { 
                Name = "Insert Config",
                Detail = new TransactionBusinessModel() 
                { 
                    Id = parameterTransaction.Id,
                    FromAccountId = parameterTransaction.FromAccountId,
                    ToAccountId = parameterTransaction.ToAccountId,
                    Amount = parameterTransaction.Amount,
                    Symbol = parameterTransaction.Symbol,
                    TotalValue = parameterTransaction.TotalValue
                } 
            };

            await _stexchangeService.UpdateBalance(trackingId, accountEntity.StemeraldUserId, symbol, "balancer", parameterTransaction.Id, differenceBalance, businessDetail, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in BalanceInsertAccountConfig. CatchBlock:");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task BalanceUpdateAccountConfig(int trackingId, AccountConfigEntity oldAccountConfigEntity, AccountConfigEntity newAccountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken)
    {
        if (oldAccountConfigEntity.Symbol != newAccountConfigEntity.Symbol)
            throw new BadRequestException("symbol could not to be changed");

        var symbol = oldAccountConfigEntity.Symbol!;

        //check symbol
        await _currencyService.GetBySymbol(symbol, cancellationToken);

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var differenceBalance = newAccountConfigEntity.Value - oldAccountConfigEntity.Value;

            var transactions = await CreateUpdateAccountConfigTransactions(accountEntity.Id, symbol, differenceBalance, cancellationToken);

            await _transactionService.Insert(transactions, cancellationToken);

            var parameterTransaction = transactions.First(t => t.FromAccountId == Account.MasterId || t.ToAccountId == Account.MasterId);

            var businessDetail = new BusinessDetailModel<TransactionBusinessModel>() 
            { 
                Name = "Update Config",
                Detail = new TransactionBusinessModel()
                {
                    Id = parameterTransaction.Id,
                    FromAccountId = parameterTransaction.FromAccountId,
                    ToAccountId = parameterTransaction.ToAccountId,
                    Amount = parameterTransaction.Amount,
                    Symbol = parameterTransaction.Symbol,
                    TotalValue = parameterTransaction.TotalValue
                }
            };

            await _stexchangeService.UpdateBalance(trackingId, accountEntity.StemeraldUserId, symbol, "balancer", parameterTransaction.Id, differenceBalance, businessDetail, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in BalanceUpdateAccountConfig. CatchBlock:");

            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<IList<TransactionEntity>> CreateInsertAccountConfigTransactions(int accountId, string symbol, decimal amount, CancellationToken cancellationToken)
    {
        var symbolPrice = await _priceService.GetStemeraldPrice(symbol, cancellationToken);
        var usdtPrice = await _priceService.GetStemeraldPrice("USDT", cancellationToken);

        var transactions = new List<TransactionEntity>
        {
            _transactionService.GetDebitTransaction(Account.MasterId, accountId, symbol, symbolPrice.DecimalTicker, usdtPrice.DecimalTicker, -amount),
            _transactionService.GetCreditTransaction(Account.B2BId, Account.MasterId, symbol, symbolPrice.DecimalTicker, usdtPrice.DecimalTicker, amount)
        };

        return transactions;
    }

    private async Task<IList<TransactionEntity>> CreateUpdateAccountConfigTransactions(int accountId, string symbol, decimal differenceAmount, CancellationToken cancellationToken)
    {
        var symbolPrice = await _priceService.GetStemeraldPrice(symbol, cancellationToken);
        var usdtPrice = await _priceService.GetStemeraldPrice("USDT", cancellationToken);

        var transactions = new List<TransactionEntity>();

        if (differenceAmount > 0)
        {
            transactions.Add(_transactionService.GetDebitTransaction(Account.MasterId, accountId, symbol, symbolPrice.DecimalTicker, usdtPrice.DecimalTicker, -differenceAmount));
            transactions.Add(_transactionService.GetCreditTransaction(Account.B2BId, Account.MasterId, symbol, symbolPrice.DecimalTicker, usdtPrice.DecimalTicker, differenceAmount));
        }
        else
        {
            transactions.Add(_transactionService.GetDebitTransaction(accountId, Account.MasterId, symbol, symbolPrice.DecimalTicker, usdtPrice.DecimalTicker, differenceAmount));
            transactions.Add(_transactionService.GetCreditTransaction(Account.MasterId, Account.B2BId, symbol, symbolPrice.DecimalTicker, usdtPrice.DecimalTicker, -differenceAmount));
        }


        return transactions;
    }
}
