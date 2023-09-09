using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class initDb : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Accounts",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                StemeraldUserId = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<int>(type: "integer", nullable: false),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ModifiedBy = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Accounts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Workers",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                Interval = table.Column<int>(type: "integer", nullable: false),
                IsRunning = table.Column<bool>(type: "boolean", nullable: false),
                StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                FailedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsEnable = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Workers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AccountConfigs",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                AccountId = table.Column<int>(type: "integer", nullable: false),
                Symbol = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                Value = table.Column<decimal>(type: "numeric", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<int>(type: "integer", nullable: false),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ModifiedBy = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AccountConfigs", x => x.Id);
                table.ForeignKey(
                    name: "FK_AccountConfigs_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Transactions",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FromAccountId = table.Column<int>(type: "integer", nullable: false),
                ToAccountId = table.Column<int>(type: "integer", nullable: false),
                Symbol = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                Amount = table.Column<decimal>(type: "numeric", nullable: false),
                TotalValue = table.Column<decimal>(type: "numeric", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<int>(type: "integer", nullable: false),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ModifiedBy = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Transactions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Transactions_Accounts_FromAccountId",
                    column: x => x.FromAccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Transactions_Accounts_ToAccountId",
                    column: x => x.ToAccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AccountConfigs_AccountId",
            table: "AccountConfigs",
            column: "AccountId");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_FromAccountId",
            table: "Transactions",
            column: "FromAccountId");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_ToAccountId",
            table: "Transactions",
            column: "ToAccountId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AccountConfigs");

        migrationBuilder.DropTable(
            name: "Transactions");

        migrationBuilder.DropTable(
            name: "Workers");

        migrationBuilder.DropTable(
            name: "Accounts");
    }
}
