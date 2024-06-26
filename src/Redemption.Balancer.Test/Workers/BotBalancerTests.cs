﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Stemeralds;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Persistence;
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

        var dbOption = new DbContextOptionsBuilder<BalancerDbContext>().Options;
        var mockContext = new Mock<BalancerDbContext>(dbOption);
        var databaseMock = new Mock<DatabaseFacade>(mockContext.Object);
        var transactionMock = new Mock<IDbContextTransaction>();

        databaseMock.Setup(s => s.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(transactionMock.Object);
        mockContext.Setup(s => s.Database).Returns(databaseMock.Object);

        _botBalancer = new BotBalancer(mockLogger.Object,
            _priceService.Object,
            _stexchangeService.Object,
            _workerService.Object,
            _accountService.Object,
            _accountConfigService.Object,
            _transactionService.Object,
            _currencyService.Object,
            mockContext.Object);
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
            { 1000, new Dictionary<string, BalanceQueryResponse>(){ { "XRP",new BalanceQueryResponse() { Available = "0.1500",Freeze= "0.0600"} },{ "IRR",new BalanceQueryResponse() { Available= "0.01000000",Freeze = "0" } } } } ,
            { 1001, new Dictionary<string, BalanceQueryResponse>(){ { "BCH",new BalanceQueryResponse() { Available = "0.025",Freeze = "0.0005" } },{ "IRR",new BalanceQueryResponse() { Available = "0.01000000", Freeze = "0" } } } }
        };


        _accountConfigService.Setup(ac => ac.GetAll(cancellationToken)).ReturnsAsync(accountConfigs);
        _accountService.Setup(ac => ac.GetAll(cancellationToken)).ReturnsAsync(accounts);

        foreach (var account in accounts)
        {
            _ = balances.TryGetValue(account.StemeraldUserId, out var accountBalances);
            var symbols = accountConfigs.Where(s => s.AccountId == account.Id).Select(s => s.Symbol!).ToArray();
            _stexchangeService.Setup(ss => ss.GetBalanceQueries(trackingId, account.StemeraldUserId, cancellationToken, symbols)).ReturnsAsync(accountBalances!);
        }

        _priceService.Setup(p => p.CalculateReferencePrice("IRR", cancellationToken)).ReturnsAsync(50312.69679600254m);
        _priceService.Setup(p => p.CalculateReferencePrice("BCH", cancellationToken)).ReturnsAsync(187.54m);
        _priceService.Setup(p => p.CalculateReferencePrice("XRP", cancellationToken)).ReturnsAsync(0.497m);

        _currencyService.Setup(c => c.GetBySymbol("IRR", cancellationToken)).ReturnsAsync(new CurrencyResponse() { NormalizationScale = -8 });
        _currencyService.Setup(c => c.GetBySymbol("BCH", cancellationToken)).ReturnsAsync(new CurrencyResponse() { NormalizationScale = -1 });
        _currencyService.Setup(c => c.GetBySymbol("XRP", cancellationToken)).ReturnsAsync(new CurrencyResponse() { NormalizationScale = -4 });

        var xrpDebitTransaction = new TransactionEntity()
        {
            Id = 2000,
            Symbol = "XRP",
            FromAccountId = 10,
            ToAccountId = Account.MasterId,
            Amount = 100,
            TotalValue = 49.7m
        };
        var xrpCreditTransaction = new TransactionEntity()
        {
            Symbol = "XRP",
            FromAccountId = Account.UserId,
            ToAccountId = 10,
            Amount = 100,
            TotalValue = 49.7m
        };

        var bchDebitTransaction = new TransactionEntity()
        {
            Id = 2001,
            Symbol = "BCH",
            FromAccountId = 11,
            ToAccountId = Account.MasterId,
            Amount = 0.245m,
            TotalValue = 45.9473m
        };

        var bchCreditTransaction = new TransactionEntity()
        {
            Symbol = "BCH",
            FromAccountId = Account.UserId,
            ToAccountId = 11,
            Amount = 0.245m,
            TotalValue = 45.9473m
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

        _transactionService.Setup(p => p.GetDebitTransaction(10, Account.MasterId, "XRP", 0.497m, -100, "balancer")).Returns(xrpDebitTransaction);
        _transactionService.Setup(p => p.GetCreditTransaction(Account.UserId, 10, "XRP", 0.497m, 100, "balancer")).Returns(xrpCreditTransaction);

        _transactionService.Setup(p => p.GetCreditTransaction(Account.UserId, 11, "BCH", 187.54m, 0.245m, "balancer")).Returns(bchCreditTransaction);
        _transactionService.Setup(p => p.GetDebitTransaction(11, Account.MasterId, "BCH", 187.54m, -0.245m, "balancer")).Returns(bchDebitTransaction);

        _transactionService.Setup(p => p.GetCreditTransaction(Account.MasterId, 11, "IRR", 50312.69679600254m, 4000000, "balancer")).Returns(irrCreditTransaction);
        _transactionService.Setup(p => p.GetDebitTransaction(11, Account.UserId, "IRR", 50312.69679600254m, -4000000, "balancer")).Returns(irrDebitTransaction);

        #endregion

        #region Act

        await _botBalancer.BalanceAsync(trackingId, cancellationToken);

        #endregion

        #region Assert

        var irrTransactions = new List<TransactionEntity>() { irrDebitTransaction, irrCreditTransaction };
        var bchTransactions = new List<TransactionEntity>() { bchCreditTransaction, bchDebitTransaction };
        var xrpTransactions = new List<TransactionEntity>() { xrpCreditTransaction, xrpDebitTransaction };

        _transactionService.Verify(s => s.Add(It.Is<IList<TransactionEntity>>(el => el.All(irrTransactions.Contains)), cancellationToken), Times.Exactly(1));
        _transactionService.Verify(s => s.Add(It.Is<IList<TransactionEntity>>(el => el.All(bchTransactions.Contains)), cancellationToken), Times.Exactly(1));
        _transactionService.Verify(s => s.Add(It.Is<IList<TransactionEntity>>(el => el.All(xrpTransactions.Contains)), cancellationToken), Times.Exactly(1));

        _stexchangeService.Verify(s => s.UpdateBalance(trackingId, 1000, "XRP", "balancer", 2000, -0.0100m, It.IsAny<BusinessDetailModel<TransactionBusinessModel>>(), cancellationToken), Times.Once());

        _stexchangeService.Verify(s => s.UpdateBalance(trackingId, 1001, "BCH", "balancer", 2001, -0.0245m, It.IsAny<BusinessDetailModel<TransactionBusinessModel>>(), cancellationToken), Times.Once());

        _stexchangeService.Verify(s => s.UpdateBalance(trackingId, 1001, "IRR", "balancer", 2002, 0.0400000000000000m, It.IsAny<BusinessDetailModel<TransactionBusinessModel>>(), cancellationToken), Times.Once());


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
                Id = Account.B2bId,
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

        _transactionService.Verify(s => s.Add(It.IsAny<IList<TransactionEntity>>(), cancellationToken), Times.Never());

        _stexchangeService.Verify(s => s.UpdateBalance(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<BusinessDetailModel<TransactionBusinessModel>>(), cancellationToken), Times.Never());

        #endregion
    }
}
