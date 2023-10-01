using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IBalanceAccountConfigService
{
    Task BalanceAddAccountConfig(int trackingId, AccountConfigEntity newAccountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken);

    Task BalanceUpdateAccountConfig(int trackingId, AccountConfigEntity oldAccountConfigEntity, AccountConfigEntity newAccountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken);

    Task BalanceDeleteAccountConfig(int trackingId, AccountConfigEntity accountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken);
}
