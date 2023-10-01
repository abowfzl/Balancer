using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Persistence;

namespace Redemption.Balancer.Api.Infrastructure.Services.AccountConfigs;

public class AccountConfigService : IAccountConfigService
{
    private readonly BalancerDbContext _dbContext;

    public AccountConfigService(BalancerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AccountConfigEntity>> GetAll(CancellationToken cancellationToken)
    {
        var accountConfigs = await _dbContext.AccountConfigs.ToListAsync(cancellationToken);

        return accountConfigs;
    }

    public async Task<AccountConfigEntity> GetById(int id, CancellationToken cancellationToken)
    {
        var accountConfig = await _dbContext.AccountConfigs.FindAsync(id, cancellationToken);

        if (accountConfig == null)
            throw new EntityNotFoundException($"No account config found for id: {id}");

        return accountConfig;
    }

    public async Task Add(AccountConfigEntity accountConfig, CancellationToken cancellationToken)
    {
        await _dbContext.AccountConfigs.AddAsync(accountConfig, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(AccountConfigEntity accountConfig, CancellationToken cancellationToken)
    {
        _dbContext.AccountConfigs.Update(accountConfig);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(AccountConfigEntity accountConfig, CancellationToken cancellationToken)
    {
        _dbContext.AccountConfigs.Remove(accountConfig);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}