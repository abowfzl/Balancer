using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IAccountService
{
    Task<AccountEntity> GetById(int id, CancellationToken cancellationToken);
    Task<IList<AccountEntity>> GetAll(CancellationToken cancellationToken);
}