using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Redemption.Balancer.Api.Application.Common.Attributes;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.AccountConfigs;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Domain.Enums;

namespace Redemption.Balancer.Api.Controllers;

public class ConfigController : ApiControllerBase
{
    private readonly IAccountConfigService _accountConfigService;
    private readonly IAccountService _accountService;
    private readonly IWorkerService _workerService;
    private readonly ICurrencyService _currencyService;
    private readonly IBalanceAccountConfigService _balanceAccountConfigService;

    private readonly IMapper _mapper;

    public ConfigController(IAccountConfigService accountConfigService,
        IWorkerService workerService,
        ICurrencyService currencyService,
        IAccountService accountService,
        IMapper mapper,
        IBalanceAccountConfigService balanceAccountConfigService)
    {
        _accountConfigService = accountConfigService;
        _workerService = workerService;
        _currencyService = currencyService;
        _accountService = accountService;
        _mapper = mapper;
        _balanceAccountConfigService = balanceAccountConfigService;
    }

    [HttpGet("[action]")]
    public async Task<List<AccountConfigOutputDto>> AccountConfigs(CancellationToken cancellationToken)
    {
        var accountConfigEntities = await _accountConfigService.GetAll(cancellationToken);

        var mappedAccountConfigs = _mapper.Map<List<AccountConfigOutputDto>>(accountConfigEntities);

        return mappedAccountConfigs;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost("[action]")]
    public async Task AccountConfig(AccountConfigInputDto inputDto, CancellationToken cancellationToken)
    {
        await ValidateInputs(inputDto, cancellationToken);

        var accountConfigEntityToAdd = _mapper.Map<AccountConfigEntity>(inputDto);
        accountConfigEntityToAdd.CreatedBy = GetUserIdFromHeader();

        var accountEntity = await _accountService.GetById(accountConfigEntityToAdd.AccountId, cancellationToken);

        var trackingId = Random.Shared.Next();

        await _balanceAccountConfigService.BalanceInsertAccountConfig(trackingId, accountConfigEntityToAdd, accountEntity, cancellationToken);

        await _accountConfigService.Insert(accountConfigEntityToAdd, cancellationToken);
    }

    [Role(new[] { Role.Admin })]
    [HttpPut("[action]/{id}")]
    public async Task AccountConfig(int id, AccountConfigInputDto inputDto, CancellationToken cancellationToken)
    {
        await ValidateInputs(inputDto, cancellationToken);

        var accountConfigEntityToUpdate = _mapper.Map<AccountConfigEntity>(inputDto);

        var accountConfigEntity = await _accountConfigService.GetById(id, cancellationToken);

        var accountEntity = await _accountService.GetById(accountConfigEntity.AccountId, cancellationToken);

        await _balanceAccountConfigService.BalanceUpdateAccountConfig(accountConfigEntity, accountConfigEntityToUpdate, accountEntity, cancellationToken);

        accountConfigEntity.ModifiedBy = GetUserIdFromHeader();
        accountConfigEntity.Value = accountConfigEntityToUpdate.Value;
        accountConfigEntity.AccountId = accountConfigEntityToUpdate.AccountId;
        accountConfigEntity.Symbol = accountConfigEntityToUpdate.Symbol;

        await _accountConfigService.Update(accountConfigEntity, cancellationToken);
    }

    [Role(new[] { Role.Admin })]
    [HttpDelete("[action]/{id}")]
    public async Task AccountConfig(int id, CancellationToken cancellationToken)
    {
        await IsWorkerRunning(cancellationToken);

        var accountConfigEntity = await _accountConfigService.GetById(id, cancellationToken);

        await _accountConfigService.Delete(accountConfigEntity, cancellationToken);
    }

    private async Task IsWorkerRunning(CancellationToken cancellationToken)
    {
        var isWorkerRunning = await _workerService.IsWorkerRunning(cancellationToken);

        if (isWorkerRunning)
            throw new ForbiddenException("The worker is running, you cannot make any changes on account configs, Please try again later.");
    }

    private async Task ValidateInputs(AccountConfigInputDto inputDto, CancellationToken cancellationToken)
    {
        await IsWorkerRunning(cancellationToken);

        await _accountService.GetById(inputDto.AccountId, cancellationToken);

        await _currencyService.GetBySymbol(inputDto.Symbol, cancellationToken);
    }
}