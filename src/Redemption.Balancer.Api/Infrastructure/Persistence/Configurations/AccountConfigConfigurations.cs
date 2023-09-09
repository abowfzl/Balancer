using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Configurations;

public static class AccountConfigConfigurations
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountConfigEntity>(entity =>
        {
            entity.ToTable("AccountConfigs");

            entity.Property(e => e.Symbol).HasMaxLength(16);

            entity.HasOne(d => d.Account)
                .WithMany(p => p.AccountConfigs)
                .HasForeignKey(d => d.AccountId);
        });
    }

}