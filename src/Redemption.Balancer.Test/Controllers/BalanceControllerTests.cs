using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Stemeralds;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Controllers;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Common.Extensions;
using Redemption.Balancer.Test.Fakes;
using Xunit;

namespace Redemption.Balancer.Test.Controllers;

public class BalanceControllerTests
{
    private readonly BalanceController _balanceController;


    private readonly Mock<ITransactionService> _transactionService;
    private readonly Mock<IPriceService> _priceService;
    private readonly Mock<IBalanceService> _balanceService;
    private readonly Mock<IMapper> _mapper;

    public BalanceControllerTests()
    {
        _transactionService = new Mock<ITransactionService>();
        _priceService = new Mock<IPriceService>();
        _balanceService = new Mock<IBalanceService>();
        _mapper = new Mock<IMapper>();

        _balanceController = new BalanceController(_transactionService.Object, _priceService.Object, _mapper.Object, _balanceService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = FakeHttpContext.CreateFakeHttpContext()
            }
        };
    }

    [Theory]
    [InlineData("USDT", 100, 0.000038622580)]
    [InlineData("IRR", 100000, 0.000000000767022)]
    [InlineData("DOGE", 5000, .00000243901591)]
    public async Task Add_Withdraw_Transaction(string symbol, decimal value, decimal referencePrice)
    {
        #region Arrange

        var cancellationToken = CancellationToken.None;

        var transactionEntityToBeAdded = new TransactionEntity()
        {
            Id = 1,
            Symbol = symbol,
            Amount = value,
            TotalValue = referencePrice * value,
            ToAccountId = Account.B2bId,
            FromAccountId = Account.MasterId,
        };

        var input = new BalanceInputDto()
        {
            Symbol = symbol,
            Value = value
        };


        _priceService.Setup(p => p.CalculateReferencePrice(symbol, cancellationToken)).ReturnsAsync(referencePrice);

        //the master is debit to B2b so value should be negative(it means that the value should be decreased from master and increase for B2b)
        _transactionService.Setup(w => w.GetDebitTransaction(Account.MasterId, Account.B2bId, symbol, referencePrice, -value, "withdraw")).Returns(transactionEntityToBeAdded);

        #endregion

        #region Act

        var response = await _balanceController.Withdraw(input, cancellationToken);

        #endregion

        #region Assert

        Assert.True(response);

        _transactionService.Verify(w => w.Add(transactionEntityToBeAdded, cancellationToken));

        #endregion
    }

    [Theory]
    [InlineData("USDT", 100, 1)]
    [InlineData("IRR", 100000, 50312.69679600254)]
    [InlineData("DOGE", 5000, 0.0063149999559843)]
    public async Task Add_Deposit_Transaction(string symbol, decimal value, decimal referencePrice)
    {
        #region Arrange

        var cancellationToken = CancellationToken.None;

        var transactionEntityToBeAdded = new TransactionEntity()
        {
            Id = 1,
            Symbol = symbol,
            Amount = value,
            TotalValue = referencePrice * value,
            ToAccountId = Account.MasterId,
            FromAccountId = Account.B2bId,
        };

        var input = new BalanceInputDto()
        {
            Symbol = symbol,
            Value = value
        };


        _priceService.Setup(p => p.CalculateReferencePrice(symbol, cancellationToken)).ReturnsAsync(referencePrice);

        //the B2b is want to credit to master so value should be positive(it means that the value should be decreased from B2b and increase for master)
        _transactionService.Setup(w => w.GetCreditTransaction(Account.B2bId, Account.MasterId, symbol, referencePrice, value, "deposit")).Returns(transactionEntityToBeAdded);

        #endregion

        #region Act

        var response = await _balanceController.Deposit(input, cancellationToken);

        #endregion

        #region Assert

        Assert.True(response);

        _transactionService.Verify(w => w.Add(transactionEntityToBeAdded, cancellationToken));

        #endregion
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Deposit_And_Withdraw_With_Negative_Number()
    {
        var cancellationToken = CancellationToken.None;

        var input = new BalanceInputDto()
        {
            Symbol = "USDT",
            Value = -10
        };

        var depositAction = async () => await _balanceController.Deposit(input, cancellationToken);

        var withdrawAction = async () => await _balanceController.Withdraw(input, cancellationToken);

        await depositAction.Should().ThrowExactlyAsync<BadRequestException>();
        await withdrawAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

}
