using Redemption.Balancer.Api.Application.Common.Models;
using StexchangeClient.Models.Response.Assets;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IStexchangeService
{
    Task UpdateBalance<T>(int requestId, int userId, string assetName, string businessType, int businessId, decimal balanceChange, BusinessDetailModel<T> details, CancellationToken cancellationToken);

    Task<Dictionary<string, BalanceQueryResponse>> GetBalanceQueries(int requestId, int userId, CancellationToken cancellationToken, params string[] assetNames);
}
