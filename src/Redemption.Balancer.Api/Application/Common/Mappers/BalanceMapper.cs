using AutoMapper;
using Redemption.Balancer.Api.Application.Common.Models.Balances;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

namespace Redemption.Balancer.Api.Application.Common.Mappers;

public class BalanceMapper : Profile
{
    public BalanceMapper()
    {
        CreateMap<BalanceStatus, BalanceStatusOutputDto>()
            .ForMember(b => b.TotalMasterBalanceInIrr, opt => opt.MapFrom(b => b.TotalBalanceInIrr))
            .ForMember(b => b.TotalMasterBalanceInUsdt, opt => opt.MapFrom(b => b.TotalBalanceInUsdt))
            .ForMember(b => b.IrrMasterBalance, opt => opt.MapFrom(b => b.IrrBalance))
            .ForMember(b => b.UsdtMasterBalance, opt => opt.MapFrom(b => b.UsdtBalance))
            .ForMember(b => b.MasterIrrDebit, opt => opt.MapFrom(b => b.IrrDebit))
            .ForMember(b => b.MasterUsdtDebit, opt => opt.MapFrom(b => b.UsdtDebit));
    }
}
