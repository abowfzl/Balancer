using AutoMapper;
using FluentAssertions;
using Moq;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Application.Common.Models.Dtos.AccountConfigs;
using Redemption.Balancer.Api.Controllers;
using Redemption.Balancer.Api.Domain.Entities;
using Xunit;

namespace Redemption.Balancer.Test.Controllers;

public class ConfigControllerTests
{
    private readonly ConfigController _configController;


    private readonly Mock<IAccountConfigService> _accountConfigService;
    private readonly Mock<IWorkerService> _workerService;
    private readonly Mock<IAccountService> _accountService;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IBalanceAccountConfigService> _balanceAccountConfigService;

    public ConfigControllerTests()
    {
        _accountConfigService = new Mock<IAccountConfigService>();
        _workerService = new Mock<IWorkerService>();
        _accountService = new Mock<IAccountService>();
        _mapper = new Mock<IMapper>();
        _balanceAccountConfigService = new Mock<IBalanceAccountConfigService>();

        _configController = new ConfigController(_accountConfigService.Object, _workerService.Object, _accountService.Object, _mapper.Object, _balanceAccountConfigService.Object);
    }

    [Fact]
    public async Task Should_Throw_Exception_To_Delete_Config_While_Worker_Is_Running()
    {
        int accountConfigId = 1;
        var cancellationToken = CancellationToken.None;

        _workerService.Setup(ws => ws.IsWorkerRunning(It.IsAny<WorkerEntity>(), cancellationToken)).ReturnsAsync(true);

        var action = async () => await _configController.AccountConfig(accountConfigId, cancellationToken);

        await action.Should().ThrowExactlyAsync<ForbiddenException>();
    }


    [Fact]
    public async Task Should_Throw_Exception_To_Update_Config_While_Worker_Is_Running()
    {
        int accountConfigId = 1;
        var cancellationToken = CancellationToken.None;

        var input = new AccountConfigInputDto()
        {
            AccountId = 1,
            Symbol = "USDT",
            Value = 100
        };

        _workerService.Setup(ws => ws.IsWorkerRunning(It.IsAny<WorkerEntity>(), cancellationToken)).ReturnsAsync(true);

        var action = async () => await _configController.AccountConfig(accountConfigId, input, cancellationToken);

        await action.Should().ThrowExactlyAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Should_Throw_Exception_To_Insert_Config_While_Worker_Is_Running()
    {
        var cancellationToken = CancellationToken.None;

        var input = new AccountConfigInputDto()
        {
            AccountId = 1,
            Symbol = "USDT",
            Value = 100
        };

        _workerService.Setup(ws => ws.IsWorkerRunning(It.IsAny<WorkerEntity>(), cancellationToken)).ReturnsAsync(true);

        var action = async () => await _configController.AccountConfig(input, cancellationToken);

        await action.Should().ThrowExactlyAsync<ForbiddenException>();
    }
}
