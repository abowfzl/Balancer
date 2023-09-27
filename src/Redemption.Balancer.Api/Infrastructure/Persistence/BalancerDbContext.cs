using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Domain.Entities;
using Redemption.Balancer.Api.Infrastructure.Persistence.Configurations;

namespace Redemption.Balancer.Api.Infrastructure.Persistence;

public class BalancerDbContext : DbContext
{
    public BalancerDbContext(DbContextOptions<BalancerDbContext> options)
        : base(options)
    {
    }

    public DbSet<AccountEntity> Accounts { get; set; }

    public DbSet<AccountConfigEntity> AccountConfigs { get; set; }

    public DbSet<TransactionEntity> Transactions { get; set; }

    public DbSet<WorkerEntity> Workers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AccountConfigConfigurations.Configure(modelBuilder);
        AccountConfigurations.Configure(modelBuilder);
        TransactionConfigurations.Configure(modelBuilder);
        WorkerConfigurations.Configure(modelBuilder);

        var mutableProperties = modelBuilder.Model.GetEntityTypes()
                                                       .SelectMany(t => t.GetProperties())
                                                       .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));
        foreach (var property in mutableProperties)
        {
            property.SetPrecision(24);
            property.SetScale(10);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ChangeTracker.DetectChanges();
        var added = this.ChangeTracker.Entries()
            .Where(t => t.State == EntityState.Added)
            .Select(t => t.Entity)
            .ToArray();

        foreach (var entity in added)
        {
            if (entity is BaseEntity baseEntity)
                baseEntity.CreatedAt = DateTime.UtcNow;
        }

        var modified = this.ChangeTracker.Entries()
            .Where(t => t.State == EntityState.Modified)
            .Select(t => t.Entity)
            .ToArray();

        foreach (var entity in modified)
        {
            if (entity is BaseEntity baseEntity)
                baseEntity.ModifiedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}