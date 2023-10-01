using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IAccountService
{
    Task Add(AccountEntity accountEntity, CancellationToken cancellationToken);
    Task<AccountEntity> GetById(int id, CancellationToken cancellationToken);
    Task<IList<AccountEntity>> GetAll(CancellationToken cancellationToken);
}