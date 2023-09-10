using AutoMapper;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Accounts;
using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Mappers;

public class AccountMapper : Profile
{
    public AccountMapper()
    {
        CreateMap<AccountDtoInput, AccountEntity>();
        CreateMap<AccountEntity, AccountOutputDto>();
    }
}
