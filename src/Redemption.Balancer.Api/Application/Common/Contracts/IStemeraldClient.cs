using Redemption.Balancer.Api.Application.Common.Models.Externals.Stemeralds;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IStemeraldClient
{
    Task<StemeraldPriceResponse> GetPriceBySymbol(string currencySymbol, CancellationToken cancellationToken);
}
