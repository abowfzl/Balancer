using Microsoft.Extensions.Logging;
using Moq;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Kenes;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Workers;
using StexchangeClient.Models.Response.Assets;
using Xunit;

namespace Redemption.Balancer.Test.Workers;

public class BotBalancerTests
{
    private readonly BotBalancer _botBalancer;

    private readonly Mock<IPriceService> _priceService;
    private readonly Mock<IStexchangeService> _stexchangeService;
    private readonly Mock<ITransactionService> _transactionService;
    private readonly Mock<IAccountConfigService> _accountConfigService;
    private readonly Mock<IAccountService> _accountService;
    private readonly Mock<IWorkerService> _workerService;
    private readonly Mock<ICurrencyService> _currencyService;


    public BotBalancerTests()
    {
        Mock<ILogger<BotBalancer>> mockLogger = new();
        _priceService = new Mock<IPriceService>();
        _stexchangeService = new Mock<IStexchangeService>();
        _transactionService = new Mock<ITransactionService>();
        _accountConfigService = new Mock<IAccountConfigService>();
        _workerService = new Mock<IWorkerService>();
        _accountService = new Mock<IAccountService>();
        _currencyService = new Mock<ICurrencyService>();

        _botBalancer = new BotBalancer(mockLogger.Object,
            _priceService.Object,
            _stexchangeService.Object,
            _workerService.Object,
            _accountService.Object,
            _accountConfigService.Object,
            _transactionService.Object,
            _currencyService.Object);
    }

