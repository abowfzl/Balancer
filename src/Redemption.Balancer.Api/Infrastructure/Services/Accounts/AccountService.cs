using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Exceptions;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Persistence;

namespace Redemption.Balancer.Api.Infrastructure.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly BalancerDbContext _dbContext;

    public AccountService(BalancerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Insert(AccountEntity accountEntity, CancellationToken cancellationToken)
    {
        _dbContext.Accounts.Add(accountEntity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AccountEntity> GetById(int id, CancellationToken cancellationToken)
    {
        var accountEntity = await _dbContext.Accounts.FindAsync(id, cancellationToken);

        if (accountEntity == null)
            throw new EntityNotFoundException($"No Account found for id {id}");

        return accountEntity;
    }

    public async Task<IList<AccountEntity>> GetAll(CancellationToken cancellationToken)
    {
        var accountEntities = await _dbContext.Accounts.ToListAsync(cancellationToken);

        return accountEntities;
    }
}