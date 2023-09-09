using AutoMapper;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.AccountConfigs;
using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Mappers;

public class AccountConfigMapper : Profile
{
    public AccountConfigMapper()
    {
        CreateMap<AccountConfigEntity, AccountConfigOutputDto>().ReverseMap();

        CreateMap<AccountConfigInputDto, AccountConfigEntity>();
    }
}