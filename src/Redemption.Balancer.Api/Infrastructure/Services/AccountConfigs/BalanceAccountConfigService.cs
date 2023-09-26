using Newtonsoft.Json;
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

            var balanceValue = PriceExtensions.Normalize(decimal.Parse(balance.Value!.Available) + decimal.Parse(balance.Value.Freeze), currency.NormalizationScale);

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

            var denormalPrice = PriceExtensions.Denormalize(differenceBalance, currency.NormalizationScale);

            _logger.LogWarning("UpdateBalance in insert config | symbol:{symbol}, StemeraldUserId:{StemeraldUserId}, denormalPrice:{denormalPrice}, detail:{businessDetail}", symbol, accountEntity.StemeraldUserId, denormalPrice, JsonConvert.SerializeObject(businessDetail));

            await _stexchangeService.UpdateBalance(trackingId, accountEntity.StemeraldUserId, symbol, "balancer", parameterTransaction.Id, denormalPrice, businessDetail, cancellationToken);

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

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var currency = await _currencyService.GetBySymbol(symbol, cancellationToken);

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

            var denormalPrice = PriceExtensions.Denormalize(differenceBalance, currency.NormalizationScale);

            _logger.LogWarning("UpdateBalance in update config | symbol:{symbol}, StemeraldUserId:{StemeraldUserId}, denormalPrice:{denormalPrice}, detail:{businessDetail}", symbol, accountEntity.StemeraldUserId, denormalPrice, JsonConvert.SerializeObject(businessDetail));

            await _stexchangeService.UpdateBalance(trackingId, accountEntity.StemeraldUserId, symbol, "balancer", parameterTransaction.Id, denormalPrice, businessDetail, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in BalanceUpdateAccountConfig. CatchBlock:");

            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task BalanceDeleteAccountConfig(int trackingId, AccountConfigEntity accountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var currency = await _currencyService.GetBySymbol(accountConfigEntity.Symbol!, cancellationToken);

            var differenceBalance = 0 - accountConfigEntity.Value;

            var transactions = await CreateDeleteAccountConfigTransactions(accountEntity.Id, accountConfigEntity.Symbol!, differenceBalance, cancellationToken);

            await _transactionService.Insert(transactions, cancellationToken);

            var parameterTransaction = transactions.First(t => t.FromAccountId == Account.MasterId || t.ToAccountId == Account.MasterId);

            var businessDetail = new BusinessDetailModel<TransactionBusinessModel>()
            {
                Name = "Delete Config",
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

            var denormalPrice = PriceExtensions.Denormalize(differenceBalance, currency.NormalizationScale);

            _logger.LogWarning("UpdateBalance in delete config | symbol:{symbol}, StemeraldUserId:{StemeraldUserId}, denormalPrice:{denormalPrice}, detail:{businessDetail}", accountConfigEntity.Symbol!, accountEntity.StemeraldUserId, denormalPrice, JsonConvert.SerializeObject(businessDetail));

            await _stexchangeService.UpdateBalance(trackingId, accountEntity.StemeraldUserId, accountConfigEntity.Symbol!, "balancer", parameterTransaction.Id, PriceExtensions.Denormalize(differenceBalance, currency.NormalizationScale), businessDetail, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in BalanceDeleteAccountConfig. CatchBlock:");

            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<IList<TransactionEntity>> CreateInsertAccountConfigTransactions(int accountId, string symbol, decimal amount, CancellationToken cancellationToken)
    {
        var currencyReferencePrice = await _priceService.CalculateReferencePrice(symbol, cancellationToken);

        var transactions = new List<TransactionEntity>
        {
            _transactionService.GetDebitTransaction(Account.MasterId, accountId, symbol, currencyReferencePrice, -amount, "insert config"),
            _transactionService.GetCreditTransaction(Account.B2BId, Account.MasterId, symbol, currencyReferencePrice, amount, "insert config")
        };

        return transactions;
    }

    private async Task<IList<TransactionEntity>> CreateUpdateAccountConfigTransactions(int accountId, string symbol, decimal differenceAmount, CancellationToken cancellationToken)
    {
        var currencyReferencePrice = await _priceService.CalculateReferencePrice(symbol, cancellationToken);

        var transactions = new List<TransactionEntity>();

        if (differenceAmount > 0)
        {
            transactions.Add(_transactionService.GetDebitTransaction(Account.MasterId, accountId, symbol, currencyReferencePrice, -differenceAmount, "update config"));
            transactions.Add(_transactionService.GetCreditTransaction(Account.B2BId, Account.MasterId, symbol, currencyReferencePrice, differenceAmount, "update config"));
        }
        else
        {
            transactions.Add(_transactionService.GetDebitTransaction(accountId, Account.MasterId, symbol, currencyReferencePrice, differenceAmount, "update config"));
            transactions.Add(_transactionService.GetCreditTransaction(Account.MasterId, Account.B2BId, symbol, currencyReferencePrice, -differenceAmount, "update config"));
        }


        return transactions;
    }

    private async Task<IList<TransactionEntity>> CreateDeleteAccountConfigTransactions(int accountId, string symbol, decimal amount, CancellationToken cancellationToken)
    {
        var currencyReferencePrice = await _priceService.CalculateReferencePrice(symbol, cancellationToken);

        var transactions = new List<TransactionEntity>
        {
            _transactionService.GetDebitTransaction(accountId,Account.MasterId, symbol, currencyReferencePrice, amount, "delete config"),
            _transactionService.GetCreditTransaction(Account.MasterId, Account.B2BId, symbol, currencyReferencePrice, -amount, "delete config")
        };

        return transactions;
    }

}
