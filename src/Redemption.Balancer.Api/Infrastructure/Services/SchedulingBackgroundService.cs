using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Infrastructure.Workers;

namespace Redemption.Balancer.Api.Infrastructure.Services;

public class SchedulingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SchedulingBackgroundService> _logger;

    public SchedulingBackgroundService(ILogger<SchedulingBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                await Task.Delay(5000, cancellationToken);

                _logger.LogInformation("{SchedulingBackground} BackgroundService Started", nameof(SchedulingBackgroundService));

                using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                {
                    var balancer = scope.ServiceProvider.GetRequiredService<IBalancer>();

                    var workerService = scope.ServiceProvider.GetRequiredService<IWorkerService>();

                    var worker = await workerService.GetByName(nameof(BotBalancer), cancellationToken);

                    await balancer.Run(worker, cancellationToken);

                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception in {SchedulingBackground} when ExecutingAsync. message:{exceptionMessage}. CatchBlock:", nameof(SchedulingBackgroundService), exception.Message);
            }
        }
    }
}