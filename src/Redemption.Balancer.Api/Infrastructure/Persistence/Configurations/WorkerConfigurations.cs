using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Domain.Entities;

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Configurations;

public static class WorkerConfigurations
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkerEntity>(entity =>
        {
            entity.ToTable("Workers");

            entity.Property(e => e.Name).HasMaxLength(16);

            entity.HasIndex(e=>e.Name).IsUnique();
        });
    }
}