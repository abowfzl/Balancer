using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IWorkerService
{
    Task<WorkerEntity> GetLatestWorker(CancellationToken cancellationToken);
    ValueTask<bool> IsWorkerRunning(WorkerEntity workerEntity, CancellationToken cancellationToken);
    ValueTask<bool> IsWorkerEnabled(WorkerEntity workerEntity, CancellationToken cancellationToken);
    ValueTask<bool> IsWorkerRunning(CancellationToken cancellationToken);
    Task Update(WorkerEntity workerEntity, CancellationToken cancellationToken);
    Task<WorkerEntity> GetByName(string name, CancellationToken cancellationToken);
    Task Insert(WorkerEntity workerEntity, CancellationToken cancellationToken);
}