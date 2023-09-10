﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Migrations;

[DbContext(typeof(BalancerDbContext))]
[Migration("20230910094613_seedDeafultAccounts")]
partial class seedDeafultAccounts
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "7.0.10")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("Redemption.Balancer.Api.Domain.Entities.AccountConfigEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<int>("AccountId")
                    .HasColumnType("integer");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int>("CreatedBy")
                    .HasColumnType("integer");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int?>("ModifiedBy")
                    .HasColumnType("integer");

                b.Property<string>("Symbol")
                    .HasMaxLength(16)
                    .HasColumnType("character varying(16)");

                b.Property<decimal>("Value")
                    .HasColumnType("numeric");

                b.HasKey("Id");

                b.HasIndex("AccountId");

                b.ToTable("AccountConfigs", (string)null);
            });

        modelBuilder.Entity("Redemption.Balancer.Api.Domain.Entities.AccountEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int>("CreatedBy")
                    .HasColumnType("integer");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int?>("ModifiedBy")
                    .HasColumnType("integer");

                b.Property<string>("Name")
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.Property<int>("StemeraldUserId")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.ToTable("Accounts", (string)null);

                b.HasData(
                    new
                    {
                        Id = 1,
                        CreatedAt = new DateTime(2023, 9, 10, 0, 00, 00, 000, DateTimeKind.Utc),
                        CreatedBy = 0,
                        Name = "Master",
                        StemeraldUserId = 0
                    },
                    new
                    {
                        Id = 2,
                        CreatedAt = new DateTime(2023, 9, 10, 0, 00, 00, 000, DateTimeKind.Utc),
                        CreatedBy = 0,
                        Name = "User",
                        StemeraldUserId = 0
                    },
                    new
                    {
                        Id = 3,
                        CreatedAt = new DateTime(2023, 9, 10, 0, 00, 00, 000, DateTimeKind.Utc),
                        CreatedBy = 0,
                        Name = "B2B",
                        StemeraldUserId = 0
                    });
            });

        modelBuilder.Entity("Redemption.Balancer.Api.Domain.Entities.TransactionEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<decimal>("Amount")
                    .HasColumnType("numeric");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int>("CreatedBy")
                    .HasColumnType("integer");

                b.Property<int>("FromAccountId")
                    .HasColumnType("integer");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int?>("ModifiedBy")
                    .HasColumnType("integer");

                b.Property<string>("Symbol")
                    .HasMaxLength(16)
                    .HasColumnType("character varying(16)");

                b.Property<int>("ToAccountId")
                    .HasColumnType("integer");

                b.Property<decimal>("TotalValue")
                    .HasColumnType("numeric");

                b.HasKey("Id");

                b.HasIndex("FromAccountId");

                b.HasIndex("ToAccountId");

                b.ToTable("Transactions", (string)null);
            });

        modelBuilder.Entity("Redemption.Balancer.Api.Domain.Entities.WorkerEntity", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CompletedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<DateTime?>("FailedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int>("Interval")
                    .HasColumnType("integer");

                b.Property<bool>("IsEnable")
                    .HasColumnType("boolean");

                b.Property<bool>("IsRunning")
                    .HasColumnType("boolean");

                b.Property<string>("Name")
                    .HasMaxLength(16)
                    .HasColumnType("character varying(16)");

                b.Property<DateTime?>("StartedAt")
                    .HasColumnType("timestamp with time zone");

                b.HasKey("Id");

                b.ToTable("Workers", (string)null);
            });

        modelBuilder.Entity("Redemption.Balancer.Api.Domain.Entities.AccountConfigEntity", b =>
            {
                b.HasOne("Redemption.Balancer.Api.Domain.Entities.AccountEntity", "Account")
                    .WithMany("AccountConfigs")
                    .HasForeignKey("AccountId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Account");
            });

        modelBuilder.Entity("Redemption.Balancer.Api.Domain.Entities.TransactionEntity", b =>
            {
                b.HasOne("Redemption.Balancer.Api.Domain.Entities.AccountEntity", "FromAccount")
                    .WithMany("FromTransactions")
                    .HasForeignKey("FromAccountId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("Redemption.Balancer.Api.Domain.Entities.AccountEntity", "ToAccount")
                    .WithMany("ToTransactions")
                    .HasForeignKey("ToAccountId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("FromAccount");

                b.Navigation("ToAccount");
            });

        modelBuilder.Entity("Redemption.Balancer.Api.Domain.Entities.AccountEntity", b =>
            {
                b.Navigation("AccountConfigs");

                b.Navigation("FromTransactions");

                b.Navigation("ToTransactions");
            });
#pragma warning restore 612, 618
    }
}