    [Fact]
    public async Task Should_Balance_Bots_Balances()
    {
        #region Arrange

        var cancellationToken = CancellationToken.None;
        var trackingId = Random.Shared.Next();

        var accountConfigs = new List<AccountConfigEntity>()
        {
            new AccountConfigEntity()
            {
                Id = 1,
                AccountId = 10,
                Symbol = "XRP",
                Value = 2000,
            },
            new AccountConfigEntity()
            {
                Id = 2,
                AccountId = 10,
                Symbol = "IRR",
                Value = 1000000,
            },
            new AccountConfigEntity()
            {
                Id = 3,
                AccountId = 11,
                Symbol = "BCH",
                Value = 0.01m,
            },
            new AccountConfigEntity()
            {
                Id = 4,
                AccountId = 11,
                Symbol = "IRR",
                Value = 5000000,
            }
        };

        var accounts = new List<AccountEntity>()
        {
            new AccountEntity()
            {
                Id =10,
                Name = "xrpirr.velkharj.1@exbito.com",
                StemeraldUserId = 1000,
            },
            new AccountEntity()
            {
                Id = 11,
                Name = "bchirr.tinytimcheh.2@exbito.com",
                StemeraldUserId = 1001,
            }
        };

        var balances = new Dictionary<int, Dictionary<string, BalanceQueryResponse>>()
        {
            { 1000, new Dictionary<string, BalanceQueryResponse>(){ { "XRP",new BalanceQueryResponse() { Available = "0.1500",Freeze= "0.0600"} },{ "IRR",new BalanceQueryResponse() { Available= "0.01000000m",Freeze = "0.50000000" } } } } ,
            { 1001, new Dictionary<string, BalanceQueryResponse>(){ { "BCH",new BalanceQueryResponse() { Available = "0.025",Freeze = "0" } },{ "IRR",new BalanceQueryResponse() { Available = "0.01000000", Freeze = "0" } } } }
        };


        _accountConfigService.Setup(ac => ac.GetAll(cancellationToken)).ReturnsAsync(accountConfigs);
        _accountService.Setup(ac => ac.GetAll(cancellationToken)).ReturnsAsync(accounts);

        foreach (var account in accounts)
        {
            _ = balances.TryGetValue(account.StemeraldUserId, out var accountBalances);
            var symbols = accountConfigs.Where(s => s.AccountId == account.Id).Select(s => s.Symbol!).ToArray();
            _stexchangeService.Setup(ss => ss.GetBalanceQueries(trackingId, account.StemeraldUserId, cancellationToken, symbols)).ReturnsAsync(accountBalances!);
        }

        _priceService.Setup(p => p.GetPrice("USDT", cancellationToken)).ReturnsAsync(new PriceResponse() { Ticker = 0.000038876168m });
        _priceService.Setup(p => p.GetPrice("IRR", cancellationToken)).ReturnsAsync(new PriceResponse() { Ticker = 0.000000000772691m });
        _priceService.Setup(p => p.GetPrice("BCH", cancellationToken)).ReturnsAsync(new PriceResponse() { Ticker = 0.007503100m });
        _priceService.Setup(p => p.GetPrice("XRP", cancellationToken)).ReturnsAsync(new PriceResponse() { Ticker = 0.000019550817m });

        _currencyService.Setup(c => c.GetBySymbol("IRR", cancellationToken)).ReturnsAsync(new CurrencyResponse() { NormalizationScale = -8 });
        _currencyService.Setup(c => c.GetBySymbol("BCH", cancellationToken)).ReturnsAsync(new CurrencyResponse() { NormalizationScale = -1 });
        _currencyService.Setup(c => c.GetBySymbol("XRP", cancellationToken)).ReturnsAsync(new CurrencyResponse() { NormalizationScale = -4 });

        var xrpDebitTransaction = new TransactionEntity()
        {
            Id = 2000,
            Symbol = "XRP",
            FromAccountId = Account.MasterId,
            ToAccountId = 10,
            Amount = 500,
            TotalValue = 251.44989855996m
        };
        var xrpCreditTransaction = new TransactionEntity()
        {
            Symbol = "XRP",
            FromAccountId = 10,
            ToAccountId = Account.UserId,
            Amount = 500,
            TotalValue = 251.44989855996m
        };

        var bchDebitTransaction = new TransactionEntity()
        {
            Id = 2001,
            Symbol = "BCH",
            FromAccountId = 11,
            ToAccountId = Account.MasterId,
            Amount = 0.24m,
            TotalValue = 46.31999738245807m
        };

        var bchCreditTransaction = new TransactionEntity()
        {
            Symbol = "BCH",
            FromAccountId = Account.UserId,
            ToAccountId = 11,
            Amount = 0.24m,
            TotalValue = 46.31999738245807m
        };

        var irrDebitTransaction = new TransactionEntity()
        {
            Id = 2002,
            Symbol = "IRR",
            FromAccountId = Account.MasterId,
            ToAccountId = 11,
            Amount = 4000000,
            TotalValue = 79.50279461700032m
        };

        var irrCreditTransaction = new TransactionEntity()
        {
            Symbol = "IRR",
            FromAccountId = 11,
            ToAccountId = Account.UserId,
            Amount = 4000000,
            TotalValue = 79.50279461700032m
        };

        _transactionService.Setup(p => p.GetCreditTransaction(Account.MasterId, 10, "XRP", 0.000019550817m, 0.000038876168m, 500)).Returns(xrpDebitTransaction);
        _transactionService.Setup(p => p.GetDebitTransaction(10, Account.UserId, "XRP", 0.000019550817m, 0.000038876168m, -500)).Returns(xrpCreditTransaction);

        _transactionService.Setup(p => p.GetCreditTransaction(Account.UserId, 11, "BCH", 0.007503100m, 0.000038876168m, 0.24m)).Returns(bchCreditTransaction);
        _transactionService.Setup(p => p.GetDebitTransaction(11, Account.MasterId, "BCH", 0.007503100m, 0.000038876168m, -0.24m)).Returns(bchDebitTransaction);

        _transactionService.Setup(p => p.GetCreditTransaction(Account.MasterId, 11, "IRR", 0.000000000772691m, 0.000038876168m, 4000000)).Returns(irrCreditTransaction);
        _transactionService.Setup(p => p.GetDebitTransaction(11, Account.UserId, "IRR", 0.000000000772691m, 0.000038876168m, -4000000)).Returns(irrDebitTransaction);

        #endregion

        #region Act

        await _botBalancer.BalanceAsync(trackingId, cancellationToken);

        #endregion

        #region Assert

        var irrTransactions = new List<TransactionEntity>() { irrDebitTransaction, irrCreditTransaction };
        var bchTransactions = new List<TransactionEntity>() { bchCreditTransaction, bchDebitTransaction };
        var xrpTransactions = new List<TransactionEntity>() { xrpCreditTransaction, xrpDebitTransaction };

        _transactionService.Verify(s => s.Insert(It.Is<IList<TransactionEntity>>(el => el.All(irrTransactions.Contains)), cancellationToken), Times.Exactly(1));
        _transactionService.Verify(s => s.Insert(It.Is<IList<TransactionEntity>>(el => el.All(bchTransactions.Contains)), cancellationToken), Times.Exactly(1));
        _transactionService.Verify(s => s.Insert(It.Is<IList<TransactionEntity>>(el => el.All(xrpTransactions.Contains)), cancellationToken), Times.Exactly(1));

        _stexchangeService.Verify(s => s.UpdateBalance(trackingId, 1000, "XRP", "autoBalance", 2000, 0.0500m, It.Is<TransactionEntity>(el => el == xrpDebitTransaction), cancellationToken), Times.Once());

        _stexchangeService.Verify(s => s.UpdateBalance(trackingId, 1001, "BCH", "autoBalance", 2001, -0.0240m, It.Is<TransactionEntity>(el => el == bchDebitTransaction), cancellationToken), Times.Once());

        _stexchangeService.Verify(s => s.UpdateBalance(trackingId, 1001, "IRR", "autoBalance", 2002, 0.0400000000000000m, It.Is<TransactionEntity>(el => el == irrDebitTransaction), cancellationToken), Times.Once());


        #endregion
    }

    [Fact]
    public async Task Should_Not_Balance_Balances_For_System_Accounts()
    {
        #region Arrange

        var cancellationToken = CancellationToken.None;
        var trackingId = Random.Shared.Next();

        var accounts = new List<AccountEntity>()
        {
            new AccountEntity()
            {
                Id =Account.UserId,
                Name = "User",
                StemeraldUserId = 0,
            },
            new AccountEntity()
            {
                Id = Account.MasterId,
                Name = "Master",
                StemeraldUserId = 0,
            },
            new AccountEntity()
            {
                Id = Account.B2BId,
                Name = "B2B",
                StemeraldUserId = 0,
            }
        };

        _accountService.Setup(ac => ac.GetAll(cancellationToken)).ReturnsAsync(accounts);

        #endregion

        #region Act

        await _botBalancer.BalanceAsync(trackingId, cancellationToken);

        #endregion

        #region Assert

        _transactionService.Verify(s => s.Insert(It.IsAny<IList<TransactionEntity>>(), cancellationToken), Times.Never());

        _stexchangeService.Verify(s => s.UpdateBalance(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<IList<TransactionEntity>>(), cancellationToken), Times.Never());

        #endregion
    }
}
