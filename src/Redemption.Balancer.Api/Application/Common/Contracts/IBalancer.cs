using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IBalancer
{
    Task Run(WorkerEntity worker, CancellationToken cancellationToken);

    Task RunManually(WorkerEntity worker, CancellationToken cancellationToken);

    Task BalanceAsync(int trackingId, CancellationToken cancellationToken);
}
