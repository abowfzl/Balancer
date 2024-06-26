﻿using FluentAssertions;
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
            B2bIrrRate = 40000
        };

        var accountId = Account.MasterId;

        var cancellationToken = CancellationToken.None;

        var expectedResult = new BalanceStatus()
        {
            TotalBalanceInIrr = -20600000m,
            TotalBalanceInUsdt = -515,
            IrrBalance = 37_000_000,
            UsdtBalance = -1440M,
            IrrDebit = -47300000,
            UsdtDebit = 1182.5m,
        };

        _transactionService.Setup(ts => ts.CalculateAccountIrrTransactions(accountId, cancellationToken, null, null)).ReturnsAsync(37_000_000);

        _transactionService.Setup(ts => ts.CalculateAccountUsdtTransactions(accountId, cancellationToken, null, null)).ReturnsAsync(-1_440);

        var result = await _balanceService.GetBalanceStatus(request, accountId, cancellationToken);

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Should_Throw_Exception_On_B2bIrrRate_Not_Provided()
    {
        var request = new BalanceStatusInputDto()
        {
            B2bIrrRate = 0
        };

        var accountId = Account.MasterId;

        var cancellationToken = CancellationToken.None;

        var action = async () => await _balanceService.GetBalanceStatus(request, accountId, cancellationToken);

        await action.Should().ThrowExactlyAsync<BadRequestException>();
    }
}
