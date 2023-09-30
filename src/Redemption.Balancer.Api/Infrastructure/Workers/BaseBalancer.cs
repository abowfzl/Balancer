using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Infrastructure.Workers;

public abstract class BaseBalancer : IBalancer
{
    private readonly IWorkerService _workerService;
    private readonly ILogger _logger;

    public BaseBalancer(IWorkerService workerService, ILogger logger)
    {
        _workerService = workerService;
        _logger = logger;
    }

    public async Task Run(WorkerEntity worker, CancellationToken cancellationToken)
    {
        if (worker is null)
            throw new ArgumentNullException(nameof(worker));

        while (cancellationToken.IsCancellationRequested is false)
        {
            var trackingId = Random.Shared.Next();

            try
            {
                worker = await _workerService.GetByName(worker.Name!, cancellationToken);

                _logger.LogInformation("Worker:{workerName} wait for Interval:{interval}", worker.Name, worker.Interval);

                await Task.Delay(TimeSpan.FromSeconds(worker.Interval), cancellationToken);

                if (await _workerService.IsWorkerEnabled(worker, cancellationToken) is false)
                {
                    _logger.LogWarning("Worker:{workerName} is disabled, cannot be started", worker.Name);
                    continue;
                }

                _logger.LogInformation("Hello from Worker:{workerName}", worker.Name);

                // ToDo: better solution when exception happened or the time isn't reach!
                if (DateTime.UtcNow >= worker.CompletedAt + TimeSpan.FromSeconds(worker.Interval) || worker.CompletedAt is null)
                {
                    if (await _workerService.IsWorkerRunning(worker, cancellationToken))
                    {
                        _logger.LogWarning("Worker:{workerName} is already Running, the loop cannot be started", worker.Name);
                        continue;
                    }

                    _logger.LogInformation("Worker:{workerName} loop started", worker.Name);

                    worker.StartedAt = DateTime.UtcNow;
                    worker.IsRunning = true;
                    await _workerService.Update(worker, cancellationToken);
                    _logger.LogInformation("Worker:{workerName} StartedAt set", worker.Name);

                    _logger.LogInformation("Worker:{workerName} BalanceAsync started", worker.Name);

                    await BalanceAsync(trackingId, cancellationToken);

                    _logger.LogInformation("Worker:{workerName} BalanceAsync finished", worker.Name);

                    worker.CompletedAt = DateTime.UtcNow;
                    worker.IsRunning = false;
                    await _workerService.Update(worker, cancellationToken);
                    _logger.LogInformation("Worker:{workerName} CompletedAt set", worker.Name);

                    _logger.LogInformation("Worker:{workerName} loop completed", worker.Name);
                }
                else
                    _logger.LogWarning("Worker:{workerName} time isn't reach to start. wait again for interval", worker.Name);

                _logger.LogInformation("Goodbye from Worker:{workerName}", worker.Name);
            }
            catch (Exception exception)
            {
                worker.FailedAt = DateTime.UtcNow;
                worker.IsRunning = false;
                await _workerService.Update(worker, cancellationToken);
                _logger.LogError("Worker:{workerName} FailedAt set", worker.Name);

                _logger.LogError(exception, "Worker:{serviceName} loop failed - {trackingId}. CatchBlock:"
                    , worker.Name,
                    trackingId);
            }
        }
    }

    public async Task RunManually(WorkerEntity worker, CancellationToken cancellationToken)
    {
        var trackingId = Random.Shared.Next();

        _logger.LogInformation("Manually Worker:{workerName} started", worker.Name);

        try
        {
            worker.StartedAt = DateTime.UtcNow;
            worker.IsRunning = true;
            await _workerService.Update(worker, cancellationToken);
            _logger.LogInformation("Manually Worker:{workerName} StartedAt set", worker.Name);

            _logger.LogInformation("Manually Worker:{workerName} BalanceAsync started", worker.Name);

            await BalanceAsync(trackingId, cancellationToken);

            _logger.LogInformation("Manually Worker:{workerName} BalanceAsync finished", worker.Name);

            worker.CompletedAt = DateTime.UtcNow;
            worker.IsRunning = false;
            await _workerService.Update(worker, cancellationToken);
            _logger.LogInformation("Manually Worker:{workerName} CompletedAt set", worker.Name);
        }
        catch (Exception exception)
        {
            worker.FailedAt = DateTime.UtcNow;
            worker.IsRunning = false;
            await _workerService.Update(worker, cancellationToken);
            _logger.LogError("Manually Worker:{workerName} FailedAt set", worker.Name);

            _logger.LogError(exception, "Manually Worker:{serviceName} failed - {trackingId}. CatchBlock:"
                , worker.Name,
                trackingId);
        }
    }

    public abstract Task BalanceAsync(int trackingId, CancellationToken cancellationToken);
}
