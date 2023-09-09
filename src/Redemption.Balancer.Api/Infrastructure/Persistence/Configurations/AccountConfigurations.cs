using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Configurations;

public static class AccountConfigurations
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountEntity>(entity =>
        {
            entity.ToTable("Accounts");

            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasMany(d => d.AccountConfigs)
                .WithOne(p => p.Account)
                .HasForeignKey(d => d.AccountId);

            entity.HasMany(d => d.FromTransactions)
                .WithOne(p => p.FromAccount)
                .HasForeignKey(d => d.FromAccountId);

            entity.HasMany(d => d.ToTransactions)
                .WithOne(p => p.ToAccount)
                .HasForeignKey(d => d.ToAccountId);
        });
    }
}