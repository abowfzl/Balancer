using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts
{
    public interface IBalanceAccountConfigService
    {
        Task BalanceInsertAccountConfig(int trackingId, AccountConfigEntity newAccountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken);

        Task BalanceUpdateAccountConfig(AccountConfigEntity oldAccountConfigEntity, AccountConfigEntity newAccountConfigEntity, AccountEntity accountEntity, CancellationToken cancellationToken);
    }
}
