using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Kenes;

namespace Redemption.Balancer.Api.Infrastructure.Services.Prices;

public class PriceService : IPriceService
{
    private readonly IKenesClient _kenesClient;

    public PriceService(IKenesClient kenesClient)
    {
        _kenesClient = kenesClient;
    }

    public async Task<PriceResponse> GetPrice(string currencySymbol, CancellationToken cancellationToken)
    {
        var price = await _kenesClient.GetPriceBySymbol(currencySymbol, cancellationToken);

        return price is null ? throw new EntityNotFoundException($"No price found for {currencySymbol}") : price;
    }
}
