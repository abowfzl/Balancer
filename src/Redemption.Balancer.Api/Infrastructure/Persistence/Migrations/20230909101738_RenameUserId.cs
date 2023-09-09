using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class RenameUserId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "UserId",
            table: "Accounts",
            newName: "StemeraldUserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "StemeraldUserId",
            table: "Accounts",
            newName: "UserId");
    }
}
