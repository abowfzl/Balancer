using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class seedDeafultAccounts : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Accounts",
            columns: new[] { "Id", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "Name", "StemeraldUserId" },
            values: new object[,]
            {
                { 1, new DateTime(2023, 9, 10, 0, 00, 00, 000, DateTimeKind.Utc), 0, null, null, "Master", 0 },
                { 2, new DateTime(2023, 9, 10, 0, 00, 00, 000, DateTimeKind.Utc), 0, null, null, "User", 0 },
                { 3, new DateTime(2023, 9, 10, 0, 00, 00, 000, DateTimeKind.Utc), 0, null, null, "B2B", 0 }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Accounts",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Accounts",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Accounts",
            keyColumn: "Id",
            keyValue: 3);
    }
}
