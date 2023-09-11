using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Kenes;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Stemeralds;
using Redemption.Balancer.Api.Infrastructure.Common.Extensions;

namespace Redemption.Balancer.Api.Infrastructure.Services.Prices;

public class PriceService : IPriceService
{
    private readonly IKenesClient _kenesClient;
    private readonly IStemeraldClient _stemeraldClient;
    private readonly ICurrencyService _currencyService;

    public PriceService(IKenesClient kenesClient,
        IStemeraldClient stemeraldClient,
        ICurrencyService currencyService)
    {
        _kenesClient = kenesClient;
        _stemeraldClient = stemeraldClient;
        _currencyService = currencyService;
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

    public async ValueTask<decimal> CalculateReferencePrice(string symbol, CancellationToken cancellationToken)
    {
        var symbolPrice = await GetStemeraldPrice(symbol, cancellationToken);
        var usdtPrice = await GetStemeraldPrice("USDT", cancellationToken);
        var baseCurrency = await _currencyService.GetBySymbol(symbol, cancellationToken);
        var quoteCurrency = await _currencyService.GetBySymbol("USDT", cancellationToken);

        var basePriceTicker = symbolPrice.DecimalTicker;
        var baseCurrencyNormalizationScale = baseCurrency.NormalizationScale;

        var quotePriceTicker = usdtPrice.DecimalTicker;
        var quoteCurrencyNormalizationScale = quoteCurrency.NormalizationScale;

        var currencyReferencePrice = PriceExtensions.CalculateSymbolPrice(
            baseCurrencyNormalizationScale,
            quoteCurrencyNormalizationScale,
            basePriceTicker,
            quotePriceTicker);

        return currencyReferencePrice;
    }
}
