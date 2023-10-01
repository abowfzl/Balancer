using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IAccountConfigService
{
    Task<List<AccountConfigEntity>> GetAll(CancellationToken cancellationToken);
    Task<AccountConfigEntity> GetById(int id, CancellationToken cancellationToken);
    Task Add(AccountConfigEntity accountConfig, CancellationToken cancellationToken);
    Task Update(AccountConfigEntity accountConfig, CancellationToken cancellationToken);
    Task Delete(AccountConfigEntity accountConfig, CancellationToken cancellationToken);
}