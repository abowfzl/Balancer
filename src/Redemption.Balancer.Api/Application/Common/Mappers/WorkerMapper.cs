using AutoMapper;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;
using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Mappers;

public class WorkerMapper : Profile
{
    public WorkerMapper()
    {
        CreateMap<WorkerInputDto, WorkerEntity>();
        CreateMap<WorkerEntity, WorkerOutputDto>();
    }
}
