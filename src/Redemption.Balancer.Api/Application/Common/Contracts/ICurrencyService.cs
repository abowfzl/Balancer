using Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface ICurrencyService
{
    Task<CurrencyResponse> GetBySymbol(string currencySymbol, CancellationToken cancellationToken);
}