using AutoMapper;
using Redemption.Balancer.Api.Application.Common.Models.Balances;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

namespace Redemption.Balancer.Api.Application.Common.Mappers;

public class BalanceMapper : Profile
{
    public BalanceMapper()
    {
        CreateMap<BalanceStatus, BalanceStatusOutputDto>()
            .ForMember(b => b.TotalMasterBalanceInIRR, opt => opt.MapFrom(b => b.TotalBalanceInIRR))
            .ForMember(b => b.TotalMasterBalanceInUSDT, opt => opt.MapFrom(b => b.TotalBalanceInUSDT))
            .ForMember(b => b.IRRMasterBalance, opt => opt.MapFrom(b => b.IRRBalance))
            .ForMember(b => b.USDTMasterBalance, opt => opt.MapFrom(b => b.USDTBalance));
    }
}
