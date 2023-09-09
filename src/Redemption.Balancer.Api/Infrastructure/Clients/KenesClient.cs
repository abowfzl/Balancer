using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models.Configs;
using Redemption.Balancer.Api.Application.Common.Models.Externals;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Kenes;

namespace Redemption.Balancer.Api.Infrastructure.Clients;

public class KenesClient : BaseClient, IKenesClient
{
    private readonly HttpClient _httpClient;
    private readonly KenesConfig _kenesConfig;

    public KenesClient(HttpClient httpClient, IOptionsMonitor<KenesConfig> kenesConfigOptions)
    {
        _httpClient = httpClient;
        _kenesConfig = kenesConfigOptions.CurrentValue;
    }

    public async Task<PriceResponse> GetPriceBySymbol(string currencySymbol, CancellationToken cancellationToken)
    {
        var requestResponse = await _httpClient.GetAsync(string.Format(_kenesConfig.GetPriceBySymbolEndPoint, currencySymbol), cancellationToken);

        var apiResult = await GetResponse<BaseResponse>(nameof(KenesClient), requestResponse);

        var priceResponse = JsonConvert.DeserializeObject<PriceResponse>(apiResult.Result.ToString()!);

        return priceResponse!;
    }
}