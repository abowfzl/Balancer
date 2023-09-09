using AutoMapper;
using Redemption.Balancer.Api.Application.Common.Models.Balances;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

namespace Redemption.Balancer.Api.Application.Common.Mappers;

public class BalanceMapper : Profile
{
    public BalanceMapper()
    {
        CreateMap<BalanceStatus, BalanceStatusOutputDto>();
    }
}
