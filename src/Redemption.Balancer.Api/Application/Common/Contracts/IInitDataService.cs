using Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IInitDataService
{
    Task InitData(InitDataInputDto inputDto, CancellationToken cancellationToken);
}
