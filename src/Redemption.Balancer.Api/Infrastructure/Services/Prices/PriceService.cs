using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Kenes;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Stemeralds;

namespace Redemption.Balancer.Api.Infrastructure.Services.Prices;

public class PriceService : IPriceService
{
    private readonly IKenesClient _kenesClient;
    private readonly IStemeraldClient _stemeraldClient;

    public PriceService(IKenesClient kenesClient,
        IStemeraldClient stemeraldClient)
    {
        _kenesClient = kenesClient;
        _stemeraldClient = stemeraldClient;
    }

    public async Task<PriceResponse> GetPrice(string currencySymbol, CancellationToken cancellationToken)
    {
        var price = await _kenesClient.GetPriceBySymbol(currencySymbol, cancellationToken);

        return price is null ? throw new EntityNotFoundException($"No price found for {currencySymbol}") : price;
    }

    public async Task<StemeraldPriceResponse> GetStemeraldPrice(string currencySymbol, CancellationToken cancellationToken)
    {
        var price = await _stemeraldClient.GetPriceBySymbol(currencySymbol, cancellationToken);

        return price is null ? throw new EntityNotFoundException($"No price found for {currencySymbol}") : price;
    }
}
