using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models.Configs;
using Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;

namespace Redemption.Balancer.Api.Infrastructure.Clients;

public class BasicClient : BaseClient, IBasicClient
{
    private readonly HttpClient _httpClient;
    private readonly BasicConfig _basicConfig;

    public BasicClient(HttpClient httpClient, IOptionsMonitor<BasicConfig> basicConfigOptions)
    {
        _httpClient = httpClient;
        _basicConfig = basicConfigOptions.CurrentValue;
    }

    public async Task<CurrencyResponse> GetCurrencyBySymbol(string currencySymbol, CancellationToken cancellationToken)
    {
        var requestResponse = await _httpClient.GetAsync(string.Format(_basicConfig.GetCurrencyBySymbolEndPoint, currencySymbol), cancellationToken);

        var apiResult = await GetResponse<BaseBasicResponse>(nameof(BasicClient), requestResponse);

        var currencyResponse = JsonConvert.DeserializeObject<CurrencyResponse>(apiResult.Result.ToString()!);

        return currencyResponse!;
    }
}
