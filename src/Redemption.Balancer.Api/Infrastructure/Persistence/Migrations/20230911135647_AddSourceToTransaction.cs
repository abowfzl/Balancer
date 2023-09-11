using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Transactions",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Transactions");
        }
    }
}
