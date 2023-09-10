using Redemption.Balancer.Api.Application.Common.Models.Externals.Kenes;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Stemeralds;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IPriceService
{
    Task<PriceResponse> GetPrice(string currencySymbol, CancellationToken cancellationToken);

    Task<StemeraldPriceResponse> GetStemeraldPrice(string currencySymbol, CancellationToken cancellationToken);
}
