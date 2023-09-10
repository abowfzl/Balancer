using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Redemption.Balancer.Api.Application.Common.Attributes;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Accounts;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Domain.Enums;

namespace Redemption.Balancer.Api.Controllers;

public class AccountController : ApiControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;

    public AccountController(IAccountService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost("[action]")]
    public async ValueTask<bool> Add(AccountDtoInput inputDto, CancellationToken cancellationToken)
    {
        var accountConfigEntityToAdd = _mapper.Map<AccountEntity>(inputDto);
        accountConfigEntityToAdd.CreatedBy = GetUserIdFromHeader();

        await _accountService.Insert(accountConfigEntityToAdd, cancellationToken);

        return true;
    }
}
