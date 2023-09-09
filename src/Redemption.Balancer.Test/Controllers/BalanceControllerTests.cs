using AutoMapper;
using FluentAssertions;
using Moq;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Controllers;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Common.Extensions;
using Xunit;

namespace Redemption.Balancer.Test.Controllers;

public class BalanceControllerTests
{
    private readonly BalanceController _balanceController;


    private readonly Mock<ITransactionService> _transactionService;
    private readonly Mock<IPriceService> _priceService;
    private readonly Mock<ICurrencyService> _currencyService;
    private readonly Mock<IBalanceService> _balanceService;
    private readonly Mock<IMapper> _mapper;

    public BalanceControllerTests()
    {
        _transactionService = new Mock<ITransactionService>();
        _priceService = new Mock<IPriceService>();
        _currencyService = new Mock<ICurrencyService>();
        _balanceService = new Mock<IBalanceService>();
        _mapper = new Mock<IMapper>();

        _balanceController = new BalanceController(_transactionService.Object, _priceService.Object, _currencyService.Object, _mapper.Object, _balanceService.Object);
    }

    [Theory]
    [InlineData("USDT", 100, 0.000038622580)]
    [InlineData("IRR", 100000, 0.000000000767022)]
    [InlineData("DOGE", 5000, .00000243901591)]
    public async Task Insert_Withdraw_Transaction(string symbol, decimal value, decimal priceTicker)
    {
        #region Arrange

        var usdtTicker = 0.000038622580m;
        var cancellationToken = CancellationToken.None;

        var transactionEntityToBeInserted = new TransactionEntity()
        {
            Id = 1,
            Symbol = symbol,
            Amount = value,
            TotalValue = PriceExtensions.CalculateDenormalizedPrice(priceTicker, usdtTicker) * value,
            ToAccountId = Account.B2BId,
            FromAccountId = Account.MasterId,
        };

        var input = new BalanceInputDto()
        {
            Symbol = symbol,
            Value = value
        };


        _priceService.Setup(p => p.GetPrice(symbol, cancellationToken)).ReturnsAsync(new Api.Application.Common.Models.Externals.Kenes.PriceResponse() { Ticker = priceTicker });
        _priceService.Setup(p => p.GetPrice("USDT", cancellationToken)).ReturnsAsync(new Api.Application.Common.Models.Externals.Kenes.PriceResponse() { Ticker = usdtTicker });

        //the master is debit to b2b so value should be negative(it means that the value should be decreased from master and increase for b2b)
        _transactionService.Setup(w => w.GetDebitTransaction(Account.MasterId, Account.B2BId, symbol, priceTicker, usdtTicker, -value)).Returns(transactionEntityToBeInserted);

        #endregion

        #region Act

        var response = await _balanceController.Withdraw(input, cancellationToken);

        #endregion

        #region Assert

        Assert.True(response);

        _transactionService.Verify(w => w.Insert(transactionEntityToBeInserted, cancellationToken));

        #endregion
    }

    [Theory]
    [InlineData("USDT", 100, 0.000038622580)]
    [InlineData("IRR", 100000, 0.000000000767022)]
    [InlineData("DOGE", 5000, .000000243901591)]
    public async Task Insert_Inject_Transaction(string symbol, decimal value, decimal priceTicker)
    {
        #region Arrange

        var usdtTicker = 0.000038622580m;
        var cancellationToken = CancellationToken.None;

        var transactionEntityToBeInserted = new TransactionEntity()
        {
            Id = 1,
            Symbol = symbol,
            Amount = value,
            TotalValue = PriceExtensions.CalculateDenormalizedPrice(priceTicker, usdtTicker) * value,
            ToAccountId = Account.MasterId,
            FromAccountId = Account.B2BId,
        };

        var input = new BalanceInputDto()
        {
            Symbol = symbol,
            Value = value
        };


        _priceService.Setup(p => p.GetPrice(symbol, cancellationToken)).ReturnsAsync(new Api.Application.Common.Models.Externals.Kenes.PriceResponse() { Ticker = priceTicker });
        _priceService.Setup(p => p.GetPrice("USDT", cancellationToken)).ReturnsAsync(new Api.Application.Common.Models.Externals.Kenes.PriceResponse() { Ticker = usdtTicker });

        //the B2B is want to credit to master so value should be positive(it means that the value should be decreased from b2b and increase for master)
        _transactionService.Setup(w => w.GetCreditTransaction(Account.B2BId, Account.MasterId, symbol, priceTicker, usdtTicker, value)).Returns(transactionEntityToBeInserted);

        #endregion

        #region Act

        var response = await _balanceController.Inject(input, cancellationToken);

        #endregion

        #region Assert

        Assert.True(response);

        _transactionService.Verify(w => w.Insert(transactionEntityToBeInserted, cancellationToken));

        #endregion
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Inject_And_Withdraw_With_Negative_Number()
    {
        var cancellationToken = CancellationToken.None;

        var input = new BalanceInputDto()
        {
            Symbol = "USDT",
            Value = -10
        };

        var injectAction = async () => await _balanceController.Inject(input, cancellationToken);

        var withdrawAction = async () => await _balanceController.Withdraw(input, cancellationToken);

        await injectAction.Should().ThrowExactlyAsync<BadRequestException>();
        await withdrawAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

}
