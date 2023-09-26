using Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IBasicClient
{
    Task<CurrencyResponse> GetCurrencyBySymbol(string currencySymbol, CancellationToken cancellationToken);

    Task<IList<CurrencyResponse>> GetCurrencies(CancellationToken cancellationToken);
}
