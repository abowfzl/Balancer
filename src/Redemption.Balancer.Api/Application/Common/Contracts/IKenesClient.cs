using Redemption.Balancer.Api.Application.Common.Models.Externals.Kenes;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IKenesClient
{
    Task<PriceResponse> GetPriceBySymbol(string currencySymbol, CancellationToken cancellationToken);
}