using Newtonsoft.Json;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Common.Extensions;
using Redemption.Balancer.Api.Infrastructure.Persistence;

namespace Redemption.Balancer.Api.Infrastructure.Workers;

public class BotBalancer : BaseBalancer
{
    private readonly ILogger<BotBalancer> _logger;
    private readonly IPriceService _priceService;
    private readonly IStexchangeService _stexchangeService;
    private readonly IAccountService _accountService;
    private readonly IAccountConfigService _accountConfigService;
    private readonly ITransactionService _transactionService;
    private readonly ICurrencyService _currencyService;
    private readonly BalancerDbContext _dbContext;

    public BotBalancer(ILogger<BotBalancer> logger,
        IPriceService priceService,
        IStexchangeService stexchangeService,
        IWorkerService workerService,
        IAccountService accountService,
        IAccountConfigService accountConfigService,
        ITransactionService transactionService,
        ICurrencyService currencyService,
        BalancerDbContext dbContext) : base(workerService, logger)
    {
        _logger = logger;
        _priceService = priceService;
        _stexchangeService = stexchangeService;
        _accountService = accountService;
        _accountConfigService = accountConfigService;
        _transactionService = transactionService;
        _currencyService = currencyService;
        _dbContext = dbContext;
    }

    public override async Task BalanceAsync(int trackingId, CancellationToken cancellationToken)
    {
        var allAccounts = await _accountService.GetAll(cancellationToken);

        var allAccountConfigs = await _accountConfigService.GetAll(cancellationToken);

        foreach (var account in allAccounts)
        {
            try
            {
                await Task.Delay(500, cancellationToken);

                if (new int[] { Account.UserId, Account.B2BId, Account.MasterId }.Contains(account.Id))
                    continue;

                _logger.LogInformation("Account:{accountName} balance process began", account.Name);

                var accountConfigs = allAccountConfigs.Where(ac => ac.AccountId == account.Id).ToList();

                var accountBalances = await _stexchangeService.GetBalanceQueries(trackingId, account.StemeraldUserId, cancellationToken, accountConfigs.Select(s => s.Symbol).ToArray()!);

                foreach (var accountConfig in accountConfigs)
                {
                    using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        await Task.Delay(500, cancellationToken);

                        _ = accountBalances.TryGetValue(accountConfig.Symbol!, out var symbolBalance);

                        var deNormalBalance = decimal.Parse(symbolBalance!.Available) + decimal.Parse(symbolBalance.Freeze);

                        _logger.LogInformation("Account:{accountName}, Symbol:{accountName} | deNormalBalance:{deNormalBalance}", account.Name, accountConfig.Symbol, deNormalBalance);

                        var currency = await _currencyService.GetBySymbol(accountConfig.Symbol!, cancellationToken);

                        var balanceValue = PriceExtensions.Normalize(deNormalBalance, currency.NormalizationScale);

                        _logger.LogInformation("Account:{accountName}, Symbol:{accountName} | configValue:{configValue}, balanceValue:{balanceValue}", account.Name, accountConfig.Symbol, accountConfig.Value, balanceValue);

                        var differenceBalance = balanceValue - accountConfig.Value;

                        if (differenceBalance != 0)
                        {
                            _logger.LogInformation("AccountId:{accountId}, Symbol:{accountName} | differenceBalance:{differenceBalance}", accountConfig.AccountId, accountConfig.Symbol, differenceBalance);

                            var transactions = await CreateAccountTransactions(accountConfig.AccountId, accountConfig.Symbol!, differenceBalance, "balancer", cancellationToken);

                            await _transactionService.Insert(transactions, cancellationToken);

                            var parameterTransaction = transactions.First(t => t.FromAccountId == Account.MasterId || t.ToAccountId == Account.MasterId);

                            var businessDetail = new BusinessDetailModel<TransactionBusinessModel>()
                            {
                                Name = "balancer",
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

                            _logger.LogInformation("AccountId:{accountId}, Symbol:{accountName} | differenceDeNormalBalance:{denormalPrice}", accountConfig.AccountId, accountConfig.Symbol, denormalPrice);

                            _logger.LogWarning("UpdateBalance in Balancer | symbol:{symbol}, StemeraldUserId:{StemeraldUserId}, denormalPrice:{denormalPrice}, detail:{businessDetail}", accountConfig.Symbol!, account.StemeraldUserId, denormalPrice, JsonConvert.SerializeObject(businessDetail));

                            //await _stexchangeService.UpdateBalance(trackingId, account.StemeraldUserId, accountConfig.Symbol!, "balancer", parameterTransaction.Id, denormalPrice, businessDetail, cancellationToken);
                        }
                        else
                            _logger.LogInformation("Account:{accountName} balance for symbol:{accountConfigSymbol} isn't changed", account.Name, accountConfig.Symbol);

                        await transaction.CommitAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Account:{accountName} balance for symbol:{accountConfigSymbol} failed. CatchBlock:", account.Name, accountConfig.Symbol);

                        await transaction.RollbackAsync(cancellationToken);
                    }
                }

                _logger.LogInformation("Account:{accountName} balance balanced", account.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Account:{accountName} balance failed. CatchBlock:", account.Name);
            }
        }
    }

    private async Task<IList<TransactionEntity>> CreateAccountTransactions(int accountId, string symbol, decimal differenceAmount, string source, CancellationToken cancellationToken)
    {
        var currencyReferencePrice = await _priceService.CalculateReferencePrice(symbol, cancellationToken);

        var transactions = new List<TransactionEntity>();

        if (differenceAmount > 0)
        {
            transactions.Add(_transactionService.GetDebitTransaction(accountId, Account.MasterId, symbol, currencyReferencePrice, -differenceAmount, source));
            transactions.Add(_transactionService.GetCreditTransaction(Account.UserId, accountId, symbol, currencyReferencePrice, differenceAmount, source));
        }
        else
        {
            transactions.Add(_transactionService.GetDebitTransaction(accountId, Account.UserId, symbol, currencyReferencePrice, differenceAmount, source));
            transactions.Add(_transactionService.GetCreditTransaction(Account.MasterId, accountId, symbol, currencyReferencePrice, -differenceAmount, source));
        }

        return transactions;
    }
}
