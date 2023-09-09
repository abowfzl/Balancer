using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Persistence;

namespace Redemption.Balancer.Api.Infrastructure.Services.Workers;

public class WorkerService : IWorkerService
{
    private readonly BalancerDbContext _dbContext;

    public WorkerService(BalancerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkerEntity> GetLatestWorker(CancellationToken cancellationToken)
    {
        var workerEntity = await _dbContext.Workers.OrderByDescending(o => o.Id).FirstOrDefaultAsync(cancellationToken);

        return workerEntity ?? throw new EntityNotFoundException("No worker found in database");
    }

    public async ValueTask<bool> IsWorkerRunning(CancellationToken cancellationToken)
    {
        var latestWorkerEntity = await GetLatestWorker(cancellationToken);

        return latestWorkerEntity.IsRunning;
    }

    public async ValueTask<bool> IsWorkerRunning(WorkerEntity workerEntity, CancellationToken cancellationToken)
    {
        return await Task.FromResult(workerEntity.IsRunning);
    }

    public async Task Update(WorkerEntity workerEntity, CancellationToken cancellationToken)
    {
        _dbContext.Workers.Update(workerEntity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkerEntity> GetByName(string name, CancellationToken cancellationToken)
    {
        var workerEntity = await _dbContext.Workers.Where(o => o.Name == name).FirstOrDefaultAsync(cancellationToken);

        return workerEntity ?? throw new EntityNotFoundException($"No worker found with name:{name}");
    }
}