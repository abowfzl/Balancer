using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Common.Extensions;

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

    public BotBalancer(ILogger<BotBalancer> logger,
        IPriceService priceService,
        IStexchangeService stexchangeService,
        IWorkerService workerService,
        IAccountService accountService,
        IAccountConfigService accountConfigService,
        ITransactionService transactionService,
        ICurrencyService currencyService) : base(workerService, logger)
    {
        _logger = logger;
        _priceService = priceService;
        _stexchangeService = stexchangeService;
        _accountService = accountService;
        _accountConfigService = accountConfigService;
        _transactionService = transactionService;
        _currencyService = currencyService;
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
                    try
                    {
                        await Task.Delay(500, cancellationToken);

                        _ = accountBalances.TryGetValue(accountConfig.Symbol!, out var symbolBalance);

                        var currency = await _currencyService.GetBySymbol(accountConfig.Symbol!, cancellationToken);

                        var differenceBalance = PriceExtensions.Normalize(decimal.Parse(symbolBalance!.Available), currency.NormalizationScale) - accountConfig.Value;

                        if (differenceBalance != 0)
                        {
                            var transactions = await CreateAccountTransactions(accountConfig.AccountId, accountConfig.Symbol!, differenceBalance, cancellationToken);

                            await _transactionService.Insert(transactions, cancellationToken);

                            var parameterTransaction = transactions.First(t => t.FromAccountId == Account.MasterId || t.ToAccountId == Account.MasterId);

                            await _stexchangeService.UpdateBalance(trackingId, account.StemeraldUserId, accountConfig.Symbol!, "autoBalance", parameterTransaction.Id, PriceExtensions.Denormalize(-differenceBalance, currency.NormalizationScale), parameterTransaction, cancellationToken);
                        }
                        else
                            _logger.LogInformation("Account:{accountName} balance for symbol:{accountConfigSymbol} isn't changed", account.Name, accountConfig.Symbol);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Account:{accountName} balance for symbol:{accountConfigSymbol} failed. CatchBlock:", account.Name, accountConfig.Symbol);
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

    private async Task<IList<TransactionEntity>> CreateAccountTransactions(int accountId, string symbol, decimal differenceAmount, CancellationToken cancellationToken)
    {
        var symbolPrice = await _priceService.GetPrice(symbol, cancellationToken);
        var usdtPrice = await _priceService.GetPrice("USDT", cancellationToken);

        var transactions = new List<TransactionEntity>();

        if (differenceAmount > 0)
        {
            transactions.Add(_transactionService.GetDebitTransaction(accountId, Account.MasterId, symbol, symbolPrice.Ticker, usdtPrice.Ticker, -differenceAmount));
            transactions.Add(_transactionService.GetCreditTransaction(Account.UserId, accountId, symbol, symbolPrice.Ticker, usdtPrice.Ticker, differenceAmount));
        }
        else
        {
            transactions.Add(_transactionService.GetDebitTransaction(accountId, Account.UserId, symbol, symbolPrice.Ticker, usdtPrice.Ticker, differenceAmount));
            transactions.Add(_transactionService.GetCreditTransaction(Account.MasterId, accountId, symbol, symbolPrice.Ticker, usdtPrice.Ticker, -differenceAmount));
        }

        return transactions;
    }
}
