using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Redemption.Balancer.Api.Application.Common.Attributes;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Domain.Enums;

namespace Redemption.Balancer.Api.Controllers;

public class WorkersController : ApiControllerBase
{
    private readonly IWorkerService _workerService;
    private readonly IBalancer _balancer;
    private readonly IMapper _mapper;

    public WorkersController(IWorkerService workerService,
        IBalancer balancer,
        IMapper mapper)
    {
        _workerService = workerService;
        _balancer = balancer;
        _mapper = mapper;
    }

    [Role(new[] { Role.Admin })]
    [HttpGet]
    public async Task<List<WorkerOutputDto>> Workers(CancellationToken cancellationToken)
    {
        var workerEntities = await _workerService.GetAll(cancellationToken);

        var mappedWorkerEntities = _mapper.Map<List<WorkerOutputDto>>(workerEntities);

        return mappedWorkerEntities;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost]
    public async ValueTask<bool> Add(WorkerInputDto inputDto, CancellationToken cancellationToken)
    {
        var workerEntityToAdd = _mapper.Map<WorkerEntity>(inputDto);

        await _workerService.Insert(workerEntityToAdd, cancellationToken);

        return true;
    }

    [Role(new[] { Role.Admin })]
    [HttpPut("{workerName}")]
    public async ValueTask<bool> Update(string workerName, WorkerInputDto inputDto, CancellationToken cancellationToken)
    {
        var worker = await _workerService.GetByName(workerName, cancellationToken);

        var workerEntityToUpdate = _mapper.Map<WorkerEntity>(inputDto);

        worker.Interval = workerEntityToUpdate.Interval;
        worker.IsEnable = workerEntityToUpdate.IsEnable;
        worker.Name = workerEntityToUpdate.Name;

        await _workerService.Update(worker, cancellationToken);

        return true;
    }

    [Role(new[] { Role.Admin })]
    [HttpDelete("{workerName}")]
    public async ValueTask<bool> Delete(string workerName, CancellationToken cancellationToken)
    {
        var worker = await _workerService.GetByName(workerName, cancellationToken);

        await _workerService.Delete(worker, cancellationToken);

        return true;
    }

    [Role(new[] { Role.Admin })]
    [HttpPost("[action]/{workerName}")]
    public async ValueTask<bool> RunManually(string workerName, CancellationToken cancellationToken)
    {
        var worker = await _workerService.GetByName(workerName, cancellationToken);

        await CheckWorkerStatus(worker, cancellationToken);

        await Task.Run(async () => await _balancer.RunManually(worker, cancellationToken), cancellationToken);

        return true;
    }

    private async Task CheckWorkerStatus(WorkerEntity worker, CancellationToken cancellationToken)
    {
        var isWorkerRunning = await _workerService.IsWorkerRunning(worker, cancellationToken);

        if (isWorkerRunning)
            throw new ForbiddenException("The worker is running, you cannot run it manually right now, Please try again later.");

        var isEnabled = await _workerService.IsWorkerEnabled(worker, cancellationToken);

        if (isEnabled is false)
            throw new ForbiddenException("The worker is disable, you cannot run it manually right now, Please try again later.");

    }
}