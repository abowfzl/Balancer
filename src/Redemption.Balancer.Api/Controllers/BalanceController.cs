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
    private readonly IMapper _mapper;
    private readonly IBalanceService _balanceService;

    public BalanceController(ITransactionService transactionService,
        IPriceService priceService,
        IMapper mapper,
        IBalanceService balanceService)
    {
        _transactionService = transactionService;
        _priceService = priceService;
        _mapper = mapper;
        _balanceService = balanceService;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost("[action]")]
    public async ValueTask<bool> Inject(BalanceInputDto inputDto, CancellationToken cancellationToken)
    {
        ValidateInputs(inputDto);

        var transaction = await CreateBalanceTransaction(Account.B2BId, Account.MasterId, inputDto.Symbol, inputDto.Value, "inject", cancellationToken);
        transaction.CreatedBy = GetUserIdFromHeader();

        await _transactionService.Add(transaction, cancellationToken);

        return true;
    }

    [Role(new[] { Role.Admin })]
    [HttpGet("[action]")]
    public async Task<BalanceStatusOutputDto> Status([FromQuery] BalanceStatusInputDto inputDto, CancellationToken cancellationToken)
    {
        var balanceStatus = await _balanceService.GetBalanceStatus(inputDto, Account.MasterId, cancellationToken);

        var mappedBalanceStatus = _mapper.Map<BalanceStatusOutputDto>(balanceStatus);

        return mappedBalanceStatus;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost("[action]")]
    public async ValueTask<bool> Withdraw(BalanceInputDto inputDto, CancellationToken cancellationToken)
    {
        ValidateInputs(inputDto);

        var transaction = await CreateBalanceTransaction(Account.MasterId, Account.B2BId, inputDto.Symbol, -inputDto.Value, "withdraw", cancellationToken);
        transaction.CreatedBy = GetUserIdFromHeader();

        await _transactionService.Add(transaction, cancellationToken);

        return true;
    }

    private async Task<TransactionEntity> CreateBalanceTransaction(int fromAccountId, int toAccountId, string symbol, decimal differenceAmount, string source, CancellationToken cancellationToken)
    {
        var currencyReferencePrice = await _priceService.CalculateReferencePrice(symbol, cancellationToken);

        TransactionEntity transaction;

        if (differenceAmount > 0)
            transaction = _transactionService.GetCreditTransaction(fromAccountId, toAccountId, symbol, currencyReferencePrice, differenceAmount, source);
        else
            transaction = _transactionService.GetDebitTransaction(fromAccountId, toAccountId, symbol, currencyReferencePrice, differenceAmount, source);

        return transaction;
    }

    private static void ValidateInputs(BalanceInputDto inputDto)
    {
        if (inputDto.Value <= 0)
            throw new BadRequestException("Property 'Value' should be greater than 0");
    }
}
