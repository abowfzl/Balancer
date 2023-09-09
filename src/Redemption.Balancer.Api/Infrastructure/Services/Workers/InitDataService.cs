using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Persistence;

namespace Redemption.Balancer.Api.Infrastructure.Services.Workers;

public class InitDataService : IInitDataService
{
    private readonly BalancerDbContext _dbContext;
    private readonly IStexchangeService _stexchangeService;
    private readonly ITransactionService _transactionService;
    private readonly IPriceService _priceService;
    private readonly IAccountService _accountService;
    private readonly ILogger<InitDataService> _logger;
    private readonly IAccountConfigService _accountConfigService;

    public InitDataService(BalancerDbContext dbContext,
        IStexchangeService stexchangeService,
        ITransactionService transactionService,
        IPriceService priceService,
        IAccountService accountService,
        ILogger<InitDataService> logger,
        IAccountConfigService accountConfigService)
    {
        _dbContext = dbContext;
        _stexchangeService = stexchangeService;
        _transactionService = transactionService;
        _priceService = priceService;
        _accountService = accountService;
        _logger = logger;
        _accountConfigService = accountConfigService;
    }

    public async Task InitData(InitDataInputDto inputDto, CancellationToken cancellationToken)
    {
        #region default Accounts

        var defaultAccounts = new List<AccountEntity>()
        {
            new AccountEntity()
            {
                Id = Account.MasterId,
                Name = "Master",
                StemeraldUserId = 0,
                CreatedBy = 0,
            },
            new AccountEntity()
            {
                Id= Account.UserId,
                Name = "User",
                StemeraldUserId = 0,
                CreatedBy = 0,
            },
            new AccountEntity()
            {
                Id = Account.B2BId,
                Name = "B2B",
                StemeraldUserId = 0,
                CreatedBy = 0,
            }
        };

        foreach (var defaultAccount in defaultAccounts)
        {
            if (!await _dbContext.Accounts.AnyAsync(a => a.Id == defaultAccount.Id, cancellationToken))
            {
                await _accountService.Insert(defaultAccount, cancellationToken);
            }
        }
        #endregion

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _logger.LogInformation("InitData Begin");

            foreach (var data in inputDto.Data)
            {
                await Task.Delay(500, cancellationToken);

                var trackingId = Random.Shared.Next();

                var account = new AccountEntity()
                {
                    Name = data.Name,
                    StemeraldUserId = data.StemeraldUserId,
                    CreatedBy = 0
                };

                await _accountService.Insert(account, cancellationToken);

                foreach (var config in data.Configs)
                {
                    var accountConfig = new AccountConfigEntity()
                    {
                        Symbol = config.Symbol,
                        Value = config.Value,
                        CreatedBy = 0
                    };

                    await _accountConfigService.Insert(accountConfig, cancellationToken);
                }

                var accountBalances = await _stexchangeService.GetBalanceQueries(trackingId, data.StemeraldUserId, cancellationToken, data.Configs.Select(s => s.Symbol).ToArray());

                foreach (var balance in accountBalances)
                {
                    var symbol = balance.Key;

                    var currentBalance = decimal.Parse(balance.Value.Available);

                    var transactions = await CreateAccountTransactions(account.Id, symbol, currentBalance, cancellationToken);

                    await _transactionService.Insert(transactions, cancellationToken);
                }
            }
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("InitData finished successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            _logger.LogError(ex, "InitData failed");

            throw;
        }
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
