using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;

namespace Redemption.Balancer.Api.Infrastructure.Services.Currencies;

public class CurrencyService : ICurrencyService
{
    private readonly IBasicClient _basicClient;

    public CurrencyService(IBasicClient basicClient)
    {
        _basicClient = basicClient;
    }

    public async Task<CurrencyResponse> GetBySymbol(string currencySymbol, CancellationToken cancellationToken)
    {
        var currency = await _basicClient.GetCurrencyBySymbol(currencySymbol, cancellationToken);

        if (currency is null)
            throw new EntityNotFoundException($"No Currency found for {currencySymbol}");

        return currency;
    }
}