using Microsoft.AspNetCore.Mvc;
using Redemption.Balancer.Api.Application.Common.Attributes;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Domain.Enums;

namespace Redemption.Balancer.Api.Controllers;

public class WorkerController : ApiControllerBase
{
    private readonly IWorkerService _workerService;
    private readonly IBalancer _balancer;

    public WorkerController(IWorkerService workerService,
        IBalancer balancer)
    {
        _workerService = workerService;
        _balancer = balancer;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost("[action]")]
    public async ValueTask<bool> RunManually(WorkerInputDto inputDto, CancellationToken cancellationToken)
    {
        var worker = await _workerService.GetByName(inputDto.Name, cancellationToken);

        await IsWorkerRunning(worker, cancellationToken);

        await Task.Run(async () => await _balancer.RunManually(worker, cancellationToken), cancellationToken);

        return true;
    }

    private async Task IsWorkerRunning(WorkerEntity worker, CancellationToken cancellationToken)
    {
        var isWorkerRunning = await _workerService.IsWorkerRunning(worker, cancellationToken);

        if (isWorkerRunning)
            throw new ForbiddenException("The worker is running, you cannot run it manually right now, Please try again later.");
    }
}