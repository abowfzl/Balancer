using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Configurations;

public static class TransactionConfigurations
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionEntity>(entity =>
        {
            entity.ToTable("Transactions");

            entity.Property(e => e.Symbol).HasMaxLength(16);


            entity.HasOne(d => d.FromAccount)
                .WithMany(p => p.FromTransactions)
                .HasForeignKey(d => d.FromAccountId);

            entity.HasOne(d => d.ToAccount)
                .WithMany(p => p.ToTransactions)
                .HasForeignKey(d => d.ToAccountId);
        });
    }
}