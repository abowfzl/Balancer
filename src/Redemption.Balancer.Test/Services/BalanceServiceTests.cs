using FluentAssertions;
using Moq;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Balances;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Services.Balances;
using Xunit;

namespace Redemption.Balancer.Test.Services;

public class BalanceServiceTests
{
    private readonly BalanceService _balanceService;

    private readonly Mock<ITransactionService> _transactionService;

    public BalanceServiceTests()
    {
        _transactionService = new Mock<ITransactionService>();

        _balanceService = new BalanceService(_transactionService.Object);
    }

    [Fact]
    public async Task Should_Get_Balance_Status()
    {
        var request = new BalanceStatusInputDto()
        {
            B2BIRRRate = 40000
        };

        var accountId = Account.MasterId;

        var cancellationToken = CancellationToken.None;

        var transactions = new List<TransactionEntity>()
        {
            new TransactionEntity()
            {
               FromAccountId = accountId,
                ToAccountId= 1001,
                Symbol = "USDT",
                Amount = 1000,
                TotalValue = 1000,
                CreatedAt = new DateTime(2023,08,28)

            },
            new TransactionEntity()
            {
                FromAccountId = 1001,
                ToAccountId=accountId,
                Symbol = "IRR",
                Amount = 50000000,
                TotalValue = 1000,
                CreatedAt = new DateTime(2023,08,28)
            },
            new TransactionEntity()
            {
                FromAccountId = accountId,
                ToAccountId = 1002,
                Symbol = "USDT",
                Amount = 500,
                TotalValue = 500,
                CreatedAt = new DateTime(2023,08,28)
            },
            new TransactionEntity()
            {
                FromAccountId = 1002,
                ToAccountId=accountId,
                Symbol = "IRR",
                Amount = 15000000,
                TotalValue = 300,
                CreatedAt = new DateTime(2023,08,28)
            },
            new TransactionEntity()
            {
                FromAccountId = accountId,
                ToAccountId= 1003,
                Symbol = "USDT",
                Amount = 100,
                TotalValue = 100,
                CreatedAt = new DateTime(2023,08,28)
            },
            new TransactionEntity()
            {
                FromAccountId = accountId,
                ToAccountId= 1003,
                Symbol = "IRR",
                Amount = 15000000,
                TotalValue = 300,
                CreatedAt = new DateTime(2023,08,28)
            },
            new TransactionEntity()
            {
                FromAccountId = 1004,
                ToAccountId= accountId,
                Symbol = "DOGE",
                Amount = 1000,
                TotalValue = 60m,
                CreatedAt = new DateTime(2023,08,28)
            },
             new TransactionEntity()
            {
                FromAccountId = accountId,
                ToAccountId= 1004,
                Symbol = "IRR",
                Amount = 3000000,
                TotalValue = 60,
                CreatedAt = new DateTime(2023,08,28)
            },
            new TransactionEntity()
            {
                FromAccountId = accountId,
                ToAccountId = 1001,
                Symbol = "IRR",
                Amount = 20000000,
                TotalValue = 333.333m,
                CreatedAt = new DateTime(2023,08,29)
            },
            new TransactionEntity()
            {
                FromAccountId = 1001,
                ToAccountId = accountId,
                Symbol = "USDT",
                Amount = 400,
                TotalValue = 400,
                CreatedAt = new DateTime(2023,08,29)
            },
            new TransactionEntity()
            {
                FromAccountId = 1002,
                ToAccountId = accountId,
                Symbol = "IRR",
                Amount = 30000000,
                TotalValue = 600,
                CreatedAt = new DateTime(2023,08,29)
            },
            new TransactionEntity()
            {
                FromAccountId = 1002,
                ToAccountId = accountId,
                Symbol = "USDT",
                Amount = 100,
                TotalValue = 100,
                CreatedAt = new DateTime(2023,08,29)
            },
            new TransactionEntity()
            {
                FromAccountId = accountId,
                ToAccountId = 1003,
                Symbol = "IRR",
                Amount = 20000000,
                TotalValue = 500,
                CreatedAt = new DateTime(2023,08,29)
            },
            new TransactionEntity()
            {
                FromAccountId = accountId,
                ToAccountId = 1003,
                Symbol = "USDT",
                Amount = 400,
                TotalValue = 400,
                CreatedAt = new DateTime(2023,08,29)
            },
        };

        var expectedResult = new BalanceStatus()
        {
            IRRGained = -20600000m,
            USDTGained = -515,
            IRRInject = -47300000,
            USDTInject = 1182.5m,
        };


        _transactionService.Setup(ts => ts.GetAccountTransactions(accountId, cancellationToken, null, null)).ReturnsAsync(transactions);

        var result = await _balanceService.GetBalanceStatus(request, accountId, cancellationToken);

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Should_Throw_Exception_On_B2BIRRRate_Not_Provided()
    {
        var request = new BalanceStatusInputDto()
        {
            B2BIRRRate = 0
        };

        var accountId = Account.MasterId;

        var cancellationToken = CancellationToken.None;

        var action = async () => await _balanceService.GetBalanceStatus(request, accountId, cancellationToken);

        await action.Should().ThrowExactlyAsync<BadRequestException>();
    }
}
