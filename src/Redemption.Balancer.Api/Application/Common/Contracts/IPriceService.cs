using Redemption.Balancer.Api.Application.Common.Models.Externals.Kenes;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IPriceService
{
    Task<PriceResponse> GetPrice(string currencySymbol, CancellationToken cancellationToken);
}
