
using Redemption.Balancer.Api.Application.Common.Contracts;
using StexchangeClient.Contracts;
using StexchangeClient.Models.Response.Assets;

namespace Redemption.Balancer.Api.Infrastructure.Services.Exchanges;

public class StexchangeService : IStexchangeService
{
    private readonly IStexchangeRestClient _stexchangeRestClient;

    public StexchangeService(IStexchangeRestClient stexchangeRestClient)
    {
        _stexchangeRestClient = stexchangeRestClient;
    }

    public async Task UpdateBalance<T>(int requestId, int userId, string assetName, string businessType, int businessId, decimal balanceChange, T details, CancellationToken cancellationToken)
    {
        var response = await _stexchangeRestClient.UpdateBalance(requestId, userId, assetName, businessType, businessId, balanceChange, details, cancellationToken);

        if (response.Status != "success")
            throw new Exception($"Can not update balance for userId:{userId}, assetName:{assetName}, amount:{balanceChange} with Status:{response.Status}");
    }

    public async Task<Dictionary<string, BalanceQueryResponse>> GetBalanceQueries(int requestId, int userId, CancellationToken cancellationToken, params string[] assetNames)
    {
        var balanceQueries = await _stexchangeRestClient.GetBalanceQueries(requestId, userId, cancellationToken, assetNames);

        return balanceQueries;
    }
}
