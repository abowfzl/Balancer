using Redemption.Balancer.Api.Application.Common.Models.Balances;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Balances;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IBalanceService
{
    Task<BalanceStatus> GetBalanceStatus(BalanceStatusInputDto request, int accountId, CancellationToken cancellationToken);
}
