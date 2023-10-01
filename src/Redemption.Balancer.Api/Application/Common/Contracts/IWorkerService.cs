using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IWorkerService
{
    ValueTask<bool> IsWorkerRunning(WorkerEntity workerEntity, CancellationToken cancellationToken);
    ValueTask<bool> IsWorkerEnabled(WorkerEntity workerEntity, CancellationToken cancellationToken);
    Task Update(WorkerEntity workerEntity, CancellationToken cancellationToken);
    Task<WorkerEntity> GetByName(string name, CancellationToken cancellationToken);
    Task Add(WorkerEntity workerEntity, CancellationToken cancellationToken);
    Task Delete(WorkerEntity workerEntity, CancellationToken cancellationToken);
    Task<IList<WorkerEntity>> GetAll(CancellationToken cancellationToken);
}