﻿using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
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

        };

        var accountId = Account.MasterId;

        var cancellationToken = CancellationToken.None;

        var expectedResult = new BalanceStatusOutputDto()
        {

        };

        var transactions = new List<TransactionEntity>() { new TransactionEntity() };

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
