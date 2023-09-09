using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Redemption.Balancer.Api.Application.Common.Attributes;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Domain.Enums;

namespace Redemption.Balancer.Api.Controllers;

public class BalanceController : ApiControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IPriceService _priceService;
    private readonly ICurrencyService _currencyService;
    private readonly IMapper _mapper;
    private readonly IBalanceService _balanceService;

    public BalanceController(ITransactionService transactionService,
        IPriceService priceService,
        ICurrencyService currencyService,
        IMapper mapper,
        IBalanceService balanceService)
    {
        _transactionService = transactionService;
        _priceService = priceService;
        _currencyService = currencyService;
        _mapper = mapper;
        _balanceService = balanceService;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost("[action]")]
    public async ValueTask<bool> Inject(BalanceInputDto inputDto, CancellationToken cancellationToken)
    {
        await ValidateInputs(inputDto, cancellationToken);

        var transaction = await CreateBalanceTransaction(Account.B2BId, Account.MasterId, inputDto.Symbol, inputDto.Value, cancellationToken);

        await _transactionService.Insert(transaction, cancellationToken);

        return true;
    }

    [Role(new[] { Role.Admin })]
    [HttpGet("[action]")]
    public async Task<BalanceStatusOutputDto> Status([FromQuery] BalanceStatusInputDto inputDto, CancellationToken cancellationToken)
    {
        ValidateInputs(inputDto);

        var balanceStatus = await _balanceService.GetBalanceStatus(inputDto, Account.MasterId, cancellationToken);

        var mappedBalanceStatus = _mapper.Map<BalanceStatusOutputDto>(balanceStatus);

        return mappedBalanceStatus;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost("[action]")]
    public async ValueTask<bool> Withdraw(BalanceInputDto inputDto, CancellationToken cancellationToken)
    {
        await ValidateInputs(inputDto, cancellationToken);

        var transaction = await CreateBalanceTransaction(Account.MasterId, Account.B2BId, inputDto.Symbol, -inputDto.Value, cancellationToken);

        await _transactionService.Insert(transaction, cancellationToken);

        return true;
    }

    private async Task<TransactionEntity> CreateBalanceTransaction(int fromAccountId, int toAccountId, string symbol, decimal differenceAmount, CancellationToken cancellationToken)
    {
        var symbolPrice = await _priceService.GetPrice(symbol, cancellationToken);
        var usdtPrice = await _priceService.GetPrice("USDT", cancellationToken);

        TransactionEntity transaction;

        if (differenceAmount > 0)
            transaction = _transactionService.GetCreditTransaction(fromAccountId, toAccountId, symbol, symbolPrice.Ticker, usdtPrice.Ticker, differenceAmount);
        else
            transaction = _transactionService.GetDebitTransaction(fromAccountId, toAccountId, symbol, symbolPrice.Ticker, usdtPrice.Ticker, differenceAmount);

        return transaction;
    }

    private static void ValidateInputs(BalanceStatusInputDto inputDto)
    {
        if (inputDto.B2BRate <= 0)
            throw new BadRequestException("Property 'B2BRate' should be greater than 0");
    }

    private async Task ValidateInputs(BalanceInputDto inputDto, CancellationToken cancellationToken)
    {
        _ = await _currencyService.GetBySymbol(inputDto.Symbol!, cancellationToken);

        if (inputDto.Value <= 0)
            throw new BadRequestException("Property 'Value' should be greater than 0");
    }
}
