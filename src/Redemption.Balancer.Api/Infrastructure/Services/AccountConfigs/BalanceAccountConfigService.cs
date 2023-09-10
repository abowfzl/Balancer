using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Common.Extensions;

namespace Redemption.Balancer.Api.Infrastructure.Services.AccountConfigs;

public class BalanceAccountConfigService : IBalanceAccountConfigService
{
    private readonly IPriceService _priceService;
    private readonly ICurrencyService _currencyService;
    private readonly IStexchangeService _stexchangeService;
    private readonly ITransactionService _transactionService;

    public BalanceAccountConfigService(IPriceService priceService,
        ICurrencyService currencyService,
        IStexchangeService stexchangeService,
        ITransactionService transactionService)
    {
        _priceService = priceService;
        _currencyService = currencyService;
        _stexchangeService = stexchangeService;
        _transactionService = transactionService;
    }

    public async Task BalanceInsertAccountConfig(int trackingId, AccountConfigEntity newAccountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken)
    {
        var accountBalances = await _stexchangeService.GetBalanceQueries(trackingId, accountEntity.StemeraldUserId, cancellationToken, newAccountConfigEntity.Symbol!);
        var balance = accountBalances.First();

        var symbol = balance.Key;

        var currency = await _currencyService.GetBySymbol(symbol, cancellationToken);

        var balanceValue = PriceExtensions.Normalize(decimal.Parse(balance.Value!.Available), currency.NormalizationScale);

        var differenceBalance = balanceValue - newAccountConfigEntity.Value;

        var transactions = await CreateAccountTransactions(accountEntity.Id, symbol, newAccountConfigEntity.Value, cancellationToken);

        await _transactionService.Insert(transactions, cancellationToken);

        var parameterTransaction = transactions.First(t => t.FromAccountId == Account.MasterId || t.ToAccountId == Account.MasterId);

        //todo: updateConfigBalancer?
        await _stexchangeService.UpdateBalance(trackingId, accountEntity.StemeraldUserId, symbol, "updateConfigBalancer", parameterTransaction.Id, differenceBalance, parameterTransaction, cancellationToken);
    }

    public async Task BalanceUpdateAccountConfig(AccountConfigEntity oldAccountConfigEntity, AccountConfigEntity newAccountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken)
    {
        //todo: do what?
        if (oldAccountConfigEntity.Symbol != newAccountConfigEntity.Symbol)
            throw new Exception("symbol could not to be changed");

        var symbol = oldAccountConfigEntity.Symbol!;

        var differenceBalance = newAccountConfigEntity.Value - oldAccountConfigEntity.Value;

        var transactions = await CreateAccountTransactions(accountEntity.Id, symbol, differenceBalance, cancellationToken);

        await _transactionService.Insert(transactions, cancellationToken);
    }

    private async Task<IList<TransactionEntity>> CreateAccountTransactions(int accountId, string symbol, decimal amount, CancellationToken cancellationToken)
    {
        var symbolPrice = await _priceService.GetPrice(symbol, cancellationToken);
        var usdtPrice = await _priceService.GetPrice("USDT", cancellationToken);

        var transactions = new List<TransactionEntity>
        {
            _transactionService.GetDebitTransaction(Account.MasterId, accountId, symbol, symbolPrice.Ticker, usdtPrice.Ticker, -amount),
            _transactionService.GetCreditTransaction(Account.B2BId, Account.MasterId, symbol, symbolPrice.Ticker, usdtPrice.Ticker, amount)
        };

        return transactions;
    }
}
