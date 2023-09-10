using Microsoft.Extensions.Options;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models.Configs;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Stemeralds;

namespace Redemption.Balancer.Api.Infrastructure.Clients;

public class StemeraldClient : BaseClient, IStemeraldClient
{
    private readonly HttpClient _httpClient;
    private readonly StemeraldConfig _stemeraldConfig;


    public StemeraldClient(HttpClient httpClient, IOptionsMonitor<StemeraldConfig> stemeraldConfigOptions)
    {
        _httpClient = httpClient;
        _stemeraldConfig = stemeraldConfigOptions.CurrentValue;
    }

    public async Task<StemeraldPriceResponse> GetPriceBySymbol(string currencySymbol, CancellationToken cancellationToken)
    {
        var requestResponse = await _httpClient.GetAsync(string.Format(_stemeraldConfig.GetCurrencyPriceEndPoint, currencySymbol), cancellationToken);

        var currencyPrice = await GetResponse<StemeraldPriceResponse>(nameof(StemeraldClient), requestResponse);

        return currencyPrice;
    }
}
