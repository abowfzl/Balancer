using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Redemption.Balancer.Api.Application.Common.Attributes;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Accounts;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Domain.Enums;

namespace Redemption.Balancer.Api.Controllers;

public class AccountsController : ApiControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;

    public AccountsController(IAccountService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<AccountOutputDto>> Accounts(CancellationToken cancellationToken)
    {
        var accountEntities = await _accountService.GetAll(cancellationToken);

        var mappedAccountEntities = _mapper.Map<List<AccountOutputDto>>(accountEntities);

        return mappedAccountEntities;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost]
    public async ValueTask<bool> Add(AccountDtoInput inputDto, CancellationToken cancellationToken)
    {
        var accountConfigEntityToAdd = _mapper.Map<AccountEntity>(inputDto);
        accountConfigEntityToAdd.CreatedBy = GetUserIdFromHeader();

        await _accountService.Add(accountConfigEntityToAdd, cancellationToken);

        return true;
    }
}
