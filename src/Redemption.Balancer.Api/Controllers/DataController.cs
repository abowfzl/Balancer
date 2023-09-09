using Microsoft.AspNetCore.Mvc;
using Redemption.Balancer.Api.Application.Common.Attributes;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;
using Redemption.Balancer.Api.Domain.Enums;

namespace Redemption.Balancer.Api.Controllers
{
    public class DataController : ApiControllerBase
    {
        private readonly IInitDataService _initDataService;

        public DataController(IInitDataService initDataService)
        {
            _initDataService = initDataService;
        }

        [Role(new[] { Role.Admin })]
        [HttpPost("[action]")]
        public async ValueTask<bool> InitData(InitDataInputDto inputDto, CancellationToken cancellationToken)
        {
            await _initDataService.InitData(inputDto, cancellationToken);

            return true;
        }
    }
}
