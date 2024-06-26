﻿using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Constants;
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

        modelBuilder.Entity<AccountEntity>().HasData(
            new AccountEntity()
            {
                Id = Account.MasterId,
                Name = "Master",
                StemeraldUserId = 0,
                CreatedBy = 0,
                CreatedAt = new DateTime(2023, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc),
            },
            new AccountEntity()
            {
                Id = Account.UserId,
                Name = "User",
                StemeraldUserId = 0,
                CreatedBy = 0,
                CreatedAt = new DateTime(2023, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc),
            },
            new AccountEntity()
            {
                Id = Account.B2bId,
                Name = "B2B",
                StemeraldUserId = 0,
                CreatedBy = 0,
                CreatedAt = new DateTime(2023, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc),
            });
    }
}