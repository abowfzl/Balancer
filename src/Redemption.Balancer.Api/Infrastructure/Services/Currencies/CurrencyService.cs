using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;
using Redemption.Balancer.Api.Constants;

namespace Redemption.Balancer.Api.Infrastructure.Services.Currencies;

public class CurrencyService : ICurrencyService
{
    private readonly IBasicClient _basicClient;
    private readonly IMemoryCache _memoryCache;

    public CurrencyService(IBasicClient basicClient,
        IMemoryCache memoryCache)
    {
        _basicClient = basicClient;
        _memoryCache = memoryCache;
    }

    public async Task<IList<CurrencyResponse>> GetAll(CancellationToken cancellationToken)
    {
        var cachedCurrencies = _memoryCache.Get<IList<CurrencyResponse>>(Keys.AllCurrencies);

        if (cachedCurrencies is not null)
            return cachedCurrencies;

        var currencies = await _basicClient.GetCurrencies(cancellationToken);

        _memoryCache.Set(Keys.AllCurrencies, currencies);

        return currencies;
    }

    public async Task<CurrencyResponse> GetBySymbol(string currencySymbol, CancellationToken cancellationToken)
    {
        var cachedCurrencies = _memoryCache.Get<IList<CurrencyResponse>>(Keys.AllCurrencies);

        var cachedCurrency = cachedCurrencies?.FirstOrDefault(x => x.Symbol == currencySymbol);

        if (cachedCurrency is not null)
            return cachedCurrency;

        var currency = await _basicClient.GetCurrencyBySymbol(currencySymbol, cancellationToken);

        if (currency is null)
            throw new EntityNotFoundException($"No Currency found for {currencySymbol}");

        return currency;
    }
}