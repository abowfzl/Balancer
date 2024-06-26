﻿using Microsoft.EntityFrameworkCore;
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

    public async ValueTask<bool> IsWorkerEnabled(WorkerEntity workerEntity, CancellationToken cancellationToken)
    {
        return await Task.FromResult(workerEntity.IsEnable);
    }

    public async Task Update(WorkerEntity workerEntity, CancellationToken cancellationToken)
    {
        _dbContext.Workers.Update(workerEntity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkerEntity> GetByName(string name, CancellationToken cancellationToken)
    {
        var workerEntity = await _dbContext.Workers.Where(o => o.Name == name).FirstOrDefaultAsync(cancellationToken);

        if (workerEntity is null)
            throw new EntityNotFoundException($"No worker found with name:{name}");

        // reload entry to fetch data from db values
        await _dbContext.Workers.Entry(workerEntity).ReloadAsync(cancellationToken);

        return workerEntity;
    }

    public async Task Add(WorkerEntity workerEntity, CancellationToken cancellationToken)
    {
        _dbContext.Workers.Add(workerEntity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(WorkerEntity workerEntity, CancellationToken cancellationToken)
    {
        _dbContext.Workers.Remove(workerEntity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<WorkerEntity>> GetAll(CancellationToken cancellationToken)
    {
        var workerEntities = await _dbContext.Workers.ToListAsync(cancellationToken);

        return workerEntities;
    }
}