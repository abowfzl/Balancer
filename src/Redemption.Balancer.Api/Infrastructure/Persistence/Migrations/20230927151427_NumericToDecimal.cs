using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Redemption.Balancer.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NumericToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalValue",
                table: "Transactions",
                type: "numeric(24,10)",
                precision: 24,
                scale: 10,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "numeric(24,10)",
                precision: 24,
                scale: 10,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "AccountConfigs",
                type: "numeric(24,10)",
                precision: 24,
                scale: 10,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalValue",
                table: "Transactions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(24,10)",
                oldPrecision: 24,
                oldScale: 10);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(24,10)",
                oldPrecision: 24,
                oldScale: 10);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "AccountConfigs",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(24,10)",
                oldPrecision: 24,
                oldScale: 10);
        }
    }
}
