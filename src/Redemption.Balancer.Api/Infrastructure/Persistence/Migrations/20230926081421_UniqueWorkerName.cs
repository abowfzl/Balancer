using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueWorkerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Workers_Name",
                table: "Workers",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workers_Name",
                table: "Workers");
        }
    }
}
